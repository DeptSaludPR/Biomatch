using System.Collections.Concurrent;
using MatchingEngine.Domain.Enums;
using MatchingEngine.Domain.Helpers;
using MatchingEngine.Domain.Models;

namespace MatchingEngine.Domain;

public static class Preprocess
{
    public static IEnumerable<PatientRecord> PreprocessData(this IEnumerable<PatientRecord> patientRecords,
        WordDictionary? firstNamesDictionary = null, WordDictionary? middleNamesDictionary = null,
        WordDictionary? lastNamesDictionary = null)
    {
        return patientRecords
            .SanitizeRecords(firstNamesDictionary, middleNamesDictionary, lastNamesDictionary)
            .OrderBy(e => e.FirstName);
    }

    private static IEnumerable<PatientRecord> SanitizeRecords(this IEnumerable<PatientRecord> patientRecords,
        WordDictionary? firstNamesDictionary = null, WordDictionary? middleNamesDictionary = null,
        WordDictionary? lastNamesDictionary = null)
    {
        var patientRecordsList = patientRecords.ToArray();
        var processedPatientRecords = new ConcurrentBag<PatientRecord>();
        Parallel.For(0, patientRecordsList.Length, index =>
        {
            var patientRecord = patientRecordsList[index];

            var normalizedFirstNames = patientRecord.FirstName
                .NormalizeNames(NameType.Name)
                .RemovePrepositions()
                .RemoveSuffixes();
            var normalizedMiddleNames = patientRecord.MiddleName
                .NormalizeNames(NameType.Name)
                .RemovePrepositions()
                .RemoveSuffixes();
            var normalizedLastNames = patientRecord.LastName
                .NormalizeNames(NameType.LastName)
                .RemovePrepositions();
            var normalizedSecondLastNames = patientRecord.SecondLastName
                .NormalizeNames(NameType.LastName)
                .RemovePrepositions();

            var personName = OrganizeNames(normalizedFirstNames, normalizedMiddleNames.ToList(),
                normalizedLastNames, normalizedSecondLastNames, middleNamesDictionary);

            var firstNames = personName.FirstName
                .SanitizeName(NameType.Name, firstNamesDictionary);

            var middleNames = personName.MiddleName
                .SanitizeName(NameType.Name, middleNamesDictionary);

            var lastNames = personName.LastName
                .SanitizeName(NameType.LastName, lastNamesDictionary);

            var secondLastNames = personName.SecondLastName
                .SanitizeName(NameType.LastName, lastNamesDictionary);

            processedPatientRecords.Add(patientRecord with
            {
                FirstName = string.Concat(firstNames),
                MiddleName = string.Concat(middleNames),
                LastName = string.Concat(lastNames),
                SecondLastName = string.Concat(secondLastNames),
                BirthDate = patientRecord.BirthDate.SanitizeBirthDate(),
                City = patientRecord.City.SanitizeWord(),
                PhoneNumber = patientRecord.PhoneNumber.Trim()
            });
        });

        return processedPatientRecords;
    }

    private static PersonName OrganizeNames(IEnumerable<string> firstNames, IReadOnlyCollection<string> middleNames,
        IEnumerable<string> lastNames, IEnumerable<string> secondLastNames,
        WordDictionary? middleNamesDictionary = null)
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
                var trySpellCheckLastName = middleNamesDictionary?.TrySpellCheck(names[1]);
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