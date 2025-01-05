using api.DTOs.Admin;
using api.Interfaces.Messages;

namespace api.Interfaces.Admin
{
    public interface IComposeMsgForIntrviews
    {
        Task<MessagesWithErrDto> InviteCandidatesForInterviews(ICollection<int> InterviewItemCandidateIds, string username);
        Task<MessagesWithErrDto> EditInviteForInterviews(ICollection<int> InterviewItemCandidateIds, string username);
        Task<MessagesWithErrDto> ComposeEmailsForEmploymentInterest(CallRecordCandidateAdviseDto Dtos, string username);
    }
}