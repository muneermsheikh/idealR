using api.DTOs.Admin;
using api.DTOs.Admin.Orders;
using api.DTOs.Customer;
using api.Entities.Admin;
using api.Entities.Admin.Order;
using api.Helpers;
using api.Interfaces.Messages;
using api.Params.Orders;

namespace api.Interfaces.Admin
{
    public interface IOrderForwardRepository
    {
        //Task<OrderForwardToAgent> GenerateOrderForwardToAgent(int orderid);
        Task<OrderForwardToHR> GenerateObjToForwardOrderToHR(int orderid);
        Task<string> InsertOrUpdateOrderForwardToAgents(ICollection<OfficialAndCustomerNameDto> officialIds, int orderid, string username);
        //Task<string> UpdateOrderForwardedToAgents(OrderForwardToAgent orderForward, string username);
        Task<string> UpdateForwardOrderToHR (int orderid, string Username);
        Task<string> EditOrderForwardCategories(ICollection<OrderForwardCategory> models, string username);
        //Task<OrderForwardToAgent> OrderFowardsOfAnOrder(int orderid);
        Task<ICollection<OrderForwardCategoryDto>> AssociatesOfOrderForwardsOfAnOrder(int orderid, string Username);
        //Task<PagedList<OrderForwardCategory>> GetPagedList(OrderFwdParams fParams);
        Task<PagedList<OrderForwardCategory>> GetPagedListDLForwarded(OrderFwdParams fParams);
        Task<bool> DeleteOrderForwardCategory(int orderforwardcategoryid);
        Task<bool> DeleteOrderForwardCategoryOfficial(int orderFwdOfficialId);
        Task<MessageWithError> ComposeMsg_AckToClient(int orderid);
        Task<bool> UpdateOrderExtn(int orderid, string fieldName, string fieldVal);
        Task<bool> UpdateOrderExtnDueToDelete(int orderid, string fieldName);
    }
}