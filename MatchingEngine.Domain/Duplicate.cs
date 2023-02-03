using System.Collections.Concurrent;
using MatchingEngine.Domain.Helpers;
using MatchingEngine.Domain.Models;

namespace MatchingEngine.Domain;

public static class Duplicate
{
    private static ConcurrentDictionary<char, int[]> GetCharactersStartAndEndIndex(ReadOnlySpan<PatientRecord> records)
    {
        var characterIndex = new ConcurrentDictionary<char, int[]>();

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
            characterIndex.TryAdd(letter, new[] {startIndex.Value, endIndex ?? records.Length - 1});
        }

        return characterIndex;
    }

    public static ConcurrentBag<PotentialDuplicate> GetPotentialDuplicates(PatientRecord[] records1,
        PatientRecord[] records2, double lowerScoreThreshold, double upperScoreThreshold)
    {
        var potentialDuplicates = new ConcurrentBag<PotentialDuplicate>();

        var records1CharacterStartAndEndIndex = GetCharactersStartAndEndIndex(records1);
        var records2CharacterStartAndEndIndex = GetCharactersStartAndEndIndex(records2);

        Parallel.ForEach(records1CharacterStartAndEndIndex, record1StartAndEnd =>
        {
            var records2StartAndEndFound =
                records2CharacterStartAndEndIndex.TryGetValue(record1StartAndEnd.Key, out var records2StartAndEnd);

            var records1ToCompare = records1.AsMemory(record1StartAndEnd.Value[0],
                record1StartAndEnd.Value[1] - record1StartAndEnd.Value[0]);

            var records2ToCompare = records2StartAndEndFound && records2StartAndEnd != null
                ? records2.AsMemory(records2StartAndEnd[0], records2StartAndEnd[1] - records2StartAndEnd[0])
                : records2;

            Parallel.For(0, records1ToCompare.Length, recordToCompareIndex =>
            {
                var primaryRecord = records1ToCompare.Span[recordToCompareIndex];
                Parallel.For(0, records2ToCompare.Length, list2Index =>
                {
                    var secondaryRecord = records2ToCompare.Span[list2Index];
                    CompareRecords(potentialDuplicates, primaryRecord, secondaryRecord,
                        lowerScoreThreshold, upperScoreThreshold);
                });
            });
        });

        return potentialDuplicates;
    }

    private static void CompareRecords(ConcurrentBag<PotentialDuplicate> potentialDuplicates,
        PatientRecord primaryRecord, PatientRecord secondaryRecord,
        double lowerScoreThreshold, double upperScoreThreshold)
    {
        //check if the first character of the first name is equal
        if (!StringHelpers.FirstCharactersAreEqual(primaryRecord.FirstName, secondaryRecord.FirstName) ||
            primaryRecord.RecordId == secondaryRecord.RecordId) return;
        //get the distance vector for the ith vector of the first table and the jth record of the second table
        var distanceVector = DistanceVector.CalculateDistance(primaryRecord, secondaryRecord);
        var tempScore = Score.CalculateFinalScore(ref distanceVector);
        if (tempScore >= lowerScoreThreshold && tempScore <= upperScoreThreshold)
        {
            potentialDuplicates.Add(
                new PotentialDuplicate(primaryRecord, secondaryRecord, distanceVector, tempScore));
        }
    }
}