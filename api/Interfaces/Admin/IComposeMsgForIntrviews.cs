using api.DTOs.Admin;
using api.Interfaces.Messages;

namespace api.Interfaces.Admin
{
    public interface IComposeMsgForIntrviews
    {
         Task<MessagesWithErrDto> InviteCandidatesForInterviews(ICollection<int> InterviewItemCandidateIds, string username);
    }
}