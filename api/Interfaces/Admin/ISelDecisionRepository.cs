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
         Task<bool> EditSelection(SelectionDecision selDecision);
         Task<bool> EditEmployment (Employment employment, string Username);
         Task<bool> DeleteSelection(int selectionId);
         Task<PagedList<SelDecisionDto>> GetSelectionDecisions (SelDecisionParams selParams);
         Task<ICollection<Message>> ComposeSelMessagesToCandidates(List<int> cvrefids, string Username);
         Task<ICollection<Message>> ComposeRejMessagesToCandidates(List<int> cvrefids, string Username);
         
    }
}