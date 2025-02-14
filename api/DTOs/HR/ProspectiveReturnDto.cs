namespace api.DTOs.HR
{
    public class ProspectiveReturnDto
    {
        public int ProspectiveCandidateId { get; set; }
        public int CandidateId { get; set; }
        public int ApplicationNo { get; set; }        
        public string CandidateUsername { get; set; }
    }
}