using api.Entities.Admin.Client;

namespace api.Interfaces.Customers
{
    public interface ICustomerReviewRepository
    {
         Task<CustomerReview> GetCustomerReview (int customerId);
         Task<CustomerReview> GetOrCreateCustomerReviewObject(int customerId, string username);
         Task<bool> InsertNewCustomerReview(CustomerReview review);
         Task<bool> UpdateCustomerReview (CustomerReview customerReview, string Username);
         Task<bool> DeleteCustomerReview (int customerId);
         Task<bool> DeleteCustomerReviewItem(int reviewItemDataId);
         Task<ICollection<string>> GetCustomerReviewStatusData();
         Task<bool> ApproveReviewItem(int customerReviewItemId, string username);
    }
}