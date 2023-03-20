using System.CommandLine;
using System.Globalization;
using System.Text;
using CsvHelper;
using Biomatch.Domain;
using Biomatch.Domain.Models;

namespace Biomatch.CLI.Commands;

public static partial class MatchingCommand
{
  public static Command GetTemplateCommand()
  {
    var command = new Command("template", "Template operations")
    {
      GetTemplateGenerateCommand(),
      GetTemplateValidateCommand(),
      GetPreprocessCommand(),
      GetTemplateDifferenceCommand()
    };

    return command;
  }

  private static Command GetTemplateGenerateCommand()
  {
    var outputOption = new Option<FileInfo>
    (name: "--output", description: "Output file path",
      getDefaultValue: () => new FileInfo("MatchingTemplate.csv"));
    outputOption.AddAlias("-o");

    var command = new Command("generate", "Creates an empty template file")
    {
      outputOption,
    };

    command.SetHandler(
      async (outputOptionValue) =>
      {
        await using var writer = new StreamWriter(outputOptionValue.FullName, false, Encoding.UTF8);
        await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.WriteHeader<PatientRecord>();
      }, outputOption);

    return command;
  }

  private static Command GetTemplateValidateCommand()
  {
    var templateFilePathArgument = new Argument<FileInfo>
      (name: "templateFilePath", description: "The path of the file to be validated");

    var command = new Command("validate", "Validates template file format")
    {
      templateFilePathArgument,
    };

    command.SetHandler(
      (templateFilePathArgumentValue) =>
      {
        try
        {
          using var readerFile1 = new StreamReader(templateFilePathArgumentValue.FullName);
          using var csvRecords1 = new CsvReader(readerFile1, CultureInfo.InvariantCulture);
          var records = csvRecords1.GetRecords<PatientRecord>();
          Console.ForegroundColor = ConsoleColor.Green;
          Console.WriteLine($"Template file is valid, and contains {records.Count():N0} records.");
        }
        catch (Exception)
        {
          Console.ForegroundColor = ConsoleColor.Red;
          Console.WriteLine(
            "Template file is invalid, use the template generate command to create a valid template file.");
        }
      }, templateFilePathArgument);

    return command;
  }

  private static Command GetPreprocessCommand()
  {
    var filePath1Argument = new Argument<FileInfo>
      (name: "templateFilePath", description: "The path of the file to be preprocessed");

    var firstNamesDictionaryFilePathOption = new Option<FileInfo>
    (name: "-dictionary-first-names", description: "First names dictionary file path",
      getDefaultValue: () => new FileInfo("Dictionaries/FirstNamesDictionary.txt"));
    firstNamesDictionaryFilePathOption.AddAlias("-df");

    var middleNamesDictionaryFilePathOption = new Option<FileInfo>
    (name: "-dictionary-middle-names", description: "Middle names dictionary file path",
      getDefaultValue: () => new FileInfo("Dictionaries/MiddleNamesDictionary.txt"));
    middleNamesDictionaryFilePathOption.AddAlias("-dm");

    var lastNamesDictionaryFilePathOption = new Option<FileInfo>
    (name: "-dictionary-last-names", description: "Last names dictionary file path",
      getDefaultValue: () => new FileInfo("Dictionaries/LastNamesDictionary.txt"));
    lastNamesDictionaryFilePathOption.AddAlias("-dl");

    var outputOption = new Option<FileInfo>
    (name: "--output", description: "Output file path",
      getDefaultValue: () => new FileInfo("ProcessedRecords.csv"));
    outputOption.AddAlias("-o");

    var command = new Command("preprocess", "Executes the preprocessing pipeline on a template")
    {
      filePath1Argument,
      firstNamesDictionaryFilePathOption,
      middleNamesDictionaryFilePathOption,
      lastNamesDictionaryFilePathOption,
      outputOption,
    };

    command.SetHandler(
      async (filePath1ArgumentValue, firstNamesDictionaryFilePathOptionValue,
        middleNamesDictionaryFilePathOptionValue, lastNamesDictionaryFilePathOptionValue, outputOptionValue) =>
      {
        using var readerFile1 = new StreamReader(filePath1ArgumentValue.FullName, Encoding.UTF8);
        using var csvRecords1 = new CsvReader(readerFile1, CultureInfo.InvariantCulture);
        var records1FromCsv = csvRecords1.GetRecords<PatientRecord>().ToList();

        WordDictionary? firstNamesDictionary = null;
        if (firstNamesDictionaryFilePathOptionValue.Exists)
        {
          firstNamesDictionary = new WordDictionary(firstNamesDictionaryFilePathOptionValue);
          Console.WriteLine(
            $"FirstNames dictionary loaded from {firstNamesDictionaryFilePathOptionValue}");
        }
        else
        {
          Console.ForegroundColor = ConsoleColor.Red;
          Console.WriteLine(
            "FirstNames dictionary not loaded, generate dictionary files to improve preprocessing.");
          Console.ResetColor();
        }

        WordDictionary? middleNamesDictionary = null;
        if (middleNamesDictionaryFilePathOptionValue.Exists)
        {
          middleNamesDictionary = new WordDictionary(middleNamesDictionaryFilePathOptionValue);
          Console.WriteLine(
            $"MiddleNames dictionary loaded from {middleNamesDictionaryFilePathOptionValue}");
        }
        else
        {
          Console.ForegroundColor = ConsoleColor.Red;
          Console.WriteLine(
            "MiddleNames dictionary not loaded, generate dictionary files to improve preprocessing.");
          Console.ResetColor();
        }

        WordDictionary? lastNamesDictionary = null;
        if (lastNamesDictionaryFilePathOptionValue.Exists)
        {
          lastNamesDictionary = new WordDictionary(lastNamesDictionaryFilePathOptionValue);
          Console.WriteLine(
            $"LastNames dictionary loaded from {lastNamesDictionaryFilePathOptionValue}");
        }
        else
        {
          Console.ForegroundColor = ConsoleColor.Red;
          Console.WriteLine(
            "LastNames dictionary not loaded, generate dictionary files to improve preprocessing.");
          Console.ResetColor();
        }

        var processedRecords =
          records1FromCsv.PreprocessData(firstNamesDictionary, middleNamesDictionary, lastNamesDictionary);

        await using var writer = new StreamWriter(outputOptionValue.FullName, false, Encoding.UTF8);
        await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        await csv.WriteRecordsAsync(processedRecords);
      },
      filePath1Argument, firstNamesDictionaryFilePathOption,
      middleNamesDictionaryFilePathOption, lastNamesDictionaryFilePathOption, outputOption);

    return command;
  }

