using api.Entities.Admin;

namespace api.DTOs.Admin
{
    public class CallRecordCandidateAdviseDto
    {
        public string PersonId { get; set; }
        public string CategoryRef { get; set; }
        public string CustomerCity { get; set; }
        public CallRecordItem CallRecordItem { get; set; }
    }
}