namespace api.Entities.Admin.Order
{
    public class OrderExtn: BaseEntity
    {
        public int OrderId { get; set; }
        public int OrderNo { get; set; }
        public int ContractReviewId { get; set; }
        public DateTime? ContractReviewedOn { get; set; }
        public string ContratReviewResult { get; set; }
        public DateTime? AcknowledgedOn { get; set; }
        public DateTime? ForwardedToHROn { get; set; }
        public DateTime? AssessmentDesignedOn { get; set; }

    }
}