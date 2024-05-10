namespace api.Entities.Admin.Order
{
    public class ContractReviewItem: BaseEntity
    {
        public int ContractReviewId { get; set; }
        public int OrderItemId { get; set; }
        public string ProfessionName { get; set; }
        public int Quantity { get; set; }
        public bool Ecnr { get; set; }
        public string SourceFrom { get; set; }
        public bool RequireAssess { get; set; }
        public int Charges { get; set; }
        public string ReviewItemStatus {get; set;}="NotReviewed";
        public ICollection<ContractReviewItemQ> ContractReviewItemQs {get; set;}     //copies data from ReviewItemData
        //public OrderItem OrderItem {get; set;}
        
    }
}