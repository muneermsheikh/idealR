using api.Entities.Admin.Client;

namespace api.Interfaces.Customers
{
    public interface ICustomerReviewRepository
    {
         Task<CustomerReview> GetCustomerReview (int customerId);
         Task<bool> UpdateCustomerReview (CustomerReview customerReview, string Username);
         Task<bool> DeleteCustomerReview (int customerId);
         Task<ICollection<string>> GetCustomerReviewStatusData();
    }
}