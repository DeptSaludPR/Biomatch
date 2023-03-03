namespace MatchingEngine.Domain.Helpers;

public static class DateTimeExtensions
{
  public static DateOnly? SanitizeBirthDate(this DateOnly? birthDate)
  {
    if (birthDate is null) return null;
    if (birthDate == default(DateOnly)) return null;

    var now = DateOnly.FromDateTime(DateTime.UtcNow);
    var minDate = new DateOnly(1900, 1, 1);

    if (birthDate < minDate) return null;

    if (birthDate > now) return null;

    return birthDate;
  }
}
