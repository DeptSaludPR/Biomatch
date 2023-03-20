namespace MatchingEngine.Domain.Models;

public record RecordMatchResult
(
  PatientRecord Value,
  PotentialMatch[] Matches
);
