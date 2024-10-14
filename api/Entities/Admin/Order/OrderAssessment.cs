namespace api.Entities.Admin.Order
{
    public class OrderAssessment: BaseEntity
    {
        public int OrderId { get; set; }
        public int OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public string DesignedByUsername { get; set; }
        public DateTime DateDesigned {get; set;}
        public ICollection<OrderAssessmentItem> OrderAssessmentItems { get; set; }
    }
}