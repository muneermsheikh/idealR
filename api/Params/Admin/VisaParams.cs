namespace api.Params.Admin
{
    public class VisaParams: PaginationParams
    {
        public int Id { get; set; }
        public int VisaItemId { get; set; }
        public string VisaNo { get; set; }
        public int CvRefId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string VisaExpiryH { get; set; }
        public DateTime VisaExpiryG { get; set; }
        public string VisaSponsorName { get; set; }
        public string VisaCategory { get; set; }
        public string VisaConsulate { get; set; }
        public bool VisaCanceled { get; set; }
        public int DepItemId { get; set; }
        public bool VisaApproved { get; set; }
    }
}