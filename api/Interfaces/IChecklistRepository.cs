using api.DTOs.HR;
using api.Entities.HR;
using api.Entities.Master;
using api.Helpers;

namespace api.Interfaces
{
    public interface IChecklistRepository
    {
        Task<ChecklistDtoObj> GetOrGenerateChecklist(int candidateId, int orderItemId, string Username);
        Task<ChecklistHR> GetChecklist(int candidateId, int orderItemId);
        Task<bool> DeleteChecklistHRDataAsync(ChecklistHRData checklistHRData);
        Task<bool> EditChecklistHRDataAsync(ChecklistHRData checklistHRData);
        Task<ICollection<ChecklistHRData>> GetChecklistHRDataListAsync();
        Task<ChecklistObj> SaveNewChecklist (ChecklistHR checklisthr, string Username);

        //checklistHR
        Task<string> EditChecklistHR(ChecklistHRDto model, string Username);
        Task<bool> DeleteChecklistHR(int id);
        Task<ChecklistHRDto> GetChecklistHRFromCandidateIdAndOrderDetailId(int candidateid, int orderitemid, string username);
        Task<ICollection<ChecklistHR>> GetChecklistHROfCandidate(int candidateid);

    }
}