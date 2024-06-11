namespace api.DTOs.Admin
{
    public class SelPendingDto
    {
        public int Id { get; set; }
        public bool Checked { get; set; }
        public int CvRefId { get; set; }
        public int OrderItemId { get; set; }
        public string CustomerName { get; set; }
        public string CategoryRefAndName { get; set; }
        public int ApplicationNo { get; set; }
        public string CandidateName { get; set; }
        public DateTime ReferredOn { get; set; }
        //public string RejectionReason { get; set; }
        public string Remarks { get; set; }
        
    }
}