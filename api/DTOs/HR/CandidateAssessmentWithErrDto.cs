using api.Entities.HR;

namespace api.DTOs.HR
{
    public class CandidateAssessmentWithErrDto
    {
        public CandidateAssessment candidateAssessment { get; set; }
        public string ErrorString { get; set; }
    }
}