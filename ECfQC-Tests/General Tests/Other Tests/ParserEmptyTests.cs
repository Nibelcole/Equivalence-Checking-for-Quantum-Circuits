using static ECfQC_Tests.TestUtility;

namespace ECfQC_Tests;


public class ParserEmptyTests
{
    DDMatrix[] actual;
    DDMatrix[] expected;

    [SetUp]
    public void Setup()
    {
        actual = Parser.Parse("..\\..\\..\\General Tests\\Other Tests\\Files\\single_only\\c1empty", 4).ToArray<DDMatrix>();

        expected = [
            DDMatrix.Extend(DDMatrix.gates["id"].Item1.Copy(), 4, 0),
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
