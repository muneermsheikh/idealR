namespace api.Entities.Admin.Client
{
    public class CustomerFeedback: BaseEntity
    {
        public int FeedbackNo { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string City  { get; set; }
        public string Country { get; set; }
        public string OfficialName { get; set; }
        public int OfficialAppUserId { get; set; }
        public string Designation { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public DateOnly DateIssued { get; set; }
        public DateOnly DateReceived { get; set; }
        public string HowReceived { get; set; }
        public string GradeAssessedByClient { get; set; }
        public string CustomerSuggestion { get; set; }
        public ICollection<FeedbackItem> FeedbackItems { get; set; }
    }
}