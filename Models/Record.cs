using System;

namespace MatchingEngine.Models
{
    public class Record 
    {
        public string IdType { get; set; }
        public Guid RecordId { get; set; }

        public Record()
        {
            IdType = "unspecifiedId";
            RecordId = Guid.NewGuid();
        }

        public Record(PatientRecord PatientRecord, string idtype, string id)
        {
            string temp_id = id;

            IdType = idtype;
            try
            {
                RecordId = new Guid(id);
            }
            catch
            {
                RecordId = Guid.NewGuid();
                Console.WriteLine($"Guid problem with id: {temp_id}");
            }
        }
    }
}