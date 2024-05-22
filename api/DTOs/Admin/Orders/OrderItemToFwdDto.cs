using api.Entities.Admin.Order;

namespace api.DTOs.Admin.Orders
{
    public class OrderItemToFwdDto
    {
        public int Id {get; set;}
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        
        public int CategoryId { get; set; }
        public string CategoryRef { get; set; }
        public string CategoryName {get; set;}
        public int Quantity {get; set;}
        public ICollection<OrderForwardCategoryOfficial> DLForwardDates { get; set; }
    }
}