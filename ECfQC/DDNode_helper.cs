using System.Numerics;
using System.Runtime.InteropServices;
using static Utility;
// Contains larger methods for DDNode. This file was created, since DDNode.cs became too cluttered.

public partial class DDMatrix
{
    public partial class DDNode
    {

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
        public static Tuple<Complex, DDNode> Construct(DDNodeList list, Complex[,] matrix, int x0, int x1, int y0, int y1, int layer)
        {
            if (layer == 0) // it should also hold that (x0 + 1 == x1 && y0 + 1 == y1) // we are at one element
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
                int halfx = (x0 + x1) / 2;
                int halfy = (y0 + y1) / 2;
                Tuple<Complex, DDNode>[] results = [
                    Construct(list,matrix, x0, halfx, y0, halfy, layer-1),
                    Construct(list,matrix, x0, halfx, halfy, y1, layer-1),
                    Construct(list,matrix, halfx, x1, y0, halfy, layer-1),
                    Construct(list,matrix, halfx, x1, halfy, y1, layer-1)
                ];

                Complex[] c = new Complex[4];
                DDNode[] n = new DDNode[4];
                bool isAllZeros = true;
                for (int i = 0; i < 4; i++)
                {
                    c[i] = results[i].Item1;
                    n[i] = results[i].Item2;
                    if (results[i].Item2 != zero)
                    {
                        isAllZeros = false;
                    }
                }
                if (isAllZeros) { return new(Complex.Zero, zero); }

                var t = new DDNode(list, layer, c, n);
                (var f, t) = t.NormalizeAndReduce();

                return new(f, t);
            }
        }


        /// <summary>
        /// Multiplies two DDNodes and records newly created nodes in the given list, so that a new DDMatrix
        /// can be constructed from the result.
        /// </summary>
        public static Tuple<Complex, DDNode> Multiply(DDNodeList list, Tuple<Complex, DDNode> first,
            Tuple<Complex, DDNode> second)
        {
            if (first.Item2 == zero || second.Item2 == zero)
            {
                return new(0, zero);
            }

            if (first.Item2 == one)
            {
                return new(second.Item1 * first.Item1, second.Item2);
            }
            else if (second.Item2 == one)
            {
                return new(first.Item1 * second.Item1, first.Item2);
            }

            Complex[] w = new Complex[4];
            DDNode[] n = new DDNode[4];

            (w[0], n[0]) = Add(list,
                Multiply(list, new(first.Item1 * first.Item2.weights[0], first.Item2.next[0]),
                                new(second.Item1 * second.Item2.weights[0], second.Item2.next[0])),
                Multiply(list, new(first.Item1 * first.Item2.weights[1], first.Item2.next[1]),
                                new(second.Item1 * second.Item2.weights[2], second.Item2.next[2])
            ));
            (w[1], n[1]) = Add(list,
                Multiply(list, new(first.Item1 * first.Item2.weights[0], first.Item2.next[0]),
                                new(second.Item1 * second.Item2.weights[1], second.Item2.next[1])),
                Multiply(list, new(first.Item1 * first.Item2.weights[1], first.Item2.next[1]),
                                new(second.Item1 * second.Item2.weights[3], second.Item2.next[3])
            ));
            (w[2], n[2]) = Add(list,
                Multiply(list, new(first.Item1 * first.Item2.weights[2], first.Item2.next[2]),
                                new(second.Item1 * second.Item2.weights[0], second.Item2.next[0])),
                Multiply(list, new(first.Item1 * first.Item2.weights[3], first.Item2.next[3]),
                                new(second.Item1 * second.Item2.weights[2], second.Item2.next[2])
            ));
            (w[3], n[3]) = Add(list,
                Multiply(list, new(first.Item1 * first.Item2.weights[2], first.Item2.next[2]),
                                new(second.Item1 * second.Item2.weights[1], second.Item2.next[1])),
                Multiply(list, new(first.Item1 * first.Item2.weights[3], first.Item2.next[3]),
                                new(second.Item1 * second.Item2.weights[3], second.Item2.next[3])
            ));
            for (int i = 0; i < 4; i++)
            {
                if (CompareComplex(w[i], 0) || n[i] == zero)
                {
                    w[i] = 0;
                    n[i] = zero;
                }
            }

            DDNode newNode = new(list, first.Item2.layer, w, n);
            (Complex c, newNode) = newNode.NormalizeAndReduce();

            return new(c, newNode);
        }

