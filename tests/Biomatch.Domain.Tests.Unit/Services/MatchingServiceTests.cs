using Biomatch.Domain.Models;
using Biomatch.Domain.Services;
using FluentAssertions;

namespace Biomatch.Domain.Tests.Unit.Services;

public class MatchingServiceTests
{
  private readonly MatchingService _sut;

  public MatchingServiceTests()
  {
    var patientRecordsToMatch = new List<IPersonRecord>
    {
      new PersonRecord(
        "123456",
        "Elvis",
        "",
        "Nieves",
        "Miranda",
        new DateOnly(1990, 02, 01),
        "Aguada",
        "7875982789"
      ),
      new PersonRecord(
        "1234568",
        "Elvis",
        "Gabriel",
        "Nieves",
        "Miranda",
        new DateOnly(1990, 02, 01),
        "Aguada",
        "7875982789"
      ),
      new PersonRecord(
        "3688374",
        "Juan",
        "",
        "Del Pueblo",
        "",
        new DateOnly(1990, 02, 01),
        "Aguada",
        "7875982789"
      ),
      new PersonRecord(
        "3697831",
        "Juan Del Puéblo",
        "",
        "",
        "",
        new DateOnly(1990, 02, 01),
        "San Juan",
        "7875982789"
      ),
      new PersonRecord(
        "1238",
        "Guillermo",
        "",
        "Perez",
        "",
        new DateOnly(1990, 01, 01),
        "San Juan",
        ""
      ),
      new PersonRecord(
        "1230",
        "Clara",
        "",
        "Pique",
        "",
        new DateOnly(1995, 01, 01),
        "Adjuntas",
        ""
      ),
      new PersonRecord(
        "1276",
        "Juan Del Puéblo",
        "",
        "",
        "",
        new DateOnly(1990, 01, 02),
        "San Juan",
        "7875982789"
      ),
      new PersonRecord(
        "1276678",
        "myrna",
        "",
        "rodriguez",
        "diaz",
        new DateOnly(1978, 01, 26),
        "Bayamón",
        ""
      ),
    };
    _sut = new MatchingService(patientRecordsToMatch);
  }

  [Fact]
  public void FindPotentialMatches_ShouldReturnPotentialMatches_WhenRecordIsPassed()
  {
    // Arrange
    var recordToMatch = new PersonRecord(
      "1234",
      "Elvis",
      "",
      "Nieves",
      "Miranda",
      new DateOnly(1990, 02, 01),
      "Aguada",
      "7875982789"
    );

    // Act
    var possibleMatches = _sut.FindPotentialMatches(recordToMatch, 0.85).ToList();

    // Assert
    possibleMatches.Should().HaveCount(2);
    possibleMatches[0].Score.Should().BeGreaterThan(0.85);
  }

  [Fact]
  public void FindPotentialMatches_ShouldReturnPotentialMatches_WhenMinimalRecordIsPassed()
  {
    // Arrange
    var recordToMatch = new PersonRecord(
      "1234",
      "maria",
      "",
      "",
      "",
      new DateOnly(1978, 05, 26),
      "Dorado",
      ""
    );

    // Act
    var possibleMatches = _sut.FindPotentialMatches(recordToMatch, 0.85).ToList();

    // Assert
    possibleMatches.Should().HaveCount(0);
  }
}
