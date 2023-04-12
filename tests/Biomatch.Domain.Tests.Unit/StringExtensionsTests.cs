using Biomatch.Domain.Helpers;
using FluentAssertions;

namespace Biomatch.Domain.Tests.Unit;

public class StringExtensionsTests
{
  [Theory]
  [InlineData("Juán", "juan")]
  [InlineData("MáRiÖ", "mario")]
  [InlineData("    ", "")]
  public void NormalizeWord_ShouldReplaceAccents_WhenStringWithAccentsIsPassed(string word, string expected)
  {
    // Arrange

    // Act
    var normalizedValue = word.NormalizeWord().ToString();

    // Assert
    normalizedValue.Should().Be(expected);
  }
}
