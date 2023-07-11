using nietras.SeparatedValues;

namespace Biomatch.CLI.Csv;

public static class DeduplicatedRecordTemplate
{
  public static void WriteToCsv(Dictionary<string, string> deduplicatedRecords, string csvFilePath)
  {
    using var writer = Sep.New(',').Writer().ToFile(csvFilePath);

    foreach (var deduplicatedRecord in deduplicatedRecords)
    {
      using var row = writer.NewRow();
      row["OriginalRecordId"].Set(deduplicatedRecord.Key);
      row["UniqueMatchRecordId"].Set(deduplicatedRecord.Value);
    }
  }
}
