using System.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using static ECfQC_Tests.TestUtility;

namespace ECfQC_Tests;


public class IdentityTests
{
    DDMatrix[] actual;
    Matrix<Complex>[] expected;

    [SetUp]
    public void Setup()
    {
        const int inputs = 7;

        expected = new Matrix<Complex>[inputs];
        for (int i = 0; i < inputs; i++)
        {
            expected[i] = CreateMatrix.DiagonalIdentity<Complex>((int) Math.Pow(2, i));
        }

        actual = new DDMatrix[expected.Length];
        for (int i = 0; i < inputs; i++)
        {
            actual[i] = DDMatrix.Identity(i);
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
