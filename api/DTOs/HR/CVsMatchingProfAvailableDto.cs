using api.Entities.HR;

namespace api.DTOs.HR
{
    public class CVsMatchingProfAvailableDto
    {
        public string CandidateId { get; set; }
        public bool  Checked { get; set; }
        public string Gender { get; set; }
        public int ApplicationNo { get; set; }
        public string FullName {get; set;}
        public string ProfessionName { get; set; }
        public string MobileNo {get; set;}
        public string City {get; set;}
        public string Source {get; set;}
       
    }
}