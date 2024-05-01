using api.DTOs.HR;
using api.Entities.HR;
using api.Helpers;
using api.Params.HR;

namespace api.Interfaces
{
    public interface ICandidateRepository
    {
         
         Task<PagedList<CandidateBriefDto>> GetCandidates(CandidateParams candidateParams);
         Task<Candidate> GetCandidate(CandidateParams candidateParams);
         Task<bool> UpdateCandidate(Candidate candidate);
         Task<bool> DeleteCandidate(int Id);
         Task<bool> InsertCandidate(Candidate candidate);
    }
}