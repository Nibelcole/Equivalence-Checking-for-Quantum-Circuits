using System.Numerics;
using static ECfQC_Tests.TestUtility;

namespace ECfQC_Tests;


public class SimulatorTests
{
    Complex[][] actual;
    Complex[][] expected;
    DDMatrix[][] circuits;
    int[] basisStateList;

    [SetUp]
    public void Setup()
    {
        const string path = "..\\..\\..\\General Tests\\Other Tests\\Files\\";
        circuits = [
            Parser.Parse(path +"single_only\\c1empty", 2).ToArray<DDMatrix>(),
            Parser.Parse(path +"single_only\\c1id", 2).ToArray<DDMatrix>(),
            Parser.Parse(path +"single_only\\c2-q3", 3).ToArray<DDMatrix>(),
            Parser.Parse(path +"single_only\\c3-1-q4", 4).ToArray<DDMatrix>(),
            Parser.Parse(path +"single_only\\c3-2-q4", 4).ToArray<DDMatrix>(),
            Parser.Parse(path +"single_only\\c3-q4", 4).ToArray<DDMatrix>(),

            Parser.Parse(path +"two_only\\c2-q3", 3).ToArray<DDMatrix>(),
            Parser.Parse(path +"two_only\\c2-q3 alt", 3).ToArray<DDMatrix>(),
            Parser.Parse(path +"two_only\\c3-1-q4", 4).ToArray<DDMatrix>(),
            Parser.Parse(path +"two_only\\c3-2-q4", 4).ToArray<DDMatrix>(),
            Parser.Parse(path +"two_only\\c3-q4", 4).ToArray<DDMatrix>(),

            Parser.Parse(path +"three_only\\c1-1-q4", 4).ToArray<DDMatrix>(),
            Parser.Parse(path +"three_only\\c1-q4", 4).ToArray<DDMatrix>(),
            Parser.Parse(path +"three_only\\c2-q4", 4).ToArray<DDMatrix>(),

            Parser.Parse(path +"multiple\\c2-q4", 4).ToArray<DDMatrix>(),
            Parser.Parse(path +"multiple\\c2-q4 alt", 4).ToArray<DDMatrix>(),
            Parser.Parse(path +"multiple\\c3-1-q4", 4).ToArray<DDMatrix>(),
            Parser.Parse(path +"multiple\\c3-2-q4", 4).ToArray<DDMatrix>(),
            Parser.Parse(path +"multiple\\c3-q4", 4).ToArray<DDMatrix>(),
        ];

        basisStateList = [
            0,1,
            1,2,2,0,3,
            0,1,0,3,2,
            0,1,1,
            0,1,2,3,2
        ];

        actual = new Complex[circuits.GetLength(0)][];
        for (int i = 0; i < actual.Length; i++)
        {
            actual[i] = Simulator.SimulateSingle(circuits[i].ToList<DDMatrix>(), (int)Math.Pow(2, circuits[i][0].qubits), basisStateList[i]);
        }

        expected = [
        [1.0,0.0,0.0,0.0,],
        [0.0,1.0,0.0,0.0,],
        [0.0,0.0,0.7071067811865475,new(-0.4999999999999999,0.4999999999999999),0.0,0.0,0.0,0.0,],
        [new(0.0,0.3535533905932737),new(-0.24999999999999992,0.24999999999999992),new(0.0,0.3535533905932737),new(-0.24999999999999992,0.24999999999999992),new(0.0,0.3535533905932737),new(-0.24999999999999992,0.24999999999999992),new(0.0,0.3535533905932737),new(-0.24999999999999992,0.24999999999999992),0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,],
        [new(0.0,0.3535533905932737),new(-0.24999999999999992,0.24999999999999992),new(-0.24999999999999992,-0.24999999999999992),new(0.0,-0.3535533905932736),new(0.0,0.3535533905932737),new(-0.24999999999999992,0.24999999999999992),new(-0.24999999999999992,-0.24999999999999992),new(0.0,-0.3535533905932736),0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,],
        [new(0.0,-0.3535533905932737),new(0.24999999999999992,-0.24999999999999992),-0.3535533905932737,new(-0.24999999999999992,-0.24999999999999992),new(0.0,-0.3535533905932737),new(0.24999999999999992,-0.24999999999999992),-0.3535533905932737,new(-0.24999999999999992,-0.24999999999999992),0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,],
        [0.4999999999999999,-0.4999999999999999,0.4999999999999999,-0.4999999999999999,0.0,0.0,0.0,0.0,],
        [0.4999999999999999,0.4999999999999999,0.4999999999999999,0.4999999999999999,0.0,0.0,0.0,0.0,],
        [0.4999999999999999,0.4999999999999999,0.0,0.0,0.0,0.0,-0.4999999999999999,-0.4999999999999999,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,],
        [0.4999999999999999,0.0,0.0,0.0,0.0,0.0,0.4999999999999999,0.0,0.0,0.4999999999999999,0.0,0.0,0.0,0.0,0.0,0.4999999999999999,],
        [0.4999999999999999,0.0,0.0,0.0,0.0,0.0,0.4999999999999999,0.0,-0.4999999999999999,0.0,0.0,0.0,0.0,0.0,-0.4999999999999999,0.0,],
        [0.24999999999999992,0.24999999999999992,-0.24999999999999992,-0.24999999999999992,0.24999999999999992,0.24999999999999992,-0.24999999999999992,-0.24999999999999992,0.24999999999999992,0.24999999999999992,-0.24999999999999992,-0.24999999999999992,0.24999999999999992,0.24999999999999992,-0.24999999999999992,-0.24999999999999992,],
        [0.24999999999999992,0.24999999999999992,0.24999999999999992,0.24999999999999992,0.24999999999999992,0.24999999999999992,0.24999999999999992,0.24999999999999992,0.24999999999999992,0.24999999999999992,0.24999999999999992,0.24999999999999992,0.24999999999999992,0.24999999999999992,0.24999999999999992,0.24999999999999992,],
        [0.24999999999999992,-0.24999999999999992,0.24999999999999992,-0.24999999999999992,0.24999999999999992,-0.24999999999999992,0.24999999999999992,-0.24999999999999992,0.24999999999999992,-0.24999999999999992,0.24999999999999992,-0.24999999999999992,0.24999999999999992,-0.24999999999999992,0.24999999999999992,-0.24999999999999992,],
        [new(0.0,-0.4999999999999999),0.0,new(0.0,-0.4999999999999999),new(0.0,-0.4999999999999999),0.0,0.0,0.0,0.0,0.0,new(0.0,-0.4999999999999999),0.0,0.0,0.0,0.0,0.0,0.0,],
        [new(0.0,-0.4999999999999999),0.0,new(0.0,-0.4999999999999999),new(0.0,0.4999999999999999),0.0,0.0,0.0,0.0,0.0,new(0.0,0.4999999999999999),0.0,0.0,0.0,0.0,0.0,0.0,],
        [0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,new(0.3535533905932737,0.3535533905932737),new(0.3535533905932737,0.3535533905932737),new(0.3535533905932737,0.3535533905932737),new(0.3535533905932737,0.3535533905932737),0.0,0.0,0.0,0.0,],
        [0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,new(-0.3535533905932737,0.3535533905932737),0.0,new(-0.3535533905932737,0.3535533905932737),new(-0.3535533905932737,0.3535533905932737),0.0,new(-0.3535533905932737,0.3535533905932737),0.0,],
        [0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,new(0.3535533905932737,0.3535533905932737),new(-0.3535533905932737,-0.3535533905932737),new(-0.3535533905932737,-0.3535533905932737),new(0.3535533905932737,0.3535533905932737),0.0,0.0,0.0,0.0,],
        ];
    }

    [Test]
    public void TestSameValues()
    {
        AssertEquals(actual, expected);
    }

    [Test]
    public void TestEqualToItself()
    {
        for (int i = 3; i<5;i++) { // we check only two of them because of performance
            Assert.That(Simulator.Simulate(circuits[i].ToList<DDMatrix>(), circuits[i].ToList<DDMatrix>(), (int)Math.Pow(2, circuits[i][0].qubits), basisStateList[i]).Item1);
        }
    }

    [Test]
    public void TestNotEqualToItself()
    {
        for (int i = 3; i<5;i++) { // we check only two of them because of performance
            Assert.That(!Simulator.Simulate(circuits[i].ToList<DDMatrix>(), circuits[i+6].ToList<DDMatrix>(), (int)Math.Pow(2, circuits[i][0].qubits), basisStateList[i]).Item1);
        }
    }

    [Test]
    public void TestProperReductionOfParsedAndExtendedCircuits()
    {
        foreach (var d in circuits)
        {
            AssertInvariants(d);
        }
    }
}
