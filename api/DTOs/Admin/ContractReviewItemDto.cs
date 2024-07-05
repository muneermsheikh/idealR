using api.Entities.Admin.Order;

namespace api.DTOs.Admin
{
    public class ContractReviewItemDto
    {
        public int Id { get; set; }
        public int ContractReviewId {get; set;}
        public string ProfessionName {get; set;}
        public int Quantity {get; set; }
        public bool Ecnr {get; set;}
        public string SourceFrom { get; set; }
        public string RequireAssess { get; set; }
        public int Charges { get; set; }
        public string HrExecUsername { get; set; }
        public string ReviewItemStatus {get; set;}
        //additiona fields
        public int SrNo { get; set; }
        
        public int OrderId { get; set; }
        public int OrderNo { get; set; }
        public DateTime OrderDate {get; set;}
        public int OrderItemId { get; set; }
        public string CustomerName {get; set;}
        
        public DateTime CompleteBefore { get; set; }
        
        public ICollection<ContractReviewItemQ> ContractReviewItemQs {get; set;}
    }
}