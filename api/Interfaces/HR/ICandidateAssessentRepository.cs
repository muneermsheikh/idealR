using api.DTOs.HR;
using api.Entities.HR;
using api.Helpers;
using api.Params.HR;

namespace api.Interfaces.HR
{
    public interface ICandidateAssessentRepository
    {
         
        
        Task<CandidateAssessment> GenerateCandidateAssessment(int candidateid, int orderItemId, string Username);
        Task<CandidateAssessment> SaveCandidateAssessment(CandidateAssessment candidateAssessment, string Username);
        Task<string> EditCandidateAssessment(CandidateAssessment candidateAssessment, string username);
        Task<bool> DeleteCandidateAssessment(int id);
        Task<bool> DeleteCandidateAssessmentItem(int id);
        Task<PagedList<CandidateAssessmentDto>> GetCandidateAssessments(CandidateAssessmentParams assessmentParams);
        Task<CandidateAssessment> GetCandidateAssessment(CandidateAssessmentParams assessmentParams);
        Task<CandidateAssessment> GetCandidateAssessmentWithItems(CandidateAssessmentParams assessmentParams);
        Task<bool> AddCandidateAssessmentItems(int candidateAssessmentId);
        Task<string> UpdateCandidateAssessmentStatus(int candidateAssessmentId, string username);
    }
}