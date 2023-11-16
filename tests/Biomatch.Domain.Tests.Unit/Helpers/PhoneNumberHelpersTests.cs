using Biomatch.Domain.Helpers;
using FluentAssertions;

namespace Biomatch.Domain.Tests.Unit.Helpers;

public class PhoneNumberHelpersTests
{
  [Theory]
  [InlineData("787-598-2789", "+1 787-598-2789")]
  [InlineData("787598278910100", "")]
  public void Parse_ShouldTryToParsePhoneNumberOrReturnEmpty_WhenPhoneNumberStringIsPassed(
    string phoneNumber,
    string expectedParsedPhoneNumber
  )
  {
    // Arrange

    // Act
    var parsedPhoneNumber = PhoneNumberHelpers.Parse(phoneNumber);

    // Assert
    parsedPhoneNumber.Should().Be(expectedParsedPhoneNumber);
  }
}
