using System.ComponentModel.DataAnnotations;
using api.Entities.Admin;
using api.Entities.Admin.Order;

namespace api.Entities.HR
{
    public class CandidateAssessment: BaseEntity
    {
        [Required]
        public int OrderItemId { get; set; }
        public int CandidateId {get; set;}
        public string CustomerName { get; set; }
        public string CategoryRefAndName { get; set; }
        public DateTime AssessedOn { get; set; }
        public int AssessedByEmployeeId { get; set; }
        public string AssessedByEmployeeName {get; set;}
        [Required]
        public bool RequireInternalReview {get; set;}
        public int ChecklistHRId {get; set;}
        [Required]
        public string AssessResult { get; set; } = "Not Assessed";
        public string Remarks { get; set; }
        public int CVRefId { get; set; }        //this field is set once this candidate is referred to client - CVRef
        public int TaskIdDocControllerAdmin {get; set;}
        public OrderItem OrderItem { get; set; }
        public ICollection<CandidateAssessmentItem> CandidateAssessmentItems { get; set; }
    }
}