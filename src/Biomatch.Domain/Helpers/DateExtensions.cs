namespace Biomatch.Domain.Helpers;

public static class DateExtensions
{
  public static DateOnly? SanitizeBirthDate(this DateOnly? birthDate)
  {
    if (birthDate is null)
      return null;
    if (birthDate == default(DateOnly))
      return null;

    var now = DateOnly.FromDateTime(DateTime.UtcNow);
    var minDate = new DateOnly(1900, 1, 1);

    if (birthDate < minDate)
      return null;

    if (birthDate > now)
      return null;

    return birthDate;
  }

  public static byte[] ToByteArray(this DateOnly date)
  {
    var buffer = new byte[8];
    var written = 0;

    var month = date.Month;
    if (month < 10)
    {
      buffer[written++] = (byte)'0';
      buffer[written++] = (byte)(month + '0');
    }
    else
    {
      buffer[written++] = (byte)(month / 10 + '0');
      buffer[written++] = (byte)(month % 10 + '0');
    }

    var day = date.Day;
    if (day < 10)
    {
      buffer[written++] = (byte)'0';
      buffer[written++] = (byte)(day + '0');
    }
    else
    {
      buffer[written++] = (byte)(day / 10 + '0');
      buffer[written++] = (byte)(day % 10 + '0');
    }

    var year = date.Year;
    buffer[written++] = (byte)(year / 1000 + '0');
    year %= 1000;
    buffer[written++] = (byte)(year / 100 + '0');
    year %= 100;
    buffer[written++] = (byte)(year / 10 + '0');
    buffer[written] = (byte)(year % 10 + '0');

    return buffer;
  }
}
