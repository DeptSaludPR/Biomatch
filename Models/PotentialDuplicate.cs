namespace MatchingEngine.Models
{
    public class PotentialDuplicate
    {
        private Record _value;
        private Record _match;
        private DistanceVector _distance;
        private double _score;

        public Record Value
        {
            get => _value;
            set => _value = value;
        }

        public Record Match
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

        public string csvFormatStringAsterisk()
        {
            string str;
            str =
                $"{Value.IdType},{Value.RecordId},{Match.IdType},{Match.RecordId},{Score},{Distance.CsvFormatString()}";
            str += $",{Value.PatientInfo.StringToCSVFormatAsterisk()},{Match.PatientInfo.StringToCSVFormatAsterisk()}";
            return str;
        }

        public PotentialDuplicate()
        {
            _value = new Record();
            _match = new Record();
            _distance = new DistanceVector();
            _score = 0.0;
        }

        public PotentialDuplicate(Record record1, Record record2, DistanceVector d, double score)
        {
            _value = record1;
            _match = record2;
            _distance = d;
            _score = score;
        }
    }
}