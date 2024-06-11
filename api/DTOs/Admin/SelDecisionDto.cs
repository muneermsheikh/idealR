namespace api.DTOs.Admin
{
    public class SelDecisionDto
    {
        public int SelDecisionId { get; set; }
        public int CVRefId { get; set; }
        public int OrderItemId { get; set; }
        public string CandidateName { get; set; }
        public int ApplicationNo { get; set; }
        public string CustomerName { get; set; }
        public string CategoryRef { get; set; }
        public DateTime ReferredOn { get; set; }
        public DateTime SelectedOn { get; set; }
        public string SelectionStatus { get; set; }
        //public string RejectionReason { get; set; }
        
    }
}