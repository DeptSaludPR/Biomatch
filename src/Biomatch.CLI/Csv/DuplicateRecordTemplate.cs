using System.Globalization;
using System.Text;
using Biomatch.Domain.Models;

namespace Biomatch.CLI.Csv;

public static class DuplicateRecordTemplate
{
  public static Task WriteToCsv(IEnumerable<DuplicateRecord> duplicateRecords, string csvFilePath)
  {
    var csvContent = new StringBuilder();
    const string header =
      "Patient1Url,Patient2Url,Score,Distance,DuplicateStatus,Error1,Error2,Error3,ProfileModified,ProfileMerged,User,Date";
    csvContent.AppendLine(header);

    foreach (var duplicateRecord in duplicateRecords)
    {
      var values = new List<string>
      {
        duplicateRecord.Patient1Url,
        duplicateRecord.Patient2Url,
        duplicateRecord.Score.ToString(CultureInfo.InvariantCulture),
        duplicateRecord.Distance,
        duplicateRecord.DuplicateStatus,
        duplicateRecord.Error1,
        duplicateRecord.Error2,
        duplicateRecord.Error3,
        duplicateRecord.ProfileModified,
        duplicateRecord.ProfileMerged,
        duplicateRecord.User,
        duplicateRecord.Date,
      };

      var escapedValues = values.Select(value => value.Contains(',') ? $"\"{value}\"" : value);
      var line = string.Join(",", escapedValues);
      csvContent.AppendLine(line);
    }

    return File.WriteAllTextAsync(csvFilePath, csvContent.ToString());
  }
}
