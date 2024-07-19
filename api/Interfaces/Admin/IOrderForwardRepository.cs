using api.DTOs.Admin;
using api.DTOs.Admin.Orders;
using api.DTOs.Customer;
using api.Entities.Admin;
using api.Entities.Admin.Order;
using api.Helpers;
using api.Params.Orders;

namespace api.Interfaces.Admin
{
    public interface IOrderForwardRepository
    {
        //Task<OrderForwardToAgent> GenerateOrderForwardToAgent(int orderid);
        Task<OrderForwardToHR> GenerateObjToForwardOrderToHR(int orderid);
        Task<string> InsertOrderForwardCategories(ICollection<OfficialAndCustomerNameDto> officialIds, int orderid, string username);
        //Task<string> UpdateOrderForwardedToAgents(OrderForwardToAgent orderForward, string username);
        Task<string> UpdateForwardOrderToHR (int orderid, string Username);
        Task<string> UpdateOrderForwardCategories(ICollection<OrderForwardCategory> models, string username);
        //Task<OrderForwardToAgent> OrderFowardsOfAnOrder(int orderid);
        Task<ICollection<OrderForwardCategoryDto>> AssociatesOfOrderForwardsOfAnOrder(int orderid, string Username);
        Task<PagedList<OrderForwardToAgentDto>> GetPagedList(OrderFwdParams fParams);
        Task<bool> DeleteOrderForward(int orderid);
        Task<bool> DeleteOrderForwardCategory(int orderitemid);
        Task<bool> DeleteOrderForwardCategoryOfficial(int orderFwdOfficialId);
    }
}