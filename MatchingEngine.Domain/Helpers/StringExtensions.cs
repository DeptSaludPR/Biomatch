using System.Globalization;
using System.Text;

namespace MatchingEngine.Domain.Helpers;

public static class StringExtensions
{
    public static string SanitizeName(this string name)
    {
        var normalizedString = name.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();

        ReadOnlySpan<char> spanValue = normalizedString;
        foreach (var c in spanValue)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);

            if (unicodeCategory == UnicodeCategory.NonSpacingMark) continue;
            if (!char.IsAsciiLetter(c)) continue;

            sb.Append(char.ToLowerInvariant(c));
        }

        return sb.ToString();
    }
}