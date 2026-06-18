using System.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using static ECfQC_Tests.TestUtility;

namespace ECfQC_Tests;


public class ConjugateTransposeTests
{
    DDMatrix[] actual;
    Matrix<Complex>[] expected;

    [SetUp]
    public void Setup()
    {
        DDMatrix[] inputs = ConstructAll(baseCases);

        expected = new Matrix<Complex>[inputs.Length];
        for (int i = 0; i < inputs.Length; i++)
        {
            expected[i] = baseCases[i].ConjugateTranspose();
        }

        actual = new DDMatrix[expected.Length];
        for (int i = 0; i < inputs.Length; i++)
        {
            inputs[i] = DDMatrix.ConjugateTransposed(inputs[i]);
            actual[i] = inputs[i];
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
