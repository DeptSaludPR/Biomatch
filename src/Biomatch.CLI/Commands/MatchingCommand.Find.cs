using System.CommandLine;
using Biomatch.CLI.Progress;
using Biomatch.CLI.Services;
using Biomatch.Domain;
using Biomatch.CLI.Csv;
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

    var sameDataSetOption = new Option<bool>(
      name: "--same-data-set",
      description: "If true, records with the same ID will be considered as matches and skipped",
      getDefaultValue: () => false
    );

    var command = new Command("duplicates", "Find duplicates records in two files")
    {
      filePath1Argument,
      filePath2Argument,
      firstNamesDictionaryFilePathOption,
      middleNamesDictionaryFilePathOption,
      lastNamesDictionaryFilePathOption,
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
      firstNamesDictionaryFilePathOption,
      middleNamesDictionaryFilePathOption,
      lastNamesDictionaryFilePathOption,
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

    var command = new Command("matches", "Match records in two files")
    {
      filePath1Argument,
      filePath2Argument,
      firstNamesDictionaryFilePathOption,
      middleNamesDictionaryFilePathOption,
      lastNamesDictionaryFilePathOption,
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
      firstNamesDictionaryFilePathOption,
      middleNamesDictionaryFilePathOption,
      lastNamesDictionaryFilePathOption,
      outputOption,
      scoreOption,
      sameDataSetOption
    );

    return command;
  }
}
