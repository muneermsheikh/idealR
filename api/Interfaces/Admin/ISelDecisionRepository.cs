using api.DTOs.Admin;
using api.Entities.HR;
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
         Task<Employment> GetEmploymentFromSelDecId(int selDecisionId);
         Task<bool> EditSelection(SelectionDecision selDecision);
         //Task<bool> EditEmployment (Employment employment, string Username);

         Task<string> DeleteSelection(int selectionId);
         Task<PagedList<SelDecisionDto>> GetSelectionDecisions (SelDecisionParams selParams);
         Task<ICollection<Message>> ComposeSelMessagesToCandidates(List<int> cvrefids, string Username);
         Task<MessageWithError> ComposeRejMessagesToCandidates(List<int> cvrefids, string Username);
         
    }
}