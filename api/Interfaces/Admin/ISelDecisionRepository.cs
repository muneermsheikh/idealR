using api.DTOs.Admin;
using api.Entities.HR;
using api.Entities.Identity;
using api.Entities.Messages;
using api.Helpers;
using api.Interfaces.Messages;
using api.Params.Admin;

namespace api.Interfaces.Admin
{
    public interface ISelDecisionRepository
    {
         Task<MessageWithError> RegisterSelections(ICollection<CreateSelDecisionDto> selDtos, string Username);
         Task<SelectionDecision> GetSelectionDecisionFromCVRefId(int cvrefid);
         Task<SelDecisionDto> GetSelDecisionDtoFromId(int selDecisionId);
         Task<PagedList<EmploymentDto>> GetEmploymentsPaged (EmploymentParams empParams);
         Task<PagedList<EmploymentsNotConcludedDto>> EmploymentsAwaitingConclusion(EmploymentParams empParams);
         Task<Employment> GetEmployment(int employmentId);
         Task<bool> EditSelection(SelectionDecision selDecision);
         //Task<bool> EditEmployment (Employment employment, string Username);

         Task<string> DeleteSelection(int selectionsId);
         Task<PagedList<SelDecisionDto>> GetSelectionDecisions (SelDecisionParams selParams);
         Task<string> ComposeAcceptanceReminderToCandidates(List<int> cvrefids, string Username);
         Task<MessagesWithErrDto> ComposeSelMessagesToCandidates(List<int> cvrefids, string Username);
         Task<MessageWithError> ComposeRejMessagesToCandidates(List<int> cvrefids, string Username);
         Task<string> HousekeepingCVRefAndSelDeecisions();
         Task<AppUser> AppUserFromCandidateId(int CandidateId);
    }
}