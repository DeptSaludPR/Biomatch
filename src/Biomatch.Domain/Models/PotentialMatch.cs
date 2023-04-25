namespace Biomatch.Domain.Models;

public readonly record struct PotentialMatch
(
  IPersonRecord Value,
  IPersonRecord Match,
  DistanceVector Distance,
  double Score
);
