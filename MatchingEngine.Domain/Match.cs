using MatchingEngine.Domain.Models;

namespace MatchingEngine.Domain;

public static class Match
{
    public static IEnumerable<PotentialDuplicate> TryMatch(IEnumerable<PatientRecord> records1,
        IEnumerable<PatientRecord> records2, double matchScoreThreshold, WordDictionary? firstNamesDictionary = null,
        WordDictionary? middleNamesDictionary = null, WordDictionary? lastNamesDictionary = null)
    {
        var preprocessedRecords1 =
            records1.PreprocessData(firstNamesDictionary, middleNamesDictionary, lastNamesDictionary).ToArray();
        var preprocessedRecords2 =
            records2.PreprocessData(firstNamesDictionary, middleNamesDictionary, lastNamesDictionary).ToArray();

        var potentialDuplicates =
            Duplicate.GetPotentialDuplicates(preprocessedRecords1, preprocessedRecords2, matchScoreThreshold, 1.0);

        var matchedRecords = potentialDuplicates
            .GroupBy(x => x.Value)
            .Select(x => x.MaxBy(y => y.Score))
            .OrderByDescending(x => x.Score)
            .ToList();

        return matchedRecords;
    }
}