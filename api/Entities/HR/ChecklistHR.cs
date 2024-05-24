using System.ComponentModel.DataAnnotations;
using api.Entities.Admin;
using api.Entities.Admin.Order;

namespace api.Entities.HR
{
    public class ChecklistHR: BaseEntity
    {
        public int CandidateId { get; set; }
        public int OrderItemId { get; set; }
        public string UserName { get; set; }
        public DateOnly CheckedOn {get; set;}
        public String UserComments {get; set;}
        [Required]
        public string HRExecUsername { get; set; }
        public int Charges {get; set;}
        public int ChargesAgreed {get; set;}
        public bool ExceptionApproved {get; set;}
        public string ExceptionApprovedBy {get; set;}
        public DateTime ExceptionApprovedOn {get; set;}
        public bool ChecklistedOk {get; set;}
        public ICollection<ChecklistHRItem> ChecklistHRItems {get; set;}
        public Candidate Candidate {get; set;}
        public OrderItem OrderItem {get; set;}

    }
}