namespace api.DTOs.Admin
{
    public class SelDecisionDto
    {
        public int Id { get; set; }  //
        public int CvRefId { get; set; }
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int CandidateId { get; set; }
        public int ProfessionId { get; set; }
        public string Gender { get; set; }
        public string SelectedAs { get; set; }  //
        public int Charges { get; set; }            //
        public string CandidateName { get; set; }
        public int ApplicationNo { get; set; }
        public string CustomerName { get; set; }
        public string CategoryRef { get; set; }
        public DateTime ReferredOn { get; set; }
        public DateTime SelectedOn { get; set; }
        public string SelectionStatus { get; set; }
        public int EmploymentId { get; set; }
        //public string RejectionReason { get; set; }
        
    }
}