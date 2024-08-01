using api.Entities.Admin.Order;

namespace api.Entities.HR
{
    public class OrderAssessmentItem: BaseEntity
    {
        public int orderAssessmentId { get; set; }
        public int OrderItemId { get; set; }
        public int ProfessionId { get; set; }
        public string ProfessionName { get; set; }
        public string CustomerName { get; set; }
        public int OrderNo { get; set; }
        public bool RequireCandidateAssessment { get; set; }=false;
        public string AssessmentRef { get; set; }
        public DateTime DateDesigned { get; set; }
        public string DesignedBy { get; set; }
        public string ApprovedBy { get; set; }
        public ICollection<OrderAssessmentItemQ> OrderAssessmentItemQs { get; set; }
    }
}