using System;
using System.Collections.Generic;
using MatchingEngine.Models;
using Microsoft.VisualBasic.FileIO;

namespace MatchingEngine
{
    public class Helpers
    {
        // getColumnNames: gets the column names of a csv file and returns it as a string array
        public string[] getColumnNames(string csv_path)
        {
            TextFieldParser csvParser = new TextFieldParser(csv_path);

            csvParser.CommentTokens = new[] { "#" };
            csvParser.SetDelimiters(",");
            csvParser.HasFieldsEnclosedInQuotes = true;

            return (csvParser.ReadFields());
        }

        public static int GetIndexOfField(string[] colNames, string desiredField)
        {
            int index;
            try
            {
                index = Array.IndexOf(colNames, desiredField);
            }
            catch (Exception e)
            {
                Console.WriteLine($"No such field exists; rerunning -1");
                Console.WriteLine(e.Message);
                index = -1;
            }

            return index;
        }

        public static bool FirstCharactersAreEqual(string A, string B)
        {
            try
            {
                return A.Substring(0, 1) == B.Substring(0, 1);
            }
            catch
            {
                return true;
            }
        }
    }


    public class csvToPatientRecord
    {
        public Record[] csvToRecordObject(string csv_file_path)
        {
            Helpers helpers = new Helpers();

            //declare the array of records to return
            //PatientRecord [] RecordsToReturn =  new PatientRecord();
            List<Record> list = new List<Record>();
            //get the Column Names
            string[] Column_Names = helpers.getColumnNames(csv_file_path);

            //and the indices
            var firstNameIndex = Helpers.GetIndexOfField(Column_Names, "FirstName");
            var middleNameIndex = Helpers.GetIndexOfField(Column_Names, "MiddleName");
            var lastNameIndex = Helpers.GetIndexOfField(Column_Names, "LastName");
            var secondLastNameIndex = Helpers.GetIndexOfField(Column_Names, "SecondLastName");
            var birthDateIndex = Helpers.GetIndexOfField(Column_Names, "BirthDate");
            var cityIndex = Helpers.GetIndexOfField(Column_Names, "City");
            var phoneNumberIndex = Helpers.GetIndexOfField(Column_Names, "PhoneNumber");
            var idTypeIndex = Helpers.GetIndexOfField(Column_Names, "IdType");
            var idIndex = Helpers.GetIndexOfField(Column_Names, "ID");

            //declare a new text field parser called csvparser
            TextFieldParser csvParser = new TextFieldParser(csv_file_path);
            //and set the variables to configure it to read csv files 
            csvParser.CommentTokens = new string[] { "#" };
            csvParser.SetDelimiters(new string[] { "," });
            csvParser.HasFieldsEnclosedInQuotes = true;
            //skip the first line 
            csvParser.ReadLine();

            // declare temporary variables to store the info as we parse ths csv
            string[] current_line;
            PatientRecord tempRecord;
            string tempIdType;
            string tempID;

            do
            {
                //Read the line
                current_line = csvParser.ReadFields();
                //empty the Record
                tempRecord = new PatientRecord();
                //set the values for the patient Info record
                tempRecord.FirstName = current_line[firstNameIndex];
                tempRecord.MiddleName = current_line[middleNameIndex];
                tempRecord.LastName = current_line[lastNameIndex];
                tempRecord.SecondLastName = current_line[secondLastNameIndex];
                tempRecord.BirthDate = current_line[birthDateIndex];
                tempRecord.City = current_line[cityIndex];
                tempRecord.PhoneNumber = current_line[phoneNumberIndex];

                //now get the id type
                tempIdType = current_line[idTypeIndex];
                tempID = current_line[idIndex];

                //generate the new record
                Record recordToStore = new Record(tempRecord, tempIdType, tempID);


                list.Add(recordToStore);
            } while (!csvParser.EndOfData);

            var RecordsToReturn = list.ToArray();
            return RecordsToReturn;
        } //end of Record


        public PatientRecord[] csvToRecords(string csv_file_path)
        {
            //Declare the Helpers class object 
            Helpers helpers = new Helpers();

            //declare the array of records to return
            //PatientRecord [] RecordsToReturn =  new PatientRecord();
            var list = new List<PatientRecord>();
            //get the Column Names
            string[] Column_Names = helpers.getColumnNames(csv_file_path);

            //and the indices
            var firstNameIndex = Helpers.GetIndexOfField(Column_Names, "FirstName");
            var middleNameIndex = Helpers.GetIndexOfField(Column_Names, "MiddleName");
            var lastNameIndex = Helpers.GetIndexOfField(Column_Names, "LastName");
            var secondLastNameIndex = Helpers.GetIndexOfField(Column_Names, "SecondLastName");
            var birthDateIndex = Helpers.GetIndexOfField(Column_Names, "BirthDate");
            var cityIndex = Helpers.GetIndexOfField(Column_Names, "City");
            var phoneNumberIndex = Helpers.GetIndexOfField(Column_Names, "PhoneNumber");

            // declare a new text field parser called csvparser
            TextFieldParser csvParser = new TextFieldParser(csv_file_path);
            //and set the variables to configure it to read csv files 
            csvParser.CommentTokens = new string[] { "#" };
            csvParser.SetDelimiters(new string[] { "," });
            csvParser.HasFieldsEnclosedInQuotes = true;
            //skip the first line 
            csvParser.ReadLine();
            // declare temporary variables to store the info as we parse ths csv
            string[] current_line;
            PatientRecord tempRecord;


            do
            {
                //Read the line
                current_line = csvParser.ReadFields();
                //empty the Record
                tempRecord = new PatientRecord
                {
                    //set the values for the patient Info record
                    FirstName = current_line[firstNameIndex],
                    MiddleName = current_line[middleNameIndex],
                    LastName = current_line[lastNameIndex],
                    SecondLastName = current_line[secondLastNameIndex],
                    BirthDate = current_line[birthDateIndex],
                    City = current_line[cityIndex],
                    PhoneNumber = current_line[phoneNumberIndex]
                };

                //now get the id type


                list.Add(tempRecord);
            } while (!csvParser.EndOfData);

            var recordsToReturn = list.ToArray();
            return recordsToReturn;
        } // end of PatientRecord
    }
}