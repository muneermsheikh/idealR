namespace api.DTOs.Admin
{
    public class SelDecisionDto
    {
        public string CustomerName { get; set; }
        public int OrderNo { get; set; }
        public string ProfessionName { get; set; }
        public DateTime SelectionDate {get; set;}
        public string SelectionStatus { get; set; }
        public string RejectionReason { get; set; }
        public DateOnly SelectedOn { get; set; }
        public int Charges {get; set;}
        //public bool Accepted {get; set;}    
    }
}