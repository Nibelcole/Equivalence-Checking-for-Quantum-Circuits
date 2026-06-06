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
        /// Returns true if and only if this node is one layer above DDNode.one
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
            Complex[] w = [this.weights[0], this.weights[1], this.weights[2], this.weights[3]];
            DDNode[] n = new DDNode[4];
            for (int i = 0; i < 4; i++)
            {
                n[i] = next[i].Copy(list);
            }
            return list.GetOrCreate(layer, w, n);
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
            return (this != zero) && (this != one)
                && CompareComplex(weights[0], Complex.One)
                && CompareComplex(weights[1], Complex.One)
                && CompareComplex(weights[2], Complex.One)
                && CompareComplex(weights[3], Complex.One)
                && next[0] == next[3] // check if it was correctly reduced
                && next[1] == zero
                && next[2] == zero
                && (next[0] == one || next[0].EqualsIdentity());
        }

        /// <summary>
        /// Normalizes and reduces this node and adds it to its nodeList.
        /// <br/><br/>
        /// Note that while this method does not contain recursive calls, it has to be called in a recursive method, 
        /// in which the nodes below this one have already called NormalizeAndReduce(). This is to avoid having to 
        /// recursively traverse the tree more than once.
        /// </summary>
        public Tuple<Complex, DDNode> NormalizeAndReduce()
        {
            // normalize node

            // check if Node should be zero and determines the first non zero weight for normalization
            bool allNodesZero = true;
            Complex firstNonZeroValue = Complex.Zero;
            for (int i = 0; i < 4; i++)
            {
                if (next[i] != zero)
                {
                    allNodesZero = false;
                    firstNonZeroValue = weights[i];
                    break;
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
    }
}