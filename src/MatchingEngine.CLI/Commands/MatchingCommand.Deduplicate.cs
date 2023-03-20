using System.CommandLine;
using System.Globalization;
using System.Text;
using CsvHelper;
using MatchingEngine.CLI.Csv;
using MatchingEngine.Domain;
using MatchingEngine.Domain.Models;

namespace MatchingEngine.CLI.Commands;

public partial class MatchingCommand
{
  public static Command GetDeduplicateCommand()
  {
    return GetDeduplicateTemplateCommand();
  }

  private static Command GetDeduplicateTemplateCommand()
  {
    var filePathArgument = new Argument<FileInfo>
      (name: "templateFilePath", description: "The path to the file to be deduplicated");

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
      getDefaultValue: () => 0.85);

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
      async (filePathArgumentValue, firstNamesDictionaryFilePathOptionValue,
        middleNamesDictionaryFilePathOptionValue, lastNamesDictionaryFilePathOptionValue, outputOptionValue,
        scoreOptionValue) =>
      {
        using var readerFile = new StreamReader(filePathArgumentValue.FullName);
        using var csvRecords = new CsvReader(readerFile, CultureInfo.InvariantCulture);
        var recordsFromCsv = csvRecords.GetRecords<PatientRecord>();

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

        var deduplicatedRecords = Deduplicate.TryDeduplicate(recordsFromCsv, scoreOptionValue,
          firstNamesDictionary, middleNamesDictionary, lastNamesDictionary);

        var deduplicatedResult = deduplicatedRecords
          .Select(x => new DeduplicatedRecord
            (
              x.Value.RecordId,
              string.Join('|', x.Matches.Select(m => m.Value.RecordId))
            )
          );
        await using var writer = new StreamWriter(outputOptionValue.FullName, false, Encoding.UTF8);
        await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        await csv.WriteRecordsAsync(deduplicatedResult);
      },
      filePathArgument, firstNamesDictionaryFilePathOption,
      middleNamesDictionaryFilePathOption, lastNamesDictionaryFilePathOption, outputOption, scoreOption);

    return command;
  }
}
