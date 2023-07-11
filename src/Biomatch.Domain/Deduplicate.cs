using Biomatch.Domain.Models;

namespace Biomatch.Domain;

public static class Deduplicate
{
  public static Dictionary<string, string> TryDeduplicate(IEnumerable<IPersonRecord> records,
    double matchScoreThreshold, WordDictionary? firstNamesDictionary = null,
    WordDictionary? middleNamesDictionary = null, WordDictionary? lastNamesDictionary = null,
    Func<int, IProgress<int>>? matchProgressReport = null)
  {
    var preprocessedRecords =
      records
        .PreprocessData(firstNamesDictionary, middleNamesDictionary, lastNamesDictionary)
        .ToArray();

    var potentialDuplicates =
      Match.GetPotentialMatchesFromSameDataSet(preprocessedRecords, preprocessedRecords, matchScoreThreshold, 1.0,
        matchProgressReport);

    var potentialMatchesGroupedByRecord = potentialDuplicates
      .GroupBy(x => x.Value)
      .ToDictionary(x => x.Key, x =>
        x.Select(y => y.Match)
          .ToList()
      );

    var results = new Dictionary<string, string>();
    var alreadyProcessed = new HashSet<IPersonRecord>();
    Console.WriteLine("Processing potential matches...");
    foreach (var potentialMatch in potentialMatchesGroupedByRecord)
    {
      if (alreadyProcessed.Contains(potentialMatch.Key)) continue;
      var matches = GetAllMatches(potentialMatch.Key, potentialMatchesGroupedByRecord);
      if (matches.Count <= 1) continue;
      alreadyProcessed.UnionWith(matches);

      var firstMatch = matches.First();
      for (var i = 1; i < matches.Count; i++)
      {
        var originalValue = matches.ElementAt(i);
        results.TryAdd(originalValue.RecordId, firstMatch.RecordId);
      }
    }

    return results;
  }

  // Use Breadth-First Search to find all matches for a given record
  private static HashSet<IPersonRecord> GetAllMatches(IPersonRecord start,
    Dictionary<IPersonRecord, List<IPersonRecord>> matches)
  {
    var visited = new HashSet<IPersonRecord>();
    var queue = new Queue<IPersonRecord>();

    visited.Add(start);
    queue.Enqueue(start);

    while (queue.Count > 0)
    {
      var current = queue.Dequeue();

      if (!matches.TryGetValue(current, out var match1)) continue;
      foreach (var match in match1)
      {
        if (visited.Contains(match)) continue;
        visited.Add(match);
        queue.Enqueue(match);
      }
    }

    return visited; // Return all visited nodes, these are all matches directly or indirectly
  }
}
