using System.Diagnostics;
using System.Globalization;
using CsvHelper;
using MatchingEngine.CLI.Csv;
using MatchingEngine.Domain;
using MatchingEngine.Domain.Models;

namespace MatchingEngine.CLI.Services;

public static class DuplicateService
{
    public static async Task RunFileComparisons(IEnumerable<PatientRecord> records1, IEnumerable<PatientRecord> records2,
        FileInfo outputFileName, bool searchAllFile1 = true, int startIndexFile1 = 1, int endIndexFile1 = 100,
        bool searchAllFile2 = true, int startIndexFile2 = 1, int endIndexFile2 = 100, bool exactMatchesAllowed = false,
        double lowerScoreThreshold = 0.65, FileInfo? logFilePath = null)
    {
        //start a stopwatch
        var timer = new Stopwatch();
        timer.Start();

        var preprocessedRecords1 = Preprocess.PreprocessData(records1).ToArray();
        var preprocessedRecords2 = Preprocess.PreprocessData(records2).ToArray();
        
        //check if the user wants to search among all entities from record 1. 
        //If true, set start_index1 to 0. Else, use the provided start index
        var startIndex1 = searchAllFile1 ? 0 : startIndexFile1;

        //check if the user wants to search among all entities from record 1. 
        //If true, set end_index1 to Records1.Length. Else, use the provided end index
        var endIndex1 = searchAllFile1 ? preprocessedRecords1.Length : endIndexFile1;

        //check if the user wants to search among all entities from record 1. 
        //If true, set start_index1 to 0. Else, use the provided start index
        var startIndex2 = searchAllFile2 ? 0 : startIndexFile2;

        //check if the user wants to search among all entities from record 1. 
        //If true, set end_index1 to Records1.Length. Else, use the provided end index
        var endIndex2 = searchAllFile2 ? preprocessedRecords2.Length : endIndexFile2;
        //check for whether the exact matches are allowed, and set the upper threshold accordingly 
        var upperScoreThreshold = exactMatchesAllowed ? 1.0 : 0.99999;

        var potentialDuplicates = Duplicate.GetPotentialDuplicates(preprocessedRecords1, preprocessedRecords2,
            startIndex1, endIndex1, startIndex2, endIndex2, lowerScoreThreshold, upperScoreThreshold);

        //write the total elapsed time on the log
        timer.Stop();

        if (logFilePath is not null)
        {
            var logTs = timer.Elapsed;
            var logElapsedTime =
                $"{logTs.Hours:00}:{logTs.Minutes:00}:{logTs.Seconds:00}.{logTs.Milliseconds / 10:00}";
        
            //open a new log document 
            var log = new StreamWriter(logFilePath.FullName);

            //write the log start time and settings 
            await log.WriteLineAsync("Log start time: " + DateTime.UtcNow.ToString("yyyy-MM-dd hh:mm:ss zzzzzz"));
            await log.WriteLineAsync(
                "\nThe variable 'exact_matches_allowed' is set to: " + exactMatchesAllowed);
            await log.WriteLineAsync("\nFor this run, we are using a score threshold of: " + lowerScoreThreshold);
            await log.WriteLineAsync("\nThe whole process took: " + logElapsedTime);
            await log.WriteLineAsync($"\nThere are {preprocessedRecords1.Length} provided records for the sample.");
            await log.WriteLineAsync($"\nThere are {preprocessedRecords2.Length} records to search for duplicates.");
            await log.WriteLineAsync($"\nThere are {potentialDuplicates.Count} potential duplicates in the provided files. This represents {potentialDuplicates.Count / (double) preprocessedRecords2.Length:P2} of the provided records.");
            await log.WriteLineAsync(
                $"\n{potentialDuplicates.Count(x => x.Score >= 0.9)} have a score of 0.9 or higher. ({potentialDuplicates.Count(x => x.Score >= 0.9) / (double) potentialDuplicates.Count:P2})");
            await log.WriteLineAsync(
                $"\n{potentialDuplicates.Count(x => x.Score is >= 0.8 and < 0.9)} have a score of >= 0.8 and < 0.9. ({potentialDuplicates.Count(x => x.Score is >= 0.8 and < 0.9) / (double) potentialDuplicates.Count:P2})");
            await log.WriteLineAsync(
                $"\n{potentialDuplicates.Count(x => x.Score is >= 0.7 and < 0.8)} have a score of >= 0.7 and < 0.8. ({potentialDuplicates.Count(x => x.Score is >= 0.7 and < 0.8) / (double) potentialDuplicates.Count:P2})");

            //get the current time 
            await log.WriteLineAsync("\nLog close time: " + DateTime.UtcNow.ToString("yyyy-MM-dd hh:mm:ss zzzzzz"));

            log.Close();
        }

        var urlDocs = potentialDuplicates
            .Select(e => new DuplicateRecord(e));
        await using var writer = new StreamWriter(outputFileName.FullName);
        await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.Context.RegisterClassMap<DuplicateRecordMap>();
        await csv.WriteRecordsAsync(urlDocs);
    }
}