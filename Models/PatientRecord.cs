namespace MatchingEngine.Models;

public readonly record struct PatientRecord
(
    Guid RecordId,
    string FirstName,
    string MiddleName,
    string LastName,
    string SecondLastName,
    string BirthDate,
    string City,
    string PhoneNumber
);