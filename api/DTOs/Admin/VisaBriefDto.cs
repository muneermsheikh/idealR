namespace api.DTOs.Admin
{
    public class VisaBriefDto
    {
        public int Id { get; set; }
        public int VisaItemId { get; set; }
        public string CustomerKnownAs { get; set; }
        public int CustomerId { get; set; }
        public string VisaNo { get; set; }
        public bool VisaCanceled { get; set; }
        public string VisaDateH { get; set; }
        public DateTime VisaDateG { get; set; }
        public string VisaConsulate { get; set; }
        public string VisaCategoryEnglish { get; set; }
        public string VisaSponsorName { get; set; }
        public int VisaQuantity { get; set; }
        public int VisaConsumed { get; set; }
        public int VisaBalance { get; set; }
        public int DepItemId { get; set; }
    }
}
