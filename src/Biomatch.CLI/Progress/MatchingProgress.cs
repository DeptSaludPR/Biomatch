namespace Biomatch.CLI.Progress;

public static class MatchingProgress
{
  public static IProgress<int> GetMatchingProgressReport(int totalRecordsToMatch)
  {
    var completedOperations = 0;
    var lastConsoleUpdateTime = DateTime.MinValue;
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"Matching {totalRecordsToMatch:N0} records...");
    var consoleTextPrinted = false;

    // Create a lock object for synchronization
    var progressLock = new object();

    // Initialize a Stopwatch to measure elapsed time
    long? startTimeStamp = null;

    var matchProgressReport = new Progress<int>(increment =>
    {
      lock (progressLock)
      {
        startTimeStamp ??= TimeProvider.System.GetTimestamp();

        completedOperations += increment;

        if ((DateTime.UtcNow - lastConsoleUpdateTime).TotalSeconds < 1 || completedOperations < 100)
          return;

        var currentPercentage = (int)((double)completedOperations / totalRecordsToMatch * 100);
        var elapsedTime = TimeProvider
          .System
          .GetElapsedTime(startTimeStamp.Value, TimeProvider.System.GetTimestamp());
        var estimatedTotalTime = elapsedTime * totalRecordsToMatch / completedOperations;
        var remainingTime = estimatedTotalTime - elapsedTime;

        if (consoleTextPrinted)
        {
          if (Console.CursorTop >= 2)
          {
            Console.SetCursorPosition(0, Console.CursorTop - 2);
          }
          else
          {
            Console.SetCursorPosition(0, 0);
          }
        }
        Console.Write(
          $"""
                       Progress: {currentPercentage, 3}% | Record match operations performed: {completedOperations:N0}
                       Total time: {elapsedTime} | Estimated time remaining: {remainingTime.Days:D2}d {remainingTime.Hours:D2}h {remainingTime.Minutes:D2}m {remainingTime.Seconds:D2}s

                       """
        );
        consoleTextPrinted = true;
        lastConsoleUpdateTime = DateTime.UtcNow;
      }
    });

    return matchProgressReport;
  }
}
