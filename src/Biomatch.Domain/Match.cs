using System.Collections.Concurrent;
using Biomatch.Domain.Models;

namespace Biomatch.Domain;

public static class Match
{
  public static IEnumerable<PotentialMatch> FindBestMatches(IEnumerable<IPersonRecord> records1,
    IEnumerable<IPersonRecord> records2, double matchScoreThreshold, WordDictionary? firstNamesDictionary = null,
    WordDictionary? middleNamesDictionary = null, WordDictionary? lastNamesDictionary = null,
    bool recordsFromSameDataSet = false, IProgress<int>? matchProgressReport = null)
  {
    var preprocessedRecords1 =
      records1.PreprocessData(firstNamesDictionary, middleNamesDictionary, lastNamesDictionary).ToArray();
    var preprocessedRecords2 =
      records2.PreprocessData(firstNamesDictionary, middleNamesDictionary, lastNamesDictionary).ToArray();

    ConcurrentBag<PotentialMatch> recordMatchResults;
    if (recordsFromSameDataSet)
      recordMatchResults =
        GetPotentialMatchesFromSameDataSet(preprocessedRecords1, preprocessedRecords2, matchScoreThreshold, 1.0,
          matchProgressReport);
    else
      recordMatchResults =
        GetPotentialMatchesFromDifferentDataSet(preprocessedRecords1, preprocessedRecords2, matchScoreThreshold, 1.0,
          matchProgressReport);

    var matchedRecords = recordMatchResults
      .GroupBy(x => x.Value)
      .Select(x => x.MaxBy(y => y.Score))
      .OrderByDescending(x => x.Score)
      .ToList();

    return matchedRecords;
  }

  public static ConcurrentBag<PotentialMatch> GetPotentialMatchesFromDifferentDataSet(
    Memory<PersonRecordForMatch> records1, Memory<PersonRecordForMatch> records2, double lowerScoreThreshold,
    double upperScoreThreshold, IProgress<int>? matchProgressReport = null)
  {
    var potentialMatches = new ConcurrentBag<PotentialMatch>();

    var records1CharacterStartAndEndIndex = GetCharactersStartAndEndIndex(records1.Span);
    var records2CharacterStartAndEndIndex = GetCharactersStartAndEndIndex(records2.Span);

    var parallelOptions = new ParallelOptions {MaxDegreeOfParallelism = Environment.ProcessorCount};
    // Maximum of 26 iterations because of letters in the alphabet
    Parallel.For(0, records1CharacterStartAndEndIndex.Length, parallelOptions, record1Index =>
    {
      var record1StartAndEnd = records1CharacterStartAndEndIndex[record1Index];
      // If the start index is -1, skip this iteration
      if (record1StartAndEnd.Item1 == -1)
        return;
      var records2StartAndEnd = records2CharacterStartAndEndIndex[record1Index];

      var records1ToCompare =
        records1.Slice(record1StartAndEnd.Item1, record1StartAndEnd.Item2 - record1StartAndEnd.Item1 + 1);
      var records2ToCompare = records2StartAndEnd.Item1 == -1
        ? records2
        : records2.Slice(records2StartAndEnd.Item1, records2StartAndEnd.Item2 - records2StartAndEnd.Item1 + 1);

      // For each record in the first table, compare it to all records in the second table
      Parallel.For(0, records1ToCompare.Length, parallelOptions, recordToCompareIndex =>
      {
        for (var i = 0; i < records2ToCompare.Length; i++)
        {
          ref var primaryRecord = ref records1ToCompare.Span[recordToCompareIndex];
          ref var secondaryRecord = ref records2ToCompare.Span[i];
          CompareRecords(potentialMatches, ref primaryRecord, ref secondaryRecord, lowerScoreThreshold,
            upperScoreThreshold);
        }

        matchProgressReport?.Report(1);
      });
    });

    return potentialMatches;
  }

  public static IEnumerable<PotentialMatch> GetPotentialMatchesFromSameDataSet(PersonRecordForMatch record,
    Span<PersonRecordForMatch> recordsToMatch, double lowerScoreThreshold, double upperScoreThreshold)
  {
    var potentialMatches = new List<PotentialMatch>();
    var recordLetterIndexFromFirstCharacter = record.FirstName[0] - 'a';

    var recordsToMatchCharacterStartAndEndIndex = GetCharactersStartAndEndIndex(recordsToMatch);
    var recordsToMatchStartAndEnd = recordsToMatchCharacterStartAndEndIndex[recordLetterIndexFromFirstCharacter];

    var recordsToCompare = recordsToMatchStartAndEnd.Item1 == -1
      ? recordsToMatch
      : recordsToMatch.Slice(recordsToMatchStartAndEnd.Item1,
        recordsToMatchStartAndEnd.Item2 - recordsToMatchStartAndEnd.Item1 + 1);

    for (var i = 0; i < recordsToCompare.Length; i++)
    {
      ref var secondaryRecord = ref recordsToCompare[i];
      if (record.RecordId == secondaryRecord.RecordId) continue;
      var potentialMatch = CompareRecords(ref record, ref secondaryRecord, lowerScoreThreshold,
        upperScoreThreshold);
      if (potentialMatch != null)
        potentialMatches.Add(potentialMatch.Value);
    }

    return potentialMatches;
  }

