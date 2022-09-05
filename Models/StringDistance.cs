using Quickenshtein;

namespace MatchingEngine.Models;

public static class StringDistance
{
    public static int GeneralDemographicFieldDistance(ReadOnlySpan<char> a, ReadOnlySpan<char> b)
    {
        int distance;
        if (a.IsEmpty || b.IsEmpty)
        {
            distance = -1;
        }
        else
        {
            distance = Levenshtein.GetDistance(a, b);
        }

        return distance;
    }
}