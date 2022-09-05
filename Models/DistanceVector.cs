namespace MatchingEngine.Models;

public readonly record struct DistanceVector
{
    public int FirstNameDistance { get; private init; }
    public int MiddleNameDistance { get; private init; }
    public int LastNameDistance { get; private init; }
    public int SecondLastNameDistance { get; private init; }
    public int BirthDateDistance { get; private init; }
    public int CityDistance { get; private init; }
    public int PhoneNumberDistance { get; private init; }

    public override string ToString()
    {
        return
            $"First Name: {FirstNameDistance} MiddleName: {MiddleNameDistance} Lastname: {LastNameDistance} SecondLastname: {SecondLastNameDistance} BirthDate: {BirthDateDistance} City: {CityDistance} PhoneNumber: {PhoneNumberDistance}";
    }

    public static DistanceVector CalculateDistance(ref PatientRecord firstRecord, ref PatientRecord secondRecord)
    {
        return new DistanceVector
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
    }
}