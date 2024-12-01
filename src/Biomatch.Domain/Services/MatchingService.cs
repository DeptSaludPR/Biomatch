using System.Collections.Concurrent;
using System.Collections.Frozen;
using Biomatch.Domain.Models;

namespace Biomatch.Domain.Services;

public sealed class MatchingService
{
  private readonly FrozenSet<string> _prepositionsToRemove;
  private readonly FrozenSet<string> _suffixesToRemove;
  private readonly WordDictionary? _firstNamesDictionary;
  private readonly WordDictionary? _middleNamesDictionary;
  private readonly WordDictionary? _lastNamesDictionary;

  private readonly ConcurrentDictionary<string, PersonRecordForMatch> _preprocessedRecordsToMatch;

  public MatchingService(
    IEnumerable<IPersonRecord> recordsToMatch,
    IEnumerable<WordFrequency>? firstNamesDictionary = null,
    IEnumerable<WordFrequency>? middleNamesDictionary = null,
    IEnumerable<WordFrequency>? lastNamesDictionary = null
  )
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
      "san",
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
      "mrs",
    }.ToFrozenSet();

    if (firstNamesDictionary is not null)
      _firstNamesDictionary = WordDictionary.CreateWordDictionary(firstNamesDictionary);
    if (middleNamesDictionary is not null)
      _middleNamesDictionary = WordDictionary.CreateWordDictionary(middleNamesDictionary);
    if (lastNamesDictionary is not null)
      _lastNamesDictionary = WordDictionary.CreateWordDictionary(lastNamesDictionary);

    var preprocessedRecords = recordsToMatch
      .PreprocessData(_firstNamesDictionary, _middleNamesDictionary, _lastNamesDictionary)
      .ToList();
    _preprocessedRecordsToMatch = new ConcurrentDictionary<string, PersonRecordForMatch>(
      preprocessedRecords.ToDictionary(e => e.RecordId)
    );
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
