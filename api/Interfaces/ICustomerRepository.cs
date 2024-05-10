using api.DTOs.Admin;
using api.Entities.Admin.Client;
using api.Entities.Identity;
using api.Helpers;
using api.Params.Admin;

namespace api.Interfaces
{
    public interface ICustomerRepository
    {
         Task<PagedList<CustomerDto>> GetCustomers(CustomerParams customerParams);
         Task<Customer> GetCustomer(CustomerParams customerParams);
         Task<Customer> GetCustomerById(int Id);
         Task<bool> UpdateCustomer(Customer customer);
         Task<bool> DeleteCustomer(int CustomerId);
         Task<bool> InsertCustomer(Customer customer);
         Task<AppUser> CreateAppUserForCustomerOfficial(CustomerOfficial official);
         Task<bool> UpdateCustomerOfficialWithAppuserId(CustomerOfficial official);
    }
}