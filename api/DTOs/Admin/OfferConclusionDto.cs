namespace api.DTOs.Admin
{
    public class OfferConclusionDto
    {
        public int EmploymentId { get; set; }
        public string AcceptedString { get; set; }      //accepted or rejected
        public bool OfferAccepted { get; set; }
        public string ConclusionStatus { get; set; }
        public DateTime ConclusionDate { get; set; }
    }
}