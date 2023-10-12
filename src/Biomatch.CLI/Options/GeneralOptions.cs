using System.CommandLine;

namespace Biomatch.CLI.Options;

public sealed record DictionaryOptions(
  Option<FileInfo> FirstNamesDictionaryFilePathOption,
  Option<FileInfo> MiddleNamesDictionaryFilePathOption,
  Option<FileInfo> LastNamesDictionaryFilePathOption
);

public static class GeneralOptions
{
  public static DictionaryOptions GetDictionaryOptions()
  {
    var firstNamesDictionaryFilePathOption = new Option<FileInfo>(
      name: "-dictionary-first-names",
      description: "First names dictionary file path",
      getDefaultValue: () => new FileInfo("Dictionaries/FirstNamesDictionary.json")
    );
    firstNamesDictionaryFilePathOption.AddAlias("-df");

    var middleNamesDictionaryFilePathOption = new Option<FileInfo>(
      name: "-dictionary-middle-names",
      description: "Middle names dictionary file path",
      getDefaultValue: () => new FileInfo("Dictionaries/MiddleNamesDictionary.json")
    );
    middleNamesDictionaryFilePathOption.AddAlias("-dm");

    var lastNamesDictionaryFilePathOption = new Option<FileInfo>(
      name: "-dictionary-last-names",
      description: "Last names dictionary file path",
      getDefaultValue: () => new FileInfo("Dictionaries/LastNamesDictionary.json")
    );
    lastNamesDictionaryFilePathOption.AddAlias("-dl");

    return new DictionaryOptions(
      firstNamesDictionaryFilePathOption,
      middleNamesDictionaryFilePathOption,
      lastNamesDictionaryFilePathOption
    );
  }
}
