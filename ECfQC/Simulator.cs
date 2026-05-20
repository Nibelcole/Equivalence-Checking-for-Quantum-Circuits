using System.CodeDom.Compiler;
using System.Numerics;
using System.Text.RegularExpressions;
using static Utility;


public static class Simulator
{
    /// <summary>
    /// Executes one simulation run and returns true if and only if both DDMatrixes 
    /// calculate the same value for the given input.
    /// <br/><br/>
    /// input refers to the basis state |input〉. It is assumed that first.size == second.size and first.size > input.
    /// </summary>
    public static bool Simulate(List<DDMatrix> first, List<DDMatrix> second, int input)
    {
        int length;
        if (first.Count == 0)
        {
            if (second.Count == 0)
            {
                return true; // empty circuits are interpreted as identity matrices
            }
            length = (int) Math.Pow(2, second[0].qubits);
        } else
        {
            length = (int) Math.Pow(2, first[0].qubits);        
        }

        Complex[] res1 = SimulateSingle(first, length, input);
        Complex[] res2 = SimulateSingle(second, length, input);

        for (int i = 0; i<length;i++)
        {
            if (!CompareComplex(res1[i], res2[i]))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Executes half of one simulation run and returns the result. A helper function for Simulate().
    /// </summary>
    private static Complex[] SimulateSingle(List<DDMatrix> first, int length, int input)
    {
        Complex[] value = new Complex[length];
        value[input] = Complex.One;

        foreach (var m in first)
        {
            value = DDMatrix.Multiply(m, value);
        }
        return value;
    }

}