using System.Numerics;
using static Utility;


public static class EQChecker
{
    /// <summary>
    /// Checks for equivalence of the two given DDMatrices with the given strategy. <br/><br/>
    /// Returns true if and only if both matrices are equivalent.
    /// </summary>
    public static bool Check(List<DDMatrix> first, List<DDMatrix> second, string strategy)
    {
        if (first.Count == 0)
        {
            if (second.Count == 0)
            {
                return true; // empty circuits are interpreted as identity matrices
            }
            
        } 
        
        return false; // TODO
    }
}