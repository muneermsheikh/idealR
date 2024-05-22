using api.Entities.Admin.Order;
using api.Entities.HR;

namespace api.Interfaces
{
    public interface IOrderAssessmentRepository
    {
        //order assessment
        Task<OrderAssessment> SaveNewOrderAssessment(OrderAssessment orderAssessment);
        Task<OrderAssessment> GetOrderAssessment (int orderId);
        
        //orderitem assessment
        Task<OrderItemAssessment> GetOrderItemAssessment(int orderItemId);
        Task<OrderItemAssessment> GenerateOrderItemAssessmentFromStddQ(int orderItemId, string loggedinUsername);
        Task<OrderItemAssessment> SaveOrderItemAssessment(OrderItemAssessment orderItemAssessment);
        Task<bool> EditOrderAssessment(OrderAssessment orderAssessment, string Username);
        Task<bool> EditOrderItemAssessment(OrderItemAssessment orderItemAssessment);
        Task<bool> DeleteOrderItemAssessment(int orderItemId);
        Task<bool> DeleteOrderItemAssessmentQ(int questionId);
 
        //orderitemassessmentQ
        Task<ICollection<AssessmentQStdd>> GetAssessmentQStdds();
        Task<ICollection<OrderItemAssessmentQ>> GetOrderItemAssessmentQs(int orderitemid);
        
    }
}