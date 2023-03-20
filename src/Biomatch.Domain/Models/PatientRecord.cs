namespace Biomatch.Domain.Models;

public readonly record struct PatientRecord
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
