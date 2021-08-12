using System;

namespace MatchingEngine.Models
{
    public class PatientRecord : Record
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string SecondLastName { get; set; }
        public string BirthDate { get; set; }
        public string City { get; set; }
        public string PhoneNumber { get; set; }

        //Create new class that inherits PatientRecords
        // public string matchfound;
        // public string algorithm;
        // public int percentage;
        // public 
        public PatientRecord()
        {
        }

        public PatientRecord(string firstName = "", string middleName = "", string lastName = "",
            string secondLastName = "", string birthDate = "", string city = "", string phoneNumber = "")
        {
            //patients will have Firstname,Lastname, MiddleName, SecondLastname, Phone Number, Socials, Place ofBirth
            FirstName = firstName;
            LastName = lastName;
            MiddleName = middleName;
            SecondLastName = secondLastName;
            BirthDate = birthDate;
            City = city;
            PhoneNumber = phoneNumber;
        } //end of PatinetInfoRecord
        //This function is to return all the public field of the class

        public PatientRecord swapLastNames()
        {
            PatientRecord P = new PatientRecord(this.FirstName, this.MiddleName, this.SecondLastName,
                this.LastName, this.BirthDate, this.City, this.PhoneNumber);
            return (P);
        }

        public string[] getDataInfo()
        {
            string[] result = new string[]
            {
                this.FirstName, this.MiddleName, this.LastName, this.SecondLastName, this.BirthDate, this.City,
                this.PhoneNumber
            };
            return (result);
        }

        //This function is to display all the public field of the class.
        //change to getDisplayIngo
        public void displayInfo()
        {
            Console.WriteLine(
                $"First Name: {this.FirstName} MiddleName: {this.MiddleName} Lastname: {this.LastName} SecondLastname: {this.SecondLastName} BirthDate: {this.BirthDate} City: {this.City} PhoneNumber: {this.PhoneNumber}");
        }

        public string getDisplayInfo()
        {
            return (
                $"First Name: {this.FirstName} MiddleName: {this.MiddleName} Lastname: {this.LastName} SecondLastname: {this.SecondLastName} BirthDate: {this.BirthDate} City: {this.City} PhoneNumber: {this.PhoneNumber}");
        }

        public string StringToCSVFormat()
        {
            string str =
                $"{this.FirstName},{this.MiddleName},{this.LastName},{this.SecondLastName},{this.BirthDate},{this.City},{this.PhoneNumber}";
            return (str);
        }

        public string StringToCSVFormatAsterisk()
        {
            string str =
                $"{this.FirstName}*{this.MiddleName}*{this.LastName}*{this.SecondLastName}*{this.BirthDate}*{this.City}*{this.PhoneNumber}*";
            return (str);
        }

        public string csvColumnNames()
        {
            string str = "FirstName,MiddleName,LastName,SecondLastName,BirthDate,City,PhoneNumber";
            return (str);
        }

        public string csvColumnNamesAsterisk()
        {
            string str = "PatientInfoAsterisk";
            return str;
        }
    }
}