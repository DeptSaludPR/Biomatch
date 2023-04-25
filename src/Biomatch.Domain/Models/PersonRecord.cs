namespace Biomatch.Domain.Models;

public readonly record struct PersonRecord
(
  string RecordId,
  string FirstName,
  string MiddleName,
  string LastName,
  string SecondLastName,
  DateOnly? BirthDate,
  string City,
  string PhoneNumber
) : IPersonRecord;
