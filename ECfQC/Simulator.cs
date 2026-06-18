using System.CodeDom.Compiler;
using System.Numerics;
using System.Text.RegularExpressions;
using static Utility;


public static class Simulator
{
    /// <summary>
    /// Executes one simulation run and returns true if and only if both DDMatrixes 
    /// calculate the same value for the given input. Additionally, the results of the calculations are also returned;
    /// <br/><br/>
    /// input refers to the basis state |input〉. It is assumed that first.size == second.size and first.size > input.
    /// </summary>
    public static Tuple<bool, Complex[], Complex[]> Simulate(List<DDMatrix> first, List<DDMatrix> second, int length, int input)
    {
        if (first.Count == 0)
        {
            if (second.Count == 0)
            {
                Complex[] value = new Complex[length];
                value[input] = Complex.One;
                return new (true, value, value); // empty circuits are interpreted as identity matrices
            }
        } 

        Complex[] res1 = SimulateSingle(first, length, input);
        Complex[] res2 = SimulateSingle(second, length, input);

        for (int i = 0; i<length;i++)
        {
            if (!CompareComplex(res1[i], res2[i]))
            {
                return new (false, res1, res2);
            }
        }
        return new (true, res1, res2);
    }

    /// <summary>
    /// Executes half of one simulation run and returns the result. A helper function for Simulate().
    /// </summary>
    public static Complex[] SimulateSingle(List<DDMatrix> first, int length, int input) // public for testing/debugging purposes
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