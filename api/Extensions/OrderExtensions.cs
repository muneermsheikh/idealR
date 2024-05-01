using api.Data;
using api.Entities.Admin.Order;
using Microsoft.EntityFrameworkCore;

namespace api.Extensions
{
    public static class OrderExtensions
    {
        public async static Task<int> GetMaxOrderNo(this DataContext context)
        {
            var orderNo = await context.Orders.MaxAsync(x => (int?)x.OrderNo) ?? 1000;

            return orderNo;
        }

        
    }
}