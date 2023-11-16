using System.Globalization;
using System.Text;
using Biomatch.Domain.Models;
using nietras.SeparatedValues;

namespace Biomatch.CLI.Csv;

public static class PersonRecordTemplate
{
  public static IEnumerable<IPersonRecord> ParseCsv(string csvFilePath)
  {
    using var reader = Sep.New(',').Reader(o => o with {  }).FromFile(csvFilePath);
    foreach (var readRow in reader)
    {
      yield return new PersonRecord(
        readRow["RecordId"].ToString(),
        readRow["FirstName"].ToString(),
        readRow["MiddleName"].ToString(),
        readRow["LastName"].ToString(),
        readRow["SecondLastName"].ToString(),
        readRow["BirthDate"].TryParse<DateOnly>(),
        readRow["City"].ToString(),
        readRow["PhoneNumber"].ToString()
      );
    }
  }

  public static Task WriteToCsv(IEnumerable<IPersonRecord> patientRecords, string csvFilePath)
  {
    var csvContent = new StringBuilder();
    const string header =
      "RecordId,FirstName,MiddleName,LastName,SecondLastName,BirthDate,City,PhoneNumber";
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

    return File.WriteAllTextAsync(csvFilePath, csvContent.ToString());
  }
}
