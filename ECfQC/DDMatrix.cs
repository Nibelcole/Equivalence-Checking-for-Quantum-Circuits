using System.ComponentModel.Design.Serialization;
using System.Numerics;
using static Utility;

/// <summary>
/// A class implementing a 2^q x 2^q matrix as a decision diagram to represent a quantum gate.
/// </summary>
public partial class DDMatrix
{
    public static readonly Dictionary<string, Tuple<DDMatrix, int>> gates = new() {
        // We use the same conventions/matrices as Qiskit to make testing easier
        {"id", new(Identity(1), 0)}, // Identity
        {"pX", new(new DDMatrix(new Complex[,] // Pauli-X
            {
                {0, 1},
                {1,0}
            }
            ), 1)
        },
        {"pY", new(new DDMatrix(new Complex[,] // Pauli-Y
            {
                {0, -Complex.ImaginaryOne},
                {Complex.ImaginaryOne,0}
            }
            ), 1)
        },
        {"pZ", new(new DDMatrix(new Complex[,] // Pauli-Z
            {
                {1, 0},
                {0,-1}
            }
            ), 1)
        },
        {"h", new(new DDMatrix(new Complex[,] // Hadamard
            {
                {1/Math.Sqrt(2), 1/Math.Sqrt(2)},
                {1/Math.Sqrt(2),-1/Math.Sqrt(2)}
            }
            ), 1)
        },
        {"s", new(new DDMatrix(new Complex[,] // S gate
            {
                {1, 0},
                {0,new(0,1)}
            }
            ), 1)
        },
        {"t", new(new DDMatrix(new Complex[,] // T gate
            {
                {1, 0},
                {0,Complex.Exp(Complex.ImaginaryOne * Math.PI / 4.0)}
            }
            ), 1)
        },
        {"cnot", new(new DDMatrix(new Complex[,] // Controlled-Not
            {
                {1,0,0,0},
                {0,0,0,1},
                {0,0,1,0},
                {0,1,0,0}
            }
            ), 2)
        },
        {"sw", new(new DDMatrix(new Complex[,] // Swap
            {
                {1,0,0,0},
                {0,0,1,0},
                {0,1,0,0},
                {0,0,0,1}
            }
            ), 2)
        },
        {"tof", new(new DDMatrix(new Complex[,] // Toffoli
            {
                {1,0,0,0,0,0,0,0},
                {0,1,0,0,0,0,0,0},
                {0,0,1,0,0,0,0,0},
                {0,0,0,0,0,0,0,1},
                {0,0,0,0,1,0,0,0},
                {0,0,0,0,0,1,0,0},
                {0,0,0,0,0,0,1,0},
                {0,0,0,1,0,0,0,0},
            }
            ), 3)
        },
    }; // the int shows the number of qubits this gate is applied to

    public readonly int qubits;

    public Complex rootWeight;
    public readonly DDNode root;

    public readonly DDNodeList nodeList = new DDNodeList();

    public DDMatrix(DDNodeList nodeList, Complex rootWeight, DDNode root, int qubits)
    {
        this.nodeList = nodeList;
        this.rootWeight = rootWeight;
        this.root = root;
        this.qubits = qubits;
    }

    public DDMatrix(Complex[,] matrix)
    {
        if (matrix.GetLength(0) != matrix.GetLength(1))
        {
            throw new Exception("Internal Error: Matrix is not square.");
        }
        if ((matrix.GetLength(0) & (matrix.GetLength(0) - 1)) != 0)
        {
            throw new Exception("Internal Error: Matrix dimension is not a power of two.");
        }

        this.qubits = (int)Math.Log2(matrix.GetLength(0));
        this.nodeList = new();
        (rootWeight, root) = DDNode.Construct(nodeList, matrix, 0, matrix.GetLength(0),
            0, matrix.GetLength(1), qubits);
        nodeList = root.RecreateNodeList(new());
    }

    /// <summary>
    /// Creates a deep copy of the given DDMatrix.
    /// </summary>
    public DDMatrix Copy()
    {
        DDNodeList list = new();
        return new DDMatrix(list, rootWeight, root.Copy(list), qubits);
    }

    /// <summary>
    /// Returns an identity matrix of size 2^qubits x 2^qubits
    /// </summary>
    public static DDMatrix Identity(int qubits)
    {
        DDNodeList list = new();
        return new DDMatrix(list, 1, DDNode.Identity(list, qubits), qubits);
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

        if (CompareComplex(Complex.Abs(rootWeight), Complex.One))
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
    public static DDMatrix ConjugateTransposed(DDMatrix matrix)
    {
        matrix = matrix.Copy();
        Dictionary<DDNode, Complex> renormalizedNodes = new();
        matrix.rootWeight *= matrix.root.Transposed(renormalizedNodes);

        foreach (DDNode d in renormalizedNodes.Keys)
        {
            for (int i = 0; i < 4; i++)
            {
                d.weights[i] = Complex.Conjugate(d.weights[i]);
            }
        }

        matrix.rootWeight = Complex.Conjugate(matrix.rootWeight);
        return matrix;
    }

    /// <summary>
    /// Returns the matrix multiplication of the two given values. first and second are not consumed by
    /// this operation.
    /// </summary>
    public static DDMatrix Multiply(DDMatrix first, DDMatrix second)
    {
        if (first.qubits != second.qubits) // for debugging purposes
        {
            throw new Exception("Internal Error: The given DDMatrices are not of" +
            " equal size: first: " + first.qubits + ", second: " + second.qubits);
        }
        DDNodeList list = new();
        var (a, b) = DDNode.Multiply(list, new(first.rootWeight, first.root), new(second.rootWeight, second.root));
        // This prevents intermediate results from being in the nodeList, despite not being in the QDD itself.
        return new DDMatrix(b.RecreateNodeList(new()), a, b, first.qubits);
    }

    /// <summary>
    /// Returns the matrix multiplication of the two given values. first and second are not consumed by
    /// this operation.
    /// </summary>
    public static Complex[] Multiply(Complex[] first, DDMatrix second)
    {
        if (((int)Math.Log2(first.Length)) != second.qubits) // for debugging purposes
        {
            throw new Exception("Internal Error: The given values are not of" +
            " equal size: first: " + Math.Log2(first.Length) + ", second: " + second.qubits);
        }
        var result = DDNode.Multiply(first, second.root, 0, first.Length);
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = result[i] * second.rootWeight;
        }
        return result;
    }

