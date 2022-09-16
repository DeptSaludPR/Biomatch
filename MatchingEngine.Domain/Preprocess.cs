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
            var firstNames = patientRecord.FirstName
                .SanitizeName(NameType.Name, firstNamesDictionary)
                .RemovePrepositions()
                .RemoveSuffixes()
                .ToArray();
            var middleNames = patientRecord.MiddleName
                .SanitizeName(NameType.Name, middleNamesDictionary)
                .RemovePrepositions()
                .RemoveSuffixes()
                .ToArray();
            var lastNames = patientRecord.LastName
                .SanitizeName(NameType.LastName, lastNamesDictionary)
                .RemovePrepositions()
                .ToArray();
            var secondLastNames = patientRecord.SecondLastName
                .SanitizeName(NameType.LastName, lastNamesDictionary)
                .RemovePrepositions()
                .ToArray();

            var firstName = firstNames.FirstOrDefault();
            var middleName = middleNames.FirstOrDefault();
            if (middleNames.Length == 0)
            {
                if (firstNames.Length > 1)
                {
                    middleName = string.Concat(firstNames.Skip(1));
                }
            }
            else
            {
                firstName = string.Concat(firstNames);
            }

            var lastName = lastNames.FirstOrDefault();
            var secondLastName = secondLastNames.FirstOrDefault();
            if (secondLastNames.Length == 0)
            {
                if (lastNames.Length > 1)
                {
                    secondLastName = string.Concat(lastNames.Skip(1));
                }
            }
            else
            {
                lastName = string.Concat(lastNames);
            }

            processedPatientRecords.Add(patientRecord with
            {
                FirstName = firstName ?? string.Empty,
                MiddleName = middleName ?? string.Empty,
                LastName = lastName ?? string.Empty,
                SecondLastName = secondLastName ?? string.Empty,
                BirthDate = patientRecord.BirthDate.SanitizeBirthDate(),
                City = patientRecord.City.SanitizeWord(),
                PhoneNumber = patientRecord.PhoneNumber.Trim()
            });
        });

        return processedPatientRecords;
    }
}