namespace Biomatch.Domain.Models;

public interface IPersonRecord
{
  string RecordId { get; }
  string FirstName { get; }
  string MiddleName { get; }
  string LastName { get; }
  string SecondLastName { get; }
  DateOnly? BirthDate { get; }
  string City { get; }
  string PhoneNumber { get; }
}
