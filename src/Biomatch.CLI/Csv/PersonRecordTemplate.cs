using System.Globalization;
using Biomatch.Domain.Models;
using nietras.SeparatedValues;

namespace Biomatch.CLI.Csv;

public static class PersonRecordTemplate
{
  public static IEnumerable<IPersonRecord> ParseCsv(string csvFilePath)
  {
    using var reader = Sep.New(',').Reader(o => o with { Unescape = true }).FromFile(csvFilePath);
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

  public static void WriteToCsv(IEnumerable<IPersonRecord> patientRecords, string csvFilePath)
  {
    using var writer = Sep.New(',').Writer().ToFile(csvFilePath);
    foreach (var patientRecord in patientRecords)
    {
      using var row = writer.NewRow();
      row["RecordId"].Set(patientRecord.RecordId);
      row["FirstName"].Set(patientRecord.FirstName);
      row["MiddleName"].Set(patientRecord.MiddleName);
      row["LastName"].Set(patientRecord.LastName);
      row["SecondLastName"].Set(patientRecord.SecondLastName);
      row["BirthDate"]
        .Set(
          patientRecord.BirthDate.HasValue
            ? patientRecord.BirthDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
            : string.Empty
        );
      row["City"].Set(patientRecord.City);
      row["PhoneNumber"].Set(patientRecord.PhoneNumber);
    }
  }
}
