// provides utility methods that didn't fit into other classes

using System.Numerics;
using System.Reflection.Metadata.Ecma335;

public static class Utility
{
    public const double delta = 1e-11;

    /// <summary>
    /// Returns true if and only if the two given Complex numbers differ by only delta.
    /// </summary>
    public static bool CompareComplex(Complex first, Complex second)
    {
        return Math.Abs(first.Real - second.Real) < delta
        && Math.Abs(first.Imaginary - second.Imaginary) < delta;
    }

    public static double Fidelity(Complex[] first, Complex[] second)
    {
        Complex inner = Complex.Zero;
        double normFirst = 0.0;
        double normSecond = 0.0;

        for (int i = 0; i < first.Length; i++)
        {
            inner += Complex.Conjugate(first[i]) * second[i];
            normFirst += first[i].Magnitude * first[i].Magnitude;
            normSecond += second[i].Magnitude * second[i].Magnitude;
        }

        return Math.Pow(inner.Magnitude, 2) / (normFirst * normSecond);
    }
}