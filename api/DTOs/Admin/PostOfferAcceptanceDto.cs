using api.Entities.Finance;

namespace api.DTOs.Admin
{
    public class PostOfferAcceptanceDto
    {
        public string OfferAccepted { get; set; }
        public int CVRefId { get; set; }
        public int EmploymentId { get; set; }
        public int CandidateId { get; set; }
        public DateTime ConclusionDate { get; set; }    
        public DateTime SelectedOn { get; set; }
        public int Charges { get; set; }
        public COA  coaDR { get; set; }
        public COA coaCR { get; set; }
    }
}