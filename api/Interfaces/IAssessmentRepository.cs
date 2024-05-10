using api.Entities.Admin.Order;
using api.Entities.HR;

namespace api.Interfaces
{
    public interface IAssessmentRepository
    {
        Task<ICollection<AssessmentQStdd>> GetAssessmentQStdds();
        Task<OrderItemAssessment> GetOrderItemAssessment(int orderItemId);
        Task<ICollection<OrderItemAssessment>> GetOrderAssessments(int orderId);
        Task<OrderItemAssessment> GenerateOrderItemAssessmentFromStddQ(int orderItemId, string loggedinUsername);
        Task<OrderItemAssessment> SaveOrderItemAssessment(OrderItemAssessment orderItemAssessment);
        Task<bool> EditOrderItemAssessment(OrderItemAssessment orderItemAssessment);
        Task<bool> DeleteOrderItemAssessment(int orderItemId);
        Task<bool> DeleteOrderItemAssessmentQ(int AssessentQId);
    }
}