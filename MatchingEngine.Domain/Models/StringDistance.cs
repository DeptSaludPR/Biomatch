using Quickenshtein;

namespace MatchingEngine.Domain.Models;

public static class StringDistance
{
    public static int GeneralDemographicFieldDistance(ReadOnlySpan<char> a, ReadOnlySpan<char> b)
    {
        int distance;
        if (a.IsEmpty && b.IsEmpty)
        {
            distance = 0;
        }
        else if (a.IsEmpty || b.IsEmpty)
        {
            distance = -1;
        }
        else
        {
            distance = Levenshtein.GetDistance(a, b);
        }

        return distance;
    }

    public static int GeneralDemographicFieldDistance(DateTime? date1, DateTime? date2)
    {
        int distance;
        if (date1 is null && date2 is null)
        {
            distance = 0;
        }
        else if (date1 is null || date2 is null)
        {
            distance = -1;
        }
        // Check for inverted day and month
        else if (date1.Value.Year == date2.Value.Year &&
                 date1.Value.Month == date2.Value.Day &&
                 date1.Value.Day == date2.Value.Month)
        {
            distance = 0;
        }
        else
        {
            distance = Levenshtein.GetDistance(date1.Value.ToShortDateString(), date2.Value.ToShortDateString());
        }

        return distance;
    }
}