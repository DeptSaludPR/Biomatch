using System.CommandLine;
using Biomatch.CLI.Csv;
using Biomatch.CLI.Progress;
using Biomatch.CLI.Services;
using Biomatch.Domain;

namespace Biomatch.CLI.Commands;

public partial class MatchingCommand
{
  public static Command GetDeduplicateCommand()
  {
    return GetDeduplicateTemplateCommand();
  }

  private static Command GetDeduplicateTemplateCommand()
  {
    var filePathArgument = new Argument<FileInfo>(
      name: "templateFilePath",
      description: "The path to the file to be deduplicated"
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
      getDefaultValue: () => new FileInfo("Duplicates.csv")
    );
    outputOption.AddAlias("-o");

    var scoreOption = new Option<double>(
      name: "--score",
      description: "Score for matching",
      getDefaultValue: () => 0.85
    );

    var command = new Command("deduplicate", "Deduplicate records from a template file")
    {
      filePathArgument,
      firstNamesDictionaryFilePathOption,
      middleNamesDictionaryFilePathOption,
      lastNamesDictionaryFilePathOption,
      outputOption,
      scoreOption,
    };

    command.SetHandler(
      (
        filePathArgumentValue,
        firstNamesDictionaryFilePathOptionValue,
        middleNamesDictionaryFilePathOptionValue,
        lastNamesDictionaryFilePathOptionValue,
        outputOptionValue,
        scoreOptionValue
      ) =>
      {
        var records1FromCsv = PersonRecordTemplate
          .ParseCsv(filePathArgumentValue.FullName)
          .ToArray();

        var (firstNamesDictionary, middleNamesDictionary, lastNamesDictionary) =
          DictionaryLoader.LoadDictionaries(
            firstNamesDictionaryFilePathOptionValue,
            middleNamesDictionaryFilePathOptionValue,
            lastNamesDictionaryFilePathOptionValue
          );

        var deduplicatedRecords = Deduplicate.TryDeduplicate(
          records1FromCsv,
          scoreOptionValue,
          firstNamesDictionary,
          middleNamesDictionary,
          lastNamesDictionary,
          MatchingProgress.GetMatchingProgressReport
        );

        DeduplicatedRecordTemplate.WriteToCsv(deduplicatedRecords, outputOptionValue.FullName);
      },
      filePathArgument,
      firstNamesDictionaryFilePathOption,
      middleNamesDictionaryFilePathOption,
      lastNamesDictionaryFilePathOption,
      outputOption,
      scoreOption
    );

    return command;
  }
}
