using System.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using static ECfQC_Tests.TestUtility;

namespace ECfQC_Tests;


public class MatrixMultiplicationTests
{
    DDMatrix[] actual;
    Matrix<Complex>[] expected;

    [SetUp]
    public void Setup()
    {
        DDMatrix placeholder = DDMatrix.Identity(1);
        Matrix<Complex> placeholder2 = new DenseMatrix(2, 2, [1, 0, 0, 1]);
        DDMatrix[] inputs = ConstructAll(baseCases);

        expected = new Matrix<Complex>[inputs.Length * inputs.Length];
        for (int i = 0; i < inputs.Length; i++)
        {
            for (int k = 0; k < inputs.Length; k++)
            {
                if (inputs[i].qubits != inputs[k].qubits) // This is admittedly not the most beautiful solution, 
                                                          // but it works even if baseCases is changed.
                {
                    expected[inputs.Length * i + k] = placeholder2;
                    continue;
                }
                expected[inputs.Length * i + k] = baseCases[i].Multiply(baseCases[k]);
            }
        }

        actual = new DDMatrix[expected.Length];
        for (int i = 0; i < inputs.Length; i++)
        {
            for (int k = 0; k < inputs.Length; k++)
            {
                if (inputs[i].qubits != inputs[k].qubits)
                {
                    actual[inputs.Length * i + k] = placeholder;
                    continue;
                }
                actual[inputs.Length * i + k] = DDMatrix.Multiply(inputs[i], inputs[k]);
            }
        }

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
