namespace Biomatch.Domain.Models;

public readonly record struct PersonRecordForMatch(
  string RecordId,
  string FirstName,
  string MiddleName,
  string LastName,
  string SecondLastName,
  string FullName,
  DateOnly? BirthDate,
  byte[] BirthDateText,
  string City,
  string PhoneNumber
) : IPersonRecord;
