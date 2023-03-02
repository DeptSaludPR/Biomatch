using FluentAssertions;
using MatchingEngine.Domain.Models;

namespace MatchingEngine.Domain.Tests.Unit;

public class PreprocessTests
{
    [Fact]
    public void PreprocessData_ShouldPreprocessPatientRecord_WhenListOfRecordsIsPassedWithNoDictionary()
    {
        // Arrange
        var patientRecords = new List<PatientRecord>
        {
            new
            (
                "1234",
                "Juan",
                "",
                "Del Pueblo",
                "Del Valle",
                new DateOnly(1990, 02, 15),
                "Aguada",
                "7875982789"
            ),
            new
            (
                "1235",
                "Maria 789 Del Pueblo Del Valle",
                "",
                "",
                "",
                new DateOnly(1990, 03, 16),
                "San Juan",
                "7875982789"
            )
        };

        // Act
        var preprocessedRecords = patientRecords.PreprocessData().ToList();

        // Assert
        preprocessedRecords.Should().HaveCount(patientRecords.Count);
        preprocessedRecords[0].FirstName.Should().Be("juan");
        preprocessedRecords[0].MiddleName.Should().Be("");
        preprocessedRecords[0].LastName.Should().Be("pueblo");
        preprocessedRecords[0].SecondLastName.Should().Be("valle");
        preprocessedRecords[0].BirthDate.Should().Be(new DateOnly(1990, 02, 15));
        preprocessedRecords[0].City.Should().Be("aguada");
        preprocessedRecords[0].PhoneNumber.Should().Be("+1 787-598-2789");
        
        preprocessedRecords[1].FirstName.Should().Be("maria");
        preprocessedRecords[1].MiddleName.Should().Be("");
        preprocessedRecords[1].LastName.Should().Be("pueblo");
        preprocessedRecords[1].SecondLastName.Should().Be("valle");
        preprocessedRecords[1].BirthDate.Should().Be(new DateOnly(1990, 03, 16));
        preprocessedRecords[1].City.Should().Be("sanjuan");
        preprocessedRecords[1].PhoneNumber.Should().Be("+1 787-598-2789");
    }
}