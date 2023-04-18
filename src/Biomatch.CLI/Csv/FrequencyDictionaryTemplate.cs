using System.Text;
using Biomatch.Domain.Models;

namespace Biomatch.CLI.Csv;

public static class FrequencyDictionaryTemplate
{
  public static async Task WriteToTabDelimitedFile(IEnumerable<FrequencyDictionary> frequencyDictionaries, string outputFilePath)
  {
    var content = new StringBuilder();

    foreach (var frequencyDictionary in frequencyDictionaries)
    {
      var line = $"{frequencyDictionary.Word}\t{frequencyDictionary.Frequency}";
      content.AppendLine(line);
    }

    await File.WriteAllTextAsync(outputFilePath, content.ToString());
  }
}
