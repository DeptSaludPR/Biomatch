using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using CsvHelper;
using MatchingEngine.CLI.Csv;
using MatchingEngine.Domain;
using MatchingEngine.Domain.Models;

namespace MatchingEngine.CLI.Services;

public static class DuplicateService
{
  public static async Task RunFileComparisons(IEnumerable<PatientRecord> records1,
    IEnumerable<PatientRecord> records2, FileInfo outputFileName, bool exactMatchesAllowed = false,
    double lowerScoreThreshold = 0.65, WordDictionary? firstNamesDictionary = null,
    WordDictionary? middleNamesDictionary = null, WordDictionary? lastNamesDictionary = null,
    FileInfo? logFilePath = null)
  {
    //start a stopwatch
    var timer = new Stopwatch();
    timer.Start();

    var preprocessedRecords1 =
      records1.PreprocessData(firstNamesDictionary, middleNamesDictionary, lastNamesDictionary).ToArray();
    var preprocessedRecords2 =
      records2.PreprocessData(firstNamesDictionary, middleNamesDictionary, lastNamesDictionary).ToArray();

    //check for whether the exact matches are allowed, and set the upper threshold accordingly
    var upperScoreThreshold = exactMatchesAllowed ? 1.0 : 0.99999;

    var potentialDuplicates = Match.GetPotentialMatches(preprocessedRecords1, preprocessedRecords2,
      lowerScoreThreshold, upperScoreThreshold);

    //write the total elapsed time on the log
    timer.Stop();

    if (logFilePath is not null)
    {
      await WriteLogFile(logFilePath, timer.Elapsed, exactMatchesAllowed, lowerScoreThreshold,
        preprocessedRecords2.Length, preprocessedRecords1.Length, potentialDuplicates);
    }

    var urlDocs = potentialDuplicates
      .Select(e => new DuplicateRecord(e));
    await using var writer = new StreamWriter(outputFileName.FullName, false, Encoding.UTF8);
    await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
    csv.Context.RegisterClassMap<DuplicateRecordMap>();
    await csv.WriteRecordsAsync(urlDocs);
  }

  private static async Task WriteLogFile(FileSystemInfo logFilePath, TimeSpan timeSpan, bool exactMatchesAllowed,
    double lowerScoreThreshold, int sampleRecordsTotal, int recordsToSearchTotal,
    ConcurrentBag<PotentialMatch> potentialDuplicates)
  {
    var logElapsedTime =
      $"{timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}.{timeSpan.Milliseconds / 10:00}";

    //open a new log document
    var log = new StreamWriter(logFilePath.FullName, false, Encoding.UTF8);

    //write the log start time and settings
    await log.WriteLineAsync("Log start time: " + DateTime.UtcNow.ToString("yyyy-MM-dd hh:mm:ss zzzzzz"));
    await log.WriteLineAsync(
      "\nThe variable 'exact_matches_allowed' is set to: " + exactMatchesAllowed);
    await log.WriteLineAsync("\nFor this run, we are using a score threshold of: " + lowerScoreThreshold);
    await log.WriteLineAsync("\nThe whole process took: " + logElapsedTime);
    await log.WriteLineAsync($"\nThere are {sampleRecordsTotal:N0} provided records for the sample.");
    await log.WriteLineAsync($"\nThere are {recordsToSearchTotal:N0} records to search for duplicates.");
    await log.WriteLineAsync(
      $"\nThere are {potentialDuplicates.Count:N0} potential duplicates in the provided files. This represents {potentialDuplicates.Count / (double) recordsToSearchTotal:P2} of the provided records.");
    await log.WriteLineAsync(
      $"\n{potentialDuplicates.Count(x => x.Score >= 0.95):N0} have a score of 0.95 or higher. ({potentialDuplicates.Count(x => x.Score >= 0.95) / (double) potentialDuplicates.Count:P2})");
    await log.WriteLineAsync(
      $"\n{potentialDuplicates.Count(x => x.Score is >= 0.9 and < 0.95):N0} have a score of 0.9 and < 0.95. ({potentialDuplicates.Count(x => x.Score is >= 0.9 and < 0.95) / (double) potentialDuplicates.Count:P2})");
    await log.WriteLineAsync(
      $"\n{potentialDuplicates.Count(x => x.Score is >= 0.85 and < 0.9):N0} have a score of >= 0.85 and < 0.9. ({potentialDuplicates.Count(x => x.Score is >= 0.85 and < 0.9) / (double) potentialDuplicates.Count:P2})");
    await log.WriteLineAsync(
      $"\n{potentialDuplicates.Count(x => x.Score is >= 0.8 and < 0.85):N0} have a score of >= 0.8 and < 0.85. ({potentialDuplicates.Count(x => x.Score is >= 0.8 and < 0.85) / (double) potentialDuplicates.Count:P2})");
    await log.WriteLineAsync(
      $"\n{potentialDuplicates.Count(x => x.Score is >= 0.75 and < 0.8):N0} have a score of >= 0.75 and < 0.8. ({potentialDuplicates.Count(x => x.Score is >= 0.75 and < 0.8) / (double) potentialDuplicates.Count:P2})");

    //get the current time
    await log.WriteLineAsync("\nLog close time: " + DateTime.UtcNow.ToString("yyyy-MM-dd hh:mm:ss zzzzzz"));

    log.Close();
  }
}