  public static ConcurrentBag<PotentialMatch> GetPotentialMatchesFromSameDataSet(Memory<PersonRecordForMatch> records1,
    Memory<PersonRecordForMatch> records2, double lowerScoreThreshold, double upperScoreThreshold,
    IProgress<int>? matchProgressReport = null)
  {
    var potentialMatches = new ConcurrentBag<PotentialMatch>();

    var records1CharacterStartAndEndIndex = GetCharactersStartAndEndIndex(records1.Span);
    var records2CharacterStartAndEndIndex = GetCharactersStartAndEndIndex(records2.Span);

    var parallelOptions = new ParallelOptions {MaxDegreeOfParallelism = Environment.ProcessorCount};
    // Maximum of 26 iterations because of letters in the alphabet
    Parallel.For(0, records1CharacterStartAndEndIndex.Length, parallelOptions, record1Index =>
    {
      var record1StartAndEnd = records1CharacterStartAndEndIndex[record1Index];
      // If the start index is -1, skip this iteration
      if (record1StartAndEnd.Item1 == -1)
        return;
      var records2StartAndEnd = records2CharacterStartAndEndIndex[record1Index];

      var records1ToCompare =
        records1.Slice(record1StartAndEnd.Item1, record1StartAndEnd.Item2 - record1StartAndEnd.Item1 + 1);
      var records2ToCompare = records2StartAndEnd.Item1 == -1
        ? records2
        : records2.Slice(records2StartAndEnd.Item1, records2StartAndEnd.Item2 - records2StartAndEnd.Item1 + 1);

      // For each record in the first table, compare it to all records in the second table
      Parallel.For(0, records1ToCompare.Length, parallelOptions, recordToCompareIndex =>
      {
        for (var i = 0; i < records2ToCompare.Length; i++)
        {
          ref var primaryRecord = ref records1ToCompare.Span[recordToCompareIndex];
          ref var secondaryRecord = ref records2ToCompare.Span[i];
          if (primaryRecord.RecordId == secondaryRecord.RecordId) continue;
          CompareRecords(potentialMatches, ref primaryRecord, ref secondaryRecord, lowerScoreThreshold,
            upperScoreThreshold);
        }

        matchProgressReport?.Report(1);
      });
    });

    return potentialMatches;
  }

  private static void CompareRecords(ConcurrentBag<PotentialMatch> potentialMatches,
    ref PersonRecordForMatch primaryRecord, ref PersonRecordForMatch secondaryRecord,
    double lowerScoreThreshold, double upperScoreThreshold)
  {
    //get the distance vector for the ith vector of the first table and the jth record of the second table
    var distanceVector = DistanceVector.CalculateDistance(ref primaryRecord, ref secondaryRecord);
    var tempScore = Score.CalculateFinalScore(ref distanceVector);
    if (tempScore >= lowerScoreThreshold && tempScore <= upperScoreThreshold)
    {
      potentialMatches.Add(
        new PotentialMatch
        (
          primaryRecord,
          secondaryRecord,
          distanceVector,
          tempScore
        )
      );
    }
  }

  private static PotentialMatch? CompareRecords(ref PersonRecordForMatch primaryRecord,
    ref PersonRecordForMatch secondaryRecord, double lowerScoreThreshold, double upperScoreThreshold)
  {
    //get the distance vector for the ith vector of the first table and the jth record of the second table
    var distanceVector = DistanceVector.CalculateDistance(ref primaryRecord, ref secondaryRecord);
    var tempScore = Score.CalculateFinalScore(ref distanceVector);
    if (tempScore >= lowerScoreThreshold && tempScore <= upperScoreThreshold)
    {
      return new PotentialMatch
      (
        primaryRecord,
        secondaryRecord,
        distanceVector,
        tempScore
      );
    }

    return null;
  }

  private static (int, int)[] GetCharactersStartAndEndIndex(ReadOnlySpan<PersonRecordForMatch> records)
  {
    var characterIndex = new (int, int)[26]; // 26 letters in the alphabet

    var currentIndex = 0;
    for (var letterIndex = 0; letterIndex < 26; letterIndex++)
    {
      var letter = (char) ('a' + letterIndex);

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

      if (startIndex != null)
      {
        characterIndex[letterIndex] = (startIndex.Value, endIndex ?? records.Length - 1);
      }
      else
      {
        characterIndex[letterIndex] = (-1, -1); // Using -1 for non-existing start and end indexes
      }
    }

    return characterIndex;
  }
}
