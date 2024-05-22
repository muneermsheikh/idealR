using api.DTOs.Process;
using api.Entities.Deployments;
using api.Helpers;
using api.Params.Deployments;

namespace api.Interfaces.Deployments
{
    public interface IDeploymentRepository
    {
        Task<PagedList<DeploymentPendingDto>> GetDeployments(DeployParams depParams);
        Task<Dep> GetDeploymentByCVRefId(int cvrefid);
        Task<string> AddDeploymentTransactions(ICollection<DepItemToAddDto> deptemsToAddDto, string Username);
        Task<string> EditDeployment (Dep dep);
        Task<string> EditDepItem (DepItem depItem);
        Task<string> DeleteDepItem (int deployItemId);
        Task<string> DeleteDep(int depId);
        Task<ICollection<DeployStatus>> GetDeploymentStatusData();
        
    }
}