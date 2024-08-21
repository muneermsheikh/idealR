namespace api.Entities.Admin
{
    public class InterviewItem: BaseEntity
    {
        public int InterviewId { get; set; }
        public string InterviewVenue { get; set; }
        public int OrderItemId { get; set; }
        public int ProfessionId { get; set; }
        public string ProfessionName { get; set;}   
        public DateTime ScheduledFrom { get; set; }
        public DateTime ScheduledUpto { get; set; }
        public DateTime ReportedAt { get; set; }  
        public DateTime InterviewedAt { get; set;}
        public string InterviewMode { get; set; }
        public string InterviewerName { get; set; }
        public int CandidateId { get; set; }
        public int ApplicationNo { get; set; }
        public string CandidateName { get; set; }
        public string InterviewerRemarks { get; set; }
        public string InterviewStatus { get; set; }
    }
}