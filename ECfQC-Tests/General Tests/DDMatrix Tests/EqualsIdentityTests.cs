using System.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using static ECfQC_Tests.TestUtility;

namespace ECfQC_Tests;


public class EqualsIdentityTests
{
    DDMatrix[] equalDD;
    DDMatrix[] equivalentDD; // specifically matrices, that are equivalent, but not equal
    DDMatrix[] notEqualDD;

    [SetUp]
    public void Setup()
    {
        const int inputs = 7;

        Matrix<Complex>[] equal = new Matrix<Complex>[inputs];
        for (int i = 0; i < inputs; i++)
        {
            equal[i] = CreateMatrix.DiagonalIdentity<Complex>((int) Math.Pow(2, i));
        }

        Matrix<Complex>[] equivalent = new Matrix<Complex>[inputs];
        var phases = new Complex[inputs] {new(-1.0/Math.Sqrt(2), 1.0/Math.Sqrt(2)), new(-1,0),
        new (1.0/Math.Sqrt(2), 1.0/Math.Sqrt(2)), new (0.3, Math.Sqrt(0.91)), 
        new (Math.Sqrt(0.91), 0.3), new(0,1), new (0,-1)};
        for (int i = 0; i < inputs; i++)
        {
            equivalent[i] = CreateMatrix.DiagonalIdentity<Complex>((int) Math.Pow(2, i))* phases[i];
        }

        Matrix<Complex>[] notEqual =
        [
            new DenseMatrix(2,2, [1,1,0,1]),
            new DenseMatrix(4,4, [1,0,0,0,0,2,0,0,0,0,1,0,0,0,0,1]),
            new DenseMatrix(4,4, [1,0,0,0,0,1,0,0,0,0,-1,0,0,0,0,1]),
            new DenseMatrix(4,4, [-1,0,0,0,0,1,0,0,0,0,-1,0,0,0,0,1]),
            new DenseMatrix(4,4, [new(0,1),0,0,0,0,1,0,0,0,0,1,0,0,0,0,1]),
            new DenseMatrix(4,4, [1,0,0,0,0,1,0,0,0,0,new(0,-1),0,0,0,0,1]),
            new DenseMatrix(4,4, [new(0,1),0,0,0,0,1,0,0,0,0,new(0,-1),0,0,0,0,1]),
            new DenseMatrix(4,4, [new(0,1),0,5,0,0,new(0,1),0,0,0,0,new(0,-1),0,0,0,0,new(0,1)]),
            new DenseMatrix(4,4, [new(0,2),0,0,0,0,new(0,2),0,0,0,0,new(0,2),0,0,0,0,new(0,2)]),
            new DenseMatrix(4,4, [new(1,1),0,0,0,0,new(1,1),0,0,0,0,new(1,1),0,0,0,0,new(1,1)]),
            new DenseMatrix(4,4, [new(1,1),0,1,2,0,new(1,1),0,0,new(8,0),-4,new(1,1),0,1,0,0,new(1,1)]),
            new DenseMatrix(4,4, [0,0,0,0,0,0,0,0,0,0,new(1,1),0,0,0,0,new(1,1)]),
            new DenseMatrix(4,4, [0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0]),
            new DenseMatrix(8,8, 
            [0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,
            ]),
            new DenseMatrix(8,8, 
            [0,0,0,0,0,0,0,1,
            0,0,0,0,0,0,1,0,
            0,0,0,0,0,1,0,0,
            0,0,0,0,1,0,0,0,
            0,0,0,1,0,0,0,0,
            0,0,1,0,0,0,0,0,
            0,1,0,0,0,0,0,0,
            1,0,0,0,0,0,0,0,
            ]),
            new DenseMatrix(8,8, 
            [0,0,0,0,0,0,0,0,
            0,0,new(4,0),0,0,0,0,0,
            0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,5,0,
            0,0,1,0,0,0,0,0,
            0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,
            ]),
            new DenseMatrix(8,8, 
            [0,0,0,0,0,0,0,1,
            0,0,0,0,0,0,1,0,
            0,0,0,0,0,2,0,0,
            0,0,0,0,1,0,0,0,
            0,0,0,1,0,0,0,0,
            0,0,1,0,0,0,0,0,
            0,1,0,0,0,0,0,0,
            1,0,0,0,0,0,0,0,
            ]),
            new DenseMatrix(8,8, 
            [1,0,0,0,0,0,0,1,
            0,1,0,0,0,0,1,0,
            0,0,1,0,0,1,0,0,
            0,0,0,1,1,0,0,0,
            0,0,0,1,1,0,0,0,
            0,0,1,0,0,1,0,0,
            0,1,0,0,0,0,1,0,
            1,0,0,0,0,0,0,1,
            ]),
            new DenseMatrix(8,8, 
            [1,0,0,0,0,0,0,0,
            0,1,0,0,0,0,0,0,
            0,0,2,0,0,0,0,0,
            0,0,0,1,0,0,0,0,
            0,0,0,0,1,0,0,0,
            0,0,0,0,0,1,0,0,
            0,0,0,0,0,0,1,0,
            0,0,0,0,0,0,0,1,
            ]),
            new DenseMatrix(8,8, 
            [2,0,0,0,0,0,0,0,
            0,2,0,0,0,0,0,0,
            0,0,2,0,0,0,0,0,
            0,0,0,2,0,0,0,0,
            0,0,0,0,2,0,0,0,
            0,0,0,0,0,2,0,0,
            0,0,0,0,0,0,2,0,
            0,0,0,0,0,0,0,2,
            ]),new DenseMatrix(8,8, 
            [1,0,0,0,0,23,0,0,
            0,1,0,0,0,0,0,0,
            0,0,1,0,0,0,0,0,
            new(1/Math.Sqrt(3), 4),0,0,1,0,0,0,0,
            0,0,0,0,1,0,99,0,
            0,0,0,0,0,1,0,0,
            0,-0.3,0,0,0,0,1,0,
            0,0,0,0,0,0,0,1,
            ]),
        ];

        equalDD = ConstructAll(equal);
        equivalentDD = ConstructAll(equivalent);
        notEqualDD = ConstructAll(notEqual);
    }

    [Test]
    public void TestCorrectResult()
    {
        for (int i = 0;i<equalDD.Length;i++)
        {
            Assert.That(equalDD[i].EqualsIdentity(), Is.EqualTo(1));
            Assert.That(equivalentDD[i].EqualsIdentity(), Is.EqualTo(2));
        }
        for (int i = 0;i<notEqualDD.Length;i++)
        {
            Assert.That(notEqualDD[i].EqualsIdentity(), Is.EqualTo(0));
        }
    }
}