        /// <summary>
        /// Adds two DDNodes. A helper method for DDNode.Multiply()
        /// </summary>
        public static Tuple<Complex, DDNode> Add(DDNodeList list, Tuple<Complex, DDNode> first, Tuple<Complex, DDNode> second)
        // public for testing/debugging purposes
        {
            if (CompareComplex(first.Item1, Complex.Zero))
                first = new(Complex.Zero, zero);
            if (CompareComplex(second.Item1, Complex.Zero))
                second = new(Complex.Zero, zero);

            if (first.Item2 == zero)
            {
                if (second.Item2 == zero)
                {
                    return new(0, zero);
                }
                else
                {
                    return new(second.Item1, list.GetOrCreate(second.Item2.Copy(list)));
                }
            }
            else if (second.Item2 == zero)
            {
                return new(first.Item1, list.GetOrCreate(first.Item2.Copy(list)));
            }

            if (first.Item2 == one && second.Item2 == one)
            {
                return new(first.Item1 + second.Item1, one);
            }
            else if (first.Item2 == one || second.Item2 == one ||
                first.Item2.layer == 0 || second.Item2.layer == 0)
            {
                throw new Exception("InternalException: Layer 0 contains more than just the one node or " +
                "the one node is present on a layer that is not 0 ("
                + first.Item2.layer + ":" + second.Item2.layer + ")");
            }

            DDNode[] n = new DDNode[4];
            Complex[] w = new Complex[4];
            for (int i = 0; i < 4; i++)
            {
                (w[i], n[i]) = Add(list, new(first.Item1 * first.Item2.weights[i], first.Item2.next[i]),
                    new(second.Item1 * second.Item2.weights[i], second.Item2.next[i]));
                if (CompareComplex(w[i], 0) || n[i] == zero)
                {
                    w[i] = 0;
                    n[i] = zero;
                }
                //w[i] = w[i] * ((first.Item2.weights[i]*first.Item1) + (second.Item1 * second.Item2.weights[i])); // Todo: Remove this after testing
            }

            DDNode newNode = new(list, first.Item2.layer, w, n); // first.Item2 is not zero, so the layer is correct

            (Complex c, newNode) = newNode.NormalizeAndReduce();

            return new(c, newNode);
        }

        /// <summary>
        /// Multiplies a horizontal vector with a DDNode using matrix multiplication, 
        /// such that the DDNode is on the right.
        /// <br/><br/>
        /// x0 and x1 are helper variables for recursion and should be set to x0=0, x1=first.Length.
        /// </summary>
        public static Complex[] Multiply(Complex[] first, DDNode second, int x0, int x1)
        {
            if (second == zero)
            {
                return new Complex[x1 - x0];
            }
            Complex[] result;
            if (second.layer == 1)
            {
                result = new Complex[2];
                result[0] = first[x0] * second.weights[0] + first[x0 + 1] * second.weights[2];
                result[1] = first[x0] * second.weights[1] + first[x0 + 1] * second.weights[3];
                return result;
            }
            Complex[][] all =
            {
                Multiply(first, second.next[0], x0, (x0+x1)/2),
                Multiply(first, second.next[1], x0, (x0+x1)/2),
                Multiply(first, second.next[2], (x0+x1)/2, x1),
                Multiply(first, second.next[3], (x0+x1)/2, x1)
            };
            result = new Complex[x1 - x0];
            for (int i = 0; i < result.Length / 2; i++)
            {
                result[i] = second.weights[0] * all[0][i] + second.weights[2] * all[2][i];
            }
            for (int i = 0; i < result.Length / 2; i++)
            {
                result[i + result.Length / 2] = second.weights[1] * all[1][i] + second.weights[3] * all[3][i];
            }
            return result;
        }

        /// <summary>
        /// Multiplies a vertical vector with a DDNode using matrix multiplication, 
        /// such that the DDNode is on the left.
        /// <br/><br/>
        /// x0 and x1 are helper variables for recursion and should be set to x0=0, x1=second.Length.
        /// </summary>
        public static Complex[] Multiply(DDNode first, Complex[] second, int x0, int x1)
        {
            if (first == zero)
            {
                return new Complex[x1 - x0];
            }
            Complex[] result;
            if (first.layer == 1)
            {
                result = new Complex[2];
                result[0] = second[x0] * first.weights[0] + second[x0 + 1] * first.weights[1];
                result[1] = second[x0] * first.weights[2] + second[x0 + 1] * first.weights[3];
                return result;
            }
            Complex[][] all =
            {
                Multiply(first.next[0], second,  x0, (x0+x1)/2),
                Multiply(first.next[1], second, (x0+x1)/2, x1),
                Multiply(first.next[2], second, x0, (x0+x1)/2),
                Multiply(first.next[3], second, (x0+x1)/2, x1)
            };
            result = new Complex[x1 - x0];
            for (int i = 0; i < result.Length / 2; i++)
            {
                result[i] = first.weights[0] * all[0][i] + first.weights[1] * all[1][i];
            }
            for (int i = 0; i < result.Length / 2; i++)
            {
                result[i + result.Length / 2] = first.weights[2] * all[2][i] + first.weights[3] * all[3][i];
            }
            return result;
        }

        /// <summary>
        /// A helper method for DDMatrix.TensorProduct()
        /// </summary>
        public static DDNode TensorProduct(DDNode current, DDMatrix second, int layer)
        {
            if (current == null) { return zero; }
            if (current == one)
            { throw new Exception("Internal Error: This case should have been handled in DDMatrix.TensorProduct()."+
            " This could mean, that there are one nodes on layers != 0."); }
            if (current.layer == 1)
            {
                DDNode[] newNext = new DDNode[4];
                for (int i = 0; i<4;i++)
                {
                    if (current.next[i] == one)
                    {
                        newNext[i] = second.root;
                    } else
                    {
                        newNext[i] = zero;
                    }
                }
                return second.nodeList.GetOrCreate(layer, current.weights, newNext);
            } else
            {
                DDNode[] newNext = new DDNode[4];
                for (int i = 0; i<4;i++)
                {
                    if (current.next[i] != zero)
                    {
                        newNext[i] = TensorProduct(current.next[i], second, layer-1);
                    } else
                    {
                        newNext[i] = zero;
                    }
                }
                return second.nodeList.GetOrCreate(layer, current.weights, newNext);
            }
        }
    }
}