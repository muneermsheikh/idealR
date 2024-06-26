using api.DTOs.Admin;
using api.DTOs.Admin.Orders;
using api.Entities.Identity;
using api.Entities.Tasks;
using api.Helpers;
using api.Interfaces.Admin;
using api.Interfaces.Orders;
using api.Params.Admin;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Repositories.Admin
{
    public class TaskRepository: ITaskRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;
        private readonly UserManager<AppUser> _userManager;
        private readonly IComposeMessagesHRRepository _msgHRRepo;
        private readonly IComposeMessagesAdminRepository _admnMsgRepo;
        private readonly DateTime _today = DateTime.Now;
        private readonly IMapper _mapper;
        public TaskRepository(DataContext context, IConfiguration config, IComposeMessagesHRRepository msgHRRepo, IComposeMessagesAdminRepository admnMsgRepo,
            UserManager<AppUser> userManager, IMapper mapper)
        {
            _mapper = mapper;
            _admnMsgRepo = admnMsgRepo;
            _msgHRRepo = msgHRRepo;
            _userManager = userManager;
            _config = config;
            _context = context;
        }

        public async Task<AppTask> GetTaskByParams(TaskParams taskParams)
        {
            var query = _context.Tasks.AsQueryable();
            
            if(!string.IsNullOrEmpty(taskParams.AssignedToUserName)) query = query.Where(x => x.AssignedToUsername.ToLower() == taskParams.AssignedToUserName.ToLower());
            if(!string.IsNullOrEmpty(taskParams.TaskOwnerUsername)) query = query.Where(x => x.TaskOwnerUsername.ToLower() == taskParams.TaskOwnerUsername.ToLower());
            if(!string.IsNullOrEmpty(taskParams.TaskStatus)) query = query.Where(x => x.TaskStatus.ToLower() == taskParams.TaskStatus.ToLower());
            if(!string.IsNullOrEmpty(taskParams.TaskType)) query = query.Where(x => x.TaskType.ToLower() == taskParams.TaskType.ToLower());
            if(!string.IsNullOrEmpty(taskParams.ResumeId)) query = query.Where(x => x.ResumeId == taskParams.ResumeId);
            if(taskParams.ApplicationNo !=0) query = query.Where(x => x.ApplicationNo == taskParams.ApplicationNo);
            if(taskParams.candidateId != 0) query = query.Where(x => x.CandidateId == taskParams.candidateId);
            if(taskParams.TaskDate.Year > 2000) query = query.Where(x => x.TaskDate == taskParams.TaskDate);

            var task = await query.FirstOrDefaultAsync();

            return task;
        }

        public  AppTask GenerateTaskItem(AppTask task, string Username, 
            string candidateDescription, string taskStatus)
        {
            
            task.TaskStatus = "Completed";
            task.CompletedOn= DateTime.UtcNow;
            var taskitem = new TaskItem {
                AppTaskId = task.Id, TransactionDate =_today,
                TaskItemDescription = (taskStatus=="Completed" ? "task completed for: " : "new task for") + candidateDescription,
                UserName = Username,
                NextFollowupOn =  _today.AddDays(5)
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
            var taskitem = new TaskItem{TransactionDate = _today,
                TaskItemDescription = "Completed task: " + taskDescription, UserName = username,
                NextFollowupOn =  _today.AddDays(5)};
            taskitemList.Add(taskitem);

            var newTask = new AppTask{
                TaskType = taskType,
                CandidateAssessmentId = candidateAssessmentId,
                TaskDate = taskDate,
                TaskOwnerUsername = taskOwnername,
                AssignedToUsername = assignedToUserName,
                OrderId = orderId, OrderNo = orderNo,
                OrderItemId = orderItemId,
                ApplicationNo = appNo,
                CandidateId = candidateId,
                TaskDescription = taskDescription,
                CompleteBy = completeBy,
                TaskStatus = taskStatus,
                TaskItems = taskitemList
            };

                return newTask;
        }
    
     //creates Task, updates DL.ForwardedToHRDeptHead, composes email msg to HRDeptHead and saves all to DB
        public async Task<AppTask> NewDLTaskForHRDept(int orderid, string Username)
        {
            //CHECK FOR THE CONSTRAINTS ORDERID + TASKTYPEID=14 UNIQUE CONSTRAINT
            var taskdate = await _context.Tasks
                .Where(x => x.TaskType.ToLower()=="orderfwdtohr" && x.OrderId==orderid)
                .FirstOrDefaultAsync();
            if (taskdate!=null) return null;

            var oBriefItems = await (from o in _context.Orders where o.Id == orderid 
                join i in _context.OrderItems on o.Id equals i.OrderId
                join aq in _context.orderItemAssessments on i.Id equals aq.OrderItemId into assessQDesigned 
                from aqDesigned in assessQDesigned.DefaultIfEmpty()
                select new OrderItemForwardDto{
                        Id = o.Id, OrderItemId = i.Id, //RequireInternalReview = i.RequireInternalReview,
                        OrderId = o.Id, OrderNo = o.OrderNo, OrderDate=o.OrderDate,
                        CustomerName = o.Customer.CustomerName, AboutEmployer= o.Customer.Introduction,
                        ProfessionId = i.ProfessionId, ProfessionName= i.Profession.ProfessionName, 
                        CategoryRef=o.OrderNo + "-" + i.SrNo + "-" + i.Profession.ProfessionName,
                        Quantity = i.Quantity, Status = o.Status, JobDescription = i.JobDescription.JobDescInBrief,
                        Remuneration =  i.Remuneration.SalaryCurrency + " " + i.Remuneration.SalaryMin + 
                        i.Remuneration.SalaryMax,  AssessmentQDesigned = aqDesigned != null})
                .ToListAsync();
    
            var taskDesc = "Order No. " + oBriefItems[0].OrderNo + " dated " + oBriefItems[0].OrderDate +
                " from " + oBriefItems[0].CustomerName + " is assigned to you for execution within the period " +
                "allotted.  You may assign tasks to suitable HR Executives depending upon their skill sets " +
                "through the system";
            var t = new AppTask{
                TaskType="OrderFwdToHR", TaskDate = _today, AssignedToUsername = _config["HRSupUsername"] ?? "admin",
                TaskOwnerUsername = Username, OrderId = orderid, OrderNo = oBriefItems[0].OrderNo, 
                TaskDescription = taskDesc, CompleteBy = _today.AddDays(7), 
                PostTaskAction = "OnlyComposeEmailAndSMSMessages" };
            
            _context.Tasks.Add(t);

            var msgs = await _admnMsgRepo.ForwardEnquiryToHRDept(await _context.Orders.FindAsync(orderid));
            if(msgs!=null) {
                //_context.Entry(msgs).State = EntityState.Added;
                _context.Messages.Add(msgs);
            }

            //update Orders.ForwaredToRDeptOn table
            var ord = await _context.Orders.Where(x => x.Id==orderid).FirstOrDefaultAsync();
            ord!.ForwardedToHRDeptOn=_today;
            _context.Entry(ord).State = EntityState.Modified;

            if (await _context.SaveChangesAsync() > 0) return t;

            return null;
            
        }

        public async Task<string> DesignOrderItemAssessmentQAndCreateTask(int orderid, string Username)
        {
            var msgs = await _msgHRRepo.ComposeEmailToDesignOrderItemAssessmentQs(orderid, Username);

            if(!string.IsNullOrEmpty(msgs.ErrorString)) return msgs.ErrorString;

            _context.Entry(msgs).State = EntityState.Added;

            return await _context.SaveChangesAsync() > 0 ? "" : "Failed to save the email message";

        }

        public async Task<string> AssignTasksToHRExecs(ICollection<int> orderItemIds, string Username)
        {
            var assignments = await (from item in _context.OrderItems where orderItemIds.Contains(item.Id)
                join order in _context.Orders on item.OrderId equals order.Id
                join rvw in _context.ContractReviewItems on item.Id equals rvw.OrderItemId
                select new OrderAssignmentDto
                {
                    CategoryRef = order.OrderNo + "-" + item.SrNo + "-" + item.Profession.ProfessionName,
                    CityOfWorking = order.CityOfWorking, CompleteBy = item.CompleteBefore, 
                    CustomerId = order.CustomerId, CustomerName = order.Customer.CustomerName,
                    HrExecUsername = rvw.HrExecUsername, OrderDate = order.OrderDate, OrderId = order.Id,
                    OrderItemId = item.Id, OrderNo = order.OrderNo, ProfessionId = item.ProfessionId,
                    ProfessionName = item.Profession.ProfessionName, ProjectManagerId = order.ProjectManagerId,
                    Quantity = item.Quantity
                }).ToListAsync();    

               var tasks = new List<AppTask>();
               var task = new AppTask();

                foreach(var t in assignments)
                {
                    var recipientObj= await _userManager.FindByNameAsync(t.HrExecUsername);
                    
                    if (t.CompleteBy.Year < 2000) t.CompleteBy = _today.AddDays(7);
                    var taskitems = new List<TaskItem>
                    {
                        new() {
                            TransactionDate = _today,
                            TaskItemDescription = "",
                            UserName = Username,
                            NextFollowupOn = _today.AddDays(5)
                        }
                    };

                    var apptask = new AppTask{TaskDate=_today, AssignedToUsername = recipientObj.UserName,
                        CompleteBy=_today.AddDays(4), OrderId=t.OrderId, OrderItemId=t.OrderItemId,
                        TaskDescription="Assignment to source suitable CVs: Category Ref: " + t.CategoryRef +
                        " for " + t.CustomerName, TaskOwnerUsername = recipientObj.UserName, TaskStatus = "Not started",
                        TaskItems = taskitems, TaskType = "AssignTaskToHRExec"};

                    _context.Entry(apptask).State = EntityState.Added;
                }
                
                var msgs = await _msgHRRepo.ComposeMessagesToHRExecToSourceCVs((ICollection<OrderItemIdAndHRExecEmpNoDto>)assignments, Username);

                foreach(var msg in msgs.Messages) {
                    _context.Entry(msg).State = EntityState.Added;
                }

                try{
                    await _context.SaveChangesAsync();
                } catch (Exception ex) {
                    throw new Exception(ex.Message);
                }

                return await _context.SaveChangesAsync() > 0 ? "" : "Failed to save the tasks and message to the database";
        }

        public async Task<string> DeleteTask(int taskId)
        {
            var task = await _context.Tasks.FindAsync(taskId);

            if(task == null) return "The task is not on record";
            _context.Tasks.Remove(task);

            _context.Entry(task).State = EntityState.Deleted;

            return await _context.SaveChangesAsync() > 0 ? "": "Failed to delete the task";
        }

        public AppTask GenerateTask(AppTask task)
        {
            throw new NotImplementedException();
        }

        public Task<string> saveTask(AppTask task)
        {
            throw new NotImplementedException();
        }

        public async Task<string> EditTask(AppTask task)
        {
            var existing = await _context.Tasks.FindAsync(task.Id);

            if(existing==null) return "No such task on record";

            _context.Entry(existing).CurrentValues.SetValues(task);

            return await _context.SaveChangesAsync() > 0 ? "" : "Failed to update the task";
        }

        public async Task<PagedList<TaskInBriefDto>> GetPagedList(TaskParams taskParams)
        {
            var query = _context.Tasks.AsQueryable();

            if(!string.IsNullOrEmpty(taskParams.AssignedToUserName)) query = query.Where(x => x.AssignedToUsername.ToLower() == taskParams.AssignedToUserName.ToLower());
            if(!string.IsNullOrEmpty(taskParams.TaskOwnerUsername)) query = query.Where(x => x.TaskOwnerUsername.ToLower() == taskParams.TaskOwnerUsername.ToLower());
            if(!string.IsNullOrEmpty(taskParams.TaskStatus)) query = query.Where(x => x.TaskStatus.ToLower() == taskParams.TaskStatus.ToLower());
            if(!string.IsNullOrEmpty(taskParams.TaskType)) query = query.Where(x => x.TaskType.ToLower() == taskParams.TaskType.ToLower());
            if(!string.IsNullOrEmpty(taskParams.ResumeId)) query = query.Where(x => x.ResumeId == taskParams.ResumeId);
            if(taskParams.ApplicationNo !=0) query = query.Where(x => x.ApplicationNo == taskParams.ApplicationNo);
            if(taskParams.candidateId != 0) query = query.Where(x => x.CandidateId == taskParams.candidateId);
            if(taskParams.TaskDate.Year > 2000) query = query.Where(x => x.TaskDate == taskParams.TaskDate);

            
            var paged = await PagedList<TaskInBriefDto>.CreateAsync(query.AsNoTracking()
                .ProjectTo<TaskInBriefDto>(_mapper.ConfigurationProvider),
                taskParams.PageNumber, taskParams.PageSize);
            
            return paged;
        }

        public async Task<string> MarkTaskAsCompleted(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if(task==null) return "invalid task id";

            task.TaskStatus = "Completed";
            task.CompletedOn=DateTime.UtcNow;

            _context.Entry(task).State = EntityState.Modified;

            return await _context.SaveChangesAsync() > 0 ? "" : "Failed to mark the task as completed";


        }

        public async Task<AppTask> GetOrGenertateTaskForResumeId(string resumeId, string username, string assignedToUsername)
        {
            var task = await _context.Tasks.Include(x => x.TaskItems)
                .Where(x => x.ResumeId == resumeId && x.TaskType == "Prospective")
                .FirstOrDefaultAsync();
            
            task ??= new AppTask{
                    TaskDate = _today,
                    AssignedToUsername = assignedToUsername,
                    TaskOwnerUsername = username,
                    TaskDescription = "Conclude interest and availability of this Resume",
                    ResumeId = resumeId, TaskType = "PortalTask", TaskStatus = "Not Started",
                    CompleteBy = _today.AddDays(2)
                };

            return task;
        }

        public async Task<string> DeleteTaskItem(int taskitemid)
        {
            var obj = await _context.TaskItems.FindAsync(taskitemid);
            if(obj == null) return "Failed to Delete the Task Item";

            _context.TaskItems.Remove(obj);
            _context.Entry(obj).State = EntityState.Deleted;

            return await _context.SaveChangesAsync() > 0 ? "" : "Failed to delete the task item";
        }
    }
}