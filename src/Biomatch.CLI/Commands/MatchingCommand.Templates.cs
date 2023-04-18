using System.CommandLine;
using Biomatch.CLI.Csv;
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
      GetPreprocessCommand()
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
        await PatientRecordWriter.WriteToCsv(Array.Empty<PatientRecord>(), outputOptionValue.FullName);
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
      async (templateFilePathArgumentValue) =>
      {
        try
        {
          var records1FromCsv = PatientRecordParser.ParseCsv(templateFilePathArgumentValue.FullName);
          List<PatientRecord> records = new();
          await foreach (var record in records1FromCsv)
          {
            records.Add(record);
          }
          Console.ForegroundColor = ConsoleColor.Green;
          Console.WriteLine($"Template file is valid, and contains {records.Count:N0} records.");
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
        var records1FromCsv = PatientRecordParser.ParseCsv(filePath1ArgumentValue.FullName);
        List<PatientRecord> records = new();
        await foreach (var record in records1FromCsv)
        {
          records.Add(record);
        }

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

        var processedRecords = records.PreprocessData(firstNamesDictionary, middleNamesDictionary, lastNamesDictionary);
        await PatientRecordWriter.WriteToCsv(processedRecords, outputOptionValue.FullName);
      },
      filePath1Argument, firstNamesDictionaryFilePathOption,
      middleNamesDictionaryFilePathOption, lastNamesDictionaryFilePathOption, outputOption);

    return command;
  }
}
