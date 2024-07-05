using api.DTOs.HR;
using api.Entities.Admin.Client;
using api.Helpers;
using api.Params;

namespace api.Interfaces.HR
{
    public interface IFeedbackRepository
    {
        Task<PagedList<FeedbackDto>> GetFeedbackList(FeedbackParams feedbackParams);
        Task<CustomerFeedback> GenerateOrGetFeedbackFromSameMonth(int CustomerId);
        Task<CustomerFeedback> GetFeedbackWithItems(int feedbackId);
        Task<ICollection<FeedbackQ>> GetFeedbackStddQs();
        Task<string> EditFeedback(CustomerFeedback feedback);
        Task<CustomerFeedback> SaveNewFeedback(FeedbackInput feedbackInput);
        Task<bool> DeleteFeedback(int FeedbackId);
        Task<bool> DeleteFeedbackItem(int FeedbackItemId);
    }
}