using System.Globalization;
using System.Text;
using Biomatch.Domain.Models;

namespace Biomatch.CLI.Csv;

public static class PotentialMatchTemplate
{
  public static Task WriteToCsv(IEnumerable<PotentialMatch> potentialMatches, string csvFilePath)
  {
    var csvContent = new StringBuilder();
    const string header =
      "Value_RecordId,Value_FirstName,Value_MiddleName,Value_LastName,Value_SecondLastName,Value_BirthDate,Value_City,Value_PhoneNumber,"
      + "Match_RecordId,Match_FirstName,Match_MiddleName,Match_LastName,Match_SecondLastName,Match_BirthDate,Match_City,Match_PhoneNumber,"
      + "FirstNameDistance,MiddleNameDistance,LastNameDistance,SecondLastNameDistance,BirthDateDistance,CityDistance,PhoneNumberDistance,"
      + "Score";
    csvContent.AppendLine(header);

    foreach (var potentialMatch in potentialMatches)
    {
      var value = potentialMatch.Value;
      var match = potentialMatch.Match;
      var distance = potentialMatch.Distance;
      var score = potentialMatch.Score;

      var valueBirthDate = value.BirthDate.HasValue
        ? value.BirthDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
        : string.Empty;

      var matchBirthDate = match.BirthDate.HasValue
        ? match.BirthDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
        : string.Empty;

      var values = new List<string>
      {
        value.RecordId,
        value.FirstName,
        value.MiddleName,
        value.LastName,
        value.SecondLastName,
        valueBirthDate,
        value.City,
        value.PhoneNumber,
        match.RecordId,
        match.FirstName,
        match.MiddleName,
        match.LastName,
        match.SecondLastName,
        matchBirthDate,
        match.City,
        match.PhoneNumber,
        distance.FirstNameDistance.ToString(),
        distance.MiddleNameDistance.ToString(),
        distance.LastNameDistance.ToString(),
        distance.SecondLastNameDistance.ToString(),
        distance.BirthDateDistance.ToString(),
        distance.CityDistance.ToString(),
        distance.PhoneNumberDistance.ToString(),
        score.ToString(CultureInfo.InvariantCulture)
      };

      var escapedValues = values.Select(v => v.Contains(',') ? $"\"{v}\"" : v);
      var line = string.Join(",", escapedValues);
      csvContent.AppendLine(line);
    }

    return File.WriteAllTextAsync(csvFilePath, csvContent.ToString());
  }
}
