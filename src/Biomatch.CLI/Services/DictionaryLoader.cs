using System.Text.Json;
using Biomatch.Domain;
using Biomatch.Domain.Models;

namespace Biomatch.CLI.Services;

public static class DictionaryLoader
{
  public static (
    WordDictionary? firstNamesDictionary,
    WordDictionary? middleNamesDictionary,
    WordDictionary? lastNamesDictionary
  ) LoadDictionaries(
    FileInfo? firstNamesDictionaryFilePath,
    FileInfo? middleNamesDictionaryFilePath,
    FileInfo? lastNamesDictionaryFilePath
  )
  {
    WordDictionary? firstNamesDictionary = null;
    if (firstNamesDictionaryFilePath is not null && firstNamesDictionaryFilePath.Exists)
    {
      var firstNameWordsFrequency = JsonSerializer.Deserialize(
        File.ReadAllText(firstNamesDictionaryFilePath.FullName),
        WordFrequencySerializationContext.Default.ListWordFrequency
      );
      if (firstNameWordsFrequency is null)
      {
        Console.WriteLine($"No words found in {firstNamesDictionaryFilePath.FullName}");
        return (null, null, null);
      }
      firstNamesDictionary = WordDictionary.CreateWordDictionary(firstNameWordsFrequency);
      Console.WriteLine($"FirstNames dictionary loaded from {firstNamesDictionaryFilePath}");
    }
    else
    {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine(
        "FirstNames dictionary not loaded, generate dictionary files to improve preprocessing."
      );
      Console.ResetColor();
    }

    WordDictionary? middleNamesDictionary = null;
    if (middleNamesDictionaryFilePath is not null && middleNamesDictionaryFilePath.Exists)
    {
      var middleNameWordsFrequency = JsonSerializer.Deserialize(
        File.ReadAllText(middleNamesDictionaryFilePath.FullName),
        WordFrequencySerializationContext.Default.ListWordFrequency
      );
      if (middleNameWordsFrequency is null)
      {
        Console.WriteLine($"No words found in {middleNamesDictionaryFilePath.FullName}");
        return (null, null, null);
      }
      middleNamesDictionary = WordDictionary.CreateWordDictionary(middleNameWordsFrequency);
      Console.WriteLine($"MiddleNames dictionary loaded from {middleNamesDictionaryFilePath}");
    }
    else
    {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine(
        "MiddleNames dictionary not loaded, generate dictionary files to improve preprocessing."
      );
      Console.ResetColor();
    }

    WordDictionary? lastNamesDictionary = null;
    if (lastNamesDictionaryFilePath is not null && lastNamesDictionaryFilePath.Exists)
    {
      var lastNameWordsFrequency = JsonSerializer.Deserialize(
        File.ReadAllText(lastNamesDictionaryFilePath.FullName),
        WordFrequencySerializationContext.Default.ListWordFrequency
      );
      if (lastNameWordsFrequency is null)
      {
        Console.WriteLine($"No words found in {lastNamesDictionaryFilePath.FullName}");
        return (null, null, null);
      }
      lastNamesDictionary = WordDictionary.CreateWordDictionary(lastNameWordsFrequency);
      Console.WriteLine($"LastNames dictionary loaded from {lastNamesDictionaryFilePath}");
    }
    else
    {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine(
        "LastNames dictionary not loaded, generate dictionary files to improve preprocessing."
      );
      Console.ResetColor();
    }

    return (firstNamesDictionary, middleNamesDictionary, lastNamesDictionary);
  }
}
