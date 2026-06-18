
using System.Numerics;
using System.Text;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using static Utility;

namespace ECfQC_Tests;

/// <summary>
/// Provides utility methods for testing
/// </summary>
public static class TestUtility
{
    // general testcases for complex matrices that are used by some tests
    // changing these will lead to other tests breaking, as they might have their own static lists
    // of test-specific results to compare this list with
    public static Matrix<Complex>[] baseCases =
    {
        new DenseMatrix(2), new DenseMatrix(4), new DenseMatrix(2), // zero matrices
        new DenseMatrix(2,2, [1,0,0,1]), //I_2
        new DenseMatrix(4, 4, [1,0,0,0,
        0,1,0,0,
        0,0,1,0,
        0,0,0,1]), //I_4
        new DenseMatrix(8, 8, [1,0,0,0,0,0,0,0,
        0,1,0,0,0,0,0,0,
        0,0,1,0,0,0,0,0,
        0,0,0,1,0,0,0,0,
        0,0,0,0,1,0,0,0,
        0,0,0,0,0,1,0,0,
        0,0,0,0,0,0,1,0,
        0,0,0,0,0,0,0,1,
        ]), //I_8
        // DenseMatrix assumes the array to be in column-major order, so we need to transpose to have
        // it in row-major order.
        new DenseMatrix(2,2, [1,-23423,0,new (0.341,-2)]).Transpose(),
        new DenseMatrix(2,2, [new(0,1),0,0.5,1]).Transpose(),
        new DenseMatrix(2,2, [1,new(0,1),0,1]).Transpose(),
        new DenseMatrix(2,2, [0,0,new(-4,3),0]).Transpose(),
        new DenseMatrix(2,2, [0,0,1,0]).Transpose(),
        new DenseMatrix(8, 8, [1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64])
        .Transpose(), //ascending values
        new DenseMatrix(8, 8, [0,0,0,0,5,6,7,8,
        0,0,0,0,0,0,0,0,
        0,0,0,0,21,22,23,24,
        3,3,3,3,29,30,31,32,
        33,34,35,36,37,38,39,40,
        41,0,43,44,45,46,47,48,
        49,50,51,52,53,54,55,56,
        57,58,59,60,61,62,63,64]).Transpose(), //ascending values with zeros
        new DenseMatrix(4,4, [1,0,1,0,
        1,0,1,0,
        1,0,1,0,
        1,2,3,0]).Transpose(),
        new DenseMatrix(4,4, [1,1,0,0,new (0,4),new (0,4),0,0,1,1,0,0,1,2,0,0]).Transpose(),
        new DenseMatrix(4,4, [2.5,1.5,new (0,5),new (0,3),
        0,0,0,0,
        1,1,0,0,
        1,2,0,0]).Transpose(),
        new DenseMatrix(8, 8, [0,0,0,0,0,0,0,0,
        0,0,0,0,0,6,0,0,
        0,0,0,0,0,0,1,0,
        0,0,0,0,0,0,0,0,
        0,0,0,0,0,0,0,0,
        0,6,0,0,0,-4,0,0,
        0,0,1,0,0,0,-2.0/3.0,0,
        0,0,0,0,0,0,0,0,
        ]).Transpose(),
        new DenseMatrix(8, 8, [1,1,1,1,0,0,0,0,
        1,1,1,1,0,6,0,0,
        1,1,1,1,0,0,0,0,
        1,1,1,1,1,1,1,1,
        0,0,Complex.ImaginaryOne,0,0,0,-2*Complex.ImaginaryOne/3.0,0,
        0,6,0,0,0,-4,0,0,
        0,0,1,0,0,0,-2.0/3.0,0,
        0,0,0,0,0,0,0,0,
        ]).Transpose(),
        new DenseMatrix(8, 8,
        [0,0,0,0,0,3.5,0,0,
        1,1,1,1,0,3,0,0,
        1,1,1,1,0,6,0,0,
        1,1,1,1,1,1,1,1,
        0,7,0,1,0,0,0,0,
        0,6,0,0,1,0,-2,0,
        0,0,1,0,0,1,0,0,
        0,0,0,0,0,0,-2,0,
        ]).Transpose(),
        new DenseMatrix(4,4, [2.5,1.5,new (0,-2.24522),new (0,5),
        new (0,5),new (0,3),0,0,
        1,1,0,0,
        1,2,new (0,-2.24522),new (0,5)]),
        new DenseMatrix(4,4,
        [5,5,-2,-2,
        5,5,-2,-2,
        -5,-5,0,0,
        -5,-5,0,0]).Transpose(),

    };

