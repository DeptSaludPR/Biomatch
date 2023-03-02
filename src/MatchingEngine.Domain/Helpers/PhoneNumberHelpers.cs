using PhoneNumbers;

namespace MatchingEngine.Domain.Helpers;

public static class PhoneNumberHelpers
{
    public static string Parse(string? phoneNumber)
    {
        string parsedPhoneNumberString;
        if (string.IsNullOrWhiteSpace(phoneNumber)) return string.Empty;
        var phoneNumberUtil = PhoneNumberUtil.GetInstance();
        try
        {
            var parsedPhoneNumber = phoneNumberUtil.Parse(phoneNumber, "US");
            parsedPhoneNumberString = phoneNumberUtil.Format(parsedPhoneNumber, PhoneNumberFormat.INTERNATIONAL);
            if (parsedPhoneNumberString == "+1 NA")
                parsedPhoneNumberString = string.Empty;
        }
        catch
        {
            return string.Empty;
        }

        return parsedPhoneNumberString;
    }
}