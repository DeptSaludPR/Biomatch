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
            if (!phoneNumberUtil.IsValidNumber(parsedPhoneNumber))
                return string.Empty;
            parsedPhoneNumberString = phoneNumberUtil.Format(parsedPhoneNumber, PhoneNumberFormat.INTERNATIONAL);
        }
        catch
        {
            return string.Empty;
        }

        return parsedPhoneNumberString;
    }
}