using System.Numerics;
using static Utility;

public partial class DDMatrix
{
    /// <summary>
    /// Represents a node in a DDMatrix.
    /// </summary>
    public partial class DDNode
    {
        public DDNodeList nodeList;
        public readonly int layer; // the height of this node in the tree starting from the leaves. DDNode.one is on layer 0
        public readonly Complex[] weights;
        public readonly DDNode[] next;

        // constants
        public static readonly DDNode zero = new DDNode(new(), -1, [], new DDNode[4]);
        public static readonly DDNode one = new DDNode(new(), 0, new Complex[4], []);

        public DDNode(DDNodeList nodeList, int layer, Complex[] weights, DDNode[] next)
        {
            this.nodeList = nodeList;
            this.layer = layer;
            this.weights = weights;
            this.next = next;
        }

        /// <summary>
        /// Returns true if and only if this node is one layer above DDNode.one. 
        /// 
        /// <br/><br/>The zero node always returns false even if it is on the correct layer as internally 
        /// it has layer -1.
        /// </summary>
        public bool IsLeaf()
        {
            return layer == 1;
        }

        /// <summary>
        /// Returns a deep copy of this DDNode
        /// </summary>
        public DDNode Copy(DDNodeList list)
        {
            if (this == zero) { return zero; }
            if (this == one) { return one; }
            DDNode[] n = new DDNode[4];
            for (int i = 0; i<4;i++)
            {
                n[i] = next[i].Copy(list);
            }
            return list.GetOrCreate(layer, weights, n);
        }

        /// <summary>
        /// Returns the root node of an identity matrix of size 2^qubits x 2^qubits.
        /// </summary>
        public static DDNode Identity(DDNodeList list, int qubits)
        {
            if (qubits == 0)
            {
                return one;
            }
            DDNode next = Identity(list, qubits - 1);
            return list.GetOrCreate(qubits, [1, 0, 0, 1], [next, zero, zero, next]);
        }

        /// <summary>
        /// Returns true if and only if this DDNode is equal to the identity matrix of the same size.<br/>
        /// 
        /// <br/><br/>
        /// Note that this differs from the similarly named method in DDMatrix (that this method is a helper
        /// function for), because this method does not account for the global phase.
        /// </summary>
        public bool EqualsIdentity()
        {
            return (this == one) || (
                (this != zero)
                && CompareComplex(weights[0], Complex.One)
                && CompareComplex(weights[1], Complex.Zero)
                && CompareComplex(weights[2], Complex.Zero)
                && CompareComplex(weights[3], Complex.One)
                && next[0] == next[3] // check if it was correctly reduced
                && next[1] == zero
                && next[2] == zero
                && next[0].EqualsIdentity()
            );
        }

        /// <summary>
        /// Recursively changes this DDNode, so it becomes the transposed of itself.
        /// </summary>
        public Complex Transposed(Dictionary<DDNode, Complex> renormalizedNodes)
        {
            if (this == zero)
            {
                return 0;
            } else if (this == one)
            {
                return 1;
            } else if (renormalizedNodes.ContainsKey(this))
            {
                return renormalizedNodes[this];
            }

            for (int i = 0; i < next.Length; i++)
            {
                weights[i] *= next[i].Transposed(renormalizedNodes);
            }

            (weights[1], weights[2]) = (weights[2], weights[1]);
            (next[1], next[2]) = (next[2], next[1]);

            var (c, n) = NormalizeAndReduce();
            if (n != this)
            {
                throw new Exception("Internal Error: Conjugate-Transposed had to create a new node, "
                + "despite the fact that this node should have already been recorded in nodeList.");
            }

            renormalizedNodes[n] = c;
            return c;
        }

        /// <summary>
        /// Normalizes and reduces this node and adds it to its nodeList.
        /// <br/><br/>
        /// Note that while this method does not contain recursive calls, it has to be called in a recursive method, 
        /// in which the nodes below this one have already called NormalizeAndReduce(). This is to avoid having to 
        /// recursively traverse the tree too many times.
        /// </summary>
        public Tuple<Complex, DDNode> NormalizeAndReduce()
        {
            // normalize node

            // check if Node should be zero and determines the first non zero weight for normalization
            bool allNodesZero = true;
            Complex firstNonZeroValue = Complex.Zero;
            for (int i = 0; i < 4; i++)
            {
                if (next[i] != zero && !CompareComplex(weights[i], 0))
                {
                    allNodesZero = false;
                    firstNonZeroValue = weights[i];
                    break;
                } else
                {
                    next[i] = zero;
                    weights[i] = Complex.Zero;
                }
            }
            if (allNodesZero)
            {
                return new(Complex.Zero, zero);
            }

            // normalize weights and create arrays
            for (int i = 0; i < 4; i++)
            {
                // normalizes the weights, so that the first non zero weight is equal to 1
                // This allows us to make DDNodes with different weights comparable
                weights[i] = weights[i] / firstNonZeroValue;
            }

            // reduce node (assuming that all nodes below have been reduced)

            DDNode n = nodeList.GetOrCreate(this);

            // return weight and new DDNode
            return new(firstNonZeroValue, n);
        }

        /// <summary>
        /// Recreates the nodelist by recursively going through all nodes and adding them to the given nodeList.
        /// This is needed to avoid certain bugs where temporary nodes are added to the nodeList.
        /// </summary>
        public DDNodeList RecreateNodeList(DDNodeList list)
        {
            if (this == zero || this == one)
            {
                return list;
            }
            foreach (var n in next)
            {
                n.RecreateNodeList(list);
            }
            list.AddIfNotPresent(layer, this);
            return list;
        }
    }
}