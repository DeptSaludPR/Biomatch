using Biomatch.Domain.Enums;
using Biomatch.Domain.Helpers;
using FluentAssertions;

namespace Biomatch.Domain.Tests.Unit;

public class StringExtensionsTests
{
  [Theory]
  [InlineData("Juán", "juan")]
  [InlineData("MáRiÖ", "mario")]
  [InlineData("    ", "")]
  public void NormalizeWord_ShouldReplaceAccents_WhenStringWithAccentsIsPassed(
    string word,
    string expected
  )
  {
    // Arrange

    // Act
    var normalizedValue = word.NormalizeWord().ToString();

    // Assert
    normalizedValue.Should().Be(expected);
  }

  [Theory]
  [InlineData("Elvís Gabriel", NameType.Name, new[] { "elvis", "gabriel" })]
  [InlineData("Nieves Miranda", NameType.LastName, new[] { "nieves", "miranda" })]
  public void NormalizeNames_ShouldFixNames_WhenNameIsPassed(
    string name,
    NameType nameType,
    string[] expected
  )
  {
    // Arrange

    // Act
    var result = name.NormalizeNames(nameType).ToList();

    // Assert
    result.Count.Should().Be(expected.Length);
    for (var i = 0; i < result.Count; i++)
    {
      result[i].Should().Be(expected[i]);
    }
  }
}
