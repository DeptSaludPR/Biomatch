using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace CSV_Example_Code
{
    public static class Run{
        

        
        
        
        
        public static void Run_TwoFileComparison(string filepath1, string filepath2, string output_file_name,
        bool search_all_file_1=true, int index_file_1=100, bool search_all_file_2=true, int index_file_2=100, 
        bool stop_at_first_match=true, bool exact_matches_allowed=false, double lower_score_treshold=0.70)
        {
            //start a stopwatch
            Stopwatch log_time=new Stopwatch();
            log_time.Start();

            //open a new log document 
            string output_Log=output_file_name+"_log";
            StreamWriter log = new StreamWriter(output_Log);

            //write the log start time
            log.WriteLine("Log start time: "+DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss zzzzzz"));


            
            // declare the classes we are going to use 
            Helpers helpers=new Helpers();
            csvToPatientInfoRecord csv = new csvToPatientInfoRecord();
            Score score= new Score();

            //check if the user wants to search among all entities from record 1. 
            
            
            //declare the csv paths 
            //string active_case_csv_path="ACTIVE_CASE_API_2021_07_23.csv";
            //string case_cvs_path="CASE_API_2021_07_23.csv";

            //extract the records from the .csvs
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            Record[] Records1=csv.csvToRecordObject(filepath1);
            stopWatch.Stop();
            //display the time elapsed
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            //Console.WriteLine("RunTime " + elapsedTime);
            
            log.WriteLine($"Converting the test csv to objects took {elapsedTime}");

            stopWatch.Restart();

            //check if the user wants to search among all entities from record 1. 
            //If true, set index1 to the Records1 length
            int index1;
            if(search_all_file_1)
            {
                index1=Records1.Length;
            } else{
                index1=index_file_1;
            }



            Record[] Records2=csv.csvToRecordObject(filepath2);
            stopWatch.Stop();
            //display the time elapsed
            ts = stopWatch.Elapsed;
            elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            //Console.WriteLine("RunTime " + elapsedTime);
            
            log.WriteLine($"Converting the case csv to objects took {elapsedTime}");
            stopWatch.Restart();

            //check if the user wants to search among all entities from record 1. 
            //If true, set index1 to the Records1 length
            int index2;
            if(search_all_file_2)
            {
                index2=Records2.Length;
            } else{
                index2=index_file_2;
            }
            

            //now declare a variable to keep track of how many comparisons were made

            //int comparisons_with_filter=0;


            log.WriteLine("Now we wil look for any matches from "+filepath1+" with "+filepath2+" filtering by first character of FirstName and stopping at the first match");
            //now, to look for the duplicates

            //*** I used to use a list but now I am writing to text directly. Idk if this is fater, but I think so ***

            //make a new streamwriter to write the potential duplicates. 
            //The alternative is to write them at the end, but I fear this would mean we end up with nothingb if the search is interrupted
            // **** Elvis if you can, look into this ****
            string output_text=output_file_name+".text";
            StreamWriter output=new StreamWriter(output_text);
            PotentialDuplicate Temp_Potential_Duplicate=new PotentialDuplicate();
            output.WriteLine( Temp_Potential_Duplicate.csvColumnNamesAsterisk());

            //declare integrs to use like in a for loop
            int i=0;
            int j=0;

            //declare a bool variable to keep of track of wether or not a match has been found 
            bool no_match_found_yet;

            //check for wether the exact matches are allowed, and set the upper treshold accordingly 
            double upper_score_treshold;
            if(exact_matches_allowed)
            {
                upper_score_treshold=1.1;
            } else{
                upper_score_treshold=1.0;
            }

            while(i<index1) 
            {
                //reset the match found variable before searching for new matches 
                no_match_found_yet=true;
                //this while loop searches for matches until one is found
                while(j<index2 & no_match_found_yet)
                {
                    //check if the first character of the first name is equal
                    // **** more work needs to go into this becasuse I am getting system exceptions ****
                    if(helpers.firstCaractersAreEqual(Records1[i].PatientInfo.FirstName, Records2[j].PatientInfo.FirstName))
                    {
                        //get the distance vector for the ith vector of the first table and the jth record of the second table
                        DistanceVector temp_dist = new DistanceVector();
                        temp_dist=temp_dist.calculateDistance(Records1[i].PatientInfo,Records2[j].PatientInfo);
                        double temp_score=score.allFieldsScore_StepMode(temp_dist);
                        
                        if(temp_score>=lower_score_treshold)
                        {
                        //update the match found bool to stop searching other matches
                        if(stop_at_first_match)
                        {
                            no_match_found_yet=false;
                        }

                        PotentialDuplicate duplicate_to_add=new PotentialDuplicate(Records1[i],Records2[j], temp_dist, temp_score);
                        //maybe we can skip this, as we are writing to output
                        //PotentialDuplicates_list.Add(duplicate_to_add);
                        output.WriteLine(duplicate_to_add.csvFormatStringAsterisk());
                        log.WriteLine("Match found at index("+i.ToString()+","+j.ToString()+")");
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
            elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}",ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds );
            log.WriteLine("RunTime for matching "+filepath1+" (up to index "+index1.ToString()+") to "+filepath2+" (up to index "+index2.ToString()+") took:  " + elapsedTime);

            //write the total elapsed time on the log
            log_time.Stop();
            TimeSpan log_ts=log_time.Elapsed;
            string log_elapsedTime=String.Format("{0:00}:{1:00}:{2:00}.{3:00}",log_ts.Hours, log_ts.Minutes, log_ts.Seconds, log_ts.Milliseconds / 10);
            log.WriteLine("\nThe whole process took: "+log_elapsedTime);

            //get the current time 
            log.WriteLine("Log close time: "+DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss zzzzzz"));

            log.Close();
            output.Close();
        }

        public static void CurrentDateTimeTest()
        {
            DateTime now =DateTime.Now;
            string current_time=now.ToString(" yyyy-MM-dd hh:mm:ss zzzzzzz");

            Console.WriteLine(current_time);

        }

        public static void Run_TwoFileComparison_v2(string filepath1, string filepath2, string output_file_name,
        bool search_all_file_1=true, int start_index_file_1=1, int end_index_file_1=100, 
        bool search_all_file_2=true,  int start_index_file_2=1, int end_index_file_2=100, 
        bool stop_at_first_match=true, bool exact_matches_allowed=false, double lower_score_treshold=0.70)
        {
            //start a stopwatch
            Stopwatch log_time=new Stopwatch();
            log_time.Start();

            //open a new log document 
            string output_Log=output_file_name+"_log.txt";
            StreamWriter log = new StreamWriter(output_Log);

            //write the log start time
            log.WriteLine("Log start time: "+DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss zzzzzz"));


            
            // declare the classes we are going to use 
            Helpers helpers=new Helpers();
            csvToPatientInfoRecord csv = new csvToPatientInfoRecord();
            Score score= new Score();

            //check if the user wants to search among all entities from record 1. 
            
            
            //declare the csv paths 
            //string active_case_csv_path="ACTIVE_CASE_API_2021_07_23.csv";
            //string case_cvs_path="CASE_API_2021_07_23.csv";

            //extract the records from the .csvs
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            Record[] Records1=csv.csvToRecordObject(filepath1);
            stopWatch.Stop();
            //display the time elapsed
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}",ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
            //Console.WriteLine("RunTime " + elapsedTime);
            
            log.WriteLine($"Converting the test csv to objects took {elapsedTime}");

            stopWatch.Restart();

            //check if the user wants to search among all entities from record 1. 
            //If true, set start_index1 to 0. Else, use the provided start index
            int start_index1;
            if(search_all_file_1)
            {
                start_index1=0;
            } else{
                start_index1=start_index_file_1;
            }

            //check if the user wants to search among all entities from record 1. 
            //If true, set end_index1 to Records1.Length. Else, use the provided end index
            int end_index1;
            if(search_all_file_1)
            {
                end_index1=Records1.Length;
            } else{
                end_index1=end_index_file_1;
            }



            Record[] Records2=csv.csvToRecordObject(filepath2);
            stopWatch.Stop();
            //display the time elapsed
            ts = stopWatch.Elapsed;
            elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}",ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
            //Console.WriteLine("RunTime " + elapsedTime);
            
            log.WriteLine($"Converting the case csv to objects took {elapsedTime}");
            stopWatch.Restart();

            //check if the user wants to search among all entities from record 1. 
            //If true, set start_index1 to 0. Else, use the provided start index
            int start_index2;
            if(search_all_file_2)
            {
                start_index2=0;
            } else{
                start_index2=start_index_file_2;
            }

            //check if the user wants to search among all entities from record 1. 
            //If true, set end_index1 to Records1.Length. Else, use the provided end index
            int end_index2;
            if(search_all_file_2)
            {
                end_index2=Records2.Length;
            } else{
                end_index2=end_index_file_2;
            }
            

            //now declare a variable to keep track of how many comparisons were made

            //int comparisons_with_filter=0;

            string optional_not;
            if(stop_at_first_match)
            {
                optional_not="";
            } else{
                optional_not=" NOT ";
            }

            log.WriteLine("Now we wil look for any matches from "+filepath1+" with "+filepath2+" filtering by first character of FirstName and"+optional_not+"stopping at the first match");
            //now, to look for the duplicates

            //*** I used to use a list but now I am writing to text directly. Idk if this is fater, but I think so ***

            //make a new streamwriter to write the potential duplicates. 
            //The alternative is to write them at the end, but I fear this would mean we end up with nothingb if the search is interrupted
            // **** Elvis if you can, look into this ****
            string output_text=output_file_name+".txt";
            StreamWriter output=new StreamWriter(output_text);
            PotentialDuplicate Temp_Potential_Duplicate=new PotentialDuplicate();
            output.WriteLine( Temp_Potential_Duplicate.csvColumnNamesAsterisk());

            //declare integrs to use like in a for loop
            int i=start_index1;
            int j=start_index2;

            //declare a bool variable to keep of track of wether or not a match has been found 
            bool no_match_found_yet;

            //check for wether the exact matches are allowed, and set the upper treshold accordingly 
            double upper_score_treshold;
            if(exact_matches_allowed)
            {
                upper_score_treshold=1.0;
            } else{
                upper_score_treshold=0.99999;
            }

            //print out the indexes to see what's happening
            Console.WriteLine("start index 1: "+start_index1.ToString());
            Console.WriteLine("end index 1: "+end_index1.ToString());
            Console.WriteLine("start index 2: "+start_index2.ToString());
            Console.WriteLine("end index 2: "+end_index2.ToString());
            while(i<end_index1) 
            {
                //reset the match found variable before searching for new matches 
                no_match_found_yet=true;
                //this while loop searches for matches until one is found
                while(j<end_index2 & no_match_found_yet)
                {
                    //check if the first character of the first name is equal
                    // **** more work needs to go into this becasuse I am getting system exceptions ****
                    if(helpers.firstCaractersAreEqual(Records1[i].PatientInfo.FirstName, Records2[j].PatientInfo.FirstName))
                    {
                        //get the distance vector for the ith vector of the first table and the jth record of the second table
                        DistanceVector temp_dist = new DistanceVector();
                        temp_dist=temp_dist.calculateDistance(Records1[i].PatientInfo,Records2[j].PatientInfo);
                        double temp_score=score.allFieldsScore_StepMode(temp_dist);
                        
                        //**** temporarily set to true to record all comparisons. I need to see why it's not recording partial matches ****
                        if(temp_score>=lower_score_treshold & temp_score<=upper_score_treshold) 
                        {
                        //update the match found bool to stop searching other matches
                        if(stop_at_first_match)
                        {
                            no_match_found_yet=false;
                        }

                        PotentialDuplicate duplicate_to_add=new PotentialDuplicate(Records1[i],Records2[j], temp_dist, temp_score);
                        //maybe we can skip this, as we are writing to output
                        //PotentialDuplicates_list.Add(duplicate_to_add);
                        output.WriteLine(duplicate_to_add.csvFormatStringAsterisk());
                        }
                    }
                    //update the counter for j
                    j++;
                }
                //reset j to start index
                j=start_index2;
                Console.WriteLine(i.ToString());
                //update the counter for i 
                i++;
            }
            stopWatch.Stop();
            ts = stopWatch.Elapsed;
            elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}",ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds );
            log.WriteLine("RunTime for matching "+filepath1+" (from "+start_index1.ToString()+" to index "+end_index1.ToString()+") to "+filepath2+" (from "+start_index2.ToString()+" to index "+end_index2.ToString()+") took:  " + elapsedTime);

            //write the total elapsed time on the log
            log_time.Stop();
            TimeSpan log_ts=log_time.Elapsed;
            string log_elapsedTime=String.Format("{0:00}:{1:00}:{2:00}.{3:00}",log_ts.Hours, log_ts.Minutes, log_ts.Seconds, log_ts.Milliseconds / 10);
            log.WriteLine("\nThe whole process took: "+log_elapsedTime);

            //get the current time 
            log.WriteLine("Log close time: "+DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss zzzzzz"));

            log.Close();
            output.Close();
        }

    }
}