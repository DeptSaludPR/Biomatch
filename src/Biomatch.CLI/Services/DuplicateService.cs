using System.Collections.Concurrent;
using Biomatch.Domain;
using Biomatch.Domain.Models;

namespace Biomatch.CLI.Services;

public static class DuplicateService
{
  public static IEnumerable<PotentialMatch> RunFileComparisons(
    IEnumerable<IPersonRecord> records1,
    IEnumerable<IPersonRecord> records2,
    bool exactMatchesAllowed = false,
    double lowerScoreThreshold = 0.85,
    WordDictionary? firstNamesDictionary = null,
    WordDictionary? middleNamesDictionary = null,
    WordDictionary? lastNamesDictionary = null,
    bool sameDataSetOptionValue = false,
    Func<int, IProgress<int>>? matchProgressReport = null
  )
  {
    var preprocessedRecords1 = records1
      .PreprocessData(firstNamesDictionary, middleNamesDictionary, lastNamesDictionary)
      .ToArray();
    var preprocessedRecords2 = records2
      .PreprocessData(firstNamesDictionary, middleNamesDictionary, lastNamesDictionary)
      .ToArray();

    //check for whether the exact matches are allowed, and set the upper threshold accordingly
    var upperScoreThreshold = exactMatchesAllowed ? 1.0 : 0.99999;

    ConcurrentBag<PotentialMatch> potentialDuplicates;
    if (sameDataSetOptionValue)
    {
      potentialDuplicates = Match.GetPotentialMatchesFromSameDataSet(
        preprocessedRecords1,
        preprocessedRecords2,
        lowerScoreThreshold,
        upperScoreThreshold,
        matchProgressReport
      );
    }
    else
    {
      potentialDuplicates = Match.GetPotentialMatchesFromDifferentDataSet(
        preprocessedRecords1,
        preprocessedRecords2,
        lowerScoreThreshold,
        upperScoreThreshold,
        matchProgressReport
      );
    }
    return potentialDuplicates;
  }
}
