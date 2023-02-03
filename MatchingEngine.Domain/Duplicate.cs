using System.Collections.Concurrent;
using MatchingEngine.Domain.Models;

namespace MatchingEngine.Domain;

public static class Duplicate
{
    private static Dictionary<char, int[]> GetCharactersStartAndEndIndex(ReadOnlySpan<PatientRecord> records)
    {
        var characterIndex = new Dictionary<char, int[]>();

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

    public static ConcurrentBag<PotentialDuplicate> GetPotentialDuplicates(Memory<PatientRecord> records1,
        Memory<PatientRecord> records2, double lowerScoreThreshold, double upperScoreThreshold)
    {
        var potentialDuplicates = new ConcurrentBag<PotentialDuplicate>();

        var records1CharacterStartAndEndIndex = GetCharactersStartAndEndIndex(records1.Span);
        var records2CharacterStartAndEndIndex = GetCharactersStartAndEndIndex(records2.Span);

        Parallel.ForEach(records1CharacterStartAndEndIndex, record1StartAndEnd =>
        {
            var records2StartAndEndFound =
                records2CharacterStartAndEndIndex.TryGetValue(record1StartAndEnd.Key, out var records2StartAndEnd);

            var records1ToCompare = records1.Slice(record1StartAndEnd.Value[0],
                record1StartAndEnd.Value[1] - record1StartAndEnd.Value[0]);

            var records2ToCompare = records2StartAndEndFound && records2StartAndEnd != null
                ? records2.Slice(records2StartAndEnd[0], records2StartAndEnd[1] - records2StartAndEnd[0])
                : records2;

            Parallel.For(0, records1ToCompare.Length, recordToCompareIndex =>
            {
                var primaryRecord = records1ToCompare.Span[recordToCompareIndex];
                for (var i = 0; i < records2ToCompare.Length; i++)
                {
                    var secondaryRecord = records2ToCompare.Span[i];
                    CompareRecords(potentialDuplicates, ref primaryRecord, ref secondaryRecord,
                        lowerScoreThreshold, upperScoreThreshold);
                }
            });
        });

        return potentialDuplicates;
    }

    private static void CompareRecords(ConcurrentBag<PotentialDuplicate> potentialDuplicates,
        ref PatientRecord primaryRecord, ref PatientRecord secondaryRecord,
        double lowerScoreThreshold, double upperScoreThreshold)
    {
        if (primaryRecord.RecordId == secondaryRecord.RecordId) return;
        //get the distance vector for the ith vector of the first table and the jth record of the second table
        var distanceVector = DistanceVector.CalculateDistance(ref primaryRecord, ref secondaryRecord);
        var tempScore = Score.CalculateFinalScore(ref distanceVector);
        if (tempScore >= lowerScoreThreshold && tempScore <= upperScoreThreshold)
        {
            potentialDuplicates.Add(
                new PotentialDuplicate(primaryRecord, secondaryRecord, distanceVector, tempScore));
        }
    }
}