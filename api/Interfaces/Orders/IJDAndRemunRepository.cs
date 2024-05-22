using api.DTOs.Admin.Orders;
using api.Entities.Admin.Order;

namespace api.Interfaces.Orders
{
    public interface IJDAndRemunRepository
    {
         //Job descriptions
        Task<JobDescription> GetJDOfOrderItem(int OrderItemId);
        Task<JobDescription> AddJobDescription(JobDescription jobDescription);
        Task<bool> EditJobDescription(JobDescription jobDescription);
        Task<bool> DeleteJobDescription (int jobDescriptionId);
        JobDescription CreateNewJobDescription(JobDescription newJD, int OrderItemId);
    // Remuneations
        Task<Remuneration> GetRemuneratinOfOrderItem(int OrderItemId);
        Task<RemunerationDto> GetRemunerationOfOrderItem(int OrderItemId);
        Task<Remuneration> AddRemuneration(Remuneration remuneration);
        Task<bool> EditRemuneration(Remuneration remuneration);
        Task<bool> DeleteRemuneration (int remunerationId);
        Remuneration CreateNewRemuneration(Remuneration remun, int orderItemId);
    }
}