    public static DDMatrix[] ConstructAll(Matrix<Complex>[] list)
    {
        DDMatrix[] t = new DDMatrix[list.Length];
        for (int i = 0; i < t.Length; i++)
        {
            t[i] = Construct(list[i]);
        }
        return t;
    }

    public static DDMatrix Construct(Matrix<Complex> matrix)
    {
        return new DDMatrix(matrix.ToArray());
    }

    public static Complex[,] ToMatrix(DDMatrix matrix)
    {
        TestContext.Out.WriteLine($"qubits = {matrix.qubits}");

        int size = (int)Math.Pow(2, matrix.qubits);

        TestContext.Out.WriteLine($"size = {size}");
        TestContext.Out.Flush();
        Complex[,] list = new Complex[size, size];

        ToMatrixHelper(list, matrix.root, matrix.rootWeight, 0, size, 0, size);
        return list;
    }

    private static void ToMatrixHelper(Complex[,] list, DDMatrix.DDNode node, Complex multiplier, int x0, int x1, int y0, int y1)
    {
        if (node == DDMatrix.DDNode.zero)
        {
            return;
        }
        if (node == DDMatrix.DDNode.one)
        {
            if (x0 + 1 != x1)
            {
                Assert.Fail("x0: " + x0 + ", x1: " + x1);
            }
            if (y0 + 1 != y1)
            {
                Assert.Fail("y0: " + y0 + ", y1: " + y1);
            }
            list[x0, y0] = multiplier;
            return;
        }
        int halfx = (x1 + x0) / 2;
        int halfy = (y1 + y0) / 2;
        ToMatrixHelper(list, node.next[0], multiplier * node.weights[0], x0, halfx, y0, halfy);
        ToMatrixHelper(list, node.next[1], multiplier * node.weights[1], x0, halfx, halfy, y1);
        ToMatrixHelper(list, node.next[2], multiplier * node.weights[2], halfx, x1, y0, halfy);
        ToMatrixHelper(list, node.next[3], multiplier * node.weights[3], halfx, x1, halfy, y1);
    }

