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
        Task<OrderAssessmentItem> GetOrderAssessmentItem(int orderItemId);
        Task<OrderAssessmentItem> GenerateOrderAssessmentItemFromStddQ(int orderItemId, string loggedinUsername);
        Task<OrderAssessmentItem> SaveOrderAssessmentItem(OrderAssessmentItem orderItemAssessment);
        Task<bool> EditOrderAssessment(OrderAssessment orderAssessment, string Username);
        Task<bool> EditOrderAssessmentItem(OrderAssessmentItem orderAssessmentItem);
        Task<bool> DeleteOrderAssessmentItem(int orderItemId);
        Task<bool> DeleteOrderAssessmentItemQ(int questionId);
 
        //orderitemassessmentQ
        Task<ICollection<AssessmentQStdd>> GetAssessmentQStdds();
        Task<ICollection<OrderAssessmentItemQ>> GetOrderAssessmentItemQs(int orderitemid);
        
    }
}