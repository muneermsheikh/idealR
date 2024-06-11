namespace api.DTOs.Admin.Orders
{
    public class OrderForwardToAgentDto
    {
        public int OrderId { get; set; }
        public int OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public ICollection<OrderForwardCategoryDto> OrderForwardCategoriesDto { get; set; }
    }
}