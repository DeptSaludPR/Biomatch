using System.Text;
using Biomatch.Domain.Models;

namespace Biomatch.CLI.Csv;

public static class FrequencyDictionaryTemplate
{
  public static Task WriteToTabDelimitedFile(
    IEnumerable<WordFrequency> frequencyDictionaries,
    string outputFilePath
  )
  {
    var content = new StringBuilder();

    foreach (var frequencyDictionary in frequencyDictionaries)
    {
      var line = $"{frequencyDictionary.Word}\t{frequencyDictionary.Frequency}";
      content.AppendLine(line);
    }

    return File.WriteAllTextAsync(outputFilePath, content.ToString());
  }
}
