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
      getDefaultValue: () => new FileInfo("Deduplicated.csv")
    );
    outputOption.AddAlias("-o");

    var scoreOption = new Option<double>(
      name: "--score",
      description: "Score for matching",
      getDefaultValue: () => 0.85
    );

    var mapOption = new Option<bool>(name: "--map", description: "Generate duplicate map");
    mapOption.AddAlias("-m");

    var outputMapOption = new Option<FileInfo>(
      name: "--output-map",
      description: "File location for duplicate map",
      getDefaultValue: () => new FileInfo("DuplicateMap.csv")
    );
    outputMapOption.AddAlias("-om");

    var dictionaryOptions = GeneralOptions.GetDictionaryOptions();

    var command = new Command("deduplicate", "Deduplicate records from a template file")
    {
      filePathArgument,
      dictionaryOptions.FirstNamesDictionaryFilePathOption,
      dictionaryOptions.MiddleNamesDictionaryFilePathOption,
      dictionaryOptions.LastNamesDictionaryFilePathOption,
      outputOption,
      scoreOption,
      mapOption,
      outputMapOption,
    };

    command.SetHandler(
      (
        filePathArgumentValue,
        firstNamesDictionaryFilePathOptionValue,
        middleNamesDictionaryFilePathOptionValue,
        lastNamesDictionaryFilePathOptionValue,
        outputOptionValue,
        scoreOptionValue,
        mapOptionValue,
        outputMapOptionValue
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

        var preprocessedRecords = records1FromCsv
          .PreprocessData(firstNamesDictionary, middleNamesDictionary, lastNamesDictionary)
          .ToArray();

        var potentialDuplicates = Match.GetPotentialMatchesFromSameDataSet(
          preprocessedRecords,
          preprocessedRecords,
          scoreOptionValue,
          1.0,
          MatchingProgress.GetMatchingProgressReport
        );

        var deduplicatedRecords = Deduplicate.TryDeduplicate(
          preprocessedRecords,
          potentialDuplicates
        );

        if (mapOptionValue)
        {
          PotentialMatchTemplate.WriteToCsv(potentialDuplicates, outputMapOptionValue.FullName);
        }

        PersonRecordTemplate.WriteToCsv(deduplicatedRecords, outputOptionValue.FullName);
      },
      filePathArgument,
      dictionaryOptions.FirstNamesDictionaryFilePathOption,
      dictionaryOptions.MiddleNamesDictionaryFilePathOption,
      dictionaryOptions.LastNamesDictionaryFilePathOption,
      outputOption,
      scoreOption,
      mapOption,
      outputMapOption
    );

    return command;
  }
}
