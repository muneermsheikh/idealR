namespace api.DTOs.HR
{
    public class CallRecordItemToCreateDto
    {
        public string Subject { get; set; }
        public int CallRecordId { get; set; }
        public string CategoryRef { get; set; }
        public string PersonType { get; set; }
        public string PersonId { get; set; }
        public string PersonName { get; set; }
        public string Status { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }   
    }
}