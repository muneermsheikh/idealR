namespace api.Entities.Admin.Client
{
    public class CustomerReview: BaseEntity
    {
        public int customerId { get; set; }
        public string CustomerName { get; set; }
        public string City { get; set; }
        public string CurrentStatus { get; set; }
        public string Remarks { get; set; }
        public ICollection<CustomerReviewItem> CustomerReviewItems { get; set; }
        
    }
}