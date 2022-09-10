using System.Globalization;
using System.Text;

namespace MatchingEngine.Domain.Helpers;

public static class StringExtensions
{
    public static List<string> SanitizeName(this string name)
    {
        var sanitizedWords = new List<string>();
        var words = name.Split(' ');
        foreach (var word in words)
        {
            var sanitizedWord = word.SanitizeWord();
            if (string.IsNullOrEmpty(sanitizedWord)) continue;

            sanitizedWords.Add(sanitizedWord);
        }

        return sanitizedWords;
    }

    public static string SanitizeWord(this string word)
    {
        var normalizedString = word.Normalize(NormalizationForm.FormD);
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