    public static void Print(Complex[,] matrix)
    {
        StringBuilder s = new();
        s.Append('{');
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            s.Append('{');
            for (int k = 0; k < matrix.GetLength(1); k++)
            {
                s.Append(matrix[i, k]).Append(", ");
            }
            s.Append('}');
            s.Append('\n');
        }
        s.Append('}');
        Console.WriteLine(s);
    }

    public static string ToString(Complex[,] m1, Matrix<Complex> m2, string identifier)
    {
        StringBuilder s = new (identifier+"actual: [\n");
        for (int x = 0; x < m1.GetLength(0); x++)
        {
            s.Append('[');
            for (int y = 0; y < m1.GetLength(1); y++)
            {
                s.Append(m1[x, y]+", ");
            }
            s.Append("]\n");
        }
        s.Append("]\n\n");
        s.Append("expected : [\n");
        for (int x = 0; x < m1.GetLength(0); x++)
        {
            s.Append('[');
            for (int y = 0; y < m1.GetLength(1); y++)
            {
                s.Append(m2[x, y]+", ");
            }
            s.Append("]\n");
        }
        s.Append("]\n\n");
        return s.ToString();
    }

    public static string ToString(Complex[][] m1, Complex[][] m2, string identifier)
    {
        StringBuilder s = new (identifier+"actual: [\n");
        for (int x = 0; x < m1.Length; x++)
        {
            s.Append('[');
            for (int y = 0; y < m1[x].Length; y++)
            {
                s.Append(m1[x] [y]+", ");
            }
            s.Append("]\n");
        }
        s.Append("]\n\n");
        s.Append("expected : [\n");
        for (int x = 0; x < m2.Length; x++)
        {
            s.Append('[');
            for (int y = 0; y < m1[x].Length; y++)
            {
                s.Append(m2[x] [y]+", ");
            }
            s.Append("]\n");
        }
        s.Append("]\n\n");
        return s.ToString();
    }

    public static void AssertEquals(DDMatrix[] first, Matrix<Complex>[] second)
    {
        for (int i = 0; i < second.Length; i++)
        {
            AssertEquals(first[i], second[i], i + "");
        }
    }

    public static void AssertEquals(DDMatrix first, Matrix<Complex> second, string identifier = "")
    {
        var m1 = ToMatrix(first);
        if (m1.GetLength(0) != second.RowCount || m1.GetLength(1) != second.ColumnCount)
        {
            Assert.Fail(identifier + ": m1.L(0): " + m1.GetLength(0) + ", s.RC: " + second.RowCount +
            ", m1.L(1): " + m1.GetLength(1) + ", s.CC: " + second.ColumnCount);
        }

        string s = ToString(m1, second, identifier);

        for (int x = 0; x < m1.GetLength(0); x++)
        {
            for (int y = 0; y < m1.GetLength(1); y++)
            {
                Assert.That(CompareComplex(m1[x, y], second[x, y]), s);
            }
        }
    }

    public static void AssertEquals(DDMatrix[] first, DDMatrix[] second)
    {
        for (int i = 0; i < second.Length; i++)
        {
            AssertEquals(first[i], second[i], i + "");
        }
    }

    public static void AssertEquals(DDMatrix first, DDMatrix second, string identifier = "")
    {
        var m1 = ToMatrix(first);
        var m2 = ToMatrix(second);
        if (m1.GetLength(0) != m2.GetLength(0) || m1.GetLength(1) != m2.GetLength(1))
        {
            Assert.Fail(identifier + ": m1.L(0): " + m1.GetLength(0) + ", m2.L(0): " + m2.GetLength(0) +
            ", m1.L(1): " + m1.GetLength(1) + ", m2.L(1): " + m2.GetLength(1));
        }
        for (int x = 0; x < m1.GetLength(0); x++)
        {
            for (int y = 0; y < m1.GetLength(1); y++)
            {
                if (!Utility.CompareComplex(m1[x, y], m2[x, y]))
                {
                    Assert.Fail(identifier + ": x: " + x + ", y: " + y);
                }
            }
        }
    }

    public static void AssertEquals(Complex[][] first, Complex[][] second)
    {
        for (int i = 0; i < second.Length; i++)
        {
            for (int k = 0; k < second[i].Length; k++)
            {
                Assert.That(CompareComplex(first[i][k], second[i][k]), 
                ToString(first, second, i+"")
                );
            }
        }
    }

    public static void AssertInvariants(DDMatrix[] matrix)
    {
        for (int i = 0; i < matrix.Length; i++)
        {
            AssertInvariants(matrix[i], i + "");
        }
    }

    /// <summary>
    /// Checks the following invariants: <br/>
    /// <br/>(1) All nodes except for the zero and one nodes of matrix are in matrix.nodeList
    /// <br/>(2) The amount of unique nodes except for the zero and one nodes is the same as matrix.nodeList.Count()
    /// <br/>(3) For all nodes n, n.layer is correct
    /// <br/>(4) All nodes in matrix.nodeList are normalized
    /// <br/>(5) No node is equivalent to any other node on the same layer (i.e. has the same values)
    /// <br/>(6) If one of the weights of a node is 0, the corresponding next DDNode is DDNode.zero
    /// <br/>
    /// The goal is to determine, whether the nodes are correctly reduced.
    /// </summary>
    public static void AssertInvariants(DDMatrix matrix, string identifier = "")
    {
        Dictionary<int, HashSet<DDMatrix.DDNode>> list = new();
        // (3)
        GetAllNodes(list, matrix.root, matrix.qubits, identifier);
        // (2)
        int count = 0;
        foreach (var (_, b) in list)
        {
            count += b.Count;
        }
        Assert.That(count, Is.EqualTo(matrix.nodeList.Count()), identifier);
        var nodeList = matrix.nodeList.GetData();
        int count2 = 0;
        foreach (var (_, b) in nodeList)
        {
            count2 += b.Count;
        }
        // (2)
        Assert.That(count, Is.EqualTo(count2), identifier);
        // (1)
        foreach (var (i, l) in list)
        {
            foreach (var n in l)
            {
                Assert.That(nodeList[i], Does.Contain(n), identifier);
            }
        }

        // (4), (6)
        foreach (var (i, l) in list)
        {
            foreach (var n in l)
            {
                Complex firstNonZeroValue = 0;
                for (int k = 0; k < 4; k++)
                {
                    // (6)
                    if (CompareComplex(n.weights[k], 0) || n.next[k] == DDMatrix.DDNode.zero)
                    {
                        Assert.Multiple(() =>
                        {
                            Assert.That(CompareComplex(n.weights[k], 0), identifier);
                            Assert.That(n.next[k], Is.EqualTo(DDMatrix.DDNode.zero), identifier);
                        });
                        continue;
                    }
                    // (4)
                    if (CompareComplex(firstNonZeroValue, 0))
                    {
                        firstNonZeroValue = n.weights[k];
                        Assert.That(CompareComplex(firstNonZeroValue, 1), identifier);
                    }
                }
            }
        }

        // (5)
        foreach (var i in list.Keys)
        {
            foreach (var n1 in list[i])
            {
                foreach (var n2 in nodeList[i])
                {
                    if (n1 != n2)
                    {
                        if (CompareComplex(n1.weights[0], n2.weights[0]) &&
                        CompareComplex(n1.weights[1], n2.weights[1]) &&
                        CompareComplex(n1.weights[2], n2.weights[2]) &&
                        CompareComplex(n1.weights[3], n2.weights[3]) &&
                        n1.next[0] == n2.next[0] &&
                        n1.next[1] == n2.next[1] &&
                        n1.next[2] == n2.next[2] &&
                        n1.next[3] == n2.next[3]
                        )
                        {
                            Assert.Fail(identifier + ": Both nodes are the equivalent, but not reduced.");
                        }
                    }
                }
            }
        }

    }

    /// <summary>
    /// Helper function for AssertInvariants
    /// </summary>
    private static void GetAllNodes(Dictionary<int, HashSet<DDMatrix.DDNode>> list,
    DDMatrix.DDNode node, int layer, string identifier = "")
    {
        if (node == DDMatrix.DDNode.zero || node == DDMatrix.DDNode.one)
        {
            return;
        }
        Assert.That(layer, Is.GreaterThan(0), identifier); // (3)
        Assert.That(layer, Is.EqualTo(node.layer), identifier); // (3)
        if (!list.ContainsKey(layer))
        {
            list[layer] = [node];
        }
        else
        {
            list[layer].Add(node);
        }
        foreach (var n in node.next)
        {
            if (n == DDMatrix.DDNode.one)
            {
                Assert.That(node.layer, Is.EqualTo(1));
            }
            GetAllNodes(list, n, layer - 1, identifier);
        }
    }


}