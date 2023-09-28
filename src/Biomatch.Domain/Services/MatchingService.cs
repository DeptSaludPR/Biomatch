using System.Collections.Concurrent;
using System.Collections.Frozen;
using Biomatch.Domain.Models;

namespace Biomatch.Domain.Services;

public class MatchingService
{
  private readonly FrozenSet<string> _prepositionsToRemove;
  private readonly FrozenSet<string> _suffixesToRemove;
  private readonly WordDictionary? _firstNamesDictionary;
  private readonly WordDictionary? _middleNamesDictionary;
  private readonly WordDictionary? _lastNamesDictionary;

  private readonly ConcurrentDictionary<string, PersonRecordForMatch> _preprocessedRecordsToMatch;

  public MatchingService(IEnumerable<IPersonRecord> recordsToMatch)
  {
    _prepositionsToRemove = new HashSet<string>
    {
      "el",
      "la",
      "los",
      "las",
      "de",
      "del",
      "en",
      "y",
      "a",
      "di",
      "da",
      "le",
      "san"
    }.ToFrozenSet();
    _suffixesToRemove = new HashSet<string>
    {
      "lcdo",
      "lcda",
      "dr",
      "dra",
      "sor",
      "jr",
      "junior",
      "sr",
      "sra",
      "ii",
      "iii",
      "mr",
      "ms",
      "mrs"
    }.ToFrozenSet();

    var preprocessedRecords = recordsToMatch.PreprocessData().ToList();
    _preprocessedRecordsToMatch = new ConcurrentDictionary<string, PersonRecordForMatch>(
      preprocessedRecords.ToDictionary(e => e.RecordId)
    );

    var firstNameFrequencyDictionary = preprocessedRecords
      .GroupBy(e => e.FirstName)
      .Where(e => e.Count() > 20 && e.Key.Length > 3)
      .Select(e => new FrequencyDictionary(e.Key, e.Count()));
    var middleNameFrequencyDictionary = preprocessedRecords
      .GroupBy(e => e.MiddleName)
      .Where(e => e.Count() > 20 && e.Key.Length > 3)
      .Select(e => new FrequencyDictionary(e.Key, e.Count()));
    var firstLastNameFrequencyDictionary = preprocessedRecords
      .GroupBy(e => e.LastName)
      .Where(e => e.Count() > 20 && e.Key.Length > 3)
      .Select(e => new FrequencyDictionary(e.Key, e.Count()));
    _firstNamesDictionary = WordDictionary.CreateWordDictionary(firstNameFrequencyDictionary);
    _middleNamesDictionary = WordDictionary.CreateWordDictionary(middleNameFrequencyDictionary);
    _lastNamesDictionary = WordDictionary.CreateWordDictionary(firstLastNameFrequencyDictionary);
  }

  public IEnumerable<PotentialMatch> FindPotentialMatches(
    IPersonRecord record,
    double matchScoreThreshold
  )
  {
    var preprocessedRecord = record.PreprocessRecord(
      _prepositionsToRemove,
      _suffixesToRemove,
      _firstNamesDictionary,
      _middleNamesDictionary,
      _lastNamesDictionary
    );

    return Match.GetPotentialMatchesFromRecords(
      preprocessedRecord,
      _preprocessedRecordsToMatch.Values,
      matchScoreThreshold,
      1.0
    );
  }

  public bool TryAddPersonToMatchData(IPersonRecord record)
  {
    var preprocessedRecord = record.PreprocessRecord(
      _prepositionsToRemove,
      _suffixesToRemove,
      _firstNamesDictionary,
      _middleNamesDictionary,
      _lastNamesDictionary
    );

    return _preprocessedRecordsToMatch.TryAdd(preprocessedRecord.RecordId, preprocessedRecord);
  }

  public bool TryRemovePersonFromMatchData(string recordId, out PersonRecordForMatch recordToRemove)
  {
    return _preprocessedRecordsToMatch.TryRemove(recordId, out recordToRemove);
  }

  public void UpdatePersonMatchData(IPersonRecord record)
  {
    var preprocessedRecord = record.PreprocessRecord(
      _prepositionsToRemove,
      _suffixesToRemove,
      _firstNamesDictionary,
      _middleNamesDictionary,
      _lastNamesDictionary
    );

    _preprocessedRecordsToMatch.AddOrUpdate(
      preprocessedRecord.RecordId,
      preprocessedRecord,
      (_, _) => preprocessedRecord
    );
  }
}
