
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Entities.Admin.Order
{
    [NotMapped]
    public class OrderItemHRAssessment: BaseEntity
    {
        public int OrderItemId { get; set; }
        public bool RequireInternalReview {get; set;}=false;
        public bool RequireAssess { get; set; }=false;
        public int? HrExecId { get; set; }
        public string HRExecName { get; set; }
        public bool NoReviewBySupervisor { get; set; }=false;
        public int? HrSupId { get; set; }
        public string HrSupName { get; set; } 
        public int? HrmId { get; set; }
        public string HrmName { get; set; }
        public OrderItem orderItem { get; set; }

    }
}