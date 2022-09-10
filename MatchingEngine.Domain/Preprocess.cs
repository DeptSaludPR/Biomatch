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
            var firstNames = patientRecord.FirstName.SanitizeName();
            var middleNames = patientRecord.MiddleName.SanitizeName();
            var lastNames = patientRecord.LastName.SanitizeName();
            var secondLastNames = patientRecord.SecondLastName.SanitizeName();

            var firstName = firstNames.FirstOrDefault();
            var middleName = middleNames.FirstOrDefault();
            if (middleNames.Count == 0)
            {
                if (firstNames.Count > 1)
                {
                    middleName = string.Join(' ', firstNames.Skip(1));
                }
            }
            else
            {
                firstName = string.Join(' ', firstNames);
            }

            var lastName = lastNames.FirstOrDefault();
            var secondLastName = secondLastNames.FirstOrDefault();
            if (secondLastNames.Count == 0)
            {
                if (lastNames.Count > 1)
                {
                    secondLastName = string.Join(' ', lastNames.Skip(1));
                }
            }
            else
            {
                lastName = string.Join(' ', lastNames.Skip(1));
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