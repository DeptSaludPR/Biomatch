using CsvHelper.Configuration;

namespace MatchingEngine.Models;

public record UrlDoc
{
    public string Patient1Url { get; }
    public string Patient2Url { get; }
    public double Score { get; }
    public string Distance { get; }
    public readonly string DuplicateStatus;
    public readonly string Error1;
    public readonly string Error2;
    public readonly string Error3;
    public readonly string ProfileModified;
    public readonly string ProfileMerged;
    public readonly string User;
    public readonly string Date;

    //Constructors
    public UrlDoc(PotentialDuplicate duplicate)
    {
        Patient1Url =
            $"https://bioportal.salud.gov.pr/administration/patients/{duplicate.Value.RecordId}/profile/general";
        Patient2Url =
            $"https://bioportal.salud.gov.pr/administration/patients/{duplicate.Match.RecordId}/profile/general";
        Distance = duplicate.Distance.ToString();
        Score = duplicate.Score;
        //The rest are blank
        DuplicateStatus = string.Empty;
        Error1 = string.Empty;
        Error2 = string.Empty;
        Error3 = string.Empty;
        ProfileModified = string.Empty;
        ProfileMerged = string.Empty;
        User = string.Empty;
        Date = string.Empty;
    }
}

public sealed class UrlDocMap : ClassMap<UrlDoc>
{
    public UrlDocMap()
    {
        Map(m => m.Patient1Url).Name("Patient 1 URL");
        Map(m => m.Patient2Url).Name("Patient 2 URL");
        Map(m => m.Distance).Name("Distance");
        Map(m => m.Error1).Name("Error 1");
        Map(m => m.Error2).Name("Error 2");
        Map(m => m.Error3).Name("Error 3");
        Map(m => m.ProfileModified).Name("Profile Modified");
        Map(m => m.ProfileMerged).Name("Profile Merged");
        Map(m => m.User).Name("User");
        Map(m => m.Date).Name("Date");
    }
}