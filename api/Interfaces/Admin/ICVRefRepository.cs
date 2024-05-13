using api.DTOs;
using api.DTOs.Admin;
using api.Entities.HR;
using api.Entities.Messages;
using api.Helpers;
using api.Interfaces.Messages;
using api.Params.Admin;

namespace api.Interfaces.Admin
{
    public interface ICVRefRepository
    {
        Task<PagedList<CVRefDto>> GetCVReferrals(CVRefParams refParams);
        Task<PagedList<CVRefDto>> GetPendingReferrals(CVRefParams refParams);
        Task<CVRefWithDepDto> GetCVRefWithDeploys(int CVRefId);
        Task<MessageWithError> MakeReferrals (ICollection<int> CVReviewIds, string Username);
        Task<bool> EditReferral (CVRef cvref);
        Task<bool> DeleteReferral (int CVRefId);
        Task<CVRef> GetCVRef (int CVRefId);
        Task<ICollection<Message>> ComposeSelectionDecisionReminderMessage(ICollection<int> cvRefIds, string username);
        Task<int> UpdateCandidateAssessmentWithCVRefId();
    }
}