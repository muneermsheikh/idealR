using api.Entities.HR;

namespace api.DTOs.HR
{
    public class CandidateAndErrorStringDto
    {
        public Candidate candidate { get; set; }
        public string ErrorString { get; set; }
    }
}