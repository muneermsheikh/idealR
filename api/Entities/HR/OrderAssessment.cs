namespace api.Entities.HR
{
    public class OrderAssessment: BaseEntity
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string DesignedByUsername { get; set; }
        public ICollection<OrderAssessmentItem> OrderAssessmentItems { get; set; }
    }
}