using CsvHelper.Configuration;
using MatchingEngine.Domain.Models;

namespace MatchingEngine.CLI.Csv;

public sealed class PatientRecordMap : ClassMap<PatientRecord>
{
    public PatientRecordMap()
    {
        Map(m => m.RecordId);
        Map(m => m.FirstName);
        Map(m => m.MiddleName);
        Map(m => m.LastName);
        Map(m => m.SecondLastName);
        Map(m => m.BirthDate);
        Map(m => m.City);
        Map(m => m.PhoneNumber);
    }
}