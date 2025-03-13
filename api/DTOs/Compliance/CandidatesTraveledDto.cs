namespace api.DTOs.Compliance
{
    public class CandidatesTraveledDto
    {
        public DateTime TraveledOn { get; set; }
        public DateTime CandidateName { get; set; }
        public int ApplicationNo { get; set; }
        public string PassportNo { get; set; }
        public string VisaNo { get; set; }
        
    }
}