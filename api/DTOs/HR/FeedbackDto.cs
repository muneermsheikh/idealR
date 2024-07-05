namespace api.DTOs.HR
{
    public class FeedbackDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public DateTime DateReceived { get; set; }
        public DateTime DateIssued { get; set; }
        public string GradeAssessedByClient { get; set; }
        public string CustomerSuggestion { get; set; }
    }
}