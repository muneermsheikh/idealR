namespace api.Entities.Admin.Order
{
    public class OrderAssessmentItem: BaseEntity
    {
        public int OrderAssessmentId { get; set; }
        public string AssessmentRef { get; set; }
        public int OrderItemId {get; set;}
        public string ProfessionGroup {get; set;}
        public string CustomerName { get; set; }
        public string ProfessionName { get; set; }
        public int ProfessionId { get; set; }
        public int OrderNo { get; set; }
        public int OrderId { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime DateDesigned { get; set; }
        public bool RequireCandidateAssessment { get; set; }
        public string DesignedBy { get; set; }
        public ICollection<OrderAssessmentItemQ> OrderAssessmentItemQs { get; set; }
    }
}