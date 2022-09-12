namespace MatchingEngine.Domain.Helpers;

public static class DateTimeExtensions
{
    public static DateTime? SanitizeBirthDate(this DateTime? birthDate)
    {
        if (birthDate is null) return null;
        if (birthDate == default(DateTime)) return null;

        var now = DateTime.UtcNow;
        var minDate = new DateTime(1900, 1, 1);

        if (birthDate < minDate) return null;

        if (birthDate > now) return null;

        return birthDate;
    }
}