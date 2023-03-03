using System.Globalization;
using System.Text;
using MatchingEngine.Domain.Enums;

namespace MatchingEngine.Domain.Helpers;

public static class StringExtensions
{
  public static IEnumerable<string> NormalizeNames(this string name, NameType nameType)
  {
    var normalizedWords = new List<string>();
    var separators = nameType switch
    {
      NameType.LastName => new[] {' ', '-', '_'},
      _ => new[] {' '}
    };

    ReadOnlySpan<string> words = name.Split(separators);
    for (var i = 0; i < words.Length; i++)
    {
      var word = words[i];
      var normalizedWord = word.NormalizeWord();
      if (string.IsNullOrWhiteSpace(normalizedWord)) continue;

      normalizedWords.Add(normalizedWord);
    }

    return normalizedWords;
  }

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
      if (wordDictionary is not null)
      {
        var suggestItems = wordDictionary.TrySpellCheck(word);
        if (suggestItems.Count == 1)
          word = suggestItems[0].term;
      }

      sanitizedWords.Add(word.SanitizeWord());
    }

    return sanitizedWords;
  }

  public static string SanitizeWord(this string word)
  {
    var sb = new StringBuilder();

    ReadOnlySpan<char> spanValue = word;
    foreach (var c in spanValue)
    {
      if (!char.IsAsciiLetter(c)) continue;

      sb.Append(char.ToLowerInvariant(c));
    }

    return sb.ToString();
  }

  public static string NormalizeWord(this string word)
  {
    var normalizedString = word.Normalize(NormalizationForm.FormD);
    var sb = new StringBuilder();

    foreach (var c in normalizedString.EnumerateRunes())
    {
      var unicodeCategory = Rune.GetUnicodeCategory(c);

      if (unicodeCategory == UnicodeCategory.NonSpacingMark) continue;

      sb.Append(Rune.ToLowerInvariant(c));
    }

    return sb.ToString().Normalize(NormalizationForm.FormC);
  }

  public static IEnumerable<string> RemoveWords(this IEnumerable<string> words, HashSet<string> wordsToRemove)
  {
    foreach (var word in words)
    {
      var hasWord = wordsToRemove.Contains(word);
      if (!hasWord)
      {
        yield return word;
      }
    }
  }
}
