using api.Entities.Admin.Order;

namespace api.DTOs.Orders
{
    public class OrderAssessmentItemDto
    {
        public OrderAssessmentItemDto()
        {
        }


        public int Id { get; set; }
        public int OrderAssessmentId { get; set; }
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public int ProfessionId { get; set; }
        public string ProfessionName { get; set; }
        public string DesignedBy { get; set; }
        public ICollection<OrderAssessmentItemQ> OrderAssessmentItemQs { get; set; }

    }
}