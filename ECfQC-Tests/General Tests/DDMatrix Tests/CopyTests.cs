using System.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using static ECfQC_Tests.TestUtility;

namespace ECfQC_Tests;


public class CopyTests
{
    DDMatrix[] actual;
    Matrix<Complex>[] expected;
    DDMatrix[] expected2;

    [SetUp]
    public void Setup()
    {
        expected = baseCases;

        expected2 = ConstructAll(expected);
        actual = new DDMatrix[expected.Length];
        for (int i = 0; i < actual.Length; i++)
        {
            actual[i] = expected2[i].Copy();
        }

    }

    [Test]
    public void TestSameValues()
    {
        AssertEquals(expected2, expected);
        AssertEquals(actual, expected);
    }

    [Test]
    public void TestProperReduction()
    {
        AssertInvariants(actual);
    }

    [Test]
    public void TestIndependency()
    {
        for (int i = 0; i<expected2.Length;i++)
        {
            var n1 = expected2[i].nodeList;
            var n2 = actual[i].nodeList;
            Assert.That(expected2[i].Count(), Is.EqualTo(actual[i].Count()));
            foreach (var (a,b) in n1.GetData())
            {
                foreach (var n in b)
                {
                    Assert.That(n2.GetData()[a], Does.Not.Contain(n));
                }
            }
        }
    }
}
