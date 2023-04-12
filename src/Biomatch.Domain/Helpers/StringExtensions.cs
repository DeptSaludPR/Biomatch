using System.Globalization;
using System.Text;
using Biomatch.Domain.Enums;

namespace Biomatch.Domain.Helpers;

public static class StringExtensions
{
  public static IEnumerable<string> NormalizeNames(this string name, NameType nameType)
  {
    var separators = nameType switch
    {
      NameType.LastName => new[] {' ', '-', '_'},
      _ => new[] {' '}
    };

    var words = name.Split(separators);
    foreach (var word in words)
    {
      if (word.Length == 0) continue;
      var normalizedWord = word.NormalizeWord();
      if (normalizedWord.Length == 0) continue;

      yield return normalizedWord.ToString();
    }
  }

  public static IEnumerable<string> SanitizeName(this string name, NameType nameType = NameType.Name,
    WordDictionary? wordDictionary = null)
  {
    var separators = nameType switch
    {
      NameType.LastName => new[] {' ', '-', '_'},
      _ => new[] {' '}
    };

    var words = name.Split(separators);
    foreach (var word in words)
    {
      if (word.Length == 0) continue;

      var wordToSanitize = word;
      if (wordDictionary is not null)
      {
        var suggestItems = wordDictionary.TrySpellCheck(word);
        if (suggestItems.Count == 1)
          wordToSanitize = suggestItems[0].term;
      }

      var sanitizedWord = wordToSanitize.SanitizeWord();
      if (sanitizedWord.Length == 0) continue;
      yield return sanitizedWord.ToString();
    }
  }

  public static StringBuilder SanitizeWord(this string word)
  {
    var sb = new StringBuilder();

    foreach (var c in word.AsSpan())
    {
      if (!char.IsAsciiLetter(c)) continue;

      sb.Append(char.ToLowerInvariant(c));
    }

    return sb;
  }

  public static StringBuilder NormalizeWord(this string word)
  {
    var normalizedString = word.Normalize(NormalizationForm.FormD);
    var sb = new StringBuilder();

    foreach (var c in normalizedString.AsSpan().EnumerateRunes())
    {
      var unicodeCategory = Rune.GetUnicodeCategory(c);

      if (unicodeCategory is UnicodeCategory.NonSpacingMark or UnicodeCategory.SpaceSeparator) continue;

      sb.Append(Rune.ToLowerInvariant(c));
    }

    return sb;
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
