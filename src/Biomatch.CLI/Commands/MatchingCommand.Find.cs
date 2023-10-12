using System.CommandLine;
using Biomatch.CLI.Progress;
using Biomatch.CLI.Services;
using Biomatch.Domain;
using Biomatch.CLI.Csv;
using Biomatch.CLI.Options;
using Biomatch.Domain.Models;

namespace Biomatch.CLI.Commands;

public static partial class MatchingCommand
{
  public static Command GetFindCommand()
  {
    var command = new Command("find", "Find operations")
    {
      GetFindMatchesCommand(),
      GetFindDuplicatesCommand(),
    };

    return command;
  }

  private static Command GetFindDuplicatesCommand()
  {
    var filePath1Argument = new Argument<FileInfo>(
      name: "templateFilePath1",
      description: "The path to the first file to be compared"
    );

    var filePath2Argument = new Argument<FileInfo>(
      name: "templateFilePath2",
      description: "The path to the second file to be compared"
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

    var sameDataSetOption = new Option<bool>(
      name: "--same-data-set",
      description: "If true, records with the same ID will be considered as matches and skipped",
      getDefaultValue: () => false
    );

    var dictionaryOptions = GeneralOptions.GetDictionaryOptions();

    var command = new Command("duplicates", "Find duplicates records in two files")
    {
      filePath1Argument,
      filePath2Argument,
      dictionaryOptions.FirstNamesDictionaryFilePathOption,
      dictionaryOptions.MiddleNamesDictionaryFilePathOption,
      dictionaryOptions.LastNamesDictionaryFilePathOption,
      outputOption,
      scoreOption,
      sameDataSetOption,
    };

    command.SetHandler(
      async (
        filePath1ArgumentValue,
        filePath2ArgumentValue,
        firstNamesDictionaryFilePathOptionValue,
        middleNamesDictionaryFilePathOptionValue,
        lastNamesDictionaryFilePathOptionValue,
        outputOptionValue,
        scoreOptionValue,
        sameDataSetOptionValue
      ) =>
      {
        var records1FromCsv = PersonRecordTemplate.ParseCsv(filePath1ArgumentValue.FullName);
        List<IPersonRecord> records1 = new();
        foreach (var record in records1FromCsv)
        {
          records1.Add(record);
        }

        var records2FromCsv = PersonRecordTemplate.ParseCsv(filePath2ArgumentValue.FullName);
        List<IPersonRecord> records2 = new();
        foreach (var record in records2FromCsv)
        {
          records2.Add(record);
        }

        var (firstNamesDictionary, middleNamesDictionary, lastNamesDictionary) =
          DictionaryLoader.LoadDictionaries(
            firstNamesDictionaryFilePathOptionValue,
            middleNamesDictionaryFilePathOptionValue,
            lastNamesDictionaryFilePathOptionValue
          );

        var potentialMatches = DuplicateService.RunFileComparisons(
          records1,
          records2,
          true,
          scoreOptionValue,
          firstNamesDictionary,
          middleNamesDictionary,
          lastNamesDictionary,
          sameDataSetOptionValue,
          MatchingProgress.GetMatchingProgressReport
        );

        var potentialDuplicatesWorkItem = new List<DuplicateRecord>();
        foreach (var potentialMatch in potentialMatches)
        {
          potentialDuplicatesWorkItem.Add(new DuplicateRecord(potentialMatch));
        }

        // Write output to file
        await DuplicateRecordTemplate.WriteToCsv(
          potentialDuplicatesWorkItem,
          outputOptionValue.FullName
        );
      },
      filePath1Argument,
      filePath2Argument,
      dictionaryOptions.FirstNamesDictionaryFilePathOption,
      dictionaryOptions.MiddleNamesDictionaryFilePathOption,
      dictionaryOptions.LastNamesDictionaryFilePathOption,
      outputOption,
      scoreOption,
      sameDataSetOption
    );

    return command;
  }

  private static Command GetFindMatchesCommand()
  {
    var filePath1Argument = new Argument<FileInfo>(
      name: "templateFilePath1",
      description: "The path to the first file to be compared"
    );

    var filePath2Argument = new Argument<FileInfo>(
      name: "templateFilePath2",
      description: "The path to the second file to be compared"
    );

    var outputOption = new Option<FileInfo>(
      name: "--output",
      description: "Output file path",
      getDefaultValue: () => new FileInfo("Matches.csv")
    );
    outputOption.AddAlias("-o");

    var scoreOption = new Option<double>(
      name: "--score",
      description: "Score for matching",
      getDefaultValue: () => 0.85
    );

    var sameDataSetOption = new Option<bool>(
      name: "--same-data-set",
      description: "If true, records with the same ID will be considered as matches and skipped",
      getDefaultValue: () => false
    );

    var dictionaryOptions = GeneralOptions.GetDictionaryOptions();

    var command = new Command("matches", "Match records in two files")
    {
      filePath1Argument,
      filePath2Argument,
      dictionaryOptions.FirstNamesDictionaryFilePathOption,
      dictionaryOptions.MiddleNamesDictionaryFilePathOption,
      dictionaryOptions.LastNamesDictionaryFilePathOption,
      outputOption,
      scoreOption,
      sameDataSetOption
    };

    command.SetHandler(
      async (
        filePath1ArgumentValue,
        filePath2ArgumentValue,
        firstNamesDictionaryFilePathOptionValue,
        middleNamesDictionaryFilePathOptionValue,
        lastNamesDictionaryFilePathOptionValue,
        outputOptionValue,
        scoreOptionValue,
        sameDataSetOptionValue
      ) =>
      {
        var records1FromCsv = PersonRecordTemplate
          .ParseCsv(filePath1ArgumentValue.FullName)
          .ToArray();
        var records2FromCsv = PersonRecordTemplate.ParseCsv(filePath2ArgumentValue.FullName);

        var (firstNamesDictionary, middleNamesDictionary, lastNamesDictionary) =
          DictionaryLoader.LoadDictionaries(
            firstNamesDictionaryFilePathOptionValue,
            middleNamesDictionaryFilePathOptionValue,
            lastNamesDictionaryFilePathOptionValue
          );

        var possibleMatches = Match.FindBestMatches(
          records1FromCsv,
          records2FromCsv,
          scoreOptionValue,
          firstNamesDictionary,
          middleNamesDictionary,
          lastNamesDictionary,
          sameDataSetOptionValue,
          MatchingProgress.GetMatchingProgressReport
        );

        await PotentialMatchTemplate.WriteToCsv(possibleMatches, outputOptionValue.FullName);
      },
      filePath1Argument,
      filePath2Argument,
      dictionaryOptions.FirstNamesDictionaryFilePathOption,
      dictionaryOptions.MiddleNamesDictionaryFilePathOption,
      dictionaryOptions.LastNamesDictionaryFilePathOption,
      outputOption,
      scoreOption,
      sameDataSetOption
    );

    return command;
  }
}
