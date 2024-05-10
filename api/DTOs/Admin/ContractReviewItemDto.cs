using api.Entities.Admin.Order;

namespace api.DTOs.Admin
{
    public class ContractReviewItemDto
    {
        public int Id { get; set; }
        public int SrNo { get; set; }
        public int ContractReviewId {get; set;}
        public int OrderId { get; set; }
        public int OrderNo { get; set; }
        public DateTime OrderDate {get; set;}
        public int OrderItemId { get; set; }
        public string CustomerName {get; set;}
        public string CategoryName {get; set;}
        public string SourceFrom { get; set; }
        public bool RequireAssess { get; set; }
        public bool Ecnr {get; set;}
        public int Quantity {get; set; }
        public int Charges { get; set; }
        public DateTime CompleteBefore { get; set; }
        public int ReviewItemStatus {get; set;}
        public ICollection<ReviewItem> ReviewItems {get; set;}
    }
}