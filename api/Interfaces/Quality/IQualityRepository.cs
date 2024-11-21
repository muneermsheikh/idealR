using api.DTOs.Admin;
using api.Helpers;
using api.Params.Objectives;

namespace api.Interfaces.Quality
{
    public interface IQualityRepository
    {
         Task<PagedList<MedicalObjective>> GetMedicalObjectives(MedicalParams medParams);
         Task<PagedList<HRObjective>> GetHRObjectives(MedicalParams medParams);
         Task<bool> SetHRTasksAsCompleted (ICollection<int> hrTaskIds, string Username);
         Task<PagedList<HRObjective>> GetPendingHRTasks(MedicalParams medParams);
         Task<string> AssignTasksToHRExecs(ICollection<int> orderItemIds, string Username);
    }
}