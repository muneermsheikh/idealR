using api.Entities.Admin;

namespace api.Interfaces
{
    public interface IFeedbackRepository
    {
         Task<Feedback> GetFeedbackFromId (int id);
         Task<ICollection<Feedback>> GetFeedbackFromCustomerName (string customerName);
         Task<bool> EditFeedback (Feedback feedback);
         Task<Feedback> InsertFeedback(Feedback feedback);
         Task<Feedback> GenerateNewFeedback(int customerId);
         Task<FeedbackItem> InsertFeedbackItem(FeedbackItem feedbackItem);
         Task<bool> DeleteFeedback (int id);
         Task<ICollection<FeedbackStddQ>> InsertFeedbackStddQs(ICollection<FeedbackStddQ> feedbackStddQs);
         Task<ICollection<FeedbackStddQ>> GetFeedbackStddQs();
    }
}