using api.Entities.Admin;

namespace api.DTOs.Admin
{
    public class CallRecordItemToAddDto
    {
        public string PersonId { get; set; }
        public string PersonType { get; set; }
        public string CategoryRef { get; set; }
        public string CustomerCity { get; set; }
        public bool AdviseByMail { get; set; }
        public bool AdviseBySMS { get; set; }
        public CallRecordItem callRecordItem { get; set; }
    }
}