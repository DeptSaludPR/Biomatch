using System;
using System.Collections.Generic;
using Microsoft.VisualBasic.FileIO;
//bebesita ua
namespace CSV_Example_Code
{
    public class Helpers
    {
        // getColumnNames: gets the column names of a csv file and returns it as a string array
        public string[] getColumnNames(string csv_path)
        {
            
            TextFieldParser csvParser=new TextFieldParser(csv_path);
            
            csvParser.CommentTokens = new string[] { "#" };
            csvParser.SetDelimiters(new string[] { "," });
            csvParser.HasFieldsEnclosedInQuotes = true;

            return(csvParser.ReadFields());
        }//end of getColumnNames

        public int getIndexOfField( string[] col_names, string desired_field)
        {
            int index;
            try
            {
                index=Array.IndexOf(col_names, desired_field);
            }
            catch(Exception e)
            {
                Console.WriteLine($"No such field exists; rerunrning -1");
                Console.WriteLine(e.Message);
                index=-1;
            }
            return(index);
        }//end of getIndexOfField

        public string addIndexToString( string str, int index)
        {
            string index_string;
            if(index==0)
            {
                index_string="";
            }
            else
            {
                index_string=$"{index}";
            }

            string s=index_string+","+str;
            return(s);
        }

        public bool firstCaractersAreEqual(string A, string B)
        {
            try
            {
                return A.Substring(0,1)==B.Substring(0,1);
            }catch
            {
                return true; 
            }
        }
    }//Helpers


    public class PatientInfoRecord
    {
        //change to private
        public string FirstName;
        public string MiddleName;
        public string LastName;
        public string SecondLastName;

        public string BirthDate;
        public string City;
        public string PhoneNumber;
        //Create new class that inherits patientinfoRecords
        // public string matchfound;
        // public string algorithm;
        // public int percentage;
        // public 
        public PatientInfoRecord()
        {
        }

        public  PatientInfoRecord(string firstName="", string middleName="", string lastName="",
        string secondLastName="", string birthDate="", string city="", string phoneNumber="")
        {
            //patients will have Firstname,Lastname, MiddleName, SecondLastname, Phone Number, Socials, Place ofBirth
            FirstName= firstName;
            LastName = lastName;
            MiddleName=middleName;
            SecondLastName=secondLastName;
            BirthDate=birthDate;
            City=city;
            PhoneNumber=phoneNumber;

        }//end of PatinetInfoRecord
        //This function is to return all the public field of the class

        public PatientInfoRecord swapLastNames( )
        {
            PatientInfoRecord P = new PatientInfoRecord(this.FirstName, this.MiddleName, this.SecondLastName,
            this.LastName, this.BirthDate, this.City, this.PhoneNumber);
            return(P);
        }
        public string[] getDataInfo()
        { 
            string [] result= new string[] {this.FirstName, this.MiddleName, this.LastName, this.SecondLastName, this.BirthDate, this.City, this.PhoneNumber};
            return(result);
        }

        //This function is to display all the public field of the class.
        //change to getDisplayIngo
        public void displayInfo()
        {
            Console.WriteLine($"First Name: {this.FirstName} MiddleName: {this.MiddleName} Lastname: {this.LastName} SecondLastname: {this.SecondLastName} BirthDate: {this.BirthDate} City: {this.City} PhoneNumber: {this.PhoneNumber}");
        }
        public string getDisplayInfo()
        {
            return ($"First Name: {this.FirstName} MiddleName: {this.MiddleName} Lastname: {this.LastName} SecondLastname: {this.SecondLastName} BirthDate: {this.BirthDate} City: {this.City} PhoneNumber: {this.PhoneNumber}");
        }

        public string StringToCSVFormat( )
        {
            string str=$"{this.FirstName},{this.MiddleName},{this.LastName},{this.SecondLastName},{this.BirthDate},{this.City},{this.PhoneNumber}";
            return(str);
        }
        public string StringToCSVFormatAsterisk( )
        {
            string str=$"{this.FirstName}*{this.MiddleName}*{this.LastName}*{this.SecondLastName}*{this.BirthDate}*{this.City}*{this.PhoneNumber}*";
            return (str);
        }
        public string csvColumnNames( )
        {
            string str="FirstName,MiddleName,LastName,SecondLastName,BirthDate,City,PhoneNumber";
            return(str);
        }

        public string csvColumnNamesAsterisk( )
        {
            string str="PatientInfoAsterisk";
            return str;
        }
    }// end of PatientInfoRecord



