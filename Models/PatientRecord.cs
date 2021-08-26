namespace MatchingEngine.Models
{
    public class PatientRecord : Record
    {
        public string FirstName { get; set; } = null!;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = null!;
        public string? SecondLastName { get; set; }
        public string BirthDate { get; set; } = null!;
        public string? City { get; set; }
        public string? PhoneNumber { get; set; }

        public string csvAsteriskString()
        {
            string csv_str;

            //Write out 
            csv_str=FirstName+"*"+MiddleName+"*"+LastName+"*";
            csv_str+=SecondLastName+"*"+BirthDate+"*"+City+"*"+PhoneNumber;
            return(csv_str);
        }
    }
}