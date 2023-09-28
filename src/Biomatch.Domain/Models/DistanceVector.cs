namespace Biomatch.Domain.Models;

public readonly record struct DistanceVector(
  int FirstNameDistance,
  int MiddleNameDistance,
  int LastNameDistance,
  int SecondLastNameDistance,
  int BirthDateDistance,
  int CityDistance,
  int PhoneNumberDistance
)
{
  public static DistanceVector CalculateDistance(
    ref PersonRecordForMatch firstRecord,
    ref PersonRecordForMatch secondRecord
  )
  {
    return new DistanceVector(
      StringDistance.GeneralDemographicFieldDistance(firstRecord.FirstName, secondRecord.FirstName),
      StringDistance.MiddleNameDemographicFieldDistance(
        firstRecord.MiddleName,
        secondRecord.MiddleName
      ),
      StringDistance.GeneralDemographicFieldDistance(firstRecord.LastName, secondRecord.LastName),
      StringDistance.GeneralDemographicFieldDistance(
        firstRecord.SecondLastName,
        secondRecord.SecondLastName
      ),
      StringDistance.DateDemographicFieldDistance(
        firstRecord.BirthDateText,
        secondRecord.BirthDateText
      ),
      StringDistance.GeneralDemographicFieldDistance(firstRecord.City, secondRecord.City),
      StringDistance.GeneralDemographicFieldDistance(
        firstRecord.PhoneNumber,
        secondRecord.PhoneNumber
      )
    );
  }
}
