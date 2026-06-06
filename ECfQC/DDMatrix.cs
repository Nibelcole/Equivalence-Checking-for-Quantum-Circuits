using System.ComponentModel.Design.Serialization;
using System.IO.Pipelines;
using System.Numerics;
using static Utility;

/// <summary>
/// A class implementing a 2^q x 2^q matrix as a decision diagram to represent a quantum gate.
/// </summary>
public partial class DDMatrix
{
    public static readonly Dictionary<string, Tuple<DDMatrix, int>> gates = new() {
        {"id", new(Identity(1), 0)},
        {"pX", new(new DDMatrix(new Complex[,]
            {
                {0, 1},
                {1,0}
            }
            ), 1)
        },
        {"pY", new(new DDMatrix(new Complex[,]
            {
                {0, -Complex.ImaginaryOne},
                {Complex.ImaginaryOne,0}
            }
            ), 1)
        },
        {"pZ", new(new DDMatrix(new Complex[,]
            {
                {1, 0},
                {0,-1}
            }
            ), 1)
        },
        {"h", new(new DDMatrix(new Complex[,]
            {
                {1/Math.Sqrt(2), 1/Math.Sqrt(2)},
                {1/Math.Sqrt(2),-1/Math.Sqrt(2)}
            }
            ), 1)
        },
        {"cnot", new(new DDMatrix(new Complex[,]
            {
                {1,0,0,0},
                {0,1,0,0},
                {0,0,0,1},
                {0,0,1,0}
            }
            ), 2)
        },
        {"sw", new(new DDMatrix(new Complex[,]
            {
                {1,0,0,0},
                {0,0,1,0},
                {0,1,0,0},
                {0,0,0,1}
            }
            ), 2)
        },
        {"t", new(new DDMatrix(new Complex[,]
            {
                {1,0,0,0,0,0,0,0},
                {0,1,0,0,0,0,0,0},
                {0,0,1,0,0,0,0,0},
                {0,0,0,1,0,0,0,0},
                {0,0,0,0,1,0,0,0},
                {0,0,0,0,0,1,0,0},
                {0,0,0,0,0,0,0,1},
                {0,0,0,0,0,0,1,0},
            }
            ), 3)
        },
    }; // the int shows the number of qubits this gate is applied to
    
    public readonly int qubits;

    public Complex rootWeight;
    public readonly DDNode root;

    public readonly DDNodeList nodeList = new DDNodeList();

    public DDMatrix(DDNodeList nodeList, Complex rootWeight, DDNode root)
    {
        this.nodeList = nodeList;
        this.rootWeight = rootWeight;
        this.root = root;
    }

    public DDMatrix(Complex[,] matrix) // it is assumed that matrix is quadratic
    {
        this.qubits = (int)Math.Log2(matrix.GetLength(0));
        this.nodeList = new();
        (rootWeight, root) = DDNode.Construct(nodeList, matrix, 0, matrix.GetLength(0), 
            0, matrix.GetLength(1), qubits);
    }

    /// <summary>
    /// Creates a deep copy of the given DDMatrix.
    /// </summary>
    public DDMatrix Copy()
    {
        DDNodeList list = new();
        return new DDMatrix(list, rootWeight, root.Copy(list));
    }

    /// <summary>
    /// Returns an identity matrix of size 2^qubits x 2^qubits
    /// </summary>
    public static DDMatrix Identity(int qubits)
    {
        DDNodeList list = new();
        return new DDMatrix(list, 1, DDNode.Identity(list, qubits));
    }

    /// <summary>
    /// Returns 1 if this matrix is exactly equal to an identity matrix.<br/>
    /// Returns 2 if this DDMatrix differs at most by a global phase vector 
    /// e^(i*alpha) from an identity matrix without being fully equal.<br/>
    /// Returns 0 otherwise.
    /// </summary>
    public int EqualsIdentity()
    {
        bool correctStructure = root.EqualsIdentity();
        if (!correctStructure)
        {
            return 0;
        }
        if (CompareComplex(rootWeight, Complex.One))
        {
            return 1;
        }

        // according to the following equation e^(i*alpha)
        // == cos(alpha) + i*sin(alpha) 
        // == Re(e^(i*alpha))+ Im(e^(i*alpha))
        // with alpha = [0, 2*pi)
        // ==> there is an alpha with e^(i*alpha) == global phase
        // if and only if arccos(Re(e^(i*alpha))) == arcsin(Im(e^(i*alpha)))
        double re = Math.Acos(rootWeight.Real);
        double im = Math.Asin(rootWeight.Imaginary);
        if (double.IsNaN(re) || double.IsNaN(im))
        {
            return 0;
        }
        // re and im need to be equal
        // We "misuse" CompareComplex here since it provides the same functionality of comparing 
        // double values with a delta, so we wont have to implement a separate CompareDouble function.
        if (CompareComplex(new(re, im), new(im, re)))
        {
            return 2;
        }
        else
        {
            return 0;
        }
    }

    /// <summary>
    /// Returns the amount of DDNodes excluding one and zero nodes.
    /// </summary>
    public int Count()
    {
        return nodeList.Count();
    }

    /// <summary>
    /// Changes this DDMatrix, so it becomes the conjugate-transposed of itself.
    /// </summary>
    public void ConjugateTransposed()
    {
        foreach (var node in nodeList.GetAll())
        {
            for (int i = 0; i<4;i++)
            {
                node.weights[i] = Complex.Conjugate(node.weights[i]);
            }
            (node.weights[1], node.weights[2]) = (node.weights[2], node.weights[1]);
            (node.next[1], node.next[2]) = (node.next[2], node.next[1]);
        }
        rootWeight = Complex.Conjugate(rootWeight);
    }

