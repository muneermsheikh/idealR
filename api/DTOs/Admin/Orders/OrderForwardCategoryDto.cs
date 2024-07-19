using api.Entities.Admin.Order;

namespace api.DTOs.Admin.Orders
{
    public class OrderForwardCategoryDto
    {
        public string CustomerName { get; set; }
        public string CustomerCity { get; set; }
        public int OrderId { get; set; }
        public int OrderNo { get; set; }
        public DateTime OrderDate { get; set; } 
        public int OrderItemId { get; set; }
        public int ProfessionId { get; set; }
        public DateTime DateTime { get; set; }
        public string ProfessionName { get; set; }
        public int Charges { get; set; }
        public ICollection<OrderForwardToOfficialDto> OfficialsDto { get; set; }
    }
}