using System.Numerics;
using static ECfQC_Tests.TestUtility;

namespace ECfQC_Tests;


public class EQCheckerTests
{
    int[] actual1;
    int[] actual2;
    int[] actual3;
    int[] expected;
    DDMatrix[][] circuits;

    [SetUp]
    public void Setup()
    {
        Tuple<int, int>[] equivalences = [
            new (0,1),
            new (2,2),
            new (3,3),
            new (3,4),
            new (3,5),

            new (6,6),
            new (6,7),
            new (8,9),
            new (8,10),

            new (11,11),
            new (11,13),
            new (12,13),

            new (14,14),
            new (14,15),
            new (14, 16),
            new (14, 18),
            new (16, 17),
            new (16, 18),

            new (10, 18),
            new (11, 3),
            new (5, 12),
            new (2, 6),

            new (19,20),
            new (20, 21),
            new (21, 22),
            new (20, 23),
            new (20,24),
            new (25, 26),
        ];

        expected = [1, 1, 1, 0, 0, 1, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 2, 1, 1, 1];

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

            Parser.Parse(path +"multiple\\c4-q1", 1).ToArray<DDMatrix>(),
            Parser.Parse(path +"multiple\\c5-q1", 1).ToArray<DDMatrix>(),
            Parser.Parse(path +"multiple\\c6-q1", 1).ToArray<DDMatrix>(),
            Parser.Parse(path +"multiple\\c7empty", 1).ToArray<DDMatrix>(),
            Parser.Parse(path +"multiple\\c8-q1", 1).ToArray<DDMatrix>(),
            Parser.Parse(path +"multiple\\c9-q1", 1).ToArray<DDMatrix>(),

            Parser.Parse(path +"multiple\\c10-q3", 3).ToArray<DDMatrix>(),
            Parser.Parse(path +"multiple\\c11-q3", 3).ToArray<DDMatrix>(),
        ];

        string[] strategies = ["alternating", "alternating-balanced", "look-ahead"];

        actual1 = new int[equivalences.GetLength(0)];
        for (int i = 0; i < actual1.Length; i++)
        {
            actual1[i] = EQChecker.Check(circuits[equivalences[i].Item1].ToList<DDMatrix>(), 
            circuits[equivalences[i].Item2].ToList<DDMatrix>(),
            "alternating",
            (int)Math.Pow(2, circuits[equivalences[i].Item1][0].qubits));
        }
        actual2 = new int[equivalences.GetLength(0)];
        for (int i = 0; i < actual2.Length; i++)
        {
            actual2[i] = EQChecker.Check(circuits[equivalences[i].Item1].ToList<DDMatrix>(), 
            circuits[equivalences[i].Item2].ToList<DDMatrix>(),
            "alternating-balanced",
            (int)Math.Pow(2, circuits[equivalences[i].Item1][0].qubits));
        }
        actual3 = new int[equivalences.GetLength(0)];
        for (int i = 0; i < actual3.Length; i++)
        {
            actual3[i] = EQChecker.Check(circuits[equivalences[i].Item1].ToList<DDMatrix>(), 
            circuits[equivalences[i].Item2].ToList<DDMatrix>(),
            "look-ahead",
            (int)Math.Pow(2, circuits[equivalences[i].Item1][0].qubits));
        }
    }

    [Test]
    public void TestSameValues()
    {
        for (int i = 0; i<actual1.Length;i++)
        {
            Assert.That(actual1[i], Is.EqualTo(expected[i]), "alternating: "+i);
        }
        for (int i = 0; i<actual2.Length;i++)
        {
            Assert.That(actual2[i], Is.EqualTo(expected[i]), "alternating-balanced: "+i);
        }
        for (int i = 0; i<actual3.Length;i++)
        {
            Assert.That(actual3[i], Is.EqualTo(expected[i]), "look-ahead: "+i);
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
