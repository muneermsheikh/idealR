using api.Entities.Tasks;
using api.Interfaces.Admin;

namespace api.Data.Repositories.Admin
{
    public class TaskRepository: ITaskRepository
    {
        public TaskRepository()
        {
        }


        public  AppTask GenerateTaskItem(AppTask task, string Username, 
            string candidateDescription, string taskStatus)
        {
            
            task.TaskStatus = "Completed";
            task.CompletedOn=DateTime.UtcNow;
            var taskitem = new TaskItem {
                AppTaskId = task.Id, TransactionDate = DateTime.UtcNow,
                TaskItemDescription = (taskStatus=="Completed" ? "task completed for: " : "new task for") + candidateDescription,
                UserName = Username,
                NextFollowupOn =  DateTime.UtcNow.AddDays(5)
            };
            var taskitems = new List<TaskItem>{taskitem};
            task.TaskItems = taskitems;

            return task;
        }

        public  AppTask GenerateAppTask(string taskType, int candidateAssessmentId, DateTime taskDate, 
            string taskOwnername, string assignedToUserName, int orderId, int orderItemId, int orderNo, int appNo, 
            int candidateId, string taskDescription, DateTime completeBy, string taskStatus, string username) 
        {
            
            var taskitemList = new List<TaskItem>();
            var taskitem = new TaskItem{TransactionDate = DateTime.UtcNow, 
                TaskItemDescription = "Completed task: " + taskDescription, UserName = username,
                NextFollowupOn =  DateTime.UtcNow.AddDays(5)};
            taskitemList.Add(taskitem);

            var newTask = new AppTask{
                TaskType = taskType,
                CandidateAssessmentId = candidateAssessmentId,
                TaskDate = DateOnly.FromDateTime(taskDate),
                TaskOwnerUsername = taskOwnername,
                AssignedToUsername = assignedToUserName,
                OrderId = orderId, OrderNo = orderNo,
                OrderItemId = orderItemId,
                ApplicationNo = appNo,
                CandidateId = candidateId,
                TaskDescription = taskDescription,
                CompleteBy = DateOnly.FromDateTime(completeBy),
                TaskStatus = taskStatus,
                TaskItems = taskitemList
            };

                return newTask;
        }
    
    }
}