namespace api.Entities.Admin.Client
{
    public class CustomerReview: BaseEntity
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string City { get; set; }
        public string CurrentStatus {get; set;} = "Active";
        public string Remarks { get; set; }
        public ICollection<CustomerReviewItem> CustomerReviewItems { get; set; }
    }
}