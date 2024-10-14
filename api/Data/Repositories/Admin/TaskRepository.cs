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
            var query = _context.Tasks.Include(x => x.TaskItems).AsQueryable();
            
            if(taskParams.TaskId > 0) {
                query = query.Where(x => x.Id == taskParams.TaskId);
            } else {
                if(!string.IsNullOrEmpty(taskParams.AssignedToUserName)) query = query.Where(x => x.AssignedToUsername.ToLower() == taskParams.AssignedToUserName.ToLower());
                if(!string.IsNullOrEmpty(taskParams.AssignedByUsername)) query = query.Where(x => x.AssignedByUsername.ToLower() == taskParams.AssignedByUsername.ToLower());
                if(!string.IsNullOrEmpty(taskParams.TaskStatus)) query = query.Where(x => x.TaskStatus.ToLower() == taskParams.TaskStatus.ToLower());
                if(!string.IsNullOrEmpty(taskParams.TaskType)) query = query.Where(x => x.TaskType.ToLower() == taskParams.TaskType.ToLower());
                if(!string.IsNullOrEmpty(taskParams.ResumeId)) query = query.Where(x => x.ResumeId == taskParams.ResumeId);
                if(taskParams.ApplicationNo !=0) query = query.Where(x => x.ApplicationNo == taskParams.ApplicationNo);
                if(taskParams.candidateId != 0) query = query.Where(x => x.CandidateId == taskParams.candidateId);
                if(taskParams.TaskDate.Year > 2000) query = query.Where(x => x.TaskDate == taskParams.TaskDate);
            }
            
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
                AssignedByUsername = taskOwnername,
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
                join aq in _context.OrderAssessmentItems on i.Id equals aq.OrderItemId into assessQDesigned 
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
                AssignedByUsername = Username, OrderId = orderid, OrderNo = oBriefItems[0].OrderNo, 
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
                join jobDescription in _context.JobDescriptions on item.Id equals jobDescription.OrderItemId into jobDes
                    from jd in jobDes.DefaultIfEmpty()
                join remuneration in _context.Remunerations on item.Id equals remuneration.OrderItemId into remunern
                    from remun in remunern.DefaultIfEmpty()
                select new OrderItemBriefDto
                {
                    OrderId = order.Id, CustomerId = order.CustomerId, CustomerName = order.Customer.CustomerName,
                    AboutEmployer = order.Customer.Introduction, OrderNo = order.OrderNo,
                    OrderDate = order.OrderDate, OrderItemId = item.Id, RequireInternalReview = rvw.RequireAssess,
                    SrNo = item.SrNo, ProfessionId = item.ProfessionId,
                    ProfessionName = item.Profession.ProfessionName, Quantity = item.Quantity, Ecnr = item.Ecnr,
                    CompleteBefore = item.CompleteBefore, Status = item.Status, 
                    HrExecUsername = rvw.HrExecUsername, JobDescription=jd, Remuneration = remun
                    
                }).ToListAsync();    
            
            if(assignments.Count==0) return "Failed to retrieve any records";

               //create task in the name of HRExecUsername
               
               var tasks = new List<AppTask>();
               var task = new AppTask();

                foreach(var t in assignments)
                {
                    var recipientObj= await _userManager.FindByNameAsync(t.HrExecUsername);
                    if(recipientObj == null) continue;
                    if (t.CompleteBefore.Year < 2000) t.CompleteBefore = _today.AddDays(7);
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
                        TaskDescription="Assignment to source suitable CVs: Category Ref: " + 
                        t.OrderNo + "-" + t.SrNo + "-" + t.ProfessionName +
                        " for " + t.CustomerName, AssignedByUsername = recipientObj.UserName, TaskStatus = "Not started",
                        TaskItems = taskitems, TaskType = "AssignTaskToHRExec"};

                    _context.Entry(apptask).State = EntityState.Added;
                }
                
                var msgs = await _msgHRRepo.ComposeMessagesToHRExecToSourceCVs(assignments, Username);

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

            var ret =  await _context.SaveChangesAsync() > 0 ? "": "Failed to delete the task";
            return ret;
        }

        public AppTask GenerateTask(AppTask task)
        {
            throw new NotImplementedException();
        }

        public Task<string> saveTask(AppTask task)
        {
            throw new NotImplementedException();
        }

        public async Task<AppTask> EditTask(AppTask newObject)
        {
            var existing = await _context.Tasks.Include(x => x.TaskItems).Where(x => x.Id == newObject.Id).FirstOrDefaultAsync();

            if(existing==null) return null;

            _context.Entry(existing).CurrentValues.SetValues(newObject);

             //delete records in existingObject that are not present in new object
            foreach (var existingItem in existing.TaskItems.ToList())
            {
                if(!newObject.TaskItems.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                {
                    _context.TaskItems.Remove(existingItem);
                    _context.Entry(existingItem).State = EntityState.Deleted; 
                }
            }

            foreach(var newItem in newObject.TaskItems)
            {
                var existingItem = existing.TaskItems
                    .Where(c => c.Id == newItem.Id && c.Id != default(int)).SingleOrDefault();
                
                if(existingItem != null)    //update navigation record
                {
                    _context.Entry(existingItem).CurrentValues.SetValues(newItem);
                    _context.Entry(existingItem).State = EntityState.Modified;
                } else {    //insert new TaskItem
                    var itemToInsert = new TaskItem
                    {
                        AppTaskId = existing.Id,
                        TransactionDate=newItem.TransactionDate,
                        TaskStatus = newItem.TaskStatus,
                        TaskItemDescription = newItem.TaskItemDescription,
                        UserName=newItem.UserName,
                        NextFollowupOn=newItem.NextFollowupOn,
                        NextFollowupByName=newItem.NextFollowupByName
                    };

                    existing.TaskItems.Add(itemToInsert);
                    _context.Entry(itemToInsert).State = EntityState.Added;
                }
            
            }

            _context.Entry(existing).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
                return existing;
            } catch (Exception ex) {
                throw new Exception(ex.Message, ex);
            }

        }

        public async Task<PagedList<TaskInBriefDto>> GetPagedList(TaskParams taskParams)
        {
            var query = _context.Tasks.OrderByDescending(x => x.TaskDate).AsQueryable();

            var temp = await query.ToListAsync();
            
            if(!string.IsNullOrEmpty(taskParams.AssignedToUserName) 
                && !string.IsNullOrEmpty(taskParams.AssignedByUsername)) {
                    query = query.Where(x => x.AssignedToUsername.ToLower() == taskParams.AssignedToUserName.ToLower() 
                        || x.AssignedByUsername.ToLower() == taskParams.AssignedByUsername.ToLower());
            } else if(!string.IsNullOrEmpty(taskParams.AssignedToUserName) && (string.IsNullOrEmpty(taskParams.AssignedByUsername))) {
                query = query.Where(x => x.AssignedToUsername.ToLower() == taskParams.AssignedToUserName.ToLower());
            } else if(!string.IsNullOrEmpty(taskParams.AssignedByUsername) && (string.IsNullOrEmpty(taskParams.AssignedToUserName)) ) {
                query = query.Where(x => x.AssignedByUsername.ToLower() == taskParams.AssignedByUsername.ToLower());
            }
                
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
                    AssignedByUsername = username,
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