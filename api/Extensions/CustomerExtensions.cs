using api.Data;
using api.DTOs.Admin;
using Microsoft.EntityFrameworkCore;

namespace api.Extensions
{
    public static class CustomerExtensions
    {
  
        public async static Task<string> CustomerNameFromId(this DataContext dataContext, int customerId)
        {
            var customername = await dataContext.Customers
                .Where(x => x.Id == customerId).Select(x => x.CustomerName).FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(customername)) return "Customer Not Found";
            return customername;
        }

        public async static Task<CustomerDto> CustomerBriefFromId(this DataContext dataContext, int customerId)
        {
            var customer = await dataContext.Customers.Where(x => x.Id == customerId)
                .Select(x => new CustomerDto{
                    CustomerType = x.CustomerType, CustomerName = x.CustomerName,
                    City = x.City, CustomerStatus = x.CustomerStatus})
                .FirstOrDefaultAsync();
                
            if (customer == null) return null;
            return customer;
        }
    }
}