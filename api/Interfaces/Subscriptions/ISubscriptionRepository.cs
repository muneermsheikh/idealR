using api.Controllers;
using api.Entities.Subscriptions;

namespace api.Interfaces.Subscriptions
{
    public interface ISubscriptionRepository
    {
        Task<bool> IsTrialActive(int AppUserId);
        Task ExtendTrialAsync(int AppUserId, int additionalDays);
        Task<Subscription> GetSubscriptionAsync(int userId);
        Task ConvertToPaidAsync(ConvertToPaidRequest request);
    }
}