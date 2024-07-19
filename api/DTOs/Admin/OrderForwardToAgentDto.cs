using api.Entities.Admin.Order;

namespace api.DTOs.Admin
{
    public class OrderForwardToAgentDto
    {
        public int Id { get; set; }
        public int OrderId {get; set;}
        public int OrderNo {get; set;}
        public DateTime OrderDate {get; set;}
        public int CustomerId {get; set;}
        public string CustomerName {get; set;}
        public string CustomerCity {get; set;}
        public int ProjectManagerId {get; set;}
        public ICollection<OrderForwardCategory> OrderForwardCategories {get; set;}
    }
}