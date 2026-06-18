using System.Numerics;
using static ECfQC_Tests.TestUtility;

namespace ECfQC_Tests;


public class ConstructionTests{
    DDMatrix[] matrices;
    int[] counts = {0,0,0,
    1,2,3,
    1,1,1,1,1,
    1+4+4*4,1+4+4*4-1-2,
    4,3,3,
    1+1+2,1+3+2+1+1,1+4+7,
    1+4,2};

    [SetUp]
    public void Setup()
    {
        matrices = ConstructAll(baseCases);
    }

    [Test]
    public void TestSameValues()
    {
        AssertEquals(matrices, baseCases);
    }

    [Test]
    public void TestCount()
    {
        if (baseCases.Length != counts.Length)
        {
            Assert.Fail("Not Updated: baseCases was changed, but the array from this testcase wasn't adjusted accordingly.");
        }
        for (int i = 0; i<baseCases.Length;i++)
        {
            if (matrices[i].Count() != counts[i])
            {
                Assert.That(matrices[i].Count(), Is.EqualTo(counts[i]), i+"");
            }
        }
        Assert.Pass();
    }

    [Test]
    public void TestProperReduction()
    {
        AssertInvariants(matrices);
    }

}
