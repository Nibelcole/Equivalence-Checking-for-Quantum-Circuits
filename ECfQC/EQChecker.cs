using System.Numerics;
using System.Runtime.ExceptionServices;
using static Utility;


public static class EQChecker
{
    /// <summary>
    /// Checks for equivalence of the two given DDMatrices with the given strategy. <br/><br/>
    /// 
    /// Returns 1 if G --> I <-- G' is exactly equal to an identity matrix.<br/>
    /// Returns 2 if G --> I <-- G' differs at most by a global phase vector 
    /// e^(i*alpha) from an identity matrix without being fully equal.<br/>
    /// Returns 0 otherwise.
    /// </summary>
    public static int Check(List<DDMatrix> first, List<DDMatrix> second, string strategy, int length, bool verbose = false)
    {
        if (first.Count == 0 && second.Count == 0)
        {
            return 1; // empty circuits are interpreted as identity matrices (Note that the parser should have already handled this though)
        }

        // shallow copy first list, so tests can reuse lists
        List<DDMatrix> firstCopy = new(first.Count);
        for (int i = 0; i < first.Count; i++)
        {
            firstCopy.Add(first[i]);
        }
        first = firstCopy;

        // invert second circuit
        List<DDMatrix> secondCopy = new(second.Count);
        for (int i = second.Count - 1; i >= 0; i--)
        {
            secondCopy.Add(DDMatrix.ConjugateTransposed(second[i]));
        }
        second = secondCopy;


        DDMatrix result;
        switch (strategy)
        {
            case "alternating":
                result = CalAlternating(first, second, length);
                break;
            case "alternating-balanced":
                result = CalAlternatingBalanced(first, second, length, verbose);
                break;
            case "look-ahead":
                result = CalLookAhead(first, second, length, verbose);
                break;
            default:
                throw new ArgumentException("Error: \"" + strategy + "\" is not a known type of strategy. " +
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
    private static DDMatrix CalAlternatingBalanced(List<DDMatrix> first, List<DDMatrix> second, int length, bool verbose)
    {
        int firstWeight = (int)Math.Max(first.Count / ((double)second.Count), 1); // Math.Max, so we don't get 0
        int secondWeight = (int)Math.Max(((double)second.Count) / first.Count, 1); // Math.Max, so we don't get 0
        if (verbose)
        {
            Console.WriteLine($"The determined ratio for \"alternating-balanced\" is {firstWeight}:{secondWeight}.");
        }
        return AlternateHelper(first, second, length, firstWeight, secondWeight);
    }

    /// <summary>
    /// A helper method for CalAlternating() and CalAlternatingBalanced()
    /// </summary>
    private static DDMatrix AlternateHelper(List<DDMatrix> first, List<DDMatrix> second, int length,
        int firstWeight, int secondWeight)
    {
        DDMatrix result = DDMatrix.Identity((int)Math.Log2(length));
        while (first.Count > 0 || second.Count > 0)
        {
            int counter = firstWeight;
            while (first.Count > 0 && counter > 0)
            {
                result = DDMatrix.Multiply(result, first[first.Count - 1]);
                first.RemoveAt(first.Count - 1);
                counter--;
            }

            counter = secondWeight;
            while (second.Count > 0 && counter > 0)
            {
                result = DDMatrix.Multiply(second[0], result);
                second.RemoveAt(0);
                counter--;
            }
        }
        return result;
    }

    /// <summary>
    /// Returns G --> I <-- G' with the strategy "look-ahead"
    /// </summary>
    private static DDMatrix CalLookAhead(List<DDMatrix> first, List<DDMatrix> second, int length, bool verbose)
    {
        DDMatrix result = DDMatrix.Identity((int)Math.Log2(length));
        while (first.Count > 0 || second.Count > 0)
        {
            DDMatrix? res1 = null;
            DDMatrix? res2 = null;

            // calculate both results
            if (first.Count > 0)
            {
                res1 = DDMatrix.Multiply(result, first[first.Count - 1]);
            }
            if (second.Count > 0)
            {
                res2 = DDMatrix.Multiply(second[0], result);
            }

            // if one is null (list empty), take the other one
            if (second.Count == 0)
            {
                result = res1;
                first.RemoveAt(first.Count - 1);
                if (verbose)
                {
                    Console.WriteLine("A gate from the first circuit was chosen.");
                }
                continue;
            }
            if (first.Count == 0)
            {
                result = res2;
                second.RemoveAt(0);
                if (verbose)
                {
                    Console.WriteLine("A gate from the second circuit was chosen.");
                }
                continue;
            }

            // take the smaller one
            if (res1.Count() >= res2.Count())
            {
                result = res2;
                second.RemoveAt(0);
                if (verbose)
                {
                    Console.WriteLine("A gate from the second circuit was chosen.");
                }
            }
            else
            {
                result = res1;
                first.RemoveAt(first.Count - 1);
                if (verbose)
                {
                    Console.WriteLine("A gate from the first circuit was chosen.");
                }
            }
        }
        return result;
    }
}