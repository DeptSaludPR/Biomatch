using System.CommandLine;
using Biomatch.CLI.Csv;
using Biomatch.CLI.Options;
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

    var dictionaryOptions = GeneralOptions.GetDictionaryOptions();

    var command = new Command("deduplicate", "Deduplicate records from a template file")
    {
      filePathArgument,
      dictionaryOptions.FirstNamesDictionaryFilePathOption,
      dictionaryOptions.MiddleNamesDictionaryFilePathOption,
      dictionaryOptions.LastNamesDictionaryFilePathOption,
      outputOption,
      scoreOption,
    };

    command.SetHandler((
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

        return PersonRecordTemplate.WriteToCsv(deduplicatedRecords, outputOptionValue.FullName);
      },
      filePathArgument,
      dictionaryOptions.FirstNamesDictionaryFilePathOption,
      dictionaryOptions.MiddleNamesDictionaryFilePathOption,
      dictionaryOptions.LastNamesDictionaryFilePathOption,
      outputOption,
      scoreOption
    );

    return command;
  }
}
