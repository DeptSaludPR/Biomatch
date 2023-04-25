using System.Globalization;
using System.Text;
using Biomatch.Domain.Models;

namespace Biomatch.CLI.Csv;

public static class PersonRecordWriter
{
  public static async Task WriteToCsv(IEnumerable<IPersonRecord> patientRecords, string csvFilePath)
  {
    var csvContent = new StringBuilder();
    const string header = "RecordId,FirstName,MiddleName,LastName,SecondLastName,BirthDate,City,PhoneNumber";
    csvContent.AppendLine(header);

    foreach (var patientRecord in patientRecords)
    {
      var birthDate = patientRecord.BirthDate.HasValue
        ? patientRecord.BirthDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
        : string.Empty;

      var values = new List<string>
      {
        patientRecord.RecordId,
        patientRecord.FirstName,
        patientRecord.MiddleName,
        patientRecord.LastName,
        patientRecord.SecondLastName,
        birthDate,
        patientRecord.City,
        patientRecord.PhoneNumber
      };

      var escapedValues = values.Select(value => value.Contains(',') ? $"\"{value}\"" : value);
      var line = string.Join(",", escapedValues);
      csvContent.AppendLine(line);
    }

    await File.WriteAllTextAsync(csvFilePath, csvContent.ToString());
  }
}
