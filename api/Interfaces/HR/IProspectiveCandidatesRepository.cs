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
        Task<PagedList<ProspectiveBriefDto>> GetProspectivePagedList(ProspectiveCandidateParams pParams);
        Task<int> ConvertProspectiveToCandidate(int prospectiveid, string Username);
        Task<ICollection<ProspectiveSummaryDto>> GetProspectiveSummary(ProspectiveSummaryParams pParams);
        Task<ProspectiveCandidate> GetProspectiveCandidate(ProspectiveSummaryParams pParams);
        Task<bool> DeleteProspectiveCandidate(int ProspectiveId);
    }
}