namespace MatchingEngine.Domain.Models;

public sealed record PatientRecord
(
    string RecordId,
    string FirstName,
    string MiddleName,
    string LastName,
    string SecondLastName,
    DateOnly? BirthDate,
    string City,
    string PhoneNumber
);