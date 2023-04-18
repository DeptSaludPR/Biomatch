using System.CommandLine;
using Biomatch.CLI.Csv;
using Biomatch.Domain;
using Biomatch.Domain.Models;

namespace Biomatch.CLI.Commands;

public static partial class MatchingCommand
{
  public static Command GetDictionaryCommand()
  {
    var command = new Command("dictionary", "Dictionary operations")
    {
      GetDictionaryGenerateCommand(),
      GetDictionaryTestCommand()
    };

    return command;
  }

  private static Command GetDictionaryGenerateCommand()
  {
    var filePathTemplateArgument = new Argument<FileInfo>
      (name: "templateFilePath", description: "The path of the template file");

    var outputOption = new Option<DirectoryInfo>
    (name: "--output", description: "Directory to save the generated files",
      getDefaultValue: () => new DirectoryInfo("Dictionaries/"));
    outputOption.AddAlias("-o");

    var command = new Command("generate", "Creates dictionary files from the provided template")
    {
      filePathTemplateArgument,
      outputOption,
    };

    command.SetHandler(
      async (filePathTemplateArgumentValue, outputOptionValue) =>
      {
        var records1FromCsv = PatientRecordParser.ParseCsv(filePathTemplateArgumentValue.FullName);
        List<PatientRecord> records1 = new();
        await foreach (var record in records1FromCsv)
        {
          records1.Add(record);
        }

        var firstNameFrequencyDictionary = records1
          .GroupBy(e => e.FirstName)
          .Where(e => e.Count() > 20 && e.Key.Length > 3)
          .Select(e => new FrequencyDictionary
          (
            e.Key,
            e.Count()
          ))
          .OrderByDescending(e => e.Frequency)
          .ToList();

        var middleNameFrequencyDictionary = records1
          .PreprocessData()
          .GroupBy(e => e.MiddleName)
          .Where(e => e.Count() > 20 && e.Key.Length > 3)
          .Select(e => new FrequencyDictionary
          (
            e.Key,
            e.Count()
          ))
          .OrderByDescending(e => e.Frequency)
          .ToList();

        var firstLastNameFrequencyDictionary = records1
          .GroupBy(e => e.LastName)
          .Where(e => e.Count() > 20 && e.Key.Length > 3)
          .Select(e => new FrequencyDictionary
          (
            e.Key,
            e.Count()
          ));

        var secondLastNameFrequencyDictionary = records1
          .GroupBy(e => e.LastName)
          .Where(e => e.Count() > 20 && e.Key.Length > 3)
          .Select(e => new FrequencyDictionary
          (
            e.Key,
            e.Count()
          ));

        var lastNameFrequencyDictionary = firstLastNameFrequencyDictionary
          .Concat(secondLastNameFrequencyDictionary)
          .GroupBy(e => e.Word)
          .Select(e => new FrequencyDictionary
          (
            e.Key,
            e.Sum(f => f.Frequency)
          ))
          .OrderByDescending(e => e.Frequency)
          .ToList();

        Directory.CreateDirectory(outputOptionValue.FullName);
        await CreateDictionaryFile(firstNameFrequencyDictionary, outputOptionValue, "FirstNamesDictionary.txt");
        await CreateDictionaryFile(middleNameFrequencyDictionary, outputOptionValue,
          "MiddleNamesDictionary.txt");
        await CreateDictionaryFile(lastNameFrequencyDictionary, outputOptionValue, "LastNamesDictionary.txt");
      }, filePathTemplateArgument, outputOption);

    return command;
  }

  private static async Task CreateDictionaryFile(IEnumerable<FrequencyDictionary> frequencyDictionary,
    DirectoryInfo directoryInfo, string fileName)
  {
    await FrequencyDictionaryTemplate.WriteToTabDelimitedFile(frequencyDictionary,
      Path.Combine(directoryInfo.FullName, fileName));
  }

  private static Command GetDictionaryTestCommand()
  {
    var filePathTemplateArgument = new Argument<FileInfo>
      (name: "dictionaryFilePath", description: "The path of the dictionary file");

    var wordArgument = new Argument<string>
      (name: "word", description: "A word to test spelling errors");

    var command = new Command("test", "Test dictionary files with spelling errors")
    {
      filePathTemplateArgument,
      wordArgument
    };

    command.SetHandler(
      (filePathTemplateArgumentValue, wordArgumentValue) =>
      {
        var sanitizedWord = wordArgumentValue.ToLower();
        var dictionary = new WordDictionary(filePathTemplateArgumentValue);
        var correctWord = dictionary.TrySpellCheck(sanitizedWord)
          .Select(e => e.term)
          .FirstOrDefault();
        if (correctWord is null)
        {
          Console.WriteLine($"No word found for {wordArgumentValue}");
        }
        else if (correctWord == sanitizedWord)
        {
          Console.WriteLine($"Word {correctWord} is correct");
        }
        else
        {
          Console.WriteLine($"Correct word for {wordArgumentValue} is {correctWord}");
        }
      }, filePathTemplateArgument, wordArgument);

    return command;
  }
}
