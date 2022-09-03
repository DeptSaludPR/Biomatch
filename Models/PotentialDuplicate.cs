namespace MatchingEngine.Models;

public readonly record struct PotentialDuplicate
(
    PatientRecord Value,
    PatientRecord Match,
    DistanceVector Distance,
    double Score
);