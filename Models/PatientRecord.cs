namespace MatchingEngine.Models;

public record PatientRecord
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