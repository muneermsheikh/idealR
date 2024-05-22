using api.DTOs.Admin.Orders;
using api.Entities.Admin;
using api.Entities.Admin.Order;

namespace api.Interfaces.Admin
{
    public interface IOrderForwardRepository
    {
        Task<OrderForwardToAgent> GenerateOrderForwardObjForAgentByOrderId(int orderid);
        Task<OrderForwardToHR> GenerateObjToForwardOrderToHR(int orderid);
        Task<string> UpdateOrderForwardedToAgents(OrderForwardToAgent orderForward);
        Task<string> UpdateForwardOrderToHR (OrderForwardToHR orderForwardToHR, string Username);
        Task<OrderForwardToAgent> OrderFowardsOfAnOrder(int orderid);
        Task<OrderForwardToAgentDto> AssociatesOfOrderForwardsOfAnOrder(int orderid, string Username);
    }
}