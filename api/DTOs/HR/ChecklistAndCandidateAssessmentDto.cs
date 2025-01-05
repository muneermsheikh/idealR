using api.Entities.HR;

namespace api.DTOs.HR
{
    public class ChecklistAndCandidateAssessmentDto
    {
        public CandidateAssessment Assessed {get; set;}
        public ChecklistHR ChecklistHR {get; set;}
        public string ErrorString {get; set;}
    }
}