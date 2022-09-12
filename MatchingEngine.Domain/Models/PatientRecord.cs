namespace MatchingEngine.Domain.Models;

public sealed record PatientRecord
(
    Guid RecordId,
    string FirstName,
    string MiddleName,
    string LastName,
    string SecondLastName,
    DateTime? BirthDate,
    string City,
    string PhoneNumber
);