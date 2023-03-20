namespace Biomatch.Domain.Models;

public readonly record struct PotentialMatch
(
  PatientRecord Value,
  PatientRecord Match,
  DistanceVector Distance,
  double Score
);
