using System.Collections.Concurrent;
using MatchingEngine.Domain.Helpers;
using MatchingEngine.Domain.Models;

namespace MatchingEngine.Domain;

public static class Duplicate
{
    private static Dictionary<char, int[]> GetCharactersStartAndEndIndex(IReadOnlyList<PatientRecord> records)
    {
        var characterIndex = new Dictionary<char, int[]>();

        var currentIndex = 0;
        for (var letter = 'a'; letter <= 'z'; letter++)
        {
            int? startIndex = null;
            int? endIndex = null;
            for (var i = currentIndex; i < records.Count; i++)
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
            characterIndex.TryAdd(letter, new[] {startIndex.Value, endIndex ?? records.Count - 1});
        }

        return characterIndex;
    }
    
    public static ConcurrentBag<PotentialDuplicate> GetPotentialDuplicates(IReadOnlyList<PatientRecord> records1,
        IReadOnlyList<PatientRecord> records2, int startIndex1, int endIndex1, int startIndex2, int endIndex2,
        double lowerScoreThreshold, double upperScoreThreshold)
    {
        var characterStartAndEndIndex = GetCharactersStartAndEndIndex(records2);
        var potentialDuplicates = new ConcurrentBag<PotentialDuplicate>();
        Parallel.For(startIndex1, endIndex1, list1Index =>
        {
            var primaryRecord = records1[list1Index];
            int[]? indices = null;
            _ = primaryRecord.FirstName.Length > 0 &&
                characterStartAndEndIndex.TryGetValue(primaryRecord.FirstName[0], out indices);

            Parallel.For(indices != null ? indices[0] : startIndex2,
                indices != null ? indices[1] : endIndex2, list2Index =>
                {
                    var tempRecord = records2[list2Index];
                    //check if the first character of the first name is equal
                    if (!StringHelpers.FirstCharactersAreEqual(primaryRecord.FirstName, tempRecord.FirstName) ||
                        primaryRecord.RecordId == tempRecord.RecordId) return;
                    //get the distance vector for the ith vector of the first table and the jth record of the second table
                    var distanceVector = DistanceVector.CalculateDistance(primaryRecord, tempRecord);
                    var tempScore = Score.allFieldsScore_StepMode(ref distanceVector);
                    if (tempScore >= lowerScoreThreshold && tempScore <= upperScoreThreshold)
                    {
                        potentialDuplicates.Add(
                            new PotentialDuplicate(primaryRecord, tempRecord, distanceVector, tempScore));
                    }
                });
        });
        

        return potentialDuplicates;
    }
}