using System.Collections.Concurrent;
using MatchingEngine.Domain.Models;

namespace MatchingEngine.Domain;

public static class Duplicate
{
    public static ConcurrentBag<PotentialDuplicate> GetPotentialDuplicates(IReadOnlyList<PatientRecord> records1,
        IReadOnlyList<PatientRecord> records2, int startIndex1, int endIndex1, int startIndex2, int endIndex2,
        double lowerScoreThreshold, double upperScoreThreshold)
    {
        var potentialDuplicates = new ConcurrentBag<PotentialDuplicate>();
        Parallel.For(startIndex1, endIndex1, list1Index =>
        {
            var primaryRecord = records1[list1Index];
            Parallel.For(startIndex2, endIndex2, list2Index =>
            {
                var tempRecord = records2[list2Index];
                //check if the first character of the first name is equal
                if (!Helpers.FirstCharactersAreEqual(primaryRecord.FirstName, tempRecord.FirstName) ||
                    primaryRecord.RecordId == tempRecord.RecordId) return;
                //get the distance vector for the ith vector of the first table and the jth record of the second table
                var distanceVector = DistanceVector.CalculateDistance(primaryRecord, tempRecord);
                var tempScore = Score.allFieldsScore_StepMode(ref distanceVector);
                if (tempScore >= lowerScoreThreshold && tempScore <= upperScoreThreshold)
                {
                    potentialDuplicates.Add(new PotentialDuplicate(primaryRecord, tempRecord, distanceVector, tempScore));
                }
            });
        });

        return potentialDuplicates;
    }
}