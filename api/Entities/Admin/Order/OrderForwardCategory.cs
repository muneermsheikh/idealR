namespace api.Entities.Admin.Order
{
    public class OrderForwardCategory: BaseEntity
    {
        public int OrderId { get; set; }
        public int OrderItemId { get; set; }
        public int OrderForwardToAgentId { get; set; }
        public int ProfessionId { get; set; }
        public string ProfessionName { get; set; }
        public int Charges { get; set; }
        public ICollection<OrderForwardCategoryOfficial> OrderForwardCategoryOfficials { get; set; }
    }
}