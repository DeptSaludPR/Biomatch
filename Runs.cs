using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using MatchingEngine.Models;

namespace MatchingEngine;

public static class Run
{
    public static async Task Run_TwoFileComparison_v2(List<PatientRecord> records1, List<PatientRecord> records2,
        string output_file_name, bool search_all_file_1 = true, int start_index_file_1 = 1, int end_index_file_1 = 100,
        bool search_all_file_2 = true, int start_index_file_2 = 1, int end_index_file_2 = 100,
        bool stop_at_first_match = false, bool exact_matches_allowed = false, double lowerScoreThreshold = 0.65)
    {
        //start a stopwatch
        var logTime = new Stopwatch();
        logTime.Start();

        //open a new log document 
        var outputLog = output_file_name + "_log.txt";
        var log = new StreamWriter(outputLog);

        //write the log start time and settings 
        await log.WriteLineAsync("Log start time: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss zzzzzz"));
        await log.WriteLineAsync("\nThe variable 'stop_at_first_match' is set to: "+stop_at_first_match.ToString());
        await log.WriteLineAsync("\nThe variable 'exact_matches_allowed' is set to: "+exact_matches_allowed.ToString());
        await log.WriteLineAsync("\nFor this run, we are using a score threshold of: " + lowerScoreThreshold);

        //check if the user wants to search among all entities from record 1. 
        //If true, set start_index1 to 0. Else, use the provided start index
        var startIndex1 = search_all_file_1 ? 0 : start_index_file_1;

        //check if the user wants to search among all entities from record 1. 
        //If true, set end_index1 to Records1.Length. Else, use the provided end index
        var endIndex1 = search_all_file_1 ? records1.Count : end_index_file_1;

        //check if the user wants to search among all entities from record 1. 
        //If true, set start_index1 to 0. Else, use the provided start index
        var startIndex2 = search_all_file_2 ? 0 : start_index_file_2;

        //check if the user wants to search among all entities from record 1. 
        //If true, set end_index1 to Records1.Length. Else, use the provided end index
        var endIndex2 = search_all_file_2 ? records2.Count : end_index_file_2;

        //declare integers to use like in a for loop
        var i = startIndex1;
        var j = startIndex2;
        var UrlDuplicates=new List<UrlDoc>();
        //var duplicatesToAdd = new List<PotentialDuplicate>();

        //check for whether the exact matches are allowed, and set the upper threshold accordingly 
        var upperScoreThreshold = exact_matches_allowed ? 1.0 : 0.99999;
        while (i < endIndex1)
        {
            //declare a bool variable to keep of track of whether or not a match has been found 
            var noMatchFoundYet = true;
            //this while loop searches for matches until one is found
            while (j < endIndex2 & noMatchFoundYet)
            {
                //check if the first character of the first name is equal
                // **** more work needs to go into this because I am getting system exceptions ****

                //****** Actually what we want to do here is block keys to limit the search space. O(m*n) is very large!!! *******
                //this checks if the blocking key condition is met 
                if (Helpers.FirstCharactersAreEqual(records1[i].FirstName,
                        records2[j].FirstName) & !(records1[i].RecordId==records2[j].RecordId) )
                {
                    //get the distance vector for the ith vector of the first table and the jth record of the second table
                    var tempDist =
                        DistanceVector.CalculateDistance(records1[i], records2[j]);
                    var tempScore = Score.allFieldsScore_StepMode(tempDist);

                    //**** temporarily set to true to record all comparisons. I need to see why it's not recording partial matches ****
                    if (tempScore >= lowerScoreThreshold & tempScore <= upperScoreThreshold)
                    {
                        //update the match found bool to stop searching other matches
                        if (stop_at_first_match)
                        {
                            noMatchFoundYet = false;
                        }

                        //duplicatesToAdd.Add(new PotentialDuplicate(records1[i], records2[j], tempDist, tempScore));
                        UrlDuplicates.Add(new UrlDoc(new PotentialDuplicate(records1[i], records2[j], tempDist, tempScore)));
                    }
                }

                //update the counter for j
                j++;
            }

            //reset j to start index
            j = startIndex2;
            //update the counter for i 
            i++;
        }

        //write the total elapsed time on the log
        logTime.Stop();
        var logTs = logTime.Elapsed;
        var logElapsedTime =
            $"{logTs.Hours:00}:{logTs.Minutes:00}:{logTs.Seconds:00}.{logTs.Milliseconds / 10:00}";
        await log.WriteLineAsync("\nThe whole process took: " + logElapsedTime);

        //get the current time 
        await log.WriteLineAsync("Log close time: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss zzzzzz"));

        log.Close();
        await using var writer = new StreamWriter(output_file_name + "_url_doc.csv");
        await using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.Context.RegisterClassMap<UrlDocMap>();
            await csv.WriteRecordsAsync(UrlDuplicates);
        }
    }

    public static async Task Run_SameFileComparison_v2(List<PatientRecord> records1, string output_file_name,
        bool stop_at_first_match = false, bool exact_matches_allowed = false, double lowerScoreThreshold = 0.70)
    {
        //start a stopwatch
        var logTime = new Stopwatch();
        logTime.Start();

        //open a new log document 
        var outputLog = output_file_name + "_log.txt";
        var log = new StreamWriter(outputLog);

        //write the log start time
        await log.WriteLineAsync("SAME FILE COMPARISON Log start time: " +
                                DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss zzzzzz"));

        //*** WE WILL BE CHECKING THE WHOLE FILE *******

        //declare integers to use like in a for loop
        var i = 0;
        var j = 1;
        var duplicatesToAdd = new List<PotentialDuplicate>();

        //check for whether the exact matches are allowed, and set the upper threshold accordingly 
        var upperScoreThreshold = exact_matches_allowed ? 1.0 : 0.99999;
        while (i < records1.Count)
        {
            //declare a bool variable to keep of track of whether or not a match has been found 
            var noMatchFoundYet = true;
            //this while loop searches for matches until one is found
            while (j < records1.Count & noMatchFoundYet)
            {
                //Temporarily, set the blocking key condition to true 
                if (true) //*** This is where the blocking key goes ******
                {
                    //get the distance vector for the ith vector of the first table and the jth record of the second table
                    var tempDist =
                        DistanceVector.CalculateDistance(records1[i], records1[j]);
                    var tempScore = Score.allFieldsScore_StepMode(tempDist);

                    //**** temporarily set to true to record all comparisons. I need to see why it's not recording partial matches ****
                    if (tempScore >= lowerScoreThreshold & tempScore <= upperScoreThreshold)
                    {
                        //update the match found bool to stop searching other matches
                        if (stop_at_first_match)
                        {
                            noMatchFoundYet = false;
                        }

                        duplicatesToAdd.Add(new PotentialDuplicate(records1[i], records1[j], tempDist, tempScore));
                    }
                }

                //update the counter for j
                j++;
            }

            //update the counter for i 
            i++;
            //reset j to start index
            j = i + 1;
        }

        //write the total elapsed time on the log
        logTime.Stop();
        var logTs = logTime.Elapsed;
        var logElapsedTime =
            $"{logTs.Hours:00}:{logTs.Minutes:00}:{logTs.Seconds:00}.{logTs.Milliseconds / 10:00}";
        await log.WriteLineAsync("\n SAME FILE COMPARISON process took: " + logElapsedTime);

        //get the current time 
        await log.WriteLineAsync("\n SAME FILE COMPARISON Log close time: " +
                                DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss zzzzzz"));

        await using (var writer = new StreamWriter("Output.csv"))
        await using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            await csv.WriteRecordsAsync(duplicatesToAdd);
        }

        log.Close();
    }
}