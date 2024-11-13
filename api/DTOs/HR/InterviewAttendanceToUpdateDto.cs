namespace api.DTOs.HR
{
    public class InterviewAttendanceToUpdateDto
    {
        public int InterviewCandidateId { get; set; }
        public DateTime? ReportedAt { get; set; }  
        public DateTime? InterviewedAt { get; set;}
        public string InterviewStatus { get; set; }
        public DateTime InterviewStatusDate { get; set; }
        public string InterviewerRemarks { get; set; }
        public string AttachmentFileNameWithPath { get; set; }
    }
}