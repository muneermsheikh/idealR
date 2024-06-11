using api.Entities.Admin;

namespace api.DTOs.Admin.Orders
{
    public class OrderItemsAndAgentsToFwdDto
    {
        public int OrderId { get; set; }
        public int OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string AboutEmployer { get; set; }
        public int ProjectManagerId {get; set;}
        public ICollection<OrderItemToFwdDto> Items {get; set;}
        public ICollection<AssociatesToFwdDto> Agents {get; set;}
        public DateTime DateForwarded {get; set;}
    }
}
