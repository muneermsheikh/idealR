using api.Entities.Admin.Client;

namespace api.DTOs.Admin
{
    public class FeedbackAndHistoryDto
    {
        public CustomerFeedback Feedback { get; set; }
        public ICollection<FeedbackHistoryDto> FeedbackHistories { get; set; }
    }

    public class FeedbackHistoryDto
    {
        public int FeedbackId { get; set; }
        public DateOnly FeedbackIssueDate { get; set; }
        //public DateOnly? FeedbackRecdDate { get; set; }
    }
}