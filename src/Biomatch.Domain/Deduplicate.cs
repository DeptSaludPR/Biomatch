using Biomatch.Domain.Models;

namespace Biomatch.Domain;

public static class Deduplicate
{
  public static IEnumerable<IPersonRecord> TryDeduplicate(
    IEnumerable<IPersonRecord> records,
    double matchScoreThreshold,
    WordDictionary? firstNamesDictionary = null,
    WordDictionary? middleNamesDictionary = null,
    WordDictionary? lastNamesDictionary = null,
    Func<int, IProgress<int>>? matchProgressReport = null
  )
  {
    var preprocessedRecords = records
      .PreprocessData(firstNamesDictionary, middleNamesDictionary, lastNamesDictionary)
      .ToArray();

    var potentialDuplicates = Match.GetPotentialMatchesFromSameDataSet(
      preprocessedRecords,
      preprocessedRecords,
      matchScoreThreshold,
      1.0,
      matchProgressReport
    );

    var potentialMatchesGroupedByRecord = potentialDuplicates
      .GroupBy(x => x.Value)
      .ToDictionary(x => x.Key, x => x.Select(y => y.Match).ToList());

    var duplicates = new Dictionary<string, IPersonRecord>();
    var uniqueDuplicateRecords = new Dictionary<string, IPersonRecord>();
    Console.WriteLine("Processing potential matches...");
    foreach (var potentialMatch in potentialMatchesGroupedByRecord)
    {
      if (duplicates.ContainsKey(potentialMatch.Key.RecordId))
        continue;
      MarkDuplicates(
        potentialMatch.Key,
        potentialMatchesGroupedByRecord,
        potentialMatch.Value,
        duplicates
      );
      uniqueDuplicateRecords.Add(potentialMatch.Key.RecordId, potentialMatch.Key);
      yield return potentialMatch.Key;
    }

    foreach (var record in preprocessedRecords)
    {
      if (uniqueDuplicateRecords.ContainsKey(record.RecordId))
        continue;
      if (duplicates.ContainsKey(record.RecordId))
        continue;
      yield return record;
    }
  }

  private static void MarkDuplicates(
    IPersonRecord originalRecord,
    Dictionary<IPersonRecord, List<IPersonRecord>> potentialDuplicates,
    List<IPersonRecord> duplicatesToMark,
    Dictionary<string, IPersonRecord> duplicates
  )
  {
    foreach (var duplicate in duplicatesToMark)
    {
      if (duplicate.RecordId == originalRecord.RecordId)
        continue;
      if (duplicates.ContainsKey(duplicate.RecordId))
        continue;
      duplicates.TryAdd(duplicate.RecordId, duplicate);
      var newDuplicatesToMark = potentialDuplicates[duplicate];
      MarkDuplicates(originalRecord, potentialDuplicates, newDuplicatesToMark, duplicates);
    }
  }
}
