using System.Numerics;
using System.Security.Cryptography;
using static Utility;

/// <summary>
/// A class implementing a 2^q x 2^q matrix as a decision diagram to represent a quantum gate
/// </summary>
public class DDMatrix
{
    public static readonly Dictionary<string, DDMatrix> gates = new() {
        {"id", Identity(1)},
    };
    public readonly int qubits;

    public readonly Complex rootWeight;
    public readonly DDNode root;

    public DDMatrix(int qubits)
    {
        this.qubits = qubits;
    }

    public DDMatrix(Complex[,] matrix)
    {
        (rootWeight, root) = DDNode.Construct(matrix, 0, matrix.GetLength(0), 0, matrix.GetLength(1));
        qubits = (int) Math.Log2(matrix.GetLength(0));
    }

    /// <summary>
    /// Returns an identity matrix of size 2^qubits x 2^qubits
    /// </summary>
    public static DDMatrix Identity(int qubits)
    {
        return new DDMatrix(0);
    }

    /// <summary>
    /// Returns true if and only if this DDMatrix is equal to the identity matrix of the same size
    /// </summary>
    public bool EqualsIdentity()
    {
        return false;
    }

    /// <summary>
    /// Returns the amount of DDNodes excluding one and zero nodes.
    /// </summary>
    public int Count()
    {
        return 0;
    }

    /// <summary>
    /// Returns the matrix multiplication of the two given values
    /// </summary>
    public static DDMatrix Multiply(DDMatrix first, DDMatrix second)
    {
        return new DDMatrix(0);
    }

    /// <summary>
    /// Returns the matrix multiplication of the two given values
    /// </summary>
    public static Complex[] Multiply(Complex[] first, DDMatrix second)
    {
        return first;
    }

    /// <summary>
    /// Returns the matrix multiplication of the two given values
    /// </summary>
    public static Complex[] Multiply(DDMatrix first, Complex[] second)
    {
        return second;
    }

    /// <summary>
    /// Extends a DDMatrix representing a gate with tensor multiplication
    /// </summary>
    public static DDMatrix Extend(DDMatrix m, int targetedBit)
    {
        return new DDMatrix(0);
    }


    public class DDNode
    {
        private readonly Complex[] weights;
        private readonly DDNode[] next;

        // constants
        public static readonly DDNode zero = new DDNode([], new DDNode[4]);
        public static readonly DDNode one = new DDNode(new Complex[4], []);

        public DDNode(Complex[]? weights, DDNode[]? next)
        {
            this.weights = weights;
            this.next = next;
        }

        /// <summary>
        /// Returns true if and only if this DDNode is equal to the identity matrix of the same size
        /// </summary>
        public bool EqualsIdentity(DDNode other)
        {
            return false;
        }

        /// <summary>
        /// Recursively constructs the DDNodes for a DDMatrix from the given matrix.<br/><br/>
        /// 
        /// The int parameters are indexes used to divide the matrix into rectangular parts.
        /// The method only considers the elements with indexes between [x0,y0] (both included) and 
        /// [x1,y1] (both excluded). This is used internally to divide the matrix into parts.<br/><br/>
        /// 
        /// This method should only be used with matrices of size 2^q x 2^q with q being an integer.
        /// If this method is called from the outside, the parameters should be 
        /// (matrix, 0, matrix.GetLength(0), 0, matrix.GetLength(1)) for a given matrix, so the 
        /// entire matrix is converted.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="x0"></param>
        /// <param name="x1"></param>
        /// <param name="y0"></param>
        /// <param name="y1"></param>
        /// <returns></returns>
        public static Tuple<Complex, DDNode> Construct(Complex[,] matrix, int x0, int x1, int y0, int y1)
        {
            if (x0 + 1 == x1 && y0 + 1 == y1) // we are at one element
            {
                if (CompareComplex(matrix[x0, y0], Complex.Zero))
                {
                    return new(Complex.Zero, zero);
                }
                else
                {
                    return new(matrix[x0, y0], one);
                }
            }
            else
            {
                Tuple<Complex, DDNode>[] results = [
                    Construct(matrix, x0, x1/2, y0, y1/2), Construct(matrix, (x0+x1)/2, x1, y0, y1/2),
                    Construct(matrix, x0, x1/2, (y0+y1)/2, y1), Construct(matrix, (x0+x1)/2, x1, (y0+y1)/2, y1)
                ];

                // check if Node should be zero and determines the first non zero weight for normalization
                bool allNodesZero = true;
                Complex firstNonZeroValue = Complex.Zero;
                for (int i = 0; i < 4; i++)
                {
                    if (results[i].Item2 != zero)
                    {
                        allNodesZero = false;
                        firstNonZeroValue = results[i].Item1;
                        break;
                    }
                }
                if (allNodesZero)
                {
                    return new(Complex.Zero, zero);
                }

                // normalize weights and create arrays
                Complex[] weights = new Complex[4];
                DDNode[] nodes = new DDNode[4];
                for (int i = 0; i < 4; i++)
                {
                    // normalizes the weights, so that the first non zero weight is equal to 1
                    // This allows us to make DDNodes with different weights comparable
                    weights[i] = results[i].Item1/firstNonZeroValue; 
                    nodes[i] = results[i].Item2;
                }

                // remove duplicates
                for (int i = 3; i>=0;i--)
                {
                    for (int k=0;k<i;k++)
                    {
                        if (nodes[i] == nodes[k])
                        {
                            nodes[k] = nodes[i];
                            break;
                        }
                    }
                }
                
                return new (firstNonZeroValue, new DDNode(weights, nodes));
            }
        }
    }
}