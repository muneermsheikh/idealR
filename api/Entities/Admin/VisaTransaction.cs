namespace api.Entities.Admin
{
    public class VisaTransaction: BaseEntity
    {
        public int ApplicationNo { get; set; }
        public string CandidateName { get; set; }
        public int VisaItemId { get; set; }
        public string VisaNo { get; set; }
        public string VisaCategory { get; set; }
        public int CvRefId { get; set; }
        public int DepItemId { get; set; }
        public DateTime VisaAppSubmitted { get; set; }
        public DateTime VisaApproved { get; set; }
        public DateTime VisaDateG { get; set; }
        public int CustomerId { get; set; }
        
    }
}