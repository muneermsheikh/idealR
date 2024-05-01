using api.DTOs.Admin.Orders;
using api.Entities.Admin.Order;
using api.Helpers;
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
        Task<OrderDisplayWithItemsDto> GetOrderByIdWithItemsAsyc (int id);
        Task<bool> ComposeMsg_AckToClient(int orderid);
            
        //order items
        Task<OrderItem> AddOrderItem(OrderItemToCreateDto orderItem);
        Task<bool> EditOrderItem(OrderItem orderItem, bool DoNotSave);
        Task<bool> DeleteOrderItem (int orderItemId);
        Task<ICollection<OrderItemBriefDto>> GetOpenOrderItemsMatchingAProfession(int professionId);
            

        //Job descriptions
        Task<JobDescription> GetJDOfOrderItem(int OrderItemId);
        Task<JobDescription> AddJobDescription(JobDescription jobDescription);
        Task<bool> EditJobDescription(JobDescription jobDescription);
        Task<bool> DeleteJobDescription (int jobDescriptionId);

    // Remuneations
        Task<Remuneration> GetRemuneratinOfOrderItem(int OrderItemId);
        Task<Remuneration> AddRemuneration(Remuneration remuneration);
        Task<bool> EditRemuneration(Remuneration remuneration);
        Task<bool> DeleteRemuneration (int remunerationId);
    
    }
}