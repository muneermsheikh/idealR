using api.DTOs.Orders;
using api.Entities.Admin.Order;

namespace api.DTOs.Admin.Orders
{
    public class OrderAssessmentItemWithErr
    {
        public OrderAssessmentItem orderAssessmentItem { get; set; }
        public string Error { get; set; }
    }
}