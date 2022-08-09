namespace MatchingEngine.Models;

public class PatientRecord : Record
{
    public string FirstName { get; set; } = null!;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = null!;
    public string? SecondLastName { get; set; }
    public string BirthDate { get; set; } = null!;
    public string? City { get; set; }
    public string? PhoneNumber { get; set; }

    public string CsvAsteriskString()
    {
        var csvStr = FirstName + "*" + MiddleName + "*" + LastName + "*";
        csvStr += SecondLastName + "*" + BirthDate + "*" + City + "*" + PhoneNumber;
        return csvStr;
    }
}