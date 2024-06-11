using api.Entities.Admin.Order;

namespace api.DTOs.Admin.Orders
{
    public class OrderItemBriefDtoWithOfficialIdsDto
    {
        public int OrderId {get; set;}
        public int OrderNo {get; set;}
        public DateTime OrderDate {get; set;}
        public int CustomerId {get; set;}
        public string customerName {get; set;}
        public string CustomerCity {get; set;}
        public int ProjectManagerId {get; set;}
        public int OfficialId { get; set; }
        public OrderItemBriefDto  orderItemBriefDto { get; set; }
    }
}