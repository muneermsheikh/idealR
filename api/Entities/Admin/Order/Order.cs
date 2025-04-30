
using System.ComponentModel.DataAnnotations;
using api.Entities.Admin.Client;

namespace api.Entities.Admin.Order
{
    public class Order: BaseEntity
    {
        [MaxLength(1), Required, RegularExpression(@"^[DdIi]*$", ErrorMessage ="Destination value should be either d or D (for domestic recruitment), or I or i (for international recruitments)")]
        public string Destination {get; set;}   //domestic or international
        public int OrderNo { get; set; }
        public DateTime OrderDate { get; set; }=DateTime.UtcNow;
        public int CustomerId { get; set; }
        public string OrderRef { get; set; }
        public DateTime OrderRefDate {get; set;}
        public int ProjectManagerId { get; set; }
        public int? SalesmanId { get; set; }
        
        public DateTime CompleteBy { get; set; }
        public string Country { get; set; }
        public string CityOfWorking { get; set; }
        public int ContractReviewId { get; set; }
        public string ContractReviewStatus {get; set;}="Not Reviewed";
        public string Status { get; set; } = "Awaiting Review";
        public DateTime? ForwardedToHRDeptOn { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        public Customer Customer {get; set;}
        public ContractReview ContractReview {get; set;}
        public ICollection<OrderForwardToAgent> DLForwarded {get; set;}
        
    }
}