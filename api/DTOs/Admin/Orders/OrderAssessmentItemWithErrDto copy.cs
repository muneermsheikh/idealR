using api.DTOs.Orders;
using api.Entities.Admin.Order;

namespace api.DTOs.Admin.Orders
{
    public class OrderAssessmentItemWithErrDto
    {
        public OrderAssessmentItemDto orderAssessmentItemDto { get; set; }
        public string Error { get; set; }
    }
}