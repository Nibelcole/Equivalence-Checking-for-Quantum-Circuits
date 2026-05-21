using System.Numerics;
using System.Runtime.ExceptionServices;
using static Utility;


public static class EQChecker
{
    /// <summary>
    /// Checks for equivalence of the two given DDMatrices with the given strategy. <br/><br/>
    /// Returns true if and only if both matrices are equivalent.
    /// </summary>
    public static bool Check(List<DDMatrix> first, List<DDMatrix> second, string strategy, int length)
    {
        if (first.Count == 0)
        {
            if (second.Count == 0)
            {
                return true; // empty circuits are interpreted as identity matrices
            }

        }

        // invert second matrix
        second.Reverse();
        foreach (var m in second)
        {
            m.ConjugateComplex();
        }

        DDMatrix result;
        switch (strategy)
        {
            case "alternating":
                result = CalAlternating(first, second, length);
                break;
            case "alternating-balanced":
            result = CalAlternatingBalanced(first, second, length);
                break;
            case "look-ahead":
            result = CalLookAhead(first, second, length);
                break;
            default:
                throw new ArgumentException("Error: " + strategy + " is not a known type of strategy. " +
                "See -h or --help for more information.");
        }

        return result.EqualsIdentity(); 
    }

    /// <summary>
    /// Returns G --> I <-- G' with the strategy "alternating"
    /// </summary>
    private static DDMatrix CalAlternating(List<DDMatrix> first, List<DDMatrix> second, int length)
    {
        return AlternateHelper(first, second, length, 1, 1);
    }

    /// <summary>
    /// Returns G --> I <-- G' with the strategy "alternating-balanced"
    /// </summary>
    private static DDMatrix CalAlternatingBalanced(List<DDMatrix> first, List<DDMatrix> second, int length)
    {
        int firstWeight = Math.Max(first.Count / second.Count, 1); // Math.Max, so we don't get 0
        int secondWeight = Math.Max(second.Count / first.Count, 1); // Math.Max, so we don't get 0
        return AlternateHelper(first, second, length, firstWeight, secondWeight);
    }

    /// <summary>
    /// A helper method for CalAlternating() and CalAlternatingBalanced()
    /// </summary>
    private static DDMatrix AlternateHelper(List<DDMatrix> first, List<DDMatrix> second, int length,
        int firstWeight, int secondWeight)
    {
        DDMatrix result = DDMatrix.Identity(length);
        while (first.Count > 0 || second.Count > 0)
        {
            int counter = firstWeight;
            while (first.Count > 0 && counter > 0)
            {
                result = DDMatrix.Multiply(first[first.Count - 1], result);
                first.RemoveAt(first.Count - 1);
                counter--;
            }

            counter = secondWeight;
            while (second.Count > 0 && counter > 0)
            {
                result = DDMatrix.Multiply(result, second[0]);
                second.RemoveAt(second.Count - 1);
                counter--;
            }
        }
        return result;
    }

    /// <summary>
    /// Returns G --> I <-- G' with the strategy "look-ahead"
    /// </summary>
    private static DDMatrix CalLookAhead(List<DDMatrix> first, List<DDMatrix> second, int length)
    {
        DDMatrix result = DDMatrix.Identity(length);
        while (first.Count > 0 || second.Count > 0)
        {
            DDMatrix? res1 = null;
            DDMatrix? res2 = null;

            // calculate both results
            if (first.Count > 0)
            {
                res1 = DDMatrix.Multiply(first[first.Count - 1], result);
            }
            if (second.Count > 0)
            {
                res2 = DDMatrix.Multiply(result, second[0]);
            }

            // if one is null (list empty), take the other one
            if (second.Count == 0)
            {
                result = res1;
                first.RemoveAt(first.Count - 1);
                continue;
            }
            if (first.Count == 0)
            {
                result = res2;
                second.RemoveAt(second.Count - 1);
                continue;
            }

            // take the smaller one
            if (res1.Count() >= res2.Count())
            {
                result = res2;
                second.RemoveAt(second.Count - 1);
            }
            else
            {
                result = res1;
                first.RemoveAt(first.Count - 1);
            }
        }
        return result;
    }
}