    /// <summary>
    /// Returns the matrix multiplication of the two given values. first and second are not consumed by
    /// this operation.
    /// </summary>
    public static DDMatrix Multiply(DDMatrix first, DDMatrix second)
    {
        DDNodeList list = new();
        var (a,b) = DDNode.Multiply(list, first.root,second.root);
        return new DDMatrix(list, a*first.rootWeight*second.rootWeight, b);
    }

    /// <summary>
    /// Returns the matrix multiplication of the two given values. first and second are not consumed by
    /// this operation.
    /// </summary>
    public static Complex[] Multiply(Complex[] first, DDMatrix second)
    {
        var result = DDNode.Multiply(first, second.root,0,first.Length);
        for (int i = 0;i<result.Length;i++)
        {
            result[i] = result[i]*second.rootWeight;
        }
        return result;
    }

    /// <summary>
    /// Returns the matrix multiplication of the two given values. first and second are not consumed by
    /// this operation.
    /// </summary>
    public static Complex[] Multiply(DDMatrix first, Complex[] second)
    {
        var result = DDNode.Multiply(first.root, second,0,second.Length);
        for (int i = 0;i<result.Length;i++)
        {
            result[i] = result[i]*first.rootWeight;
        }
        return result;
    }

    /// <summary>
    /// Extends a DDMatrix representing a gate with tensor multiplication. a, b and c are the targeted bits.
    /// 
    /// <br/><br/>
    /// If a is not given, the identity matrix of the correct size will be returned instead.
    /// m is consumed by this operation. Do not use it afterwards.
    /// </summary>
    public static DDMatrix Extend(DDMatrix m, int qubits, int a = -1, int b = -1, int c = -1)
    {
        if (a == -1) // zero arguments
        {
            return Identity(qubits);
        }
        // one argument or (two arguments and no permutation is required) or (three arguments and no permutation is required)
        if (b == -1 || a == b - 1 || (a == b - 1 && b == c - 1))
        {
            DDMatrix result;
            int i = 1;
            if (a == 0)
            {
                result = m;
                if (b != -1)
                {
                    if (c != -1)
                    {
                        i++;
                    }
                }
            }
            else
            {
                result = Identity(qubits);
            }

            for (; i < qubits; i++)
            {
                DDMatrix next;
                if (i == a)
                {
                    next = m;
                    if (b != -1)
                    {
                        if (c != -1)
                        {
                            i++;
                        }
                    }
                }
                else
                {
                    next = Identity(qubits);
                }
                result = TensorProduct(result, next);
            }
            return result;
        }

        List<DDMatrix> swapGates = new();
        if (c == -1) // two arguments and permutation is required
        {
            if (a < b) // move b to a
            {
                while (a != b - 1)
                {
                    swapGates.Add(Extend(gates["swap"].Item1.Copy(), qubits, b - 1, b));
                    b--;
                }
            }
            else // move a to b
            {
                while (a != b - 1)
                {
                    swapGates.Add(Extend(gates["swap"].Item1.Copy(), qubits, a-1, a));
                    a--;
                }
                b++;
            }
        }
        else // three arguments and permutation is required
        {

            if (a < b) // move b to a+1
            {
                if (c > a && c < b)
                {
                    c++;
                }
                while (a != b - 1)
                {
                    swapGates.Add(Extend(gates["swap"].Item1.Copy(), qubits, b - 1, b));
                    b--;
                }
            }
            else // move a to b-1
            {
                if (c < a && c > b)
                {
                    c++;
                }
                while (a != b - 1)
                {
                    swapGates.Add(Extend(gates["swap"].Item1.Copy(), qubits, a - 1, a));
                    a--;
                }
                b++;
            }

            if (c<a) // move c to a+2 from below
            {
                while (c != b +1)
                {
                    swapGates.Add(Extend(gates["swap"].Item1.Copy(), qubits, c, c+1));
                    c++;
                }
                a--;
                b--;
            } else // move c to a+2 from above
            {
                while (c != b +1)
                {
                    swapGates.Add(Extend(gates["swap"].Item1.Copy(), qubits, c-1, c));
                    c--;
                }
            }

        }
        DDMatrix modifiedM = Extend(m, qubits, a, b);
        foreach (var s in swapGates)
        {
            modifiedM = Multiply(s, modifiedM);
            s.ConjugateTransposed();
        }
        foreach (var s in swapGates)
        {
            modifiedM = Multiply(modifiedM, s);
        }
        return modifiedM;
    }

    /// <summary>
    /// Returns the tensor product of two matrices. 
    /// 
    /// <br/><br/>
    /// The two DDMatrices are consumed in the process. 
    /// Do not use them afterwards.
    /// 
    /// <br/><br/>
    /// Note: We do not allow multiple DDMatrices to use the same DDNodes, which is why second is also 
    /// seen as "consumed", despite the fact that it itself is not changed.
    /// </summary>
    public static DDMatrix TensorProduct(DDMatrix first, DDMatrix second)
    {
        Complex w = first.rootWeight * second.rootWeight;
        var all = first.nodeList.GetAll();
        foreach (var n in all)
        {
            for (int i = 0; i < 4; i++)
            {
                if (n.next[i] == DDNode.one)
                {
                    n.next[i] = second.root;
                }
            }
        }
        var all2 = second.nodeList.GetAll();
        foreach (var n in all2)
        {
            first.nodeList.GetOrCreate(n); // if a new node is created, the new node will have the first.nodeList
                        // in its own variable nodeList (instead of second.nodeList)
        }
        return new DDMatrix(first.nodeList, w, first.root);
    }

}