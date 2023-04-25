using Quickenshtein;

namespace Biomatch.Domain.Models;

public static class StringDistance
{
  public static int GeneralDemographicFieldDistance(ReadOnlySpan<char> a, ReadOnlySpan<char> b)
  {
    if (a.IsEmpty && b.IsEmpty)
    {
      return 0;
    }

    if (a.IsEmpty || b.IsEmpty)
    {
      return -1;
    }

    return Levenshtein.GetDistance(a, b);
  }

  public static int DateDemographicFieldDistance(ReadOnlySpan<byte> date1, ReadOnlySpan<byte> date2)
  {
    if (date1.IsEmpty && date2.IsEmpty)
    {
      return 0;
    }

    if (date1.IsEmpty || date2.IsEmpty)
    {
      return -1;
    }

    // Check for inverted day and month
    var month1 = date1[..2];
    var day1 = date1.Slice(2, 2);
    var month2 = date2[..2];
    var day2 = date2.Slice(2, 2);
    var date1Year = date1[4..];
    var date2Year = date2[4..];

    if (date1Year.SequenceEqual(date2Year) &&
        month1.SequenceEqual(day2) &&
        day1.SequenceEqual(month2))
    {
      return 0;
    }

    var distance = 0;
    for (var i = 0; i < 8; i++)
    {
      if (date1[i] != date2[i])
      {
        distance++;
      }
    }

    return distance;
  }
}
