using api.Entities.HR;

namespace api.DTOs.HR
{
    public class ChecklistHRDto
    {
        public int Id {get; set;}
        public int CandidateId { get; set; }
        public int OrderItemId { get; set; }
        public string UserName {get; set;}
        public DateOnly CheckedOn {get; set;}
        public string HRExecUsername { get; set; }
        public int Charges {get; set;}
        public bool ExceptionApproved {get; set;}
        public string ExceptionApprovedBy {get; set;}
        public DateTime ExceptionApprovedOn {get; set;}
        public bool ChecklistedOk {get; set;}
        public bool RequireInternalReview {get; set;}
        //following members additional
        public int ApplicationNo { get; set; }
        public string CandidateName { get; set; }
        public bool AssessmentIsNull {get; set;}
        
        public ICollection<ChecklistHRItem> ChecklistHRItems {get; set;}
    }
}