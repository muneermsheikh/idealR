namespace api.DTOs.Admin.Orders
{
    public class OrderForwardToAgentDtoTODELETE
    {
        public int OrderId { get; set; }
        public int OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCity { get; set; }
        public ICollection<OrderForwardCategoryDto> OrderForwardCategoriesDto { get; set; }
    }
}