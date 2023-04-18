using System.Text;
using Biomatch.Domain.Models;

namespace Biomatch.CLI.Csv;

public static class PatientRecordParser
{
  public static async IAsyncEnumerable<PatientRecord> ParseCsv(string csvFilePath)
  {
    using var reader = new StreamReader(File.OpenRead(csvFilePath));
    var headerLine = await reader.ReadLineAsync(); // Skip the header line

    while (!reader.EndOfStream)
    {
      var line = await reader.ReadLineAsync();
      if (string.IsNullOrWhiteSpace(line))
      {
        continue;
      }

      var valuesCount = CountCsvLineValues(line);
      while (valuesCount < 8 && !reader.EndOfStream)
      {
        var nextLine = await reader.ReadLineAsync();
        if (string.IsNullOrWhiteSpace(nextLine))
        {
          throw new Exception($"Missing values in line: {line}");
        }

        var nextLineValues = CountCsvLineValues(nextLine);
        if (nextLineValues >= 8)
        {
          throw new Exception($"Missing values in line: {line}");
        }
        line += nextLine;
        valuesCount = CountCsvLineValues(line);
      }

      if (valuesCount < 8)
      {
        throw new Exception($"Missing values in line: {line}");
      }

      var values = SplitCsvLine(line);


      var recordId = values[0];
      var firstName = values[1];
      var middleName = values[2];
      var lastName = values[3];
      var secondLastName = values[4];

      DateOnly? birthDate = null;
      if (!string.IsNullOrEmpty(values[5]))
      {
        birthDate = DateOnly.FromDateTime(DateTime.Parse(values[5]));
      }

      var city = values[6];
      var phoneNumber = values[7];

      var patientRecord = new PatientRecord(recordId, firstName, middleName, lastName, secondLastName, birthDate, city,
        phoneNumber);
      yield return patientRecord;
    }
  }

  private static int CountCsvLineValues(string line)
  {
    var values = 0;
    var inQuotes = false;

    foreach (var ch in line)
    {
      if (ch == '\"')
      {
        inQuotes = !inQuotes;
      }
      else if (ch == ',' && !inQuotes)
      {
        values++;
      }
    }

    return values + 1;
  }

  private static string[] SplitCsvLine(string line)
  {
    var values = new List<string>();
    var currentVal = new StringBuilder();
    var inQuotes = false;

    foreach (var ch in line)
    {
      if (ch == '\"')
      {
        inQuotes = !inQuotes;
      }
      else if (ch == ',' && !inQuotes)
      {
        values.Add(currentVal.ToString());
        currentVal.Clear();
      }
      else
      {
        currentVal.Append(ch);
      }
    }

    values.Add(currentVal.ToString());
    return values.ToArray();
  }
}
