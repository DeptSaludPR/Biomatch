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
    }
}