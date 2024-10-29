using api.Entities.HR;

namespace api.Entities.Admin
{
    public class IntervwItemCandidate: BaseEntity
    {
        public int InterviewItemId { get; set; }
        public DateTime ScheduledFrom { get; set; }
        //public DateTime ScheduledUpto { get; set; }
        public DateTime? ReportedAt { get; set; }  
        public DateTime? InterviewedAt { get; set;}
        public int CandidateId { get; set; }
        public string PersonId {get; set;}
        public int ProspectiveCandidateId { get; set; }
        public int ApplicationNo { get; set; }
        public string CandidateName { get; set; }
        public string InterviewerRemarks { get; set; }
        public string InterviewStatus { get; set; }
        public string AttachmentFileNameWithPath { get; set; }
    }
}