namespace MatchingEngine.Models;

public record PotentialDuplicate
(
    PatientRecord Value,
    PatientRecord Match,
    DistanceVector Distance,
    double Score
);