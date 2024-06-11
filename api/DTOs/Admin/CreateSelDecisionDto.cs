namespace api.DTOs.Admin
{
    public class CreateSelDecisionDto
    {
        public int CVRefId { get; set; }
        public DateTime DecisionDate { get; set; }
        public string SelectionStatus { get; set; }
        //public string RejectionReason { get; set; }
        public string Remarks { get; set; }
    }
}