using api.Entities.Admin.Order;

namespace api.DTOs.Admin.Orders
{
    public class OrderAssessmentItemWithErrDto
    {
        public OrderAssessmentItem orderAssessmentItem { get; set; }
        public string Error { get; set; }
    }
}