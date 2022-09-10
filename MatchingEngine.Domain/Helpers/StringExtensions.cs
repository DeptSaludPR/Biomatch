using System.Globalization;
using System.Text;

namespace MatchingEngine.Domain.Helpers;

public static class StringExtensions
{
    public static IEnumerable<string> SanitizeName(this string name)
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

    public static IEnumerable<string> RemovePrepositions(this IEnumerable<string> words)
    {
        var prepositions = new List<string>
        {
            "el", "la", "los", "las", "de", "del", "en", "y", "a", "di", "da", "le", "san"
        };

        return words.Where(w => !prepositions.Contains(w));
    }
    
    public static IEnumerable<string> RemoveSuffixes(this IEnumerable<string> words)
    {
        var suffixes = new List<string>
        {
            "lcdo", "lcda", "dr", "dra", "sor", "jr", "junior", "sr", "sra", "ii", "iii"
        };

        return words.Where(w => !suffixes.Contains(w));
    }
}