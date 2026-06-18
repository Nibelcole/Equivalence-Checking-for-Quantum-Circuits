using static ECfQC_Tests.TestUtility;

namespace ECfQC_Tests;


public class ParserTests
{
    DDMatrix[] actual;
    DDMatrix[] expected;

    [SetUp]
    public void Setup()
    {
        actual = Parser.Parse("..\\..\\..\\General Tests\\Other Tests\\Files\\multiple\\c3-2-q4", 4).ToArray<DDMatrix>();

        expected = [
            DDMatrix.Extend(DDMatrix.gates["h"].Item1.Copy(), 4, 0),
            DDMatrix.Extend(DDMatrix.gates["h"].Item1.Copy(), 4, 1),
            DDMatrix.Extend(DDMatrix.gates["pX"].Item1.Copy(), 4, 0),
            DDMatrix.Extend(DDMatrix.gates["pY"].Item1.Copy(), 4, 0),
            DDMatrix.Extend(DDMatrix.gates["cnot"].Item1.Copy(), 4, 0, 1),
            DDMatrix.Extend(DDMatrix.gates["pY"].Item1.Copy(), 4, 3),
            DDMatrix.Extend(DDMatrix.gates["t"].Item1.Copy(), 4, 3),
            DDMatrix.Extend(DDMatrix.gates["cnot"].Item1.Copy(), 4, 2,3),
            DDMatrix.Extend(DDMatrix.gates["s"].Item1.Copy(), 4, 2),
            DDMatrix.Extend(DDMatrix.gates["sw"].Item1.Copy(), 4, 1,0),
            DDMatrix.Extend(DDMatrix.gates["tof"].Item1.Copy(), 4, 1,2,3),
            DDMatrix.Extend(DDMatrix.gates["tof"].Item1.Copy(), 4, 0,3,2),
            DDMatrix.Extend(DDMatrix.gates["pY"].Item1.Copy(), 4, 2),
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
