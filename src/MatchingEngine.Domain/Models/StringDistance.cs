using System.Buffers;
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

  public static int GeneralDemographicFieldDistance(DateOnly? date1, DateOnly? date2)
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
      var date1Buffer = ArrayPool<char>.Shared.Rent(9);
      var date2Buffer = ArrayPool<char>.Shared.Rent(9);
      distance = Levenshtein.GetDistance(ToShortDateReadOnlySpan(date1.Value, date1Buffer),
        ToShortDateReadOnlySpan(date2.Value, date2Buffer));
      ArrayPool<char>.Shared.Return(date1Buffer);
      ArrayPool<char>.Shared.Return(date2Buffer);
    }

    return distance;
  }

  private static ReadOnlySpan<char> ToShortDateReadOnlySpan(DateOnly date, char[] buffer)
  {
    var written = 0;

    var month = date.Month;
    if (month < 10)
    {
      buffer[written++] = '0';
      buffer[written++] = (char) (month + '0');
    }
    else
    {
      buffer[written++] = (char) (month / 10 + '0');
      buffer[written++] = (char) (month % 10 + '0');
    }

    var day = date.Day;
    if (day < 10)
    {
      buffer[written++] = '0';
      buffer[written++] = (char) (day + '0');
    }
    else
    {
      buffer[written++] = (char) (day / 10 + '0');
      buffer[written++] = (char) (day % 10 + '0');
    }

    var year = date.Year;
    buffer[written++] = (char) (year / 1000 + '0');
    year %= 1000;
    buffer[written++] = (char) (year / 100 + '0');
    year %= 100;
    buffer[written++] = (char) (year / 10 + '0');
    buffer[written++] = (char) (year % 10 + '0');

    return buffer.AsSpan(0, written);
  }
}
