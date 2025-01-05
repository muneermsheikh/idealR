using api.Entities.Admin;
using api.Entities.Admin.Order;

namespace api.Entities.HR
{
    public class ChecklistHR: BaseEntity
    {
        public int CandidateId { get; set; }
        public int OrderItemId { get; set; }
        public int UserId { get; set; }
        public DateTime CheckedOn {get; set;}
        public string HrExecUsername { get; set; }
        public String HrExecComments {get; set;}
        public int Charges {get; set;}
        public int ChargesAgreed {get; set;}
        public bool ExceptionApproved {get; set;}
        public string ExceptionApprovedBy {get; set;}
        public DateTime ExceptionApprovedOn {get; set;}
        public string SalaryOffered {get; set;}
        public int SalaryExpectation {get; set;}
        public bool ChecklistedOk {get; set;}
        public string Username {get; set;}
        public ICollection<ChecklistHRItem> ChecklistHRItems {get; set;}
        //public Candidate Candidate {get; set;}
        //public OrderItem OrderItem {get; set;}

    }
}