namespace api.Entities.Admin.Order
{
    public class OrderForwardToAgent: BaseEntity
    {
        public int OrderId {get; set;}
        public int OrderNo {get; set;}
        public DateTime OrderDate {get; set;}
        public int CustomerId {get; set;}
        public string customerName {get; set;}
        public string CustomerCity {get; set;}
        public int ProjectManagerId {get; set;}
        //public ICollection<OrderForwardCategory> OrderForwardCategories {get; set;}
    }
}