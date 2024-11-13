using api.Entities.Admin;

namespace api.DTOs.HR
{
    public class InterviewAttendanceDto
    {
        public int OrderNo { get; set; }
        public string CustomerName { get; set; }
        public int Id { get; set; }
        public int InterviewId { get; set; }
        public string InterviewVenue { get; set; }
        public int OrderItemId { get; set; }
        //public int ProfessionId { get; set; }
        public string ProfessionName { get; set;}   
        public DateTime ScheduledFrom { get; set; }
        //public DateTime ScheduledUpto { get; set; }
        public DateTime ReportedAt { get; set; }  
        public DateTime InterviewedAt { get; set;}
        public string InterviewMode { get; set; }
        //public string InterviewerName { get; set; }
        //public int CandidateId { get; set; }
        public string PersonId { get; set; }
        public int InterviewItemCandidateId { get; set; }
        public int ApplicationNo { get; set; }
        public string CandidateName { get; set; }
        public string InterviewStatus { get; set; }
        public string InterviewerRemarks { get; set; }
        public string AttachmentFileNameWithPath { get; set; }
    }
}