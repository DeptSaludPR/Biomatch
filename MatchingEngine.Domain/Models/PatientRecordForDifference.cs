namespace MatchingEngine.Domain.Models;

public sealed record PatientRecordForDifference
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