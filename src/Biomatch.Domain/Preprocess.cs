using System.Collections.Concurrent;
using System.Collections.Frozen;
using Biomatch.Domain.Enums;
using Biomatch.Domain.Helpers;
using Biomatch.Domain.Models;

namespace Biomatch.Domain;

public static class Preprocess
{
  public static PersonRecordForMatch PreprocessRecord(this IPersonRecord patientRecord,
    FrozenSet<string> prepositionsToRemove, FrozenSet<string> suffixesToRemove,
    WordDictionary? firstNamesDictionary = null, WordDictionary? middleNamesDictionary = null,
    WordDictionary? lastNamesDictionary = null)
  {
    var normalizedFirstNames = patientRecord.FirstName
      .NormalizeNames(NameType.Name)
      .RemoveWords(prepositionsToRemove)
      .RemoveWords(suffixesToRemove);
    var normalizedMiddleNames = patientRecord.MiddleName
      .NormalizeNames(NameType.Name)
      .RemoveWords(prepositionsToRemove)
      .RemoveWords(suffixesToRemove);
    var normalizedLastNames = patientRecord.LastName
      .NormalizeNames(NameType.LastName)
      .RemoveWords(prepositionsToRemove);
    var normalizedSecondLastNames = patientRecord.SecondLastName
      .NormalizeNames(NameType.LastName)
      .RemoveWords(prepositionsToRemove);

    var personName = OrganizeNames(normalizedFirstNames, normalizedMiddleNames.ToList(),
      normalizedLastNames, normalizedSecondLastNames, lastNamesDictionary);

    var firstNames = personName.FirstName
      .SanitizeName(NameType.Name, firstNamesDictionary);

    var middleNames = personName.MiddleName
      .SanitizeName(NameType.Name, middleNamesDictionary);

    var lastNames = personName.LastName
      .SanitizeName(NameType.LastName, lastNamesDictionary);

    var secondLastNames = personName.SecondLastName
      .SanitizeName(NameType.LastName, lastNamesDictionary);

    return new PersonRecordForMatch
    (
      patientRecord.RecordId,
      string.Concat(firstNames),
      string.Concat(middleNames),
      string.Concat(lastNames),
      string.Concat(secondLastNames),
      patientRecord.BirthDate.SanitizeBirthDate(),
      patientRecord.BirthDate.HasValue
        ? patientRecord.BirthDate.Value.ToByteArray()
        : Array.Empty<byte>(),
      patientRecord.City.SanitizeWord().ToString(),
      PhoneNumberHelpers.Parse(patientRecord.PhoneNumber)
    );
  }

  public static IEnumerable<PersonRecordForMatch> PreprocessData(this IEnumerable<IPersonRecord> patientRecords,
    WordDictionary? firstNamesDictionary = null, WordDictionary? middleNamesDictionary = null,
    WordDictionary? lastNamesDictionary = null)
  {
    return patientRecords
      .SanitizeRecords(firstNamesDictionary, middleNamesDictionary, lastNamesDictionary)
      .OrderBy(e => e.FirstName);
  }

  private static IEnumerable<PersonRecordForMatch> SanitizeRecords(this IEnumerable<IPersonRecord> patientRecords,
    WordDictionary? firstNamesDictionary = null, WordDictionary? middleNamesDictionary = null,
    WordDictionary? lastNamesDictionary = null)
  {
    var patientRecordsList = patientRecords.ToArray();
    var prepositions = new HashSet<string>
    {
      "el", "la", "los", "las", "de", "del", "en", "y", "a", "di", "da", "le", "san"
    }.ToFrozenSet(true);
    var suffixes = new HashSet<string>
    {
      "lcdo", "lcda", "dr", "dra", "sor", "jr", "junior", "sr", "sra", "ii", "iii", "mr", "ms", "mrs"
    }.ToFrozenSet(true);
    var processedPatientRecords = new ConcurrentBag<PersonRecordForMatch>();
    Parallel.For(0, patientRecordsList.Length, index =>
    {
      var patientRecord = patientRecordsList[index];

      var normalizedFirstNames = patientRecord.FirstName
        .NormalizeNames(NameType.Name)
        .RemoveWords(prepositions)
        .RemoveWords(suffixes);
      var normalizedMiddleNames = patientRecord.MiddleName
        .NormalizeNames(NameType.Name)
        .RemoveWords(prepositions)
        .RemoveWords(suffixes);
      var normalizedLastNames = patientRecord.LastName
        .NormalizeNames(NameType.LastName)
        .RemoveWords(prepositions);
      var normalizedSecondLastNames = patientRecord.SecondLastName
        .NormalizeNames(NameType.LastName)
        .RemoveWords(prepositions);

      var personName = OrganizeNames(normalizedFirstNames, normalizedMiddleNames.ToList(),
        normalizedLastNames, normalizedSecondLastNames, lastNamesDictionary);

      var firstNames = personName.FirstName
        .SanitizeName(NameType.Name, firstNamesDictionary);

      var middleNames = personName.MiddleName
        .SanitizeName(NameType.Name, middleNamesDictionary);

      var lastNames = personName.LastName
        .SanitizeName(NameType.LastName, lastNamesDictionary);

      var secondLastNames = personName.SecondLastName
        .SanitizeName(NameType.LastName, lastNamesDictionary);

      processedPatientRecords.Add(new PersonRecordForMatch
      (
        patientRecord.RecordId,
        string.Concat(firstNames),
        string.Concat(middleNames),
        string.Concat(lastNames),
        string.Concat(secondLastNames),
        patientRecord.BirthDate.SanitizeBirthDate(),
        patientRecord.BirthDate.HasValue
          ? patientRecord.BirthDate.Value.ToByteArray()
          : Array.Empty<byte>(),
        patientRecord.City.SanitizeWord().ToString(),
        PhoneNumberHelpers.Parse(patientRecord.PhoneNumber)
      ));
    });

    return processedPatientRecords;
  }

  private static PersonName OrganizeNames(IEnumerable<string> firstNames, IReadOnlyCollection<string> middleNames,
    IEnumerable<string> lastNames, IEnumerable<string> secondLastNames,
    WordDictionary? lastNamesDictionary = null)
  {
    var names = firstNames.Concat(middleNames).Concat(lastNames).Concat(secondLastNames).ToArray();
    var firstName = string.Empty;
    var middleName = string.Empty;
    var lastName = string.Empty;
    var secondLastName = string.Empty;

    switch (names.Length)
    {
      case > 3:
        firstName = names[0];
        middleName = names[1];
        lastName = names[2];
        secondLastName = string.Join(' ', names.Skip(3));
        break;
      case 3:
      {
        var trySpellCheckLastName = lastNamesDictionary?.TrySpellCheck(names[1]);
        firstName = names[0];

        if (trySpellCheckLastName?.Count > 0)
        {
          lastName = names[1];
          secondLastName = names[2];
        }
        else if (middleNames.Count == 1 || names[1].Length <= 3)
        {
          middleName = names[1];
          lastName = names[2];
        }
        else
        {
          lastName = names[1];
          secondLastName = names[2];
        }

        break;
      }
      case 2:
        firstName = names[0];
        middleName = string.Empty;
        lastName = names[1];
        secondLastName = string.Empty;
        break;
      case 1:
        firstName = names[0];
        middleName = string.Empty;
        lastName = string.Empty;
        secondLastName = string.Empty;
        break;
    }

    return new PersonName
    (
      firstName,
      middleName,
      lastName,
      secondLastName
    );
  }
}
