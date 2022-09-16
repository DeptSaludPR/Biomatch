using System.Globalization;
using System.Text;
using MatchingEngine.Domain.Enums;

namespace MatchingEngine.Domain.Helpers;

public static class StringExtensions
{
    public static IEnumerable<string> SanitizeName(this string name, NameType nameType = NameType.Name,
        WordDictionary? wordDictionary = null)
    {
        var sanitizedWords = new List<string>();
        var separators = nameType switch
        {
            NameType.LastName => new[] {' ', '-', '_'},
            _ => new[] {' '}
        };

        ReadOnlySpan<string> words = name.Split(separators);
        for (var i = 0; i < words.Length; i++)
        {
            var word = words[i];
            var sanitizedWord = word.NormalizeWord();
            if (string.IsNullOrEmpty(sanitizedWord)) continue;
            if (wordDictionary is not null)
            {
                var suggestItems = wordDictionary.TrySpellCheck(sanitizedWord);
                if (suggestItems.Count == 1)
                    sanitizedWord = suggestItems[0].term;
            }

            sanitizedWords.Add(sanitizedWord.SanitizeWord());
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
    
    public static string NormalizeWord(this string word)
    {
        var normalizedString = word.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();

        ReadOnlySpan<char> spanValue = normalizedString;
        foreach (var c in spanValue)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);

            if (unicodeCategory == UnicodeCategory.NonSpacingMark) continue;

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
            "lcdo", "lcda", "dr", "dra", "sor", "jr", "junior", "sr", "sra", "ii", "iii", "mr", "ms"
        };

        return words.Where(w => !suffixes.Contains(w));
    }
}