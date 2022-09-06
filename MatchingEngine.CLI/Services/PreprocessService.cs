using MatchingEngine.Domain.Helpers;
using MatchingEngine.Domain.Models;

namespace MatchingEngine.CLI.Services;

public static class PreprocessService
{
    public static IEnumerable<PatientRecord> PreprocessData(IEnumerable<PatientRecord> patientRecords)
    {
        return patientRecords
            .OrderBy(e => e.FirstName)
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
            ));
    }
}