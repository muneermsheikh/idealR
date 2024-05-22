using api.Entities.HR;

namespace api.DTOs.HR
{
    public class CandidateAssessmentAndChecklistDto
    {
        public CandidateAssessment Assessed {get; set;}
        public ChecklistHRDto ChecklistHRDto {get; set;}
        public string ErrorString {get; set;}
    }
}