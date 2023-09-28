using Biomatch.Domain.Helpers;
using Biomatch.Domain.Models;
using FluentAssertions;

namespace Biomatch.Domain.Tests.Unit.Models;

public class DistanceVectorTests
{
  [Fact]
  public void CalculateDistance_ShouldGet0DistanceOnBirthDate_WhenRecordsWithBirthDatesWithMixedDayAndMonthArePassed()
  {
    // Arrange
    var birthDate1 = new DateOnly(1990, 02, 01);
    var birthDate2 = new DateOnly(1990, 01, 02);
    var record1 = new PersonRecordForMatch(
      "3688374",
      "Juan",
      "",
      "Del Pueblo",
      "",
      birthDate1,
      birthDate1.ToByteArray(),
      "Aguada",
      "7875982789"
    );
    var record2 = new PersonRecordForMatch(
      "3688374",
      "Juan",
      "",
      "Del Pueblo",
      "",
      birthDate2,
      birthDate2.ToByteArray(),
      "Aguada",
      "7875982789"
    );

    // Act
    var distanceVector = DistanceVector.CalculateDistance(ref record1, ref record2);

    // Assert
    distanceVector.BirthDateDistance.Should().Be(0);
  }
}
