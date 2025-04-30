using api.DTOs.Admin;
using api.DTOs.HR;
using api.Entities.Admin;
using api.Entities.HR;
using api.Helpers;
using api.Params.Admin;
using api.Params.HR;

namespace api.Interfaces.HR
{
    public interface IInterviewRepository
    {
         Task<InterviewWithErrDto> GetOrGenerateInterviewR(int OrderNo);
         Task<PagedList<InterviewBriefDto>> GetInterviewPagedList(InterviewParams iParams);
         Task<InterviewWithErrDto> SaveNewInterview(Intervw interview);
         Task<Intervw> UpdateInterviewHeader(Intervw model);
         Task<InterviewItemWithErrDto> SaveNewInterviewItem(IntervwItem interview, string loggedInUsername); //including add new candidates
         Task<bool> DeleteInterview(int InterviewId);
         Task<Intervw> EditInterview(Intervw interview);
         Task<InterviewItemWithErrDto> EditInterviewItem(IntervwItem interviewItem, string Username);
         Task<bool>DeleteInterviewItem(int InterviewItemId);
         Task<PagedList<InterviewAttendanceDto>> GetInterviewAttendancePagedList(AttendanceParams aParams);
         Task<ICollection<InterviewAttendanceToUpdateDto>> UpdateInterviewAttendance(ICollection<InterviewAttendanceToUpdateDto> attendanceDtos);
         Task<ICollection<IntervwAttendance>> GetAttendanceOfACandidate(int interviewCandidateId);
         Task<ICollection<AttendanceStatus>> GetAttendanceStatusData();
         Task<MessagesWithErrDto> ComposeInterviewInvitationMessages(ICollection<int> InterviewCandidateIds, string Username);
         Task<bool> DeleteInterviewItemCandidate(int InterviewCandidateId);
         Task<string> UpdateInterviewCandidateAttachmentFileName(IntervwItemCandidate interviewCandidate);
         Task<ICollection<InterviewMatchingCategoryDto>> GetInterviewBriefMatching(string categoryName);
         Task<bool> SaveNewInterviewItemCandidate(IntervwItemCandidate itemCandidate);
    }
}