using Biomatch.Domain.Models;
using FluentAssertions;

namespace Biomatch.Domain.Tests.Unit;

public class DeduplicateTests
{
  [Fact]
  public void TryDeduplicate_ShouldReturnDuplicateRecords_WhenListOfRecordsIsPassedWithNoDictionary()
  {
    // Arrange
    var patientRecords = new List<IPersonRecord>
    {
      new PersonRecord
      (
        "3688374",
        "Juan",
        "",
        "Del Pueblo",
        "",
        new DateOnly(1990, 02, 01),
        "Aguada",
        "7875982789"
      ),
      new PersonRecord
      (
        "3697831",
        "Juan Del Puéblo",
        "",
        "",
        "",
        new DateOnly(1990, 02, 01),
        "San Juan",
        "7875982789"
      ),
      new PersonRecord
      (
        "1238",
        "Guillermo",
        "",
        "Perez",
        "",
        new DateOnly(1990, 01, 01),
        "San Juan",
        ""
      ),
      new PersonRecord
      (
        "1230",
        "Clara",
        "",
        "Pique",
        "",
        new DateOnly(1995, 02, 01),
        "Adjuntas",
        ""
      ),
      new PersonRecord
      (
        "1875",
        "Clara",
        "",
        "1234",
        "Pique",
        new DateOnly(1995, 01, 02),
        "Adjuntas",
        ""
      ),
      new PersonRecord
      (
        "1276",
        "Juan Del Puéblo",
        "",
        "",
        "",
        new DateOnly(1990, 01, 02),
        "San Juan",
        "7875982789"
      ),
    };

    // Act
    var duplicatesToFix = Deduplicate
      .TryDeduplicate(patientRecords, 0.85)
      .ToList();

    // Assert
    duplicatesToFix.Should().HaveCount(2);
    var duplicateUniqueRecordIds = duplicatesToFix.GroupBy(e => e.Value).ToList();
    duplicateUniqueRecordIds.Should().HaveCount(1);
  }
}
