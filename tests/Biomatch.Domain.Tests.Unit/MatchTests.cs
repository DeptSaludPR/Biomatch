using Biomatch.Domain.Models;
using FluentAssertions;

namespace Biomatch.Domain.Tests.Unit;

public class MatchTests
{
  [Fact]
  public void CompareRecords_ShouldReturnPotentialMatch_WhenMatchIsFound()
  {
    var patientRecords = new List<IPersonRecord>
    {
      new PersonRecord(
        "1230",
        "Clara",
        "",
        "Pique",
        "",
        new DateOnly(1995, 02, 01),
        "Adjuntas",
        ""
      ),
      new PersonRecord(
        "1875",
        "Clara",
        "",
        "Pique",
        "",
        new DateOnly(1995, 01, 02),
        "Adjuntas",
        ""
      ),
    };

    var preprocessedRecords = patientRecords.PreprocessData().ToArray();

    // Act
    var duplicatesToFix = Match.CompareRecords(
      ref preprocessedRecords[0],
      ref preprocessedRecords[1],
      0.60,
      1.0
    );

    // Assert
    duplicatesToFix.Should().NotBeNull();
  }
}
