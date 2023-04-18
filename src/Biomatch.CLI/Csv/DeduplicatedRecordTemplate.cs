using System.Text;

namespace Biomatch.CLI.Csv;

public static class DeduplicatedRecordTemplate
{
  public static async Task WriteToCsv(IEnumerable<DeduplicatedRecord> deduplicatedRecords, string csvFilePath)
  {
    var csvContent = new StringBuilder();
    const string header = "RecordId,DuplicateRecordIds";
    csvContent.AppendLine(header);

    foreach (var deduplicatedRecord in deduplicatedRecords)
    {
      var values = new List<string>
      {
        deduplicatedRecord.RecordId,
        deduplicatedRecord.DuplicateRecordIds
      };

      var escapedValues = values.Select(value => value.Contains(',') ? $"\"{value}\"" : value);
      var line = string.Join(",", escapedValues);
      csvContent.AppendLine(line);
    }

    await File.WriteAllTextAsync(csvFilePath, csvContent.ToString());
  }
}
