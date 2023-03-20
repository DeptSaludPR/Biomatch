using MatchingEngine.Domain.Models;

namespace MatchingEngine.Domain;

public static class Deduplicate
{
  public static IEnumerable<RecordMatchResult> TryDeduplicate(IEnumerable<PatientRecord> records,
    double matchScoreThreshold, WordDictionary? firstNamesDictionary = null,
    WordDictionary? middleNamesDictionary = null, WordDictionary? lastNamesDictionary = null)
  {
    var preprocessedRecords =
      records.PreprocessData(firstNamesDictionary, middleNamesDictionary, lastNamesDictionary).ToArray();

    var potentialDuplicates =
      Match.GetPotentialMatches(preprocessedRecords, preprocessedRecords, matchScoreThreshold, 1.0);

    var potentialMatches = potentialDuplicates
      .GroupBy(x => x.Value)
      .ToDictionary(x => x.Key, x => x.OrderByDescending(y => y.Score)
        .Select(e => new PotentialMatch
          (
            x.Key,
            e.Match,
            e.Distance,
            e.Score
          )
        )
        .ToArray());

    var potentialMatchesList = new List<RecordMatchResult>();

    foreach (var record in preprocessedRecords)
    {
      if (potentialMatches.TryGetValue(record, out var matches))
      {
        potentialMatchesList.Add(new RecordMatchResult(record, matches));
        potentialMatches.Remove(record);
        var innerMatchesToRemove = potentialMatches.Where(x => x.Value.Any(y => y.Value == record)).ToList();
        foreach (var innerMatch in innerMatchesToRemove)
        {
          potentialMatches.Remove(innerMatch.Key);
        }
      }
      else if (potentialMatchesList.Any(x => x.Matches.Any(y => y.Value == record)))
      {
        // Do nothing
      }
      else
      {
        potentialMatchesList.Add(new RecordMatchResult(record, Array.Empty<PotentialMatch>()));
      }
    }

    return potentialMatchesList;
  }
}
