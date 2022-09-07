using MatchingEngine.Domain.Helpers;
using MatchingEngine.Domain.Models;

namespace MatchingEngine.Domain;

public static class Preprocess
{
    public static IEnumerable<PatientRecord> PreprocessData(IEnumerable<PatientRecord> patientRecords)
    {
        return patientRecords
            .Select(e => new PatientRecord
            (
                e.RecordId,
                e.FirstName.SanitizeName(),
                e.MiddleName.SanitizeName(),
                e.LastName.SanitizeName(),
                e.SecondLastName.SanitizeName(),
                e.BirthDate.Trim(),
                e.City.SanitizeName(),
                e.PhoneNumber.ToLowerInvariant().Trim()
            ))
            .OrderBy(e => e.FirstName);
    }
}