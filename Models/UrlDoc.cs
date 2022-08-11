using CsvHelper.Configuration;

namespace MatchingEngine.Models;

public record class UrlDoc
{
    public string Patient1URL { get;}
    public string Patient2URL { get; }
    public double Score { get;}
    public string Distance { get; }
    public string DuplicateStatus;
    public string Error1;
    public string Error2;
    public string Error3;
    public string ProfileModified;
    public string ProfileMerged;
    public string User;
    public string Date;

    //Constructors
    public UrlDoc(PotentialDuplicate Duplicate )
    {
        Patient1URL="https://bioportal.salud.gov.pr/administration/patients/"+Duplicate.Value.RecordId.ToString()+"/profile/general";
        Patient2URL="https://bioportal.salud.gov.pr/administration/patients/"+Duplicate.Match.RecordId.ToString()+"/profile/general";
        Distance=Duplicate.Distance.ToString();
        Score=Duplicate.Score;
        //The rest are blank
        DuplicateStatus="";
        Error1=" ";
        Error2=" ";
        Error3=" ";
        ProfileModified=" ";
        ProfileMerged=" ";
        User=" ";
        Date=" ";

    }


}

public sealed class UrlDocMap : ClassMap<UrlDoc>
{
    public UrlDocMap()
    {
        Map(m => m.Patient1URL).Name("Patient 1 URL");
        Map(m => m.Patient2URL).Name("Patient 2 URL");
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