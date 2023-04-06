using System.Collections.Concurrent;
using Biomatch.Domain.Models;

namespace Biomatch.Domain;

public static class Match
{
  public static IEnumerable<PotentialMatch> TryMatchSingleRecord(IEnumerable<PatientRecord> records1,
    IEnumerable<PatientRecord> records2, double matchScoreThreshold, WordDictionary? firstNamesDictionary = null,
    WordDictionary? middleNamesDictionary = null, WordDictionary? lastNamesDictionary = null)
  {
    var preprocessedRecords1 =
      records1.PreprocessData(firstNamesDictionary, middleNamesDictionary, lastNamesDictionary).ToArray();
    var preprocessedRecords2 =
      records2.PreprocessData(firstNamesDictionary, middleNamesDictionary, lastNamesDictionary).ToArray();

    var recordMatchResults =
      GetPotentialMatches(preprocessedRecords1, preprocessedRecords2, matchScoreThreshold, 1.0);

    var matchedRecords = recordMatchResults
      .GroupBy(x => x.Value)
      .Select(x => x.MaxBy(y => y.Score))
      .OrderByDescending(x => x.Score)
      .ToList();

    return matchedRecords;
  }

  public static ConcurrentBag<PotentialMatch> GetPotentialMatches(Memory<PatientRecord> records1,
    Memory<PatientRecord> records2, double lowerScoreThreshold, double upperScoreThreshold)
  {
    var potentialMatches = new ConcurrentBag<PotentialMatch>();

    var records1CharacterStartAndEndIndex = GetCharactersStartAndEndIndex(records1.Span);
    var records2CharacterStartAndEndIndex = GetCharactersStartAndEndIndex(records2.Span);

    // Maximum of 27 iterations because of letters in the alphabet
    Parallel.ForEach(records1CharacterStartAndEndIndex, record1StartAndEnd =>
    {
      var records2StartAndEndFound =
        records2CharacterStartAndEndIndex.TryGetValue(record1StartAndEnd.Key, out var records2StartAndEnd);

      var records1ToCompare = records1.Slice(record1StartAndEnd.Value.Item1,
        record1StartAndEnd.Value.Item2 - record1StartAndEnd.Value.Item1 + 1);

      var records2ToCompare = records2StartAndEndFound
        ? records2.Slice(records2StartAndEnd.Item1, records2StartAndEnd.Item2 - records2StartAndEnd.Item1 + 1)
        : records2;
      // For each record in the first table, compare it to all records in the second table
      Parallel.For(0, records1ToCompare.Length, recordToCompareIndex =>
      {
        var primaryRecord = records1ToCompare.Span[recordToCompareIndex];
        for (var i = 0; i < records2ToCompare.Length; i++)
        {
          var secondaryRecord = records2ToCompare.Span[i];
          CompareRecords(potentialMatches, ref primaryRecord, ref secondaryRecord,
            lowerScoreThreshold, upperScoreThreshold);
        }
      });
    });

    return potentialMatches;
  }

  private static void CompareRecords(ConcurrentBag<PotentialMatch> potentialMatches,
    ref PatientRecord primaryRecord, ref PatientRecord secondaryRecord,
    double lowerScoreThreshold, double upperScoreThreshold)
  {
    if (primaryRecord.RecordId == secondaryRecord.RecordId) return;
    //get the distance vector for the ith vector of the first table and the jth record of the second table
    var distanceVector = DistanceVector.CalculateDistance(ref primaryRecord, ref secondaryRecord);
    var tempScore = Score.CalculateFinalScore(ref distanceVector);
    if (tempScore >= lowerScoreThreshold && tempScore <= upperScoreThreshold)
    {
      potentialMatches.Add(
        new PotentialMatch(primaryRecord, secondaryRecord, distanceVector, tempScore));
    }
  }

  private static Dictionary<char, (int, int)> GetCharactersStartAndEndIndex(ReadOnlySpan<PatientRecord> records)
  {
    var characterIndex = new Dictionary<char, (int, int)>();

    var currentIndex = 0;
    for (var letter = 'a'; letter <= 'z'; letter++)
    {
      int? startIndex = null;
      int? endIndex = null;
      for (var i = currentIndex; i < records.Length; i++)
      {
        if (startIndex == null && records[i].FirstName.StartsWith(letter))
        {
          startIndex = i;
          continue;
        }

        if (startIndex == null || records[i].FirstName.StartsWith(letter)) continue;

        currentIndex = i;
        endIndex = i - 1;
        break;
      }

      if (startIndex == null) continue;
      characterIndex.TryAdd(letter, (startIndex.Value, endIndex ?? records.Length - 1));
    }

    return characterIndex;
  }
}
