namespace MatchingEngine.Models;

public class DistanceVector : PatientRecord
{
    public int FirstNameDistance { get; set; }
    public int MiddleNameDistance { get; set; }
    public int LastNameDistance { get; set; }
    public int SecondLastNameDistance { get; set; }
    public int BirthDateDistance { get; set; }
    public int CityDistance { get; set; }
    public int PhoneNumberDistance { get; set; }

    public DistanceVector(int firstName = 0, int middleName = 0, int lastName = 0, int secondLastName = 0,
        int birthDate = 0,
        int city = 0, int phoneNumber = 0)
    {
        FirstNameDistance = firstName;
        MiddleNameDistance = middleName;
        LastNameDistance = lastName;
        SecondLastNameDistance = secondLastName;
        BirthDateDistance = birthDate;
        CityDistance = city;
        PhoneNumberDistance = phoneNumber;
    }

    public override string ToString()
    {
        return
            $"First Name: {FirstNameDistance} MiddleName: {MiddleNameDistance} Lastname: {LastNameDistance} SecondLastname: {SecondLastNameDistance} BirthDate: {BirthDateDistance} City: {CityDistance} PhoneNumber: {PhoneNumberDistance}";
    }

    public static DistanceVector CalculateDistance(PatientRecord firstRecord, PatientRecord secondRecord)
    {
        DistanceVector result = new()
        {
            FirstNameDistance = StringDistance.GeneralDemographicFieldDistance(firstRecord.FirstName,
                secondRecord.FirstName),
            MiddleNameDistance =
                StringDistance.GeneralDemographicFieldDistance(firstRecord.MiddleName, secondRecord.MiddleName),
            LastNameDistance =
                StringDistance.GeneralDemographicFieldDistance(firstRecord.LastName, secondRecord.LastName),
            SecondLastNameDistance = StringDistance.GeneralDemographicFieldDistance(firstRecord.SecondLastName,
                secondRecord.SecondLastName),
            BirthDateDistance =
                StringDistance.GeneralDemographicFieldDistance(firstRecord.BirthDate, secondRecord.BirthDate),
            CityDistance = StringDistance.GeneralDemographicFieldDistance(firstRecord.City, secondRecord.City),
            PhoneNumberDistance =
                StringDistance.GeneralDemographicFieldDistance(firstRecord.PhoneNumber, secondRecord.PhoneNumber)
        };
        return result;
    }
}