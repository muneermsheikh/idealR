namespace api.Entities.Admin.Client
{
    public class CustomerReviewItem: BaseEntity
    {
        public int CustomerReviewId { get; set; }
        public DateOnly ReviewTransactionDate { get; set; }
        public int CustomerReviewDataId { get; set; }
        public string Remarks { get; set; }
        public string ApprovedBySupUsername { get; set; }
        public DateOnly ApprovedOn { get; set; }
        public string Username { get; set; }
    }
}