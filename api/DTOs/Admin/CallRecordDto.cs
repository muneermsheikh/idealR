namespace api.DTOs.Admin
{
    public class CallRecordDto
    {
        public int Id { get; set; }
        public string PersonType { get; set; }
        public string PersonId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string MobileNo { get; set; }
        public string CategoryRef { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Username { get; set; }    
        public DateTime ConcludedOn { get; set; }
        public string Status {get; set;}
    }
}