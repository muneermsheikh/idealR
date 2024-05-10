using api.Data;
using api.Entities.Admin.Order;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace api.Extensions
{
    public static class OrderExtensions
    {
        public async static Task<int> GetMaxOrderNo(this DataContext context)
        {
            var orderNo = await context.Orders.MaxAsync(x => (int?)x.OrderNo) ?? 1000;

            return orderNo;
        }

        public async static Task<int> GetOrderNoFromOrderItemId(this DataContext context, int orderItemId)
        {
            var orderno = await (from Orders in context.Orders
                join items in context.OrderItems on Orders.Id equals items.OrderId
                where items.Id == orderItemId
                select Orders.OrderNo)
                .FirstOrDefaultAsync();


            return orderno;
        }
        
        public async static Task<int> GetSrNoFromOrderItemId(this DataContext context, int orderItemId)
        {
            var SrNo = await context.OrderItems.Where(x => x.Id == orderItemId)
                .Select(x => x.SrNo).FirstOrDefaultAsync();
            return SrNo;
        }
        public async static Task<string> GetCustomerNameFromOrderItemId(this DataContext context, int orderItemId)
        {
            var CustomerName = await (from Orders in context.Orders
                join items in context.OrderItems on Orders.Id equals items.OrderId
                where items.Id == orderItemId
                select Orders.Customer.CustomerName)
                .FirstOrDefaultAsync();


            return CustomerName;
        }
        
        public async static Task<string> GetOrderItemDescriptionFromOrderItemId(this DataContext context, int orderItemId)
        {
            var CustomerName = await (from Orders in context.Orders
                join items in context.OrderItems on Orders.Id equals items.OrderId
                where items.Id == orderItemId
                select Orders.OrderNo + "-" + items.SrNo + "-" + items.Profession.ProfessionName +  " for " + Orders.Customer.CustomerName
            )
            .FirstOrDefaultAsync();


            return CustomerName;
        }

        public async static Task<int> GetServiceChargesFromOrderItemId(this DataContext context, int orderItemId)
        {
            var charges = await (from rvw in context.ContractReviewItems 
                where rvw.OrderItemId==orderItemId
                join qs in context.ContractReviewItemQs  on rvw.Id equals qs.ContractReviewItemId
                    where qs.SrNo==9
                select qs.ResponseText
            ).FirstOrDefaultAsync();

            return string.IsNullOrEmpty(charges) ? 0 : Convert.ToInt32(charges);
        }
        public async static Task<bool> RequireAssessment(this DataContext context, int orderItemId)
        {
            var assess = await (from rvw in context.ContractReviewItems
                where rvw.OrderItemId==orderItemId
                select rvw.RequireAssess
            ).FirstOrDefaultAsync();

            return assess;
        }
    }
    
}