  private static Command GetTemplateDifferenceCommand()
  {
    var filePathTemplate1Argument = new Argument<FileInfo>
      (name: "templateFilePath1", description: "The path of the first file");

    var filePathTemplate2Argument = new Argument<FileInfo>
      (name: "templateFilePath2", description: "The path of the second file");

    var outputOption = new Option<FileInfo>
    (name: "--output", description: "Output file path",
      getDefaultValue: () => new FileInfo("Differences.csv"));
    outputOption.AddAlias("-o");

    var command = new Command("difference", "Gets the difference between two templates duplicate records")
    {
      filePathTemplate1Argument,
      filePathTemplate2Argument,
      outputOption,
    };

    command.SetHandler(
      async (filePathTemplate1ArgumentValue, filePathTemplate2ArgumentValue, outputOptionValue) =>
      {
        using var readerFile1 = new StreamReader(filePathTemplate1ArgumentValue.FullName);
        using var csvRecords1 = new CsvReader(readerFile1, CultureInfo.InvariantCulture);
        var records1FromCsv = csvRecords1.GetRecords<PatientRecord>().ToList();

        using var reader = new StreamReader(filePathTemplate2ArgumentValue.FullName);
        using var csvRecords2 = new CsvReader(reader, CultureInfo.InvariantCulture);
        var records2FromCsv = csvRecords2.GetRecords<PatientRecord>().ToList();

        var differences = records1FromCsv.Except(records2FromCsv);

        var newDifferences = new List<PatientRecordForDifference>();
        foreach (var difference in differences)
        {
          var differentRecord = records2FromCsv.Find(e => e.RecordId == difference.RecordId);
          if (differentRecord != null)
          {
            newDifferences.Add(new PatientRecordForDifference
            (
              difference.RecordId,
              difference.FirstName != differentRecord.FirstName
                ? $"{difference.FirstName} -> {differentRecord.FirstName}"
                : difference.FirstName,
              difference.MiddleName != differentRecord.MiddleName
                ? $"{difference.MiddleName} -> {differentRecord.MiddleName}"
                : difference.MiddleName,
              difference.LastName != differentRecord.LastName
                ? $"{difference.LastName} -> {differentRecord.LastName}"
                : difference.LastName,
              difference.SecondLastName != differentRecord.SecondLastName
                ? $"{difference.SecondLastName} -> {differentRecord.SecondLastName}"
                : difference.SecondLastName,
              difference.BirthDate != differentRecord.BirthDate
                ? $"{difference.BirthDate} -> {differentRecord.BirthDate}"
                : difference.BirthDate?.ToShortDateString() ?? "",
              difference.City != differentRecord.City
                ? $"{difference.City} -> {differentRecord.City}"
                : difference.City,
              difference.PhoneNumber != differentRecord.PhoneNumber
                ? $"{difference.PhoneNumber} -> {differentRecord.PhoneNumber}"
                : difference.PhoneNumber
            ));
          }
        }

        await using var writer = new StreamWriter(outputOptionValue.FullName, false, Encoding.UTF8);
        await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        await csv.WriteRecordsAsync(newDifferences);
      }, filePathTemplate1Argument, filePathTemplate2Argument, outputOption);

    return command;
  }
}
