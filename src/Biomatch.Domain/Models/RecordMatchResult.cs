namespace Biomatch.Domain.Models;

public record RecordMatchResult
(
  PatientRecord Value,
  List<PotentialMatch> Matches
);
