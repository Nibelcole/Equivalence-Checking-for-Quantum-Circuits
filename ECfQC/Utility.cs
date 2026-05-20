// provides utility methods that didn't fit into other classes

using System.Numerics;

public static class Utility
{
    public const double delta = 1e-12;

    /// <summary>
    /// Returns true if and only if the two given Complex numbers differ by only delta.
    /// </summary>
    public static bool CompareComplex(Complex first, Complex second)
    {
        return Math.Abs(first.Real - second.Real) < delta 
        && Math.Abs(first.Imaginary - second.Imaginary) < delta;
    }
}