using System;

namespace MatchingEngine.Models
{
    public class Record
    {
        private PatientRecord _patientInfo = new PatientRecord();
        private string _idType = "unspecified";
        private Guid _RecordId = Guid.NewGuid();


        public PatientRecord PatientInfo
        {
            get => _patientInfo;
            set => _patientInfo = value;
        }

        public string IdType
        {
            get => _idType;
            set => _idType = value;
        }

        public Guid RecordId
        {
            get => _RecordId;
            set => _RecordId = value;
        }

        public Record()
        {
            _patientInfo = new PatientRecord();
            _idType = "unspecifiedId";
            _RecordId = Guid.NewGuid();
        }

        public Record(PatientRecord PatientRecord, string idtype)
        {
            _patientInfo = PatientRecord;
            _idType = idtype;
            _RecordId = Guid.NewGuid();
        }

        public Record(PatientRecord PatientRecord, string idtype, string id)
        {
            string temp_id = id;

            _patientInfo = PatientRecord;
            _idType = idtype;
            try
            {
                _RecordId = new Guid(id);
            }
            catch
            {
                _RecordId = Guid.NewGuid();
                Console.WriteLine($"Guid problem with id: {temp_id}");
            }
        }
    }
}