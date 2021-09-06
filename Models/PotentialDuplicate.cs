namespace MatchingEngine.Models
{
    public class PotentialDuplicate
    {
        private PatientRecord _value;
        private PatientRecord _match;
        private DistanceVector _distance;
        private double _score;

        public PatientRecord Value
        {
            get => _value;
            set => _value = value;
        }

        public PatientRecord Match
        {
            get => _match;
            set => _match = value;
        }

        public DistanceVector Distance
        {
            get => _distance;
            set => _distance = value;
        }

        public double Score
        {
            get => _score;
            set => _score = value;
        }


        public string csvColumnNames()
        {
            string col_names;
            col_names = "Value.IdType,Value.ID,Match.IdType,Match.ID,Score,Distance";
            col_names +=
                ",Value.FirstName,Value.MiddleName,Value.LastName,Value.SecondLastName,Value.BirthDate,Value.City,Velue.PhoneNumber";
            col_names +=
                ",Match.FirstName,Match.MiddleName,Match.LastName,Match.SecondLastName,Match.BirthDate,Match.City,Match.PhoneNumber";
            return (col_names);
        }

        public string csvColumnNamesAsterisk()
        {
            string col_names;
            col_names = "Value.IdType,Value.ID,Match.IdType,Match.ID,Score,Distance";
            col_names += ",Value.PatientInfoAsterisk";
            col_names += ",Match.PatientInfoAterisk";


            return (col_names);
        }

        public string csvColumnNamesUrl()
        {
            string col_names;
            col_names = "Patient 1 URL,Patient 2 URL, Score";
            return (col_names);
        }





        public string csvLineAsterisk()
        {
            string csv_str;

            //add the id type and id for record 1 
            csv_str=_value.IdType+","+_value.RecordId.ToString()+",";

            //add the id type and id for record 2
            csv_str+=_match.IdType+","+_match.RecordId.ToString()+",";

            //add the score 
            csv_str+=Score.ToString()+",";

            //add the distance vector
            csv_str+=Distance.vectorString()+",";

            //add the patient info asterisk string for value
            csv_str+=_value.csvAsteriskString()+",";

            //add the patient info asterisk string for match
            csv_str+=_match.csvAsteriskString();

            return csv_str;
        }

        public string urlCsvLine()
        {
            string url_csv_str;

            url_csv_str="https://bioportal.salud.gov.pr/administration/patients/"+_value.RecordId.ToString()+"/profile/general"+",";
            url_csv_str+="https://bioportal.salud.gov.pr/administration/patients/"+_match.RecordId.ToString()+"/profile/general"+",";
            url_csv_str+=Score.ToString();
            return(url_csv_str);
        }
        
        
        public PotentialDuplicate()
        {
            _value = new PatientRecord();
            _match = new PatientRecord();
            _distance = new DistanceVector();
            _score = 0.0;
        }

        public PotentialDuplicate(PatientRecord record1, PatientRecord record2, DistanceVector d, double score)
        {
            _value = record1;
            _match = record2;
            _distance = d;
            _score = score;
        }
    }
}