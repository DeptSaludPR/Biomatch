using CsvHelper.Configuration;
using MatchingEngine.Domain.Models;

namespace MatchingEngine.CLI.Csv;

public sealed class DuplicateRecordMap : ClassMap<DuplicateRecord>
{
  public DuplicateRecordMap()
  {
    Map(m => m.Patient1Url).Name("Patient 1 URL");
    Map(m => m.Patient2Url).Name("Patient 2 URL");
    Map(m => m.Distance).Name("Distance");
    Map(m => m.Score).Name("Score");
    Map(m => m.DuplicateStatus).Name("Duplicate Status");
    Map(m => m.Error1).Name("Error 1");
    Map(m => m.Error2).Name("Error 2");
    Map(m => m.Error3).Name("Error 3");
    Map(m => m.ProfileModified).Name("Profile Modified");
    Map(m => m.ProfileMerged).Name("Profile Merged");
    Map(m => m.User).Name("User");
    Map(m => m.Date).Name("Date");
  }
}
