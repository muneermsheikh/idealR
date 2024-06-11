using api.DTOs.Admin;
using api.DTOs.Admin.Orders;
using api.Entities.Admin;
using api.Entities.Admin.Order;
using api.Helpers;
using api.Params.Orders;

namespace api.Interfaces.Admin
{
    public interface IOrderForwardRepository
    {
        Task<OrderForwardToAgent> GenerateOrderForwardToAgent(int orderid);
        Task<OrderForwardToHR> GenerateObjToForwardOrderToHR(int orderid);
        Task<string> InsertOrderForwardedToAgents(OrderForwardToAgent orderForward, string username);
        Task<string> UpdateOrderForwardedToAgents(OrderForwardToAgent orderForward, string username);
        Task<string> UpdateForwardOrderToHR (OrderForwardToHR orderForwardToHR, string Username);
        Task<OrderForwardToAgent> OrderFowardsOfAnOrder(int orderid);
        Task<OrderForwardToAgentDto> AssociatesOfOrderForwardsOfAnOrder(int orderid, string Username);
        Task<PagedList<OrderForwardToAgentDto>> GetPagedListOfOrderFwds(OrderFwdParams fParams);
        Task<bool> DeleteOrderForward(int orderid);
        Task<bool> DeleteOrderForwardCategory(int orderitemid);
        Task<bool> DeleteOrderForwardCategoryOfficial(int orderFwdOfficialId);
    }
}