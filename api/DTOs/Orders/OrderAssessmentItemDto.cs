using api.Entities.Admin.Order;

namespace api.DTOs.Orders
{
    public class OrderAssessmentItemDto
    {
        public int Id { get; set; }
        public int orderAssessmentId { get; set; }
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public int professionId { get; set; }
        public string professionName { get; set; }
        public ICollection<OrderAssessmentItemQ> orderAssessmentItemQs { get; set; }

    }
}