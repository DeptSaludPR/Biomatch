using System.Diagnostics;
using System.Text;

namespace Biomatch.CLI.Progress;

public static class MatchingProgress
{
  public static IProgress<int> GetMatchingProgressReport(int totalRecordsToMatch)
  {
    var completedOperations = 0;
    var lastConsoleUpdateTime = DateTime.MinValue;
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"Matching {totalRecordsToMatch:N0} records...");
    var cursorPositionTop = Console.CursorTop;

    // Create a lock object for synchronization
    var progressLock = new object();

    // Initialize a Stopwatch to measure elapsed time
    var stopwatch = Stopwatch.StartNew();

    var textBuilder = new StringBuilder();

    var matchProgressReport = new Progress<int>(increment =>
    {
      lock (progressLock)
      {
        completedOperations += increment;

        if ((DateTime.UtcNow - lastConsoleUpdateTime).TotalSeconds < 1 || completedOperations < 100) return;

        var currentPercentage = (int) ((double) completedOperations / totalRecordsToMatch * 100);
        var elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
        var estimatedTotalSeconds = elapsedSeconds * totalRecordsToMatch / completedOperations;
        var remainingSeconds = estimatedTotalSeconds - elapsedSeconds;

        var remainingTime = TimeSpan.FromSeconds(remainingSeconds);

        textBuilder.Append(
          $"Progress: {currentPercentage,3}% | Record match operations performed: {completedOperations:N0}\n");
        textBuilder.Append($"Total time: {stopwatch.Elapsed} | ");
        textBuilder.Append(
          $"Estimated time remaining: {remainingTime.Days:D2}d {remainingTime.Hours:D2}h {remainingTime.Minutes:D2}m {remainingTime.Seconds:D2}\n");

        Console.SetCursorPosition(0, cursorPositionTop);
        Console.Write(textBuilder);
        textBuilder.Clear();
        lastConsoleUpdateTime = DateTime.UtcNow;
      }
    });

    return matchProgressReport;
  }
}
