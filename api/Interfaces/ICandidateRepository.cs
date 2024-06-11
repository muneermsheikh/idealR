using api.DTOs;
using api.DTOs.HR;
using api.Entities.HR;
using api.Helpers;
using api.Params.HR;

namespace api.Interfaces
{
    public interface ICandidateRepository
    {
         
         Task<PagedList<CandidateBriefDto>> GetCandidates(CandidateParams candidateParams);
         Task<PagedList<cvsAvailableDto>> GetAvailableCandidates(CandidateParams candidateParams);
         Task<CandidateBriefDto> GetCandidateBriefFromParams(CandidateParams candParams);
         Task<Candidate> GetCandidate(CandidateParams candidateParams);
         Task<Candidate> GetCandidateById(int candidateid);
         Task<bool> UpdateCandidate(Candidate candidate);
         Task<bool> DeleteCandidate(int Id);
         Task<bool> InsertCandidate(Candidate candidate);
         Task<bool> CheckPPExists(string PPNo);
         Task<bool> AadharNoExists(string aadharNo);
         Task<Candidate> CreateCandidateAsync(RegisterDto registerDto, string Username);
         Task<int> GetApplicationNoFromCandidateId(int candidateId);
         Task<int> GetAppUserIdOfCandidate(int candidateid);

         //attachments
         Task<ICollection<UserAttachment>> AddAndSaveUserAttachments(ICollection<UserAttachment> userAttachments, string username);
         Task<bool> DeleteUserAttachment(int userAttachmentId);
         Task<ICollection<UserAttachment>> UpdateCandidateAttachments(ICollection<UserAttachment> userAttachments);
         //Task<bool> UpdateCandidateAttachmentsWithFileNames (ICollection<string> filenames, int candidateId);
         Task<UserAttachment> GetUserAttachmentById (int attachmentId);
         Task<ICollection<UserAttachment>> GetUserAttachmentByCandidateId (int candidateid);
    }
}