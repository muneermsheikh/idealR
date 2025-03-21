using System.Collections.ObjectModel;
using api.DTOs.Admin;
using api.DTOs.Customer;
using api.Entities.Admin.Client;
using api.Entities.Identity;
using api.Helpers;
using api.Params.Admin;

namespace api.Interfaces
{
    public interface ICustomerRepository
    {
         Task<PagedList<CustomerDto>> GetCustomers(CustomerParams customerParams);
         Task<ICollection<CustomerDto>> GetCustomerList(string customerType);
         Task<Customer> GetCustomer(CustomerParams customerParams);
         Task<ICollection<CustomerAndOfficialsDto>> GetCustomerAndOfficials(string customerType);
         Task<CustomerOfficialDto> GetCustomerOfficialDto(int CustomerOfficialId);
         Task<ICollection<string>> GetCustomerCities(string customerType);
         Task<Customer> GetCustomerById(int Id);
         Task<ICollection<ClientIdAndNameDto>> GetCustomerIdAndNames(string customerType);
         Task<bool> UpdateCustomer(Customer customer);
         Task<bool> DeleteCustomer(int CustomerId);
         Task<bool> DeleteCustomerOfficial(int officialId);
         Task<string> InsertCustomer(Customer customer);
         
         Task<AppUser> CreateAppUserForCustomerOfficial(CustomerOfficial official, string CustomerName);
         Task<bool> UpdateCustomerOfficialWithAppuserId(CustomerOfficial official);
        Task<string> WriteCustomerExcelToDB(string fileNameWithPath, string username);
         
         Task<ICollection<OfficialAndCustomerNameDto>> GetOfficialsAndCustomerNames(string customerType);

    }
}