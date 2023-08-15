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

  private PersonRecordForMatch[] _preprocessedRecordsToMatch;

  public MatchingService(IEnumerable<IPersonRecord> recordsToMatch)
  {
    _prepositionsToRemove = new HashSet<string>
    {
      "el", "la", "los", "las", "de", "del", "en", "y", "a", "di", "da", "le", "san"
    }.ToFrozenSet();
    _suffixesToRemove = new HashSet<string>
    {
      "lcdo", "lcda", "dr", "dra", "sor", "jr", "junior", "sr", "sra", "ii", "iii", "mr", "ms", "mrs"
    }.ToFrozenSet();
    _preprocessedRecordsToMatch = recordsToMatch.PreprocessData().ToArray();
    var firstNameFrequencyDictionary = _preprocessedRecordsToMatch
      .GroupBy(e => e.FirstName)
      .Where(e => e.Count() > 20 && e.Key.Length > 3)
      .Select(e => new FrequencyDictionary
      (
        e.Key,
        e.Count()
      ));
    var middleNameFrequencyDictionary = _preprocessedRecordsToMatch
      .GroupBy(e => e.MiddleName)
      .Where(e => e.Count() > 20 && e.Key.Length > 3)
      .Select(e => new FrequencyDictionary
      (
        e.Key,
        e.Count()
      ));
    var firstLastNameFrequencyDictionary = _preprocessedRecordsToMatch
      .GroupBy(e => e.LastName)
      .Where(e => e.Count() > 20 && e.Key.Length > 3)
      .Select(e => new FrequencyDictionary
      (
        e.Key,
        e.Count()
      ));
    _firstNamesDictionary = WordDictionary.CreateWordDictionary(firstNameFrequencyDictionary);
    _middleNamesDictionary = WordDictionary.CreateWordDictionary(middleNameFrequencyDictionary);
    _lastNamesDictionary = WordDictionary.CreateWordDictionary(firstLastNameFrequencyDictionary);
  }

  public IEnumerable<PotentialMatch> FindPotentialMatches(IPersonRecord record, double matchScoreThreshold)
  {
    var preprocessedRecord =
      record.PreprocessRecord(_prepositionsToRemove, _suffixesToRemove, _firstNamesDictionary, _middleNamesDictionary,
        _lastNamesDictionary);

    return Match.GetPotentialMatchesFromRecords(preprocessedRecord, _preprocessedRecordsToMatch,
      matchScoreThreshold, 1.0);
  }

  public void AddPersonToMatchData(IPersonRecord record)
  {
    var preprocessedRecord =
      record.PreprocessRecord(_prepositionsToRemove, _suffixesToRemove, _firstNamesDictionary, _middleNamesDictionary,
        _lastNamesDictionary);

    _preprocessedRecordsToMatch = _preprocessedRecordsToMatch.Append(preprocessedRecord)
      .OrderBy(e => e.FirstName)
      .ToArray();
  }

  public void RemovePersonFromMatchData(string recordId)
  {
    _preprocessedRecordsToMatch = _preprocessedRecordsToMatch.Where(e => e.RecordId != recordId).ToArray();
  }

  public void UpdatePersonMatchData(IPersonRecord record)
  {
    var preprocessedRecord =
      record.PreprocessRecord(_prepositionsToRemove, _suffixesToRemove, _firstNamesDictionary, _middleNamesDictionary,
        _lastNamesDictionary);

    _preprocessedRecordsToMatch = _preprocessedRecordsToMatch
      .Where(e => e.RecordId != record.RecordId)
      .Append(preprocessedRecord)
      .OrderBy(e => e.FirstName)
      .ToArray();
  }
}
