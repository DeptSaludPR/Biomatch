using System.Globalization;
using Biomatch.Domain.Models;
using nietras.SeparatedValues;

namespace Biomatch.CLI.Csv;

public static class PotentialMatchTemplate
{
  public static void WriteToCsv(IEnumerable<PotentialMatch> potentialMatches, string csvFilePath)
  {
    using var writer = Sep.New(',').Writer().ToFile(csvFilePath);

    foreach (var potentialMatch in potentialMatches)
    {
      using var row = writer.NewRow();
      row["Value_RecordId"].Set(potentialMatch.Value.RecordId);
      row["Value_FirstName"].Set(potentialMatch.Value.FirstName);
      row["Value_MiddleName"].Set(potentialMatch.Value.MiddleName);
      row["Value_LastName"].Set(potentialMatch.Value.LastName);
      row["Value_SecondLastName"].Set(potentialMatch.Value.SecondLastName);
      row["Value_BirthDate"]
        .Set(
          potentialMatch.Value.BirthDate.HasValue
            ? potentialMatch.Value.BirthDate.Value.ToString(
              "yyyy-MM-dd",
              CultureInfo.InvariantCulture
            )
            : string.Empty
        );
      row["Value_City"].Set(potentialMatch.Value.City);
      row["Value_PhoneNumber"].Set(potentialMatch.Value.PhoneNumber);
      row["Match_RecordId"].Set(potentialMatch.Match.RecordId);
      row["Match_FirstName"].Set(potentialMatch.Match.FirstName);
      row["Match_MiddleName"].Set(potentialMatch.Match.MiddleName);
      row["Match_LastName"].Set(potentialMatch.Match.LastName);
      row["Match_SecondLastName"].Set(potentialMatch.Match.SecondLastName);
      row["Match_BirthDate"]
        .Set(
          potentialMatch.Match.BirthDate.HasValue
            ? potentialMatch.Match.BirthDate.Value.ToString(
              "yyyy-MM-dd",
              CultureInfo.InvariantCulture
            )
            : string.Empty
        );
      row["Match_City"].Set(potentialMatch.Match.City);
      row["Match_PhoneNumber"].Set(potentialMatch.Match.PhoneNumber);
      row["FirstNameDistance"].Set(potentialMatch.Distance.FirstNameDistance.ToString());
      row["MiddleNameDistance"].Set(potentialMatch.Distance.MiddleNameDistance.ToString());
      row["LastNameDistance"].Set(potentialMatch.Distance.LastNameDistance.ToString());
      row["SecondLastNameDistance"].Set(potentialMatch.Distance.SecondLastNameDistance.ToString());
      row["BirthDateDistance"].Set(potentialMatch.Distance.BirthDateDistance.ToString());
      row["CityDistance"].Set(potentialMatch.Distance.CityDistance.ToString());
      row["PhoneNumberDistance"].Set(potentialMatch.Distance.PhoneNumberDistance.ToString());
      row["Score"].Set(potentialMatch.Score.ToString(CultureInfo.InvariantCulture));
    }
  }
}
