namespace api.DTOs.Admin
{
    public class OfferConclusionDto
    {
        public int EmploymentId { get; set; }
        public string acceptedString { get; set; }
        public DateOnly ConclusionDate { get; set; }
    }
}