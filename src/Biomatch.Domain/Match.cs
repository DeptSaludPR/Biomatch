using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Biomatch.Domain.Models;

namespace Biomatch.Domain;

public static class Match
{
  public static IEnumerable<PotentialMatch> FindBestMatches(IEnumerable<PatientRecord> records1,
    IEnumerable<PatientRecord> records2, double matchScoreThreshold, WordDictionary? firstNamesDictionary = null,
    WordDictionary? middleNamesDictionary = null, WordDictionary? lastNamesDictionary = null,
    IProgress<int>? matchProgressReport = null)
  {
    var preprocessedRecords1 =
      records1.PreprocessData(firstNamesDictionary, middleNamesDictionary, lastNamesDictionary).ToArray();
    var preprocessedRecords2 =
      records2.PreprocessData(firstNamesDictionary, middleNamesDictionary, lastNamesDictionary).ToArray();

    var recordMatchResults =
      GetPotentialMatches(preprocessedRecords1, preprocessedRecords2, matchScoreThreshold, 1.0, matchProgressReport);

    var matchedRecords = recordMatchResults
      .GroupBy(x => x.Value)
      .Select(x => x.MaxBy(y => y.Score))
      .OrderByDescending(x => x.Score)
      .ToList();

    return matchedRecords;
  }

  public static ConcurrentBag<PotentialMatch> GetPotentialMatches(Memory<PatientRecord> records1,
    Memory<PatientRecord> records2, double lowerScoreThreshold, double upperScoreThreshold,
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
          CompareRecords(potentialMatches, ref records1ToCompare.Span[recordToCompareIndex],
            ref records2ToCompare.Span[i],
            lowerScoreThreshold, upperScoreThreshold);
        }

        matchProgressReport?.Report(1);
      });
    });

    return potentialMatches;
  }

  public static ConcurrentBag<PotentialMatch> GetPotentialMatchesV2(Memory<PatientRecord> records1,
    Memory<PatientRecord> records2, double lowerScoreThreshold, double upperScoreThreshold,
    IProgress<int>? progress = null)
  {
    var potentialMatches = new ConcurrentBag<PotentialMatch>();

    var records1CharacterStartAndEndIndex = GetCharactersStartAndEndIndex(records1.Span);
    var records2CharacterStartAndEndIndex = GetCharactersStartAndEndIndex(records2.Span);

    ConcurrentDictionary<PatientRecord, Memory<PatientRecord>> recordSlicesToCompare = new();

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
      for (var recordToCompareIndex = 0; recordToCompareIndex < records1ToCompare.Length; recordToCompareIndex++)
      {
        recordSlicesToCompare.TryAdd(Unsafe.AsRef(records1ToCompare.Span[recordToCompareIndex]), records2ToCompare);
      }
    });


    var partitionerRecordsToCompare = Partitioner.Create(recordSlicesToCompare);
    Parallel.ForEach(partitionerRecordsToCompare, parallelOptions, recordToCompare =>
    {
      for (var i = 0; i < recordToCompare.Value.Length; i++)
      {
        CompareRecords(potentialMatches, ref Unsafe.AsRef(recordToCompare.Key), ref recordToCompare.Value.Span[i],
          lowerScoreThreshold, upperScoreThreshold);
      }

      progress?.Report(1);
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

  private static (int, int)[] GetCharactersStartAndEndIndex(ReadOnlySpan<PatientRecord> records)
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
