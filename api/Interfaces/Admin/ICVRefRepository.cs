using api.DTOs;
using api.DTOs.Admin;
using api.DTOs.HR;
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
        //Task<PagedList<SelPendingDto>> GetCVReferralsPending(CVRefParams refParams);
        //Task<PagedList<CVRefDto>> GetPendingReferrals(CVRefParams refParams);
        Task<CVRefWithDepDto> GetCVRefWithDeploys(int CVRefId);
        Task<MessageWithError> MakeReferrals (ICollection<int> CVReviewIds, string Username);
        Task<bool> EditReferral (CVRef cvref);
        Task<bool> DeleteReferral (int CVRefId);
        Task<CVRefDto> GetCVRefDto (int CVRefId);
        Task<string> ComposeSelectionDecisionReminderMessage(int customerId, string username);
        Task<int> UpdateCandidateAssessmentWithCVRefId();
        Task<ICollection<ProspectiveHeaderDto>> GetCVReferredOrderNoHeaders(string status);
        
    }
}