namespace Biomatch.Domain.Models;

public readonly record struct DuplicateRecord
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
  public DuplicateRecord(PotentialMatch match)
  {
    Patient1Url =
      $"https://bioportal.salud.gov.pr/administration/patients/{match.Value.RecordId}/profile/general";
    Patient2Url =
      $"https://bioportal.salud.gov.pr/administration/patients/{match.Match.RecordId}/profile/general";
    Distance = match.Distance.ToString();
    Score = match.Score;
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
