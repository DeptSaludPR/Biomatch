using FluentAssertions;
using MatchingEngine.Domain.Helpers;

namespace MatchingEngine.Domain.Tests.Unit.Helpers;

public class PhoneNumberHelpersTests
{
  [Theory]
  [InlineData("7875982789", "+1 787-598-2789")]
  [InlineData("787598278910100", "")]
  [InlineData("+1 NA", "")]
  public void Parse_ShouldTryToParsePhoneNumberOrReturnEmpty_WhenLPhoneNumberStringIsPassed(string phoneNumber,
    string expectedParsedPhoneNumber)
  {
    // Arrange

    // Act
    var parsedPhoneNumber = PhoneNumberHelpers.Parse(phoneNumber);

    // Assert
    parsedPhoneNumber.Should().Be(expectedParsedPhoneNumber);
  }
}
