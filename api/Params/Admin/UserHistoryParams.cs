namespace api.Params.Admin
{
    public class UserHistoryParams: PaginationParams
    {
        public int UserHistoryId {get; set;}
        public int CandidateId { get; set; }
        public string ResumeId { get; set; }
        public string CandidateName { get; set; }
        public int ApplicationNo {get; set;}
        public string CategoryRef { get; set; }
        public string Status { get; set; }
        public bool? Concluded {get; set;}
        public string UserName {get; set;}
    }
}