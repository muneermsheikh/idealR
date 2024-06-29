using api.DTOs.Process;
using api.Entities.Admin;
using api.Entities.Deployments;
using api.Helpers;
using api.Params.Deployments;

namespace api.Interfaces.Deployments
{
    public interface IDeploymentRepository
    {
        Task<PagedList<DeploymentPendingDto>> GetDeployments(DeployParams depParams);
        Task<Dep> GetDeploymentByCVRefId(int cvrefid);
        Task<Dep> GetDeploymentByDepId(int depid);
        Task<DepPendingDtoWithErr> AddDeploymentItems(ICollection<DepItemToAddDto> deptemsToAddDto, string Username);
        Task<DepPendingDtoWithErr> InsertDepItemsWithFlights(ICollection<DepItemWithFlightDto> depItemsWithFlight, string username);
        Task<string> EditDeployment (Dep dep);
        Task<string> EditDepItem (DepItem depItem);
        Task<string> DeleteDepItem (int deployItemId);
        Task<string> DeleteDep(int depId);
        Task<ICollection<DeployStatus>> GetDeploymentStatusData();
        Task<ICollection<DeployStatusAndName>> GetDeploymentSeqAndStatus();
        Task<DepItem> GetNextDepItemToAddFomCVRefId(int cvRefId);
        Task<ICollection<FlightDetail>> GetFlightData();
        Task<string> EditCandidateFlight(CandidateFlight candidateFlight);
        Task<string> DeleteCandidateFlight(int CandidateFlightId);
        Task<CandidateFlight> GetCandidateFlightFromCVRefId(int cvRefId);
        
    }
}