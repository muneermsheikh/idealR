using api.DTOs.Admin;
using api.DTOs.Admin.Orders;
using api.Entities.Messages;
using api.Entities.Tasks;
using api.Helpers;
using api.Params.Admin;
using api.Params.Objectives;

namespace api.Interfaces.Admin
{
    public interface ITaskRepository
    {
        Task<AppTask> GetTaskByParams(TaskParams taskParams);
        AppTask GenerateTask(AppTask task);
        Task<string>saveTask(AppTask task);
        Task<AppTask>EditTask(AppTask task);
        Task<string> MarkTaskAsCompleted(int id);
        Task<string> DeleteTask(int TaskId);
        Task<string> DeleteTaskItem(int taskitemid);
        Task<PagedList<TaskInBriefDto>> GetPagedList(TaskParams taskParams);
        AppTask GenerateTaskItem(AppTask task, string Username, 
            string candidateDescription, string taskStatus);
        
        AppTask GenerateAppTask(string taskType, int candidateAssessmentId, DateTime taskDate, 
            string taskOwnername, string assignedToUserName, int orderId, int orderItemId, int orderNo, int appNo, 
            int candidateId, string taskDescription, DateTime completeBy, string taskStatus, string username);
        Task<string> DesignOrderItemAssessmentQAndCreateTask(int orderid, string Username);
        Task<string> AssignTasksToHRExecs(ICollection<int> orderItemIds, string Username);
        Task<AppTask> GetOrGenertateTaskForResumeId(string ResumeId, string username, string assignedToUsername);

        Task<PagedList<MedicalObjective>> GetMedicalObjectives(string datefrom, string dateupto);
    }
}