namespace api.Entities.Admin
{
    public class CallRecord: BaseEntity
    {
        public string Gender { get; set; }
        public string Source { get; set; } = "Unknown";
        public DateTime? date { get; set; }     //latest date of transaction
        public string CategoryRef { get; set; }
        public int ProfessionId { get; set; }
        public string ProfessionName { get; set; }
        public int? OrderItemId { get; set; }
        public string PersonType { get; set; }
        public string PersonId { get; set; }
        public string PersonName { get; set; }
        public string Subject { get; set; }
        public string PhoneNo {get; set;}
        public string Email { get; set; }
        public string Status {get; set;}
        public DateTime? StatusDate { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Username { get; set; }
        public DateTime? ConcludedOn {get; set;}
        public ICollection<CallRecordItem> CallRecordItems {get; set;}
    }
}