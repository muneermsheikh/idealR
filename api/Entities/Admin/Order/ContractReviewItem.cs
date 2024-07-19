using System.ComponentModel.DataAnnotations;

namespace api.Entities.Admin.Order
{
    public class ContractReviewItem: BaseEntity
    {
        public int ContractReviewId { get; set; }
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public string ProfessionName { get; set; }
        public int Quantity { get; set; }
        public bool Ecnr { get; set; }
        public string SourceFrom { get; set; }
        [MaxLength(1)]
        public string RequireAssess { get; set; }
        public int Charges { get; set; }
        public string HrExecUsername { get; set; }
        public string ReviewItemStatus {get; set;}="NotReviewed";
         
        public ICollection<ContractReviewItemQ> ContractReviewItemQs {get; set;}     //copies data from ReviewItemData
        //public OrderItem OrderItem {get; set;}
        
    }
}