    /// <summary>
    /// Returns the matrix multiplication of the two given values. first and second are not consumed by
    /// this operation.
    /// </summary>
    public static Complex[] Multiply(DDMatrix first, Complex[] second)
    {
        if (first.qubits != ((int)Math.Log2(second.Length))) // for debugging purposes
        {
            throw new Exception("Internal Error: The given values are not of" +
            " equal size: first: " + first.qubits + ", second: " + Math.Log2(second.Length));
        }
        var result = DDNode.Multiply(first.root, second, 0, second.Length);
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = result[i] * first.rootWeight;
        }
        return result;
    }

    /// <summary>
    /// Extends a DDMatrix representing a gate with tensor multiplication. a, b and c are the targeted bits.
    /// 
    /// <br/><br/>
    /// If a is not given, the identity matrix of the correct size will be returned instead.
    /// </summary>
    public static DDMatrix Extend(DDMatrix m, int qubits, int a = -1, int b = -1, int c = -1)
    {
        if ((a == b && a != -1) || (b == c && b != -1) || (a == c && a != -1))
        {
            throw new Exception("Internal Error: two equal arguments were given to Extend(), which should " +
            "have been caught by the Parser.");
        }
        if (a == -1) // zero arguments
        {
            return Identity(qubits);
        }
        // one argument or (two arguments and no permutation is required) or (three arguments and no permutation is required)
        if (b == -1 || (a == b - 1 && c == -1) || (a == b - 1 && b == c - 1))
        {
            DDMatrix result;
            int i = 1;
            if (a == 0)
            {
                result = m;
                if (b != -1)
                {
                    i++;
                    if (c != -1)
                    {
                        i++;
                    }
                }
            }
            else
            {
                result = Identity(1);
            }

            for (; i < qubits; i++)
            {
                DDMatrix next;
                if (i == a)
                {
                    next = m;
                    if (b != -1)
                    {
                        i++;
                        if (c != -1)
                        {
                            i++;
                        }
                    }
                }
                else
                {
                    next = Identity(1);
                }
                result = TensorProduct(next, result);
            }
            return result;
        }

        List<int> targets = [a, b];
        if (b == -1) {throw new Exception("b is -1");}
        
        if (c != -1)
            targets.Add(c);

        if (targets.Count != m.qubits)
        {
            throw new Exception($"Internal Error: The given number of arguments {targets.Count} doesn't "+
            $" match the matrix ({m.qubits}), " +
            "meaning something is wrong with DDMatrix.gates or the parser.");
        }

        int[] position = Enumerable.Range(0, qubits).ToArray();
        int min = targets.Min();

        List<DDMatrix> swapGates = new();

        for (int i = 0; i < targets.Count; i++)
        {
            int desiredPosition = min + i;

            int current = Array.IndexOf(position, targets[i]);

            while (current > desiredPosition)
            {
                swapGates.Add(Extend(gates["sw"].Item1.Copy(), qubits, current - 1, current));

                (position[current - 1], position[current]) = (position[current], position[current - 1]);
                current--;
            }

            while (current < desiredPosition)
            {
                swapGates.Add(Extend(gates["sw"].Item1.Copy(), qubits, current - 1, current));

                (position[current], position[current + 1]) = (position[current + 1], position[current]);
                current++;
            }
        }

        DDMatrix modifiedM = null!;

        for (int q = 0; q < qubits; q++)
        {
            DDMatrix next;

            if (q == min)
            {
                next = m;
                q += m.qubits - 1;
            }
            else
            {
                next = Identity(1);
            }

            if (modifiedM == null)
            {
                modifiedM = next;
            } else {
                modifiedM = TensorProduct(next, modifiedM);
            }
        }

        for (int i = swapGates.Count - 1; i >= 0; i--)
        {
            // Since the inverse of a swap gate is itself, ConjugateTransposed() isn't needed
            modifiedM = Multiply(swapGates[i], modifiedM);
            modifiedM = Multiply(modifiedM, swapGates[i]);
        }

        return modifiedM;
    }

    /// <summary>
    /// Returns the tensor product of two matrices. This does not consume the inputs.
    /// </summary>
    public static DDMatrix TensorProduct(DDMatrix first, DDMatrix second)
    {
        Complex w = first.rootWeight * second.rootWeight;
        if (first.root == DDNode.zero || second.root == DDNode.zero || CompareComplex(w, 0))
        {
            return new DDMatrix(new(), 0, DDNode.zero, first.qubits + second.qubits);
        }
        else if (first.root == DDNode.one)
        {
            var t = second.Copy();
            t.rootWeight = w;
            return t;
        }
        else if (second.root == DDNode.one)
        {
            var t = first.Copy();
            t.rootWeight = w;
            return t;
        }

        DDMatrix secondCopy = second.Copy();
        DDNode newRoot = DDNode.TensorProduct(first.root, secondCopy, first.qubits + secondCopy.qubits);
        return new DDMatrix(secondCopy.nodeList, w, newRoot, first.qubits + secondCopy.qubits);
    }

}