using System.Numerics;
using MathNet.Numerics.LinearAlgebra;
using static ECfQC_Tests.TestUtility;

namespace ECfQC_Tests;

// This only tests tensor multiplication with DDMatrix.Identity(1) 
// (as that is the only this DDMatrix.TensorProduct() is used for in the code).
public class TensorMultiplicationTests
{
    DDMatrix[] actual;
    Matrix<Complex>[] expected;

    [SetUp]
    public void Setup()
    {        
        DDMatrix[] inputs = ConstructAll(baseCases);

        expected = new Matrix<Complex>[inputs.Length];
        for (int i = 0; i < inputs.Length/2; i++)
        {
            expected[i] = baseCases[i].KroneckerProduct(CreateMatrix.DiagonalIdentity<Complex>(2));
        }
        for (int i = inputs.Length/2; i < inputs.Length; i++)
        {
            expected[i] = CreateMatrix.DiagonalIdentity<Complex>(2).KroneckerProduct(baseCases[i]);
        }

        actual = new DDMatrix[expected.Length];
        for (int i = 0; i < inputs.Length/2; i++)
        {
            actual[i] = DDMatrix.TensorProduct(inputs[i], DDMatrix.Identity(1));
        }
        for (int i = inputs.Length/2; i < inputs.Length; i++)
        {
            actual[i] = DDMatrix.TensorProduct(DDMatrix.Identity(1), inputs[i]);
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
