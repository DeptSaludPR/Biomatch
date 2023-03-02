namespace MatchingEngine.Domain.Models;

public readonly record struct PersonName
(
    string FirstName,
    string MiddleName,
    string LastName,
    string SecondLastName
);