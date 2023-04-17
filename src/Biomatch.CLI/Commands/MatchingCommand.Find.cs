using System.CommandLine;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Biomatch.CLI.Services;
using CsvHelper;
using Biomatch.Domain;
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
    var filePath1Argument = new Argument<FileInfo>
      (name: "templateFilePath1", description: "The path to the first file to be compared");

    var filePath2Argument = new Argument<FileInfo>
      (name: "templateFilePath2", description: "The path to the second file to be compared");

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
      getDefaultValue: () => new FileInfo("Duplicates.csv"));
    outputOption.AddAlias("-o");

    var scoreOption = new Option<double>
    (name: "--score",
      description: "Score for matching",
      getDefaultValue: () => 0.8);

    var logPathOption = new Option<FileInfo?>
    (name: "--log",
      description: "Log file path. If not provided log file will not be generated.");
    logPathOption.AddAlias("-l");

    var command = new Command("duplicates", "Find duplicates records in two files")
    {
      filePath1Argument,
      filePath2Argument,
      firstNamesDictionaryFilePathOption,
      middleNamesDictionaryFilePathOption,
      lastNamesDictionaryFilePathOption,
      outputOption,
      scoreOption,
      logPathOption
    };

    command.SetHandler(
      async (filePath1ArgumentValue, filePath2ArgumentValue, firstNamesDictionaryFilePathOptionValue,
        middleNamesDictionaryFilePathOptionValue, lastNamesDictionaryFilePathOptionValue, outputOptionValue,
        scoreOptionValue, logPathValue) =>
      {
        using var readerFile1 = new StreamReader(filePath1ArgumentValue.FullName);
        using var csvRecords1 = new CsvReader(readerFile1, CultureInfo.InvariantCulture);
        var records1FromCsv = csvRecords1.GetRecords<PatientRecord>();

        using var reader = new StreamReader(filePath2ArgumentValue.FullName);
        using var csvRecords2 = new CsvReader(reader, CultureInfo.InvariantCulture);
        var records2FromCsv = csvRecords2.GetRecords<PatientRecord>();

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

        await DuplicateService.RunFileComparisons(records1FromCsv, records2FromCsv,
          outputOptionValue, true,
          scoreOptionValue, firstNamesDictionary, middleNamesDictionary, lastNamesDictionary, logPathValue);
      },
      filePath1Argument, filePath2Argument, firstNamesDictionaryFilePathOption,
      middleNamesDictionaryFilePathOption, lastNamesDictionaryFilePathOption, outputOption, scoreOption,
      logPathOption);

    return command;
  }

  private static Command GetFindMatchesCommand()
  {
    var filePath1Argument = new Argument<FileInfo>
      (name: "templateFilePath1", description: "The path to the first file to be compared");

    var filePath2Argument = new Argument<FileInfo>
      (name: "templateFilePath2", description: "The path to the second file to be compared");

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
      getDefaultValue: () => new FileInfo("Duplicates.csv"));
    outputOption.AddAlias("-o");

    var scoreOption = new Option<double>
    (name: "--score",
      description: "Score for matching",
      getDefaultValue: () => 0.8);

    var command = new Command("matches", "Match records in two files")
    {
      filePath1Argument,
      filePath2Argument,
      firstNamesDictionaryFilePathOption,
      middleNamesDictionaryFilePathOption,
      lastNamesDictionaryFilePathOption,
      outputOption,
      scoreOption,
    };

    command.SetHandler(
      async (filePath1ArgumentValue, filePath2ArgumentValue, firstNamesDictionaryFilePathOptionValue,
        middleNamesDictionaryFilePathOptionValue, lastNamesDictionaryFilePathOptionValue, outputOptionValue,
        scoreOptionValue) =>
      {
        using var readerFile1 = new StreamReader(filePath1ArgumentValue.FullName);
        using var csvRecords1 = new CsvReader(readerFile1, CultureInfo.InvariantCulture);
        var records1FromCsv = csvRecords1.GetRecords<PatientRecord>().ToArray();

        using var reader = new StreamReader(filePath2ArgumentValue.FullName);
        using var csvRecords2 = new CsvReader(reader, CultureInfo.InvariantCulture);
        var records2FromCsv = csvRecords2.GetRecords<PatientRecord>();

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


        var totalRecordsToMatch = records1FromCsv.Length;
        var completedOperations = 0;
        var lastReportedPercentage = -1;
        var lastReportedTime = DateTime.MinValue;
        var cursorPosition = Console.GetCursorPosition();
        Console.ForegroundColor = ConsoleColor.Yellow;

        // Create a lock object for synchronization
        var progressLock = new object();

        // Initialize a Stopwatch to measure elapsed time
        var stopwatch = Stopwatch.StartNew();

        var progress = new Progress<int>(increment =>
        {
          lock (progressLock)
          {
            completedOperations += increment;
            var currentPercentage = (int) ((double) completedOperations / totalRecordsToMatch * 100);
            if (currentPercentage != lastReportedPercentage &&
                (DateTime.UtcNow - lastReportedTime).TotalSeconds >= 1)
            {
              lastReportedPercentage = currentPercentage;
              lastReportedTime = DateTime.UtcNow;
              var elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
              var estimatedTotalSeconds = elapsedSeconds * totalRecordsToMatch / completedOperations;
              var remainingSeconds = estimatedTotalSeconds - elapsedSeconds;

              var remainingTime = TimeSpan.FromSeconds(remainingSeconds);
              var remainingTimeString = $"{remainingTime.Minutes:D2}m {remainingTime.Seconds:D2}s";

              Console.SetCursorPosition(0, cursorPosition.Top);
              Console.Write($"Progress: {currentPercentage,3}% | Record match operations completed: {completedOperations}\n");
              Console.Write($"Total time: {stopwatch.Elapsed} | Estimated time remaining: {remainingTimeString}\n");
            }
          }
        });

        var possibleMatches = Match.TryMatchSingleRecord(records1FromCsv, records2FromCsv, scoreOptionValue,
          firstNamesDictionary, middleNamesDictionary, lastNamesDictionary, progress);

        Console.ResetColor();
        Console.WriteLine($"Elapsed time: {stopwatch.Elapsed}");
        await using var writer = new StreamWriter(outputOptionValue.FullName, false, Encoding.UTF8);
        await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        await csv.WriteRecordsAsync(possibleMatches);
      },
      filePath1Argument, filePath2Argument, firstNamesDictionaryFilePathOption,
      middleNamesDictionaryFilePathOption, lastNamesDictionaryFilePathOption, outputOption, scoreOption);

    return command;
  }
}
