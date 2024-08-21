using api.DTOs.HR;
using api.Entities.Admin;
using api.Entities.HR;
using api.Helpers;
using api.Params.HR;

namespace api.Interfaces.HR
{
    public interface IInterviewRepository
    {
         Task<InterviewWithErrDto> GetOrGenerateInterviewR(int OrderNo);
         Task<PagedList<InterviewBriefDto>> GetInterviewPagedList(InterviewParams iParams);
         Task<InterviewWithErrDto> SaveNewInterview(Intervw interview);
         Task<InterviewItemWithErrDto> SaveNewInterviewItem(IntervwItem interview);
         Task<bool> DeleteInterview(int InterviewId);
         Task<Intervw> EditInterview(Intervw interview);
         Task<InterviewItemWithErrDto> EditInterviewItem(IntervwItem interviewItem);
    }
}