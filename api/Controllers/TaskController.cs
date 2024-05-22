using api.DTOs.Admin;
using api.DTOs.Admin.Orders;
using api.Entities.Identity;
using api.Entities.Messages;
using api.Entities.Tasks;
using api.Errors;
using api.Extensions;
using api.Helpers;
using api.Interfaces.Admin;
using api.Interfaces.Orders;
using api.Params;
using api.Params.Admin;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    public class TaskController : BaseApiController
    {
        private readonly ITaskRepository _taskRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly IComposeMessagesHRRepository _msgHRRepo;
        public TaskController(ITaskRepository taskRepo, UserManager<AppUser> userManager, IComposeMessagesHRRepository msgHRRepo)
        {
            _msgHRRepo = msgHRRepo;
            _userManager = userManager;
            _taskRepo = taskRepo;
        }

        [HttpGet("taskbyid/{taskid}")]
        public async Task<ActionResult<AppTask>> GetTaskByParams(int taskid)
        {
            var uParams = new TaskParams {TaskId = taskid};
            var task = await _taskRepo.GetTaskByParams(uParams);
            if(task==null) return BadRequest(new ApiException(400, "Bad Request", "failed to retrieve the task object"));

            return Ok(task);
        }

         [HttpGet("task/{orderid}/{tasktype}")]
        public async Task<ActionResult<AppTask>> GetTaskByParams(int orderid, string tasktype)
        {
            var uParams = new TaskParams {OrderId = orderid, TaskType=tasktype};
            var task = await _taskRepo.GetTaskByParams(uParams);
            if(task==null) return BadRequest(new ApiException(400, "Bad Request", "failed to retrieve the task object"));

            return Ok(task);
        }


        [HttpGet("pagedTasks")]
        public async Task<ActionResult<PagedList<TaskInBriefDto>>> OpenOrderItemCategories(TaskParams taskParams)
        {
            var pagedList = await _taskRepo.GetPagedList(taskParams);

            if(pagedList.Count ==0) return BadRequest("No order items on record matching the criteria");

            Response.AddPaginationHeader(new PaginationHeader(pagedList.CurrentPage, 
                pagedList.PageSize, pagedList.TotalCount, pagedList.TotalPages));
            
            return Ok(pagedList);
            
        }

       //assign task to HR Sup or HR Manager to design AssessmentQ for the 
        //order, if the flag RequireAssess is set to true
        [HttpPost("design/{orderId}")]
        public async Task<ActionResult<Message>> AssignTaskToDesignOrderAssessmentQ(int orderId)
        {
            var msg = await  _msgHRRepo.ComposeEmailToDesignOrderItemAssessmentQs(orderId, User.GetUsername());
            
            if (msg != null) return Ok(msg);

            return BadRequest(new ApiException (404, "Failed to create tasks for the HR Executives"));
        }

        [HttpDelete("task/{taskid}")]
        public async Task<ActionResult<string>> DeleteTask(int taskid)
        {
            return await _taskRepo.DeleteTask(taskid);
        }

        [HttpDelete("taskitem/{taskitemid}")]
        public async Task<ActionResult<string>> DeleteTaskItem(int taskitemid)
        {
            return await _taskRepo.DeleteTaskItem(taskitemid);
        }

        [HttpPost("assignToHRExecs")]
        public async Task<ActionResult<string>> AssignHRExecTasks(ICollection<OrderItemIdAndHRExecEmpNoDto> dto)
        {
            var strErr = await _taskRepo.AssignTasksToHRExecs(dto, User.GetUsername());
            
            if(string.IsNullOrEmpty(strErr)) return BadRequest(new ApiException(404, "failed to create tasks", strErr));

            return "";
        }

              
        [HttpGet("generateTask")]
        public AppTask GenerateTask(AppTask task)
        {
            var generated =  _taskRepo.GenerateTask(task);

            return generated;
        }

        [HttpPost]
        public async Task<ActionResult<AppTask>> SaveTask(AppTask task)
        {
            var err = await _taskRepo.saveTask(task);

            if(string.IsNullOrEmpty(err)) return Ok("");

            return BadRequest(new ApiException(400, "Bad Request", err));
        }

        [HttpPut("task")]
        public async Task<ActionResult<string>> EditTask(AppTask task)
        {
            var err = await _taskRepo.EditTask(task);

            return err;
        }

        [HttpPut("completeTask/{id}")]
        public async Task<ActionResult<string>> MarkTaskAsCompleted(int id)
        {
            var err = await _taskRepo.MarkTaskAsCompleted(id);

            if(!string.IsNullOrEmpty(err)) return BadRequest(new ApiException(400, "Bad Request", err));

            return Ok("updated");
        }

        [HttpGet("prospectivetaskForResumeId/{resumeid}")]
        public async Task<ActionResult<AppTask>> GetTaskForResumeId(string resumeid, string assignedToUsername)
        {
            return await _taskRepo.GetOrGenertateTaskForResumeId(
                resumeid, User.GetUsername(), assignedToUsername);
        }
    }
}