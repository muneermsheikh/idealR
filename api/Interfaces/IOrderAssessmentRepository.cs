using api.DTOs.Admin.Orders;
using api.Entities.Admin.Order;
using api.Entities.HR;

namespace api.Interfaces
{
    public interface IOrderAssessmentRepository
    {
        //order assessment
        Task<OrderAssessment> SaveNewOrderAssessment(OrderAssessment orderAssessment);
        Task<OrderAssessment> GetOrderAssessment (int orderId, string Username);
        
        //orderitem assessment
        Task<OrderAssessmentItemWithErrDto> GetOrCreateOrderAssessmentItem(int orderItemId, string username);
        //Task<OrderAssessmentItem> GenerateOrderAssessmentItemFromStddQ(int orderItemId, string loggedinUsername);
        Task<OrderAssessmentItem> SaveOrderAssessmentItem(OrderAssessmentItem orderItemAssessment);
        Task<string> EditOrderAssessment(OrderAssessment orderAssessment, string Username);
        Task<bool> EditOrderAssessmentItem(OrderAssessmentItem orderAssessmentItem);
        Task<bool> DeleteOrderAssessmentItem(int orderItemId);
        Task<bool> DeleteOrderAssessmentItemQ(int questionId);
 
        //orderitemassessmentQ
        Task<ICollection<OrderAssessmentItemQ>> GetAssessmentQStdds();
        Task<ICollection<OrderAssessmentItemQ>> GetCustomAssessmentQsForAProfession(int professionid);
        Task<ICollection<OrderAssessmentItemQ>> GetOrderAssessmentItemQs(int orderitemid);
        
    }
}