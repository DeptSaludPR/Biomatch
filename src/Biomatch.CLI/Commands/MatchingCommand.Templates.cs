using System.CommandLine;
using Biomatch.CLI.Csv;
using Biomatch.CLI.Services;
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
    };

    return command;
  }

  private static Command GetTemplateGenerateCommand()
  {
    var outputOption = new Option<FileInfo>(
      name: "--output",
      description: "Output file path",
      getDefaultValue: () => new FileInfo("MatchingTemplate.csv")
    );
    outputOption.AddAlias("-o");

    var command = new Command("generate", "Creates an empty template file") { outputOption, };

    command.SetHandler(
      async outputOptionValue =>
      {
        await PersonRecordTemplate.WriteToCsv(
          Array.Empty<IPersonRecord>(),
          outputOptionValue.FullName
        );
      },
      outputOption
    );

    return command;
  }

  private static Command GetTemplateValidateCommand()
  {
    var templateFilePathArgument = new Argument<FileInfo>(
      name: "templateFilePath",
      description: "The path of the file to be validated"
    );

    var command = new Command("validate", "Validates template file format")
    {
      templateFilePathArgument,
    };

    command.SetHandler(
      templateFilePathArgumentValue =>
      {
        try
        {
          var recordsFromCsv = PersonRecordTemplate
            .ParseCsv(templateFilePathArgumentValue.FullName)
            .ToArray();

          Console.ForegroundColor = ConsoleColor.Green;
          Console.WriteLine(
            $"Template file is valid, and contains {recordsFromCsv.Length:N0} records."
          );
        }
        catch (Exception)
        {
          Console.ForegroundColor = ConsoleColor.Red;
          Console.WriteLine(
            "Template file is invalid, use the template generate command to create a valid template file."
          );
        }
      },
      templateFilePathArgument
    );

    return command;
  }

  private static Command GetPreprocessCommand()
  {
    var filePath1Argument = new Argument<FileInfo>(
      name: "templateFilePath",
      description: "The path of the file to be preprocessed"
    );

    var firstNamesDictionaryFilePathOption = new Option<FileInfo>(
      name: "-dictionary-first-names",
      description: "First names dictionary file path",
      getDefaultValue: () => new FileInfo("Dictionaries/FirstNamesDictionary.txt")
    );
    firstNamesDictionaryFilePathOption.AddAlias("-df");

    var middleNamesDictionaryFilePathOption = new Option<FileInfo>(
      name: "-dictionary-middle-names",
      description: "Middle names dictionary file path",
      getDefaultValue: () => new FileInfo("Dictionaries/MiddleNamesDictionary.txt")
    );
    middleNamesDictionaryFilePathOption.AddAlias("-dm");

    var lastNamesDictionaryFilePathOption = new Option<FileInfo>(
      name: "-dictionary-last-names",
      description: "Last names dictionary file path",
      getDefaultValue: () => new FileInfo("Dictionaries/LastNamesDictionary.txt")
    );
    lastNamesDictionaryFilePathOption.AddAlias("-dl");

    var outputOption = new Option<FileInfo>(
      name: "--output",
      description: "Output file path",
      getDefaultValue: () => new FileInfo("ProcessedRecords.csv")
    );
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
      async (
        filePath1ArgumentValue,
        firstNamesDictionaryFilePathOptionValue,
        middleNamesDictionaryFilePathOptionValue,
        lastNamesDictionaryFilePathOptionValue,
        outputOptionValue
      ) =>
      {
        var records1FromCsv = PersonRecordTemplate.ParseCsv(filePath1ArgumentValue.FullName);
        List<IPersonRecord> records = new();
        foreach (var record in records1FromCsv)
        {
          records.Add(record);
        }

        var (firstNamesDictionary, middleNamesDictionary, lastNamesDictionary) =
          DictionaryLoader.LoadDictionaries(
            firstNamesDictionaryFilePathOptionValue,
            middleNamesDictionaryFilePathOptionValue,
            lastNamesDictionaryFilePathOptionValue
          );

        var processedRecords = records.PreprocessData(
          firstNamesDictionary,
          middleNamesDictionary,
          lastNamesDictionary
        );
        await PersonRecordTemplate.WriteToCsv(
          processedRecords.OfType<IPersonRecord>(),
          outputOptionValue.FullName
        );
      },
      filePath1Argument,
      firstNamesDictionaryFilePathOption,
      middleNamesDictionaryFilePathOption,
      lastNamesDictionaryFilePathOption,
      outputOption
    );

    return command;
  }
}