    public class csvToPatientInfoRecord
    {
        public Record[] csvToRecordObject(string csv_file_path)
        {
            Helpers helpers=new Helpers();

            //declare the array of records to return
            //PatientInfoRecord [] RecordsToReturn =  new PatientInfoRecord();
            List<Record> list = new List<Record>();
            //get the Column Names
            string[] Column_Names=helpers.getColumnNames(csv_file_path);

            //and the indices
            int firstNameIndex=helpers.getIndexOfField(Column_Names, "FirstName");
            int middleNameIndex=helpers.getIndexOfField(Column_Names,"MiddleName");
            int lastNameIndex=helpers.getIndexOfField(Column_Names,"LastName");
            int secondLastNameIndex=helpers.getIndexOfField(Column_Names,"SecondLastName");
            int birthDateIndex=helpers.getIndexOfField(Column_Names,"BirthDate");
            int cityIndex=helpers.getIndexOfField(Column_Names,"City");
            int phoneNumberIndex=helpers.getIndexOfField(Column_Names,"PhoneNumber");
            int idTypeIndex=helpers.getIndexOfField(Column_Names,"IdType");
            int idIndex=helpers.getIndexOfField(Column_Names,"ID");

            //declare a new text field parser called csvparser
            TextFieldParser csvParser= new TextFieldParser(csv_file_path);
            //and set the variables to configure it to read csv files 
            csvParser.CommentTokens             = new string[] { "#" };
            csvParser.SetDelimiters(new string[] { "," });
            csvParser.HasFieldsEnclosedInQuotes = true;
            //skip the first line 
            csvParser.ReadLine();

            // declare temporary variables to store the info as we parse ths csv
            string [] current_line;
            PatientInfoRecord tempRecord;
            string tempIdType;
            string tempID;

            do
            {
                //Read the line
                current_line = csvParser.ReadFields();
                //empty the Record
                tempRecord= new PatientInfoRecord();
                //set the values for the patient Info record
                tempRecord.FirstName     =current_line[firstNameIndex];
                tempRecord.MiddleName    =current_line[middleNameIndex];
                tempRecord.LastName      =current_line[lastNameIndex];
                tempRecord.SecondLastName=current_line[secondLastNameIndex];
                tempRecord.BirthDate     =current_line[birthDateIndex];
                tempRecord.City          =current_line[cityIndex];
                tempRecord.PhoneNumber   =current_line[phoneNumberIndex];

                //now get the id type
                tempIdType               =current_line[idTypeIndex];
                tempID                   =current_line[idIndex];

                //generate the new record
                Record recordToStore= new Record(tempRecord, tempIdType, tempID);



                list.Add(recordToStore);
            }while(!csvParser.EndOfData);
            var RecordsToReturn = list.ToArray();
            return RecordsToReturn;
        }//end of Record






        public PatientInfoRecord[] csvToRecords(string csv_file_path)
        {
            //Declare the Helpers class object 
            Helpers helpers=new Helpers();

            //declare the array of records to return
            //PatientInfoRecord [] RecordsToReturn =  new PatientInfoRecord();
            var list = new List<PatientInfoRecord>();
            //get the Column Names
            string[] Column_Names=helpers.getColumnNames(csv_file_path);

            //and the indices
            int firstNameIndex=helpers.getIndexOfField(Column_Names, "FirstName");
            int middleNameIndex=helpers.getIndexOfField(Column_Names,"MiddleName");
            int lastNameIndex=helpers.getIndexOfField(Column_Names,"LastName");
            int secondLastNameIndex=helpers.getIndexOfField(Column_Names,"SecondLastName");
            int birthDateIndex=helpers.getIndexOfField(Column_Names,"BirthDate");
            int cityIndex=helpers.getIndexOfField(Column_Names,"City");
            int phoneNumberIndex=helpers.getIndexOfField(Column_Names,"PhoneNumber");

            // declare a new text field parser called csvparser
            TextFieldParser csvParser= new TextFieldParser(csv_file_path);
            //and set the variables to configure it to read csv files 
            csvParser.CommentTokens             = new string[] { "#" };
            csvParser.SetDelimiters(new string[] { "," });
            csvParser.HasFieldsEnclosedInQuotes = true;
            //skip the first line 
            csvParser.ReadLine();
            // declare temporary variables to store the info as we parse ths csv
            string [] current_line;
            PatientInfoRecord tempRecord;
            

            
            do
            {
                //Read the line
                current_line = csvParser.ReadFields();
                //empty the Record
                tempRecord= new PatientInfoRecord();
                //set the values for the patient Info record
                tempRecord.FirstName     =current_line[firstNameIndex];
                tempRecord.MiddleName    =current_line[middleNameIndex];
                tempRecord.LastName      =current_line[lastNameIndex];
                tempRecord.SecondLastName=current_line[secondLastNameIndex];
                tempRecord.BirthDate     =current_line[birthDateIndex];
                tempRecord.City          =current_line[cityIndex];
                tempRecord.PhoneNumber   =current_line[phoneNumberIndex];

                //now get the id type



                list.Add(tempRecord);
            }while(!csvParser.EndOfData);

            var RecordsToReturn = list.ToArray();
            return RecordsToReturn;
        }// end of PatientInfoRecord
    }// end of csvToPatientInfoRecord 
}//end of CSV_Example_Code