namespace api.Params.Admin
{
    public class CallRecordParams: PaginationParams
    {
        public int Id { get; set; }
        public string IncomingOutgoing { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string PersonType { get; set; }
        public string PhoneNo { get; set; }
        public string PersonId { get; set; }
        public string Subject { get; set; }
        public string Status { get; set; }
        public string StatusClass { get; set; }
        public string Username { get; set; }
        public string CategoryRef { get; set; }
        public DateTime DateRegistered { get; set; }
        public string AdvisoryBy { get; set; }
        public string GistOfDiscussions { get; set; }

    }
}