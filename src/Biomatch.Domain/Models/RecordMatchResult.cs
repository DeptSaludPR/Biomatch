namespace Biomatch.Domain.Models;

public record RecordMatchResult
(
  IPersonRecord Value,
  List<PotentialMatch> Matches
);
