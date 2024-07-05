namespace api.Entities.Admin.Client
{
    public class CustomerReviewItem: BaseEntity
    {
        public int CustomerReviewId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string   Username { get; set; }
        public string CustomerReviewStatus { get; set; }
        public string Remarks { get; set; }
        public DateTime ApprovedOn { get; set; }
        public string ApprovedByUsername {get; set;}
    }
}