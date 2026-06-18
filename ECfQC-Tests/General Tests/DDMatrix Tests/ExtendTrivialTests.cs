using System.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using static ECfQC_Tests.TestUtility;

namespace ECfQC_Tests;


public class ExtendTrivialTests
{
    DDMatrix[] actual;
    Matrix<Complex>[] expected;

    [SetUp]
    public void Setup()
    {
        expected = [
            // identity
            CreateMatrix.DiagonalIdentity<Complex>(2),
            CreateMatrix.DiagonalIdentity<Complex>(32),

            // trivial cases
            Matrix.Build.DenseOfArray(new Complex[,] // Pauli-X
            {
                {0, 1},
                {1,0}
            }),
            Matrix.Build.DenseOfArray(new Complex[,] // Pauli-Y
            {
                {0, -Complex.ImaginaryOne},
                {Complex.ImaginaryOne,0}
            }
            ),Matrix.Build.DenseOfArray(new Complex[,] // Pauli-Z
            {
                {1, 0},
                {0,-1}
            }
            ),Matrix.Build.DenseOfArray(new Complex[,] // Hadamard
            {
                {1/Math.Sqrt(2), 1/Math.Sqrt(2)},
                {1/Math.Sqrt(2),-1/Math.Sqrt(2)}
            }
            ),Matrix.Build.DenseOfArray(new Complex[,] // S gate
            {
                {1, 0},
                {0,new(0,1)}
            }
            ),Matrix.Build.DenseOfArray(new Complex[,] // T gate
            {
                {1, 0},
                {0,Complex.Pow(Math.E, new Complex(0,1)*new Complex(Math.PI/4.0,0))}
            }
            ),Matrix.Build.DenseOfArray(new Complex[,] // Controlled-Not
            {
                {1,0,0,0},
                {0,0,0,1},
                {0,0,1,0},
                {0,1,0,0}
            }
            ),Matrix.Build.DenseOfArray(new Complex[,] // Swap
            {
                {1,0,0,0},
                {0,0,1,0},
                {0,1,0,0},
                {0,0,0,1}
            }
            ),Matrix.Build.DenseOfArray(new Complex[,] // Toffoli
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
            )
        ];
        actual = [
            // identity
            DDMatrix.Extend(DDMatrix.gates["id"].Item1.Copy(), 1),
            DDMatrix.Extend(DDMatrix.gates["id"].Item1.Copy(), 5),
            // trivial cases
            DDMatrix.Extend(DDMatrix.gates["pX"].Item1.Copy(), 1, 0),
            DDMatrix.Extend(DDMatrix.gates["pY"].Item1.Copy(), 1, 0),
            DDMatrix.Extend(DDMatrix.gates["pZ"].Item1.Copy(), 1, 0),
            DDMatrix.Extend(DDMatrix.gates["h"].Item1.Copy(), 1, 0),
            DDMatrix.Extend(DDMatrix.gates["s"].Item1.Copy(), 1, 0),
            DDMatrix.Extend(DDMatrix.gates["t"].Item1.Copy(), 1, 0),
            DDMatrix.Extend(DDMatrix.gates["cnot"].Item1.Copy(), 2, 0, 1),
            DDMatrix.Extend(DDMatrix.gates["sw"].Item1.Copy(), 2, 0, 1),
            DDMatrix.Extend(DDMatrix.gates["tof"].Item1.Copy(), 3, 0, 1, 2),
        ];
    }

    [Test]
    public void TestSameValues()
    {
        AssertEquals(actual, expected);
    }

    [Test]
    public void TestProperReduction()
    {
        AssertInvariants(actual);
    }
}
