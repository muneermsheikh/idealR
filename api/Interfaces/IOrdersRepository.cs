using api.DTOs.Admin.Orders;
using api.Entities.Admin.Order;
using api.Helpers;
using api.Interfaces.Messages;
using api.Params.Orders;

namespace api.Interfaces
{
    public interface IOrdersRepository
    {
        Task<Order> CreateOrderAsync(OrderToCreateDto dto);
        Task<bool> EditOrder(Order order);
        Task<bool> DeleteOrder(int orderid);
        Task<PagedList<OrderBriefDto>> GetOrdersAllAsync(OrdersParams orderParams);
        Task<OrderDisplayDto> GetOrderByIdWithAllRelatedProperties (int id);
        Task<Order> GetOrderByIdWithItemsAsyc (int id);
        Task<ICollection<OrderItemBriefDto>> GetOpenOrderItemsBriefDto();
                    
        //order items
        Task<PagedList<OpenOrderItemCategoriesDto>> GetOpenItemCategories(OpenOrderItemsParams orderItemParams);
        Task<OrderItem> AddOrderItem(OrderItemToCreateDto orderItem);
        Task<bool> EditOrderItem(OrderItem orderItem, bool DoNotSave);
        Task<bool> DeleteOrderItem (int orderItemId);
        Task<ICollection<OrderItemBriefDto>> GetOpenOrderItemsMatchingAProfession(int professionId);
        Task<PagedList<OrderItemBriefDto>> GetOpenOrderItems(OpenOrderItemsParams itemParams);
        Task<OrderItemBriefDto> GetOrderItemBrief(int orderitemid);
        Task<string> GetOrderItemRefCode(int orderitemid);
        Task<ICollection<OpenOrderItemCategoriesDto>> GetOpenItemCategoryList();
        Task<string> WriteOrdersExcelToDB(string fileNameWithPath, string Username);
        
    }
}