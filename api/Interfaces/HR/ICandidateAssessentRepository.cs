using api.DTOs.HR;
using api.Entities.HR;
using api.Helpers;
using api.Params.HR;

namespace api.Interfaces.HR
{
    public interface ICandidateAssessentRepository
    {
        Task<CandidateAssessmentAndChecklistDto> GetChecklistAndAssessment(int candidateid, int orderitemid, string username);
        Task<CandidateAssessmentWithErrDto> GenerateCandidateAssessment(int candidateid, int orderItemId, string Username);
        Task<CandidateAssessmentWithErrDto> SaveCandidateAssessment(CandidateAssessment candidateAssessment, string Username);
        Task<CandidateAssessmentWithErrDto> EditCandidateAssessment(CandidateAssessment candidateAssessment, string username);
        Task<bool> DeleteCandidateAssessment(int id);
        Task<bool> DeleteCandidateAssessmentItem(int id);
        Task<PagedList<CandidateAssessedDto>> GetCandidateAssessments(CandidateAssessmentParams assessmentParams);
        Task<ICollection<CandidateAssessedShortDto>> GetCandidateAssessmentsByCandidateId(int candidateId);
        
        Task<CandidateAssessment> GetCandidateAssessment(CandidateAssessmentParams assessmentParams);
        Task<CandidateAssessmentAndChecklistDto> GetCandidateAssessmentWithChecklistByAssessmentId(int candidateAssessmentId, string username);
        Task<CandidateAssessment> GetCandidateAssessmentWithItems(CandidateAssessmentParams assessmentParams);
        Task<bool> AddCandidateAssessmentItems(int candidateAssessmentId);
        Task<string> UpdateCandidateAssessmentStatus(int candidateAssessmentId, string username);
        Task<CandidateAssessmentDto> GetCandidateAssessmentDtoWithItems(int candidateid, int orderitemid);
        Task<ChecklistAndCandidateAssessmentDto> SaveNewChecklist (ChecklistHR checklisthr, string Username);
    }
}