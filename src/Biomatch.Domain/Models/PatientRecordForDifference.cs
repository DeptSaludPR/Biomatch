namespace Biomatch.Domain.Models;

public sealed record PatientRecordForDifference
(
  string RecordId,
  string FirstName,
  string MiddleName,
  string LastName,
  string SecondLastName,
  string BirthDate,
  string City,
  string PhoneNumber
);
