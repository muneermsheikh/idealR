using api.Entities.Admin.Order;

namespace api.DTOs.Admin.Orders
{
    public class OrderForwardCategoryDto
    {
        public int OrderItemId { get; set; }
        public int ProfessionId { get; set; }
        public string ProfessionName { get; set; }
        public int Charges { get; set; }
        public ICollection<OrderForwardToOfficialDto> OfficialsDto { get; set; }
    }
}