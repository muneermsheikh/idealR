using api.Data;
using Microsoft.EntityFrameworkCore;

namespace api.Extensions
{
    public static class CustomerExtensions
    {
  
        public async static Task<string> CustomerNameFromId(this DataContext dataContext, int customerId)
        {
            var customername = await dataContext.Customers.Where(x => x.Id == customerId).Select(x => x.CustomerName).FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(customername)) return "Customer Not Found";
            return customername;
        }
    }
}