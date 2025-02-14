using api.DTOs;
using api.DTOs.HR;
using api.Entities.Admin;
using api.Entities.HR;
using api.Entities.Messages;
using api.Helpers;
using api.HR.DTOs;
using api.Params.Admin;
using api.Params.HR;

namespace api.Interfaces.HR
{
    public interface IProspectiveCandidatesRepository
    {
        Task<PagedList<ProspectiveBriefDto>> GetProspectivePagedList(ProspectiveCandidateParams pParams);
        Task<ProspectiveReturnDto> ConvertProspectiveToCandidate(int prospectiveid, string KnownAs, string Email, string Username);
        //Task<ICollection<ProspectiveReturnDto>> ConvertProspectiveToCandidates(ICollection<int> prospectiveids, string Username);
        Task<bool> ConvertProspectiveNoDeleteFromProspective(ICollection<int> ProspectiveCandidateIds, string Username);
        Task<ICollection<ProspectiveSummaryDto>> GetProspectiveSummary(ProspectiveSummaryParams pParams);
        Task<ProspectiveCandidate> GetProspectiveCandidate(ProspectiveSummaryParams pParams);
        Task<bool> DeleteProspectiveCandidate(int ProspectiveId);
        Task<ICollection<ProspectiveHeaderDto>> GetProspectiveHeaders(string status);
        Task<ICollection<ProspectiveBriefDto>> GetProspectiveList(string orderno, string status);
        Task<ICollection<ComposeCallRecordMessageDto>> ComposeCallRecordMessages(ICollection<ComposeCallRecordMessageDto> dtos, string loggedInUsername);
        Task<bool> InsertAudioFiles(ICollection<AudioMessage> audioMessages);
        Task<PagedList<AudioMessageDto>> GetAudioMessagePagedList(AudioMessageParams audioParams);
        Task<bool> SetAudioText(SetAudioText audioText);
    }
}