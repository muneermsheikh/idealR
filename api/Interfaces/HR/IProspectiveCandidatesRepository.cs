using api.DTOs;
using api.DTOs.HR;
using api.Entities.HR;
using api.Helpers;
using api.Params.Admin;
using api.Params.HR;

namespace api.Interfaces.HR
{
    public interface IProspectiveCandidatesRepository
    {
        Task<PagedList<ProspectiveBriefDto>> GetProspectivePagedList(CallRecordParams pParams);
        Task<UserDto> ConvertProspectiveToCandidate(ProspectiveCandidateAddDto dto);
        Task<ICollection<ProspectiveSummaryDto>> GetProspectiveSummary(ProspectiveSummaryParams pParams);
        Task<ProspectiveCandidate> GetProspectiveCandidate(ProspectiveSummaryParams pParams);
    }
}