using System;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using CsvHelper;
using MatchingEngine.Models;

namespace MatchingEngine
{
    public static class Run
    {
        public static void Run_TwoFileComparison(string filepath1, string filepath2, string output_file_name,
            bool search_all_file_1 = true, int index_file_1 = 100, bool search_all_file_2 = true,
            int index_file_2 = 100,
            bool stop_at_first_match = true, bool exact_matches_allowed = false, double lower_score_treshold = 0.70)
        {
            //start a stopwatch
            Stopwatch log_time = new Stopwatch();
            log_time.Start();

            //open a new log document 
            string output_Log = output_file_name + "_log";
            StreamWriter log = new StreamWriter(output_Log);

            //write the log start time
            log.WriteLine("Log start time: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss zzzzzz"));


            // declare the classes we are going to use 
            Helpers helpers = new Helpers();
            csvToPatientRecord csv = new csvToPatientRecord();
            Score score = new Score();

            //check if the user wants to search among all entities from record 1. 


            //declare the csv paths 
            //string active_case_csv_path="ACTIVE_CASE_API_2021_07_23.csv";
            //string case_cvs_path="CASE_API_2021_07_23.csv";

            //extract the records from the .csvs
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            using var readerFile1 = new StreamReader(filepath1);
            using var csvRecords1 = new CsvReader(readerFile1, CultureInfo.InvariantCulture);
            var records1FromCsv = csvRecords1.GetRecords<PatientRecord>();
            var records1 = records1FromCsv.ToList();
            stopWatch.Stop();
            //display the time elapsed
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            //Console.WriteLine("RunTime " + elapsedTime);

            log.WriteLine($"Converting the test csv to objects took {elapsedTime}");

            stopWatch.Restart();

            //check if the user wants to search among all entities from record 1. 
            //If true, set index1 to the Records1 length
            int index1;
            if (search_all_file_1)
            {
                index1 = records1.Count;
            }
            else
            {
                index1 = index_file_1;
            }


            using var readerFile2 = new StreamReader(filepath2);
            using var csvRecords2 = new CsvReader(readerFile2, CultureInfo.InvariantCulture);
            var records2FromCsv = csvRecords2.GetRecords<PatientRecord>();
            var records2 = records2FromCsv.ToList();
            stopWatch.Stop();
            //display the time elapsed
            ts = stopWatch.Elapsed;
            elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            //Console.WriteLine("RunTime " + elapsedTime);

            log.WriteLine($"Converting the case csv to objects took {elapsedTime}");
            stopWatch.Restart();

            //check if the user wants to search among all entities from record 1. 
            //If true, set index1 to the Records1 length
            int index2;
            if (search_all_file_2)
            {
                index2 = records2.Count;
            }
            else
            {
                index2 = index_file_2;
            }


            //now declare a variable to keep track of how many comparisons were made

            //int comparisons_with_filter=0;


            log.WriteLine("Now we wil look for any matches from " + filepath1 + " with " + filepath2 +
                          " filtering by first character of FirstName and stopping at the first match");
            //now, to look for the duplicates

            //*** I used to use a list but now I am writing to text directly. Idk if this is fater, but I think so ***

            //make a new streamwriter to write the potential duplicates. 
            //The alternative is to write them at the end, but I fear this would mean we end up with nothingb if the search is interrupted
            // **** Elvis if you can, look into this ****
            string output_text = output_file_name + ".text";
            StreamWriter output = new StreamWriter(output_text);
            PotentialDuplicate Temp_Potential_Duplicate = new PotentialDuplicate();
            output.WriteLine(Temp_Potential_Duplicate.csvColumnNamesAsterisk());

            //declare integrs to use like in a for loop
            int i = 0;
            int j = 0;

            //declare a bool variable to keep of track of wether or not a match has been found 
            bool no_match_found_yet;

            //check for wether the exact matches are allowed, and set the upper treshold accordingly 
            double upper_score_treshold;
            if (exact_matches_allowed)
            {
                upper_score_treshold = 1.1;
            }
            else
            {
                upper_score_treshold = 1.0;
            }

