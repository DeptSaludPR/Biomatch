using Biomatch.Domain;

namespace Biomatch.CLI.Services;

public static class DictionaryLoader
{
  public static (
    WordDictionary? firstNamesDictionary,
    WordDictionary? middleNamesDictionary,
    WordDictionary? lastNamesDictionary) LoadDictionaries(
    FileInfo? firstNamesDictionaryFilePath,
    FileInfo? middleNamesDictionaryFilePath,
    FileInfo? lastNamesDictionaryFilePath)
  {
    WordDictionary? firstNamesDictionary = null;
    if (firstNamesDictionaryFilePath is not null && firstNamesDictionaryFilePath.Exists)
    {
      firstNamesDictionary = new WordDictionary(firstNamesDictionaryFilePath);
      Console.WriteLine(
        $"FirstNames dictionary loaded from {firstNamesDictionaryFilePath}");
    }
    else
    {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine(
        "FirstNames dictionary not loaded, generate dictionary files to improve preprocessing.");
      Console.ResetColor();
    }

    WordDictionary? middleNamesDictionary = null;
    if (middleNamesDictionaryFilePath is not null && middleNamesDictionaryFilePath.Exists)
    {
      middleNamesDictionary = new WordDictionary(middleNamesDictionaryFilePath);
      Console.WriteLine(
        $"MiddleNames dictionary loaded from {middleNamesDictionaryFilePath}");
    }
    else
    {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine(
        "MiddleNames dictionary not loaded, generate dictionary files to improve preprocessing.");
      Console.ResetColor();
    }

    WordDictionary? lastNamesDictionary = null;
    if (lastNamesDictionaryFilePath is not null && lastNamesDictionaryFilePath.Exists)
    {
      lastNamesDictionary = new WordDictionary(lastNamesDictionaryFilePath);
      Console.WriteLine(
        $"LastNames dictionary loaded from {lastNamesDictionaryFilePath}");
    }
    else
    {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine(
        "LastNames dictionary not loaded, generate dictionary files to improve preprocessing.");
      Console.ResetColor();
    }

    return (firstNamesDictionary, middleNamesDictionary, lastNamesDictionary);
  }
}
