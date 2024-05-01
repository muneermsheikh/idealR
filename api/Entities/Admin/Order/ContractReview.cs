namespace api.Entities.Admin.Order
{
    public class ContractReview : BaseEntity
    {
        public int OrderId { get; set; }
        public int OrderNo {get; set; }
        public DateTime OrderDate {get; set;}
        public int CustomerId {get; set;}
        public string CustomerName {get; set;}
        public int ReviewedBy { get; set; }
        public DateTime ReviewedOn { get; set; } = DateTime.Now;
        public string ReviewStatus { get; set; } = "NotReviewed";
        public bool ReleasedForProduction { get; set; }=false;
        public ICollection<ContractReviewItem> ContractReviewItems {get; set; }
        public Order Order { get; set; }
    }
}