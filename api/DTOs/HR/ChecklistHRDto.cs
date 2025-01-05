using api.Entities.HR;

namespace api.DTOs.HR
{
    public class ChecklistHRDto
    {
        public int Id {get; set;}
        public int CandidateId { get; set; }
        public int OrderItemId { get; set; }
        public string UserName {get; set;}
        public string HrExecUsername { get; set; }
        public DateTime CheckedOn {get; set;}
        public int Charges {get; set;}
        public int ChargesAgreed { get; set; }
        public bool ExceptionApproved {get; set;}
        public string ExceptionApprovedBy {get; set;}
        public DateTime ExceptionApprovedOn {get; set;}
        public string SalaryOffered {get; set;}
        public bool ChecklistedOk {get; set;}
        public bool RequireInternalReview {get; set;}
        //following members additional
        public int ApplicationNo { get; set; }  //new
        public string CandidateName { get; set; } //new
        public string CategoryRef { get; set; } 
        public string OrderRef { get; set; }
        public bool AssessmentIsNull {get; set;}
        
        public ICollection<ChecklistHRItem> ChecklistHRItems {get; set;}
    }
}
