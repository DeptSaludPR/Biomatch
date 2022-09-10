using MatchingEngine.Domain.Enums;
using MatchingEngine.Domain.Helpers;
using MatchingEngine.Domain.Models;

namespace MatchingEngine.Domain;

public static class Preprocess
{
    public static IEnumerable<PatientRecord> PreprocessData(IEnumerable<PatientRecord> patientRecords)
    {
        var processedPatientRecords = new List<PatientRecord>();
        foreach (var patientRecord in patientRecords)
        {
            var firstNames = patientRecord.FirstName
                .SanitizeName()
                .RemovePrepositions()
                .RemoveSuffixes()
                .ToList();
            var middleNames = patientRecord.MiddleName
                .SanitizeName()
                .RemovePrepositions()
                .RemoveSuffixes()
                .ToList();
            var lastNames = patientRecord.LastName
                .SanitizeName(NameType.LastName)
                .RemovePrepositions()
                .ToList();
            var secondLastNames = patientRecord.SecondLastName
                .SanitizeName(NameType.LastName)
                .RemovePrepositions()
                .ToList();

            var firstName = firstNames.FirstOrDefault();
            var middleName = middleNames.FirstOrDefault();
            if (middleNames.Count == 0)
            {
                if (firstNames.Count > 1)
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
            if (secondLastNames.Count == 0)
            {
                if (lastNames.Count > 1)
                {
                    secondLastName = string.Concat(lastNames.Skip(1));
                }
            }
            else
            {
                lastName = string.Concat(lastNames);
            }

            processedPatientRecords.Add(new PatientRecord
            (
                patientRecord.RecordId,
                firstName ?? string.Empty,
                middleName ?? string.Empty,
                lastName ?? string.Empty,
                secondLastName ?? string.Empty,
                patientRecord.BirthDate.Trim(),
                patientRecord.City.SanitizeWord(),
                patientRecord.PhoneNumber.Trim()
            ));
        }

        return processedPatientRecords
            .OrderBy(e => e.FirstName);
    }
}