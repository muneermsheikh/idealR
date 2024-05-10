using api.Entities.Master;

namespace api.Entities.Admin.Order
{
    public class OrderItem: BaseEntity
    {
        public int OrderId { get; set; }
        public int SrNo { get; set; }
        public int ProfessionId { get; set; }
        public string SourceFrom { get; set; }
        public int Quantity { get; set; }
        public int MinCVs { get; set; }
        public int MaxCVs { get; set; }
        public bool Ecnr { get; set; }=false;
        public DateOnly CompleteBefore { get; set; }
        public int Charges { get; set; }
        public string Status { get; set; }="Not Started";
        public bool Checked {get; set;}
        public string ReviewItemStatus { get; set; }="NotReviewed";
        public ICollection<OrderItemAssessmentQ> OrderItemAssessmentQs { get; set; }
        public Profession Profession { get; set; }
        public JobDescription JobDescription { get; set; }
        public Remuneration Remuneration { get; set; }
        public ContractReviewItem ContractReviewItem {get; set;}
    }
}