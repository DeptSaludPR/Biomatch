using System.Diagnostics;
using System.Globalization;
using CsvHelper;
using MatchingEngine.CLI.Csv;
using MatchingEngine.Domain;
using MatchingEngine.Domain.Models;

namespace MatchingEngine.CLI.Services;

public class DuplicateService
{
    public static async Task RunFileComparisons(PatientRecord[] records1, PatientRecord[] records2,
        string outputFileName, bool searchAllFile1 = true, int startIndexFile1 = 1, int endIndexFile1 = 100,
        bool searchAllFile2 = true, int startIndexFile2 = 1, int endIndexFile2 = 100, bool exactMatchesAllowed = false,
        double lowerScoreThreshold = 0.65)
    {
        //start a stopwatch
        var logTime = new Stopwatch();
        logTime.Start();

        //open a new log document 
        var outputLog = outputFileName + "_log.txt";
        var log = new StreamWriter(outputLog);

        //write the log start time and settings 
        await log.WriteLineAsync("Log start time: " + DateTime.UtcNow.ToString("yyyy-MM-dd hh:mm:ss zzzzzz"));
        await log.WriteLineAsync(
            "\nThe variable 'exact_matches_allowed' is set to: " + exactMatchesAllowed);
        await log.WriteLineAsync("\nFor this run, we are using a score threshold of: " + lowerScoreThreshold);

        //check if the user wants to search among all entities from record 1. 
        //If true, set start_index1 to 0. Else, use the provided start index
        var startIndex1 = searchAllFile1 ? 0 : startIndexFile1;

        //check if the user wants to search among all entities from record 1. 
        //If true, set end_index1 to Records1.Length. Else, use the provided end index
        var endIndex1 = searchAllFile1 ? records1.Length : endIndexFile1;

        //check if the user wants to search among all entities from record 1. 
        //If true, set start_index1 to 0. Else, use the provided start index
        var startIndex2 = searchAllFile2 ? 0 : startIndexFile2;

        //check if the user wants to search among all entities from record 1. 
        //If true, set end_index1 to Records1.Length. Else, use the provided end index
        var endIndex2 = searchAllFile2 ? records2.Length : endIndexFile2;
        //check for whether the exact matches are allowed, and set the upper threshold accordingly 
        var upperScoreThreshold = exactMatchesAllowed ? 1.0 : 0.99999;

        var potentialDuplicates = Duplicate.GetPotentialDuplicates(records1, records2, startIndex1, endIndex1,
            startIndex2, endIndex2, lowerScoreThreshold, upperScoreThreshold);

        //write the total elapsed time on the log
        logTime.Stop();
        var logTs = logTime.Elapsed;
        var logElapsedTime =
            $"{logTs.Hours:00}:{logTs.Minutes:00}:{logTs.Seconds:00}.{logTs.Milliseconds / 10:00}";
        await log.WriteLineAsync("\nThe whole process took: " + logElapsedTime);
        await log.WriteLineAsync(
            $"\nOf the total of {potentialDuplicates.Count} potential duplicates, {potentialDuplicates.Count(x => x.Score >= 0.9)} have a score of 0.9 or higher.");
        await log.WriteLineAsync(
            $"\nOf the total of {potentialDuplicates.Count} potential duplicates, {potentialDuplicates.Count(x => x.Score >= 0.8)} have a score of 0.8 or higher.");
        await log.WriteLineAsync(
            $"\nOf the total of {potentialDuplicates.Count} potential duplicates, {potentialDuplicates.Count(x => x.Score >= 0.7)} have a score of 0.7 or higher.");

        //get the current time 
        await log.WriteLineAsync("Log close time: " + DateTime.UtcNow.ToString("yyyy-MM-dd hh:mm:ss zzzzzz"));

        log.Close();
        var urlDocs = potentialDuplicates
            .Select(e => new DuplicateRecord(e));
        await using var writer = new StreamWriter(outputFileName + "_url_doc.csv");
        await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.Context.RegisterClassMap<DuplicateRecordMap>();
        await csv.WriteRecordsAsync(urlDocs);
    }
}