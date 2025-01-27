using System.Data.Common;
using System.Security.Cryptography.Xml;
using api.DTOs.Admin;
using api.DTOs.HR;
using api.Entities.HR;
using api.Entities.Tasks;
using api.Extensions;
using api.Helpers;
using api.Interfaces.Admin;
using api.Interfaces.Messages;
using api.Params.Admin;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace api.Data.Repositories.Admin
{
    public class CVRefRepository : ICVRefRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IComposeMessagesAdminRepository _composeMsgAdmin;
        private readonly IConfiguration _config;
        private readonly ITaskRepository _taskRepo;
        private readonly IQueryableRepository _queryRepo;
        private readonly IMessageRepository _msgRepo;
        readonly int _docControllerAdminAppUserId=0;
        readonly string _docControllerAdminAppUsername= "";
        readonly string _docControllerAdminAppUserEmail= "";
        private readonly ILogger<CVRefRepository> _logger;

        public CVRefRepository(DataContext context, IMapper mapper, IComposeMessagesAdminRepository composeMsgAdmin, ILogger<CVRefRepository> logger,
            IConfiguration config, ITaskRepository taskRepo, IQueryableRepository queryRepo, IMessageRepository msgRepo)
        {
            _logger = logger;
            _msgRepo = msgRepo;
            _queryRepo = queryRepo;
            _taskRepo = taskRepo;
            _config = config;
            _composeMsgAdmin = composeMsgAdmin;
            _mapper = mapper;
            _context = context;
            _docControllerAdminAppUserId=  Convert.ToInt32(_config["DocControllerAdminAppUserId"] ?? "0");
            _docControllerAdminAppUsername=  _config["DocControllerAdminAppUsername"] ?? "";
            _docControllerAdminAppUserEmail= _config["DocControllerAdminAppUserEmail"] ?? "";

        }

        public async Task<bool> DeleteReferral(int CVRefId)
        {
            var cvref = await _context.CVRefs.FindAsync(CVRefId);
            _context.Entry(cvref).State = EntityState.Deleted;
            try {
                await _context.SaveChangesAsync();
            } catch {
                throw new Exception("Failed to delete the CV Referral - check if there are related selection or deployment records");
            }
            return true;
        }
    
        public async Task<bool> EditReferral(CVRef cvref)
        {
            if(!await _context.CVRefIsEditable(cvref.Id)) throw new Exception("The CV Referral is not editable");
            
            _context.Entry(cvref).State = EntityState.Modified;

            try {await _context.SaveChangesAsync();} catch (Exception ex) { throw new Exception(ex.Message, ex);};

            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateException ex) {
                Console.Write("CVRefRepository EditReferral" + " - " + ex.InnerException.Message);
                return false;
            //} catch (SQLITE_CONSTRAINT_UNIQUE)
            } catch (Exception ex) {
                Console.Write(ex.InnerException.Message);
                return false;
            }
            return true;
        }

        /*public async Task<PagedList<CVRefDto>> GetPendingReferrals(CVRefParams refParams)
        {
            var query = (from cvref in _context.CVRefs 
                    where cvref.RefStatus.ToLower() == "referred" && 
                        (cvref.SelectionStatus =="" || cvref.SelectionStatus == null)
                join item in _context.OrderItems on cvref.OrderItemId equals item.Id 
                join o in _context.Orders on item.OrderId equals o.Id
                join cv in _context.Candidates on cvref.CandidateId equals cv.Id 
        
                select new CVRefDto{
                    CVRefId = cvref.Id,
                    Checked = false,
                    CustomerId = o.CustomerId,
                    CustomerName = o.Customer.CustomerName,
                    CandidateId = cvref.CandidateId,
                    CandidateName = cv.FullName,
                    ApplicationNo = cv.ApplicationNo,
                    OrderId = item.OrderId,
                    OrderNo = o.OrderNo,
                    OrderDate = o.OrderDate,
                    OrderItemId = cvref.OrderItemId,
                    ProfessionName = item.Profession.ProfessionName,
                    CategoryRef = o.OrderNo + "-" + item.SrNo,
                    //PPNo = cv.PpNo,
                    ReferredOn = cvref.ReferredOn,
                    RefStatus = cvref.RefStatus
                })
                .AsQueryable();
    
            var paged = await PagedList<CVRefDto>.CreateAsync(
                query.AsNoTracking()
                .ProjectTo<CVRefDto>(_mapper.ConfigurationProvider)
                , refParams.PageNumber, refParams.PageSize);
    

            return paged;
        }
        /*
        
        //flg function deleted, as it is taken care fo GetCVReferrals
        /*public async Task<PagedList<SelPendingDto>> GetCVReferralsPending(CVRefParams refParams)
        {
            var query = (from cvref in _context.CVRefs 
                    //where cvref.SelectionStatus == "" || cvref.SelectionStatus==null
                join item in _context.OrderItems on cvref.OrderItemId equals item.Id 
                join o in _context.Orders on item.OrderId equals o.Id
                join cv in _context.Candidates on cvref.CandidateId equals cv.Id 
        
                select new SelPendingDto{
                    Id = cvref.Id,
                    Checked = false,
                    CvRefId = cvref.Id,
                    OrderId = o.Id,
                    OrderItemId = item.Id,
                    CustomerName = o.Customer.CustomerName,
                    CustomerId = o.CustomerId,
                    CategoryRefAndName = o.OrderNo + "-" + item.SrNo + "-" + item.Profession.ProfessionName,
                    ApplicationNo = cv.ApplicationNo,
                    CandidateName = cv.FullName,
                    ReferredOn = cvref.ReferredOn,
                    SelectionStatus = cvref.SelectionStatus,
                    SelectionStatusDate = cvref.SelectionStatusDate
                }).AsQueryable();

            if(refParams.OrderId > 0) query = query.Where(x => x.OrderId == refParams.OrderId);
            if(refParams.OrderItemId > 0) query = query.Where(x => x.OrderItemId==refParams.OrderItemId);
            if(refParams.CustomerId > 0) query = query.Where(x => x.CustomerId == refParams.CustomerId);
            if(!string.IsNullOrEmpty(refParams.RefStatus) ) {
                if(refParams.SelectionStatus == null) {
                    query = query.Where(x => x.SelectionStatus == null || x.SelectionStatus == "");
                } else {
                    query = query.Where(x => x.SelectionStatus == refParams.SelectionStatus);
                }
            }
           
            
            var paged = await PagedList<SelPendingDto>.CreateAsync(
                query.AsNoTracking()
                //.ProjectTo<SelPendingDto>(_mapper.ConfigurationProvider)
                , refParams.PageNumber, refParams.PageSize);

            return paged;
        }
        */

        public async Task<PagedList<CVRefDto>> GetCVReferrals(CVRefParams refParams)
        {
            var query =(from cvref in _context.CVRefs
                join item in _context.OrderItems on cvref.OrderItemId equals item.Id 
                join cat in _context.Professions on item.ProfessionId equals cat.Id
                join o in _context.Orders on item.OrderId equals o.Id
                join cv in _context.Candidates on cvref.CandidateId equals cv.Id 
        
                select new CVRefDto{
                    CVRefId = cvref.Id,
                    Checked = false,
                    CustomerId = o.CustomerId,
                    CustomerName = o.Customer.CustomerName,
                    CandidateId = cvref.CandidateId,
                    CandidateName = cv.FullName,
                    ApplicationNo = cv.ApplicationNo,
                    OrderId = item.OrderId,
                    OrderNo = o.OrderNo,
                    OrderDate = o.OrderDate,
                    OrderItemId = cvref.OrderItemId,
                    ProfessionName = cat.ProfessionName,
                    CategoryRefAndName = o.OrderNo + "-" + item.SrNo + "-" + cat.ProfessionName,
                    //PPNo = cv.PpNo,
                    ReferredOn = cvref.ReferredOn,
                    RefStatus = cvref.RefStatus,
                    SelectionStatus = cvref.SelectionStatus,
                    SelectionStatusDate = cvref.SelectionStatusDate
                })
                .AsQueryable();
    
            //if(refParams.OrderItemId  > 0) query = query.Where(x => x.OrderItemId == refParams.OrderItemId);
            if(refParams.CustomerId > 0) query = query.Where(x => x.CustomerId == refParams.CustomerId);
            if(refParams.OrderNo > 0) query = query.Where(x => x.OrderNo == refParams.OrderNo);
            if(refParams.OrderId != 0) query = query.Where(x => x.OrderId == refParams.OrderId);
            if(refParams.CandidateId != 0) query = query.Where(x => x.CandidateId == refParams.CandidateId);
            //if(!string.IsNullOrEmpty(refParams.RefStatus)) 
                //query = query.Where(x => x.RefStatus.ToLower() == refParams.RefStatus.ToLower());
            var selStatus = refParams.SelectionStatus ?? "";
            if(!string.IsNullOrEmpty(selStatus))  {
                if(selStatus == "Pending") {
                    query = query.Where(x => x.RefStatus.ToLower()=="referred" 
                        && (x.SelectionStatus==null || x.SelectionStatus==""));
                } else if(selStatus== "Rejected") {
                    query = query.Where(x => x.SelectionStatus.ToLower().Contains("rejected"));
                } else {
                    query = query.Where(x => x.SelectionStatus.ToLower() == refParams.SelectionStatus.ToLower());
                }
            }
                

            /*if(refParams.ProfessionId !=0) {
                var orderItemIds = await _context.OrderItems.
                    Where(x => x.ProfessionId == refParams.ProfessionId)
                    .Select(x => x.Id).ToListAsync();
                if(orderItemIds != null && orderItemIds.Count > 0) {
                    query = query.Where(x => orderItemIds.Contains(x.OrderItemId));
                }
            } */

            /*if(refParams.AgentId != 0) {
                var candidateIds = await _context.Candidates.Where(x => x.CustomerId == refParams.AgentId).Select(x => x.Id).ToListAsync();
                if(candidateIds != null && candidateIds.Count > 0) {
                    query = query.Where(x => candidateIds.Contains(x.CandidateId));
                }
            }*/

            var paged = await PagedList<CVRefDto>.CreateAsync(
                query.AsNoTracking()
                .ProjectTo<CVRefDto>(_mapper.ConfigurationProvider)
                , refParams.PageNumber, refParams.PageSize);

            return paged;
        }

        public async Task<CVRefWithDepDto> GetCVRefWithDeploys(int CVRefId)
        {
            var query =await (from cvref in _context.CVRefs 
                    join item in _context.OrderItems on cvref.OrderItemId equals item.Id 
                    join o in _context.Orders on item.OrderId equals o.Id
                    join cv in _context.Candidates on cvref.CandidateId equals cv.Id 
                    join dep in _context.Deps on cvref.Id equals dep.CvRefId
           
                    select new CVRefWithDepDto{
                        CVRefId = cvref.Id,
                        Checked = false,
                        CustomerId = o.CustomerId,
                        CustomerName = o.Customer.CustomerName,
                        CandidateId = cvref.CandidateId,
                        CandidateName = cv.FullName,
                        ApplicationNo = cv.ApplicationNo,
                        OrderId = item.OrderId,
                        OrderNo = o.OrderNo,
                        OrderDate = o.OrderDate,
                        OrderItemId = cvref.OrderItemId,
                        ProfessionName = item.Profession.ProfessionName,
                        CategoryRef = o.OrderNo + "-" + item.SrNo,
                        //PPNo = cv.PpNo,
                        ReferredOn = cvref.ReferredOn,
                        Deployments = (ICollection<DTOs.Process.DeployDto>)dep
                    })
                    .FirstOrDefaultAsync();

              return query;  
        }

        public async Task<CVRef> GetReferralById(int cvrefid)
        {
            return await _context.CVRefs.FindAsync(cvrefid);
        }

        public async Task<MessageWithError> MakeReferrals(ICollection<int> CandidateAssessmentIds, string Username)
        {
            //todo - implement CVRefRestriction checking
            var dtoToReturn = new MessageWithError();

            var cvrefsAlreadyReferred = await _context.CVRefs.Where(x => CandidateAssessmentIds.Contains(x.CandidateAssessmentId)).ToListAsync();

            if(cvrefsAlreadyReferred.Count > 0) {       //remove the entry from CandidateAsessmentIds and also update so that it is marked as referred and not available to refer further
                foreach(var id in cvrefsAlreadyReferred) {      //candidateAssessment with CVRefId
                    CandidateAssessmentIds.Remove(id.CandidateAssessmentId);
                    var assessment = await _context.CandidateAssessments.FindAsync(id.CandidateAssessmentId);
                    if(assessment != null) {
                        assessment.CVRefId = id.Id;
                        _context.Entry(assessment).State = EntityState.Modified;
                    }
                }
                await _context.SaveChangesAsync();
            }
       
            if(CandidateAssessmentIds.Count == 0) {
                dtoToReturn.ErrorString = "All the CVs have been already referred to clients";
                return dtoToReturn;
            }

            var itemdetails = await (from candAssess in _context.CandidateAssessments 
                    where CandidateAssessmentIds.Contains(candAssess.Id)
                join cand in _context.Candidates on candAssess.CandidateId equals cand.Id
                join i in _context.OrderItems on candAssess.OrderItemId equals i.Id
                join ordr in _context.Orders on i.OrderId equals ordr.Id
                join chklst in _context.ChecklistHRs on 
                    new {candAssess.CandidateId, candAssess.OrderItemId} equals 
                    new {chklst.CandidateId, chklst.OrderItemId}
                select new CandidatesAssessedButNotRefDto {
                    CvRefId = 0,
                    OrderDate = ordr.OrderDate,
                    SrNo = i.SrNo,
                    PPNo = cand.PpNo,
                    CustomerCity = ordr.CityOfWorking,
                    CandidateAssessment = candAssess,
                    OrderItemId=i.Id,
                    OrderItemSrNo = i.SrNo,
                    ProfessionId = i.ProfessionId,
                    OrderId = ordr.Id,
                    OrderNo = ordr.OrderNo,
                    CustomerName = ordr.Customer.CustomerName,
                    CustomerId = ordr.Customer.Id,
                    ProfessionName = i.Profession.ProfessionName,
                    CandidateId = cand.Id,
                    Ecnr = cand.Ecnr,
                    ApplicationNo = cand.ApplicationNo,
                    PassportNo = cand.PpNo,
                    CandidateName = cand.FullName,
                    DocControllerAdminTaskId = candAssess.TaskIdDocControllerAdmin,
                    ChargesAgreed = chklst==null ? 0 : chklst.ChargesAgreed,
                    HRExecUsername = chklst.HrExecUsername,
                    SalaryExpectation = chklst.SalaryExpectation,
                    Grade = candAssess.AssessResult,
                    TaskDescription= "CV approved to send to client: Application No.:" + 
                        cand.ApplicationNo + ", Candidate: " + cand.FullName +
                        "forward to: " +  ordr.Customer.CustomerName + " against requirement " + 
                        ordr.OrderNo + "-" + i.SrNo + "-" + i.Profession.ProfessionName +
                        ", Cleared to send by: " + candAssess.AssessedByEmployeeName + " on " + candAssess.AssessedOn,
                    Candidatedescription = "Application " + cand.ApplicationNo + " - " + cand.FullName + " referred to " +
                        ordr.Customer.CustomerName + " for " + ordr.OrderNo + "-" + i.SrNo + "-" + 
                        i.Profession.ProfessionName + " on " + DateTime.UtcNow
                }).ToListAsync();
            

            DateTime dateTimeNow = DateTime.Now;
            
            var cvrefs = new List<CVRef>();         //collection to compose messages in stage 6
          
            foreach(var q in itemdetails)
            {
                //add the record for cVRef 
                var cvref =new CVRef{
                    CandidateAssessmentId = q.CandidateAssessment.Id,
                    OrderItemId=q.OrderItemId, CandidateId = q.CandidateId, 
                    CustomerId =  q.CustomerId, ReferredOn = dateTimeNow, 
                    HRExecUsername = q.HRExecUsername, RefStatus = "Referred",
                    RefStatusDate = DateTime.UtcNow
                };
                
                _context.Entry(cvref).State= EntityState.Added;
                cvrefs.Add(cvref);
            }
           
            bool isSaved = false;
            do
                {
                    try
                    {
                        await _context.SaveChangesAsync();
                        isSaved = true;
                    }
                    catch (DbUpdateException ex)
                    {
                        foreach (var entry in ex.Entries) {
                            Console.Write("CVRefRepository.MakeReferrals Exception - " + ex.InnerException.Message);

                            entry.State = EntityState.Detached; // Remove from context so won't try saving again.
                            dtoToReturn.ErrorString += ex.Message;
                        }
                    }
                }
            while (!isSaved);

            if(!isSaved) return dtoToReturn;
            
            dtoToReturn.Notification = await TasksPostCVRef(Username, itemdetails);
            
            return dtoToReturn;
        }
        
        public async Task<int> UpdateCandidateAssessmentWithCVRefId()
        {
            var candAssessments = await (from assessment in _context.CandidateAssessments where assessment.CVRefId == 0
                join cvref in _context.CVRefs on new {assessment.CandidateId, assessment.OrderItemId}
                    equals new {cvref.CandidateId, cvref.OrderItemId}
                select new {assessment, cvrefid=cvref.Id}).ToListAsync();
            
            if(candAssessments.Count > 0) {
                foreach(var candAssess in candAssessments){
                    var assess = candAssess.assessment;
                    assess.CVRefId = candAssess.cvrefid;
                    _context.Entry(assess).State= EntityState.Modified;
                }

                return await _context.SaveChangesAsync();
            }

            return 0;

        }
        private async Task<string> TasksPostCVRef(string Username, ICollection<CandidatesAssessedButNotRefDto> candidatesNotRefDto)
        {
            //candidatesNotRefDto - CVs selected by user to forward to clients
            //1 - update candidateAssessment.CVRefId value
            //2 - mark DocControllerAdminTasks as completed
            //3 - create cvfwdtask - DocController to register CVRef in the system
            //4 - create task to Doc Controller to send the email to the client
            //5 - create selectionTasks
            //6 - compose cv forward message
            //7 - Update HR Exec Task concerning tasks to HR Exec

            var dateTimeNow = DateTime.UtcNow;
            string ErrorString ="";     //candidateAssessments are already present in MakeReferrals, consider using that 
                    //data, instead of retrieving from the dB as done below

            var query = await (from candAssess in _context.CandidateAssessments where 
                    candidatesNotRefDto.Select(x => x.CandidateAssessment.Id).ToList()
                    .Contains(candAssess.Id)
                join cvref in _context.CVRefs on 
                    new {candId=candAssess.CandidateId, orderitemid=candAssess.OrderItemId}
                    equals new {candId=cvref.CandidateId, orderitemid=cvref.OrderItemId}
                select new{candidateassessment=candAssess, cvrefid=cvref.Id}
            ).ToListAsync();
            
            //1 - update candidateAssessment.CVRefId value
            foreach(var item in query) {
                var candassess=item.candidateassessment;
                candassess.CVRefId = item.cvrefid;
                _context.Entry(candassess).State=EntityState.Modified;

            }
            
            //2 - mark DocControllerAdminTasks as completed
                var docControllerAdmTaskIds = candidatesNotRefDto.Select(x => x.DocControllerAdminTaskId).ToList();
                foreach(var id in docControllerAdmTaskIds) {
                    if(id != 0) {
                        var task = await _context.Tasks.Include(x => x.TaskItems).Where(x => x.Id==id).FirstOrDefaultAsync();
                        if(task != null) {
                            task.TaskStatus = "Completed";
                            task.CompletedOn = DateTime.UtcNow;
                        
                            task.TaskItems.Add(new TaskItem{AppTaskId=id, 
                                TransactionDate=DateTime.UtcNow,
                                TaskItemDescription="Task Completed", UserName=Username,
                                TaskStatus=task.TaskStatus});
                            _context.Entry(task).State = EntityState.Modified;
                        }
                    }
                }
                
            //create various tasks
            string categoryDescription="";
            
            //7 - HRExecTask
            var tasksHRExec = await _context.HRTasks.Where(x => 
                candidatesNotRefDto.Select(x => x.OrderItemId).ToList().Contains(x.OrderItemId))
                .ToListAsync();

            foreach(var item in candidatesNotRefDto) 
            {
                //3 - create cvfwdtask - DocController to register CVRef in the system
                categoryDescription ="Candidate-" + await _context.GetCandidateDescriptionFromCandidateId(item.CandidateId) + 
                        " refer to " + await _context.GetOrderItemDescriptionFromOrderItemId(item.OrderItemId);
                
                var cvrefid=query.Where(x => x.candidateassessment.Id == item.CandidateAssessment.Id)
                    .FirstOrDefault()?.cvrefid;

                var cvfwdTask = new AppTask{
                    TaskDate=DateTime.UtcNow, 
                    CVRefId = cvrefid,
                    AssignedToUsername=_docControllerAdminAppUsername, 
                    AssignedByUsername = Username, 
                    CompleteBy=DateTime.UtcNow.AddDays(5), 
                    TaskDescription= "Forward CV to customer through the system - " + categoryDescription, 
                    TaskType="CVFwdTask", 
                    TaskStatus="Not Started", 
                    OrderId=item.OrderId, OrderItemId=item.OrderItemId,
                    CandidateId = item.CandidateId, CandidateAssessmentId = item.CandidateAssessment.Id,
                    OrderNo=item.OrderNo, ApplicationNo=item.ApplicationNo, PostTaskAction="Do not auto-send message",
                    TaskItems = new List<TaskItem>(){
                        new() { TaskItemDescription="Refer CVs to customer in the system", 
                        NextFollowupByName = _docControllerAdminAppUsername, TaskStatus="Not Started" }
                     }
                };
                _context.Entry(cvfwdTask).State = EntityState.Added;
            
                //4 - create task to Doc Controller to send the email to the client
                var cvrefTask = new AppTask{TaskDate=DateTime.UtcNow, 
                    AssignedToUsername=_docControllerAdminAppUsername,
                    AssignedByUsername = Username, CompleteBy=DateTime.UtcNow.AddDays(5), 
                    TaskDescription= "Send CVs to clients by email -" + categoryDescription, 
                    CandidateAssessmentId = item.CandidateAssessment.Id,CVRefId=cvrefid,
                    TaskType="CVRefByMail", TaskStatus="Not Started", ApplicationNo=item.ApplicationNo,
                    CandidateId=item.CandidateId, OrderItemId=item.OrderItemId, OrderId = item.OrderId, 
                    OrderNo=item.OrderNo, PostTaskAction="Do not auto-send message",
                    TaskItems = new List<TaskItem>(){
                        new() { TaskItemDescription="Refer CVs to customer by email", 
                        NextFollowupByName = _docControllerAdminAppUsername, TaskStatus="Not Started" }
                    }};
                _context.Entry(cvrefTask).State = EntityState.Added;
           
                 //5 - create selectionTasks
                var selTask = new AppTask{TaskDate=DateTime.UtcNow, 
                    AssignedToUsername=_docControllerAdminAppUsername, CVRefId = cvrefid,
                    AssignedByUsername = Username, CompleteBy= DateTime.UtcNow.AddDays(5), 
                    TaskDescription= "Follow up with clients for selection-" + categoryDescription, 
                    CandidateAssessmentId = item.CandidateAssessment.Id, OrderId = item.OrderId,
                    TaskType="SelectionFollowupWithClient", TaskStatus="Not Started", 
                    CandidateId = item.CandidateId, OrderItemId=item.OrderItemId, OrderNo=item.OrderNo,
                    ApplicationNo=item.ApplicationNo, PostTaskAction="Do not auto-send message",
                    TaskItems = new List<TaskItem>(){
                    new() { TaskItemDescription="Follow up with clients for selection", 
                        NextFollowupByName = _docControllerAdminAppUsername, TaskStatus="Not Started"}
                    }};
                       
                var hrtask = tasksHRExec.Where(x => x.OrderItemId == item.OrderItemId
                    && x.AssignedToUsername == item.HRExecUsername).FirstOrDefault();

                var hritem = new HRTaskItem{TransactionDate=DateTime.UtcNow, 
                    ApplicationNo=item.ApplicationNo, CandidateId=item.CandidateId,
                    HRExecutiveUsername=item.HRExecUsername, CandidateAssessmentId=item.CandidateAssessment.Id,
                    CVRefId=item.CvRefId,
                    Remarks="Candidate " + item.ApplicationNo + "-" + item.CandidateName +
                    "- for " + item.CustomerName + " " + item.CustomerCity + " forwarded today."};

                if(hrtask != null)
                {
                    hritem.HRTaskId=hrtask.Id;
                    _context.HRTaskItems.Add(hritem); 
                } else {
                    hrtask = new HRTask {TaskDate=DateTime.UtcNow, AssignedByUsername = Username, 
                        AssignedToUsername=Username, CompleteBy=DateTime.UtcNow.AddDays(5), OrderId=item.OrderId,
                        OrderItemId=item.OrderItemId, OrderNo = item.OrderNo, 
                        QntyAssigned = await _context.OrderItems.Where(x => x.Id==item.OrderItemId)
                            .Select(x => x.Quantity).FirstOrDefaultAsync(), TaskStatus = "In Process",
                        TaskDescription = "New HR Executive Assignment Task Created for " + Username + 
                            " since the User does not have a task assigned to him. ",
                        HRTaskItems = new List<HRTaskItem>{hritem}};
                    _context.HRTasks.Add(hrtask);                 
                    //**TODO** raise alert logged in HR Executive has not been assigned this task    
                }
            }
            
            //UPDATE HRTasks.QntyDelivered
            var ids = tasksHRExec.Select(x => x.Id).ToList();
            var taskToUpdate = await _context.HRTasks.Include(x => x.HRTaskItems)
                .Where(x => ids.Contains(x.Id))
                .ToListAsync(); 

            foreach(var t in taskToUpdate) {
                t.QntyDelivered = t.HRTaskItems.Count;
                t.TaskStatusDate = DateTime.UtcNow;
                t.TaskStatus = t.QntyDelivered >= t.QntyDelivered ? "Completed" : "In Process";
                _context.Entry(t).State = EntityState.Modified;
            }

            try {
                await _context.SaveChangesAsync();  //this is required, as flg function reads from tasks generated
            } catch (DbException ex) {
                if(ex.Message.Contains("IX_HRTasks_OrderItemId_AssignedToUsername")
                    || ex.Message.Contains("The duplicate key value is")) {
                    ErrorString = "Unique Index key (Tasks_OrderItemId + Username) violation";
                } else {
                    ErrorString += ex.Message;
                }
            } catch (Exception ex) {
                ErrorString += ex.Message;
                _logger.LogError(ex.StackTrace);
            }
            if(!string.IsNullOrEmpty(ErrorString)) return ErrorString;
                        
            //6 - compose cv forward message
            var msgWithErr = await _msgRepo.GenerateMessageForCVForward(candidatesNotRefDto, "CVFwdToCustomer", Username);
            //above function also saves the msg object to database.
            if(!string.IsNullOrEmpty(msgWithErr.ErrorString)) {
                ErrorString += msgWithErr.ErrorString;
            } else {
                foreach(var msg in msgWithErr.Messages) {_context.Entry(msg).State = EntityState.Added;}
                var ct = await _context.SaveChangesAsync();
            }
            
            return ErrorString;
        }
            
        public async Task<CVRefDto> GetCVRefDto(int CVRefId)
        {
            var query = await (from cvref in _context.CVRefs where cvref.Id == CVRefId 
                join item in _context.OrderItems on cvref.OrderItemId equals item.Id 
                join o in _context.Orders on item.OrderId equals o.Id
                join cv in _context.Candidates on cvref.CandidateId equals cv.Id 
        
                select new CVRefDto{
                    CVRefId = cvref.Id,
                    Checked = false,
                    CustomerId = o.CustomerId,
                    CustomerName = o.Customer.CustomerName,
                    CandidateId = cvref.CandidateId,
                    CandidateName = cv.FullName,
                    ApplicationNo = cv.ApplicationNo,
                    OrderId = item.OrderId,
                    OrderNo = o.OrderNo,
                    OrderDate = o.OrderDate,
                    OrderItemId = cvref.OrderItemId,
                    ProfessionName = item.Profession.ProfessionName,
                    CategoryRefAndName = o.OrderNo + "-" + item.SrNo,
                    //PPNo = cv.PpNo,
                    ReferredOn = cvref.ReferredOn,
                    RefStatus = cvref.RefStatus
                }).FirstOrDefaultAsync();
                
            return query;
        }

        public async Task<string> ComposeSelectionDecisionReminderMessage(int CustomerId, string username)
        {
            var msg = await _composeMsgAdmin.ComposeSelDecisionRemindersToClient(CustomerId, username);
            if(msg == null) return "Failed to compose the message";
            
            _context.Entry(msg).State = EntityState.Added;

            return await _context.SaveChangesAsync() > 0 ? "" : "Message composed, but failed to save it";
        }

        public async Task<ICollection<ProspectiveHeaderDto>> GetCVReferredOrderNoHeaders(string status)
        {
            var dto = new List<ProspectiveHeaderDto>();

            dto = status.ToLower() switch
            {
                "pending" => await _context.CVRefs.Where(x => x.SelectionStatus == null)
                        .Select(x => new ProspectiveHeaderDto
                        {
                            Orderno = x.CategoryRef.Substring(0, 6)
                        })
                    .Distinct()
                    .ToListAsync(),
                _ => await _context.CVRefs.Where(x => x.SelectionStatus.StartsWith(status))
                        .Select(x => new ProspectiveHeaderDto
                        {
                            Orderno = x.CategoryRef.Substring(0, 6)
                        })
                    .Distinct()
                    .ToListAsync(),
            };
            return dto;
        }
    }
}

