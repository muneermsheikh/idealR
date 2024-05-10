using api.Entities.Tasks;

namespace api.Interfaces.Admin
{
    public interface ITaskRepository
    {
        AppTask GenerateTaskItem(AppTask task, string Username, 
            string candidateDescription, string taskStatus);
        
        AppTask GenerateAppTask(string taskType, int candidateAssessmentId, DateTime taskDate, 
            string taskOwnername, string assignedToUserName, int orderId, int orderItemId, int orderNo, int appNo, 
            int candidateId, string taskDescription, DateTime completeBy, string taskStatus, string username);
    }
}