            while (i < index1)
            {
                //reset the match found variable before searching for new matches 
                no_match_found_yet = true;
                //this while loop searches for matches until one is found
                while (j < index2 & no_match_found_yet)
                {
                    //check if the first character of the first name is equal
                    // **** more work needs to go into this becasuse I am getting system exceptions ****
                    if (Helpers.FirstCharactersAreEqual(records1[i].FirstName,
                        records2[j].FirstName))
                    {
                        //get the distance vector for the ith vector of the first table and the jth record of the second table
                        DistanceVector temp_dist = new DistanceVector();
                        temp_dist = DistanceVector.CalculateDistance(records1[i], records2[j]);
                        double temp_score = score.allFieldsScore_StepMode(temp_dist);

                        if (temp_score >= lower_score_treshold)
                        {
                            //update the match found bool to stop searching other matches
                            if (stop_at_first_match)
                            {
                                no_match_found_yet = false;
                            }

                            PotentialDuplicate duplicate_to_add =
                                new PotentialDuplicate(records1[i], records2[j], temp_dist, temp_score);
                            //maybe we can skip this, as we are writing to output
                            //PotentialDuplicates_list.Add(duplicate_to_add);
                            // output.WriteLine(duplicate_to_add.csvFormatStringAsterisk());
                            log.WriteLine("Match found at index(" + i.ToString() + "," + j.ToString() + ")");
                        }
                    }

                    //update the counter for j
                    j++;
                }

                //update the counter for i 
                i++;
                Console.WriteLine(i.ToString());
            }

            stopWatch.Stop();
            ts = stopWatch.Elapsed;
            elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}", ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds);
            log.WriteLine("RunTime for matching " + filepath1 + " (up to index " + index1.ToString() + ") to " +
                          filepath2 + " (up to index " + index2.ToString() + ") took:  " + elapsedTime);

            //write the total elapsed time on the log
            log_time.Stop();
            TimeSpan log_ts = log_time.Elapsed;
            string log_elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", log_ts.Hours, log_ts.Minutes,
                log_ts.Seconds, log_ts.Milliseconds / 10);
            log.WriteLine("\nThe whole process took: " + log_elapsedTime);

            //get the current time 
            log.WriteLine("Log close time: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss zzzzzz"));

            log.Close();
            output.Close();
        }

        public static void CurrentDateTimeTest()
        {
            DateTime now = DateTime.Now;
            string current_time = now.ToString(" yyyy-MM-dd hh:mm:ss zzzzzzz");

            Console.WriteLine(current_time);
        }

        public static void Run_TwoFileComparison_v2(string filepath1, string filepath2, string output_file_name,
            bool search_all_file_1 = true, int start_index_file_1 = 1, int end_index_file_1 = 100,
            bool search_all_file_2 = true, int start_index_file_2 = 1, int end_index_file_2 = 100,
            bool stop_at_first_match = true, bool exact_matches_allowed = false, double lower_score_treshold = 0.70)
        {
            //start a stopwatch
            Stopwatch logTime = new Stopwatch();
            logTime.Start();

            //open a new log document 
            string outputLog = output_file_name + "_log.txt";
            var log = new StreamWriter(outputLog);

            //write the log start time
            log.WriteLine("Log start time: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss zzzzzz"));


            // declare the classes we are going to use 
            Score score = new();

            //check if the user wants to search among all entities from record 1. 


            //declare the csv paths 
            //string active_case_csv_path="ACTIVE_CASE_API_2021_07_23.csv";
            //string case_cvs_path="CASE_API_2021_07_23.csv";

            //extract the records from the .csvs
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            using var readerFile1 = new StreamReader(filepath1);
            using var csvRecords1 = new CsvReader(readerFile1, CultureInfo.InvariantCulture);
            var records1FromCsv = csvRecords1.GetRecords<PatientRecord>();
            var records1 = records1FromCsv.ToList();
            stopWatch.Stop();
            //display the time elapsed
            var ts = stopWatch.Elapsed;
            string elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds:000}";

            log.WriteLine($"Converting the test csv to objects took {elapsedTime}");

            stopWatch.Restart();

            //check if the user wants to search among all entities from record 1. 
            //If true, set start_index1 to 0. Else, use the provided start index
            var startIndex1 = search_all_file_1 ? 0 : start_index_file_1;

            //check if the user wants to search among all entities from record 1. 
            //If true, set end_index1 to Records1.Length. Else, use the provided end index
            var endIndex1 = search_all_file_1 ? records1.Count : end_index_file_1;


            using var reader = new StreamReader(filepath2);
            using var csvRecords2 = new CsvReader(reader, CultureInfo.InvariantCulture);
            var records2FromCsv = csvRecords2.GetRecords<PatientRecord>();
            var records2 = records2FromCsv.ToList();

            // Record[] records2 = csv.csvToRecordObject(filepath2);
            stopWatch.Stop();
            //display the time elapsed
            ts = stopWatch.Elapsed;
            elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds:000}";
            //Console.WriteLine("RunTime " + elapsedTime);

            log.WriteLine($"Converting the case csv to objects took {elapsedTime}");
            stopWatch.Restart();

            //check if the user wants to search among all entities from record 1. 
            //If true, set start_index1 to 0. Else, use the provided start index
            var startIndex2 = search_all_file_2 ? 0 : start_index_file_2;

            //check if the user wants to search among all entities from record 1. 
            //If true, set end_index1 to Records1.Length. Else, use the provided end index
            var endIndex2 = search_all_file_2 ? records2.Count : end_index_file_2;


            //now declare a variable to keep track of how many comparisons were made

            //int comparisons_with_filter=0;

            string optionalNot = stop_at_first_match ? "" : " NOT ";

            log.WriteLine("Now we wil look for any matches from " + filepath1 + " with " + filepath2 +
                          " filtering by first character of FirstName and" + optionalNot +
                          "stopping at the first match");
            //now, to look for the duplicates

            //*** I used to use a list but now I am writing to text directly. Idk if this is fater, but I think so ***

            //make a new streamwriter to write the potential duplicates. 
            //The alternative is to write them at the end, but I fear this would mean we end up with nothingb if the search is interrupted
            // **** Elvis if you can, look into this ****
            string outputText = output_file_name + ".txt";
            var output = new StreamWriter(outputText);
            var tempPotentialDuplicate = new PotentialDuplicate();
            output.WriteLine(tempPotentialDuplicate.csvColumnNamesAsterisk());

            //declare integers to use like in a for loop
            var i = startIndex1;
            var j = startIndex2;


            //check for wether the exact matches are allowed, and set the upper treshold accordingly 
            var upperScoreTreshold = exact_matches_allowed ? 1.0 : 0.99999;
            while (i < endIndex1)
            {
                //declare a bool variable to keep of track of wether or not a match has been found 
                var noMatchFoundYet = true;
                //this while loop searches for matches until one is found
                while (j < endIndex2 & noMatchFoundYet)
                {
                    //check if the first character of the first name is equal
                    // **** more work needs to go into this becasuse I am getting system exceptions ****
                    if (Helpers.FirstCharactersAreEqual(records1[i].FirstName,
                        records2[j].FirstName))
                    {
                        //get the distance vector for the ith vector of the first table and the jth record of the second table
                        var tempDist =
                            DistanceVector.CalculateDistance(records1[i], records2[j]);
                        var tempScore = score.allFieldsScore_StepMode(tempDist);

                        //**** temporarily set to true to record all comparisons. I need to see why it's not recording partial matches ****
                        if (tempScore >= lower_score_treshold & tempScore <= upperScoreTreshold)
                        {
                            //update the match found bool to stop searching other matches
                            if (stop_at_first_match)
                            {
                                noMatchFoundYet = false;
                            }

                            var duplicateToAdd =
                                new PotentialDuplicate(records1[i], records2[j], tempDist, tempScore);
                            //maybe we can skip this, as we are writing to output
                            //PotentialDuplicates_list.Add(duplicate_to_add);
                            // output.WriteLine(duplicateToAdd.csvFormatStringAsterisk());
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

            stopWatch.Stop();
            ts = stopWatch.Elapsed;
            elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds:000}";
            log.WriteLine("RunTime for matching " + filepath1 + " (from " + startIndex1 + " to index " +
                          endIndex1 + ") to " + filepath2 + " (from " + startIndex2 +
                          " to index " + endIndex2 + ") took:  " + elapsedTime);

            //write the total elapsed time on the log
            logTime.Stop();
            var logTs = logTime.Elapsed;
            string logElapsedTime =
                $"{logTs.Hours:00}:{logTs.Minutes:00}:{logTs.Seconds:00}.{logTs.Milliseconds / 10:00}";
            log.WriteLine("\nThe whole process took: " + logElapsedTime);

            //get the current time 
            log.WriteLine("Log close time: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss zzzzzz"));

            log.Close();
            output.Close();
        }
    }
}