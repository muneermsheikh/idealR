namespace api.Params.HR
{
    public class ProspectiveCandidateParams: PaginationParams
    {
        public int Id { get; set; }
        public int ResumeId { get; set; }
        public string Source {get; set;}
        public string CandidateName { get; set;}
        public string PhoneNo { get; set; }
        public string Email { get; set; }
        public DateTime DateRegistered { get; set; }
        public string CategoryRef { get; set; }
        public int OrderItemId { get; set; }
        public string Status { get; set; }
        public string StatusClass { get; set; }
    }
}