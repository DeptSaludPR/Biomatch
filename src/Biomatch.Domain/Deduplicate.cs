using Biomatch.Domain.Models;

namespace Biomatch.Domain;

public static class Deduplicate
{
  public static IEnumerable<RecordMatchResult> TryDeduplicate(IEnumerable<IPersonRecord> records,
    double matchScoreThreshold, WordDictionary? firstNamesDictionary = null,
    WordDictionary? middleNamesDictionary = null, WordDictionary? lastNamesDictionary = null)
  {
    var preprocessedRecords =
      records
        .PreprocessData(firstNamesDictionary, middleNamesDictionary, lastNamesDictionary)
        .ToArray();

    var potentialDuplicates =
      Match.GetPotentialMatchesFromSameDataSet(preprocessedRecords, preprocessedRecords, matchScoreThreshold, 1.0);

    var potentialDuplicatesGrouped = potentialDuplicates
      .GroupBy(x => x.Value);

    var potentialMatchesList = new Dictionary<IPersonRecord, RecordMatchResult>();
    var innerRecordsAdded = new HashSet<IPersonRecord>();

    foreach (var potentialDuplicate in potentialDuplicatesGrouped)
    {
      if (innerRecordsAdded.Contains(potentialDuplicate.Key)) continue;

      if (potentialMatchesList.TryGetValue(potentialDuplicate.Key, out var recordMatchResult))
      {
        recordMatchResult.Matches.AddRange(potentialDuplicate.ToList());
        foreach (var innerMatch in potentialDuplicate)
        {
          innerRecordsAdded.Add(innerMatch.Match);
        }
      }
      else
      {
        potentialMatchesList.Add(potentialDuplicate.Key, new RecordMatchResult
          (
            potentialDuplicate.Key,
            potentialDuplicate.ToList()
          )
        );
        innerRecordsAdded.Add(potentialDuplicate.Key);
        foreach (var innerMatch in potentialDuplicate)
        {
          innerRecordsAdded.Add(innerMatch.Match);
        }
      }
    }

    return potentialMatchesList.Values;
  }

}
