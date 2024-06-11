using api.DTOs.Admin;
using api.Entities.HR;
using api.Entities.Tasks;
using api.Extensions;
using api.Helpers;
using api.Interfaces.Admin;
using api.Interfaces.Messages;
using api.Params.Admin;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

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

        public CVRefRepository(DataContext context, IMapper mapper, IComposeMessagesAdminRepository composeMsgAdmin, 
            IConfiguration config, ITaskRepository taskRepo, IQueryableRepository queryRepo, IMessageRepository msgRepo)
        {
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

        public async Task<PagedList<CVRefDto>> GetPendingReferrals(CVRefParams refParams)
        {
            var query = (from cvref in _context.CVRefs 
                    where cvref.RefStatus.ToLower() == "referred" && cvref.SelectionStatus ==""
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

         public async Task<PagedList<SelPendingDto>> GetCVReferralsPending(CVRefParams refParams)
        {
            var query = (from cvref in _context.CVRefs 
                    where cvref.RefStatus=="Referred" && cvref.SelectionStatus != "Selected"
                join item in _context.OrderItems on cvref.OrderItemId equals item.Id 
                join o in _context.Orders on item.OrderId equals o.Id
                join cv in _context.Candidates on cvref.CandidateId equals cv.Id 
        
                select new SelPendingDto{
                    Id = cvref.Id,
                    Checked = false,
                    CvRefId = cvref.Id,
                    OrderItemId = item.Id,
                    CustomerName = o.Customer.CustomerName,
                    CategoryRefAndName = o.OrderNo + "-" + item.SrNo + "-" + item.Profession.ProfessionName,
                    ApplicationNo = cv.ApplicationNo,
                    CandidateName = cv.FullName,
                    ReferredOn = cvref.ReferredOn
                }).AsQueryable();

    
            var paged = await PagedList<SelPendingDto>.CreateAsync(
                query.AsNoTracking()
                //.ProjectTo<SelPendingDto>(_mapper.ConfigurationProvider)
                , refParams.PageNumber, refParams.PageSize);

            return paged;
        }

        public async Task<PagedList<CVRefDto>> GetCVReferrals(CVRefParams refParams)
        {
            var query =(from cvref in _context.CVRefs
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
    
            if(refParams.OrderItemId  > 0) query = query.Where(x => x.OrderItemId == refParams.OrderItemId);
            if(refParams.CustomerId > 0) query = query.Where(x => x.CustomerId == refParams.CustomerId);
            if(refParams.CandidateId != 0) query = query.Where(x => x.CandidateId == refParams.CandidateId);
            if(!string.IsNullOrEmpty(refParams.RefStatus)) 
                query = query.Where(x => x.RefStatus.ToLower() == refParams.RefStatus.ToLower());
            if(!string.IsNullOrEmpty(refParams.SelectionStatus)) 
                query = query.Where(x => x.SelectionStatus.ToLower() == refParams.SelectionStatus.ToLower());

            if(refParams.ProfessionId !=0) {
                var orderItemIds = await _context.OrderItems.
                    Where(x => x.ProfessionId == refParams.ProfessionId)
                    .Select(x => x.Id).ToListAsync();
                if(orderItemIds != null && orderItemIds.Count > 0) {
                    query = query.Where(x => orderItemIds.Contains(x.OrderItemId));
                }
            }

            if(refParams.AgentId != 0) {
                var candidateIds = await _context.Candidates.Where(x => x.CustomerId == refParams.AgentId).Select(x => x.Id).ToListAsync();
                if(candidateIds != null && candidateIds.Count > 0) {
                    query = query.Where(x => candidateIds.Contains(x.CandidateId));
                }
            }

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
                    join dep in _context.Processes on cvref.Id equals dep.CVRefId
           
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

        private async Task<ICollection<CandidatesAssessedButNotRefDto>> CandidatesAssessedButNotReferred(ICollection<CandidateAssessment> shortlistedCVsNotReferred)
        {
            var itemdetails = await (from r in _context.CandidateAssessments where shortlistedCVsNotReferred
                    .Select(x => x.Id).ToList().Contains(r.Id) 
                join i in _context.OrderItems on r.OrderItemId equals i.Id 
                join ordr in _context.Orders on i.OrderId equals ordr.Id 
                join cand in _context.Candidates on r.CandidateId equals cand.Id
                join lst in _context.ChecklistHRs on new {a=cand.Id, b=i.Id} 
                    equals new {a=lst.CandidateId, b=lst.OrderItemId} into chklst
                from checklist in chklst.DefaultIfEmpty()
                select new CandidatesAssessedButNotRefDto {
                        CandidateAssessment = r,
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
                        DocControllerAdminTaskId = r.TaskIdDocControllerAdmin,
                        ChargesAgreed = checklist==null ? 0 : checklist.ChargesAgreed,
                        HRExecUsername = checklist.HrExecUsername,
                        TaskDescription= "CV approved to send to client: Application No.:" + 
                            cand.ApplicationNo + ", Candidate: " + cand.FullName +
                            "forward to: " +  ordr.Customer.CustomerName + " against requirement " + 
                            ordr.OrderNo + "-" + i.SrNo + "-" + i.Profession.ProfessionName +
                            ", Cleared to send by: " + r.AssessedByEmployeeName + " on " + r.AssessedOn,
                        Candidatedescription = "Application " + cand.ApplicationNo + " - " + cand.FullName + " referred to " +
                            ordr.Customer.CustomerName + " for " + ordr.OrderNo + "-" + i.SrNo + "-" + 
                            i.Profession.ProfessionName + " on " + DateTime.UtcNow
                    }
            ).ToListAsync();
            
            return itemdetails;
        }

        public async Task<MessageWithError> MakeReferrals(ICollection<int> CandidateAssessmentIds, string Username)
        {
            //todo - implement CVRefRestriction checking
            var dtoToReturn = new MessageWithError();

            DateTime dateTimeNow = DateTime.Now;
            
            var cvrefs = new List<CVRef>();         //collection to compose messages in stage 6
            
            var assessments = await _context.CandidateAssessments.Where(x => CandidateAssessmentIds.Contains(x.Id))
                .ToListAsync();
 
            //assessed and shortlisted CVs, but not referred to client
            var shortlistedCVsNotReferred = await _context.CandidateAssessments
                .Where(x => CandidateAssessmentIds.Contains(x.Id) && x.CVRefId == 0 ).ToListAsync();
            if (shortlistedCVsNotReferred.Count == 0) {
                dtoToReturn.ErrorString = "All candidates are already referred";;
                return dtoToReturn;
            }
            
            var ids = await _context.CVRefs.Where(x => shortlistedCVsNotReferred.Select(m => m.CVRefId).ToList().Contains(x.Id)).Select(x => x.Id).ToListAsync();
            
            foreach(var id in ids) {
                var itemToRemove = shortlistedCVsNotReferred.Where(x => x.CVRefId == id).FirstOrDefault();
                shortlistedCVsNotReferred.Remove(itemToRemove);
            }
            
            if (shortlistedCVsNotReferred == null || shortlistedCVsNotReferred.Count == 0)  {
                dtoToReturn.ErrorString ="no data available to create CV Referrals";
                return dtoToReturn;
            }

            //extract data for writing to CVRef tables
            var itemdetails = await CandidatesAssessedButNotReferred(shortlistedCVsNotReferred);
            if (itemdetails.Count==0) {
                dtoToReturn.ErrorString= "Failed to retrieve relevant data to create CV Referras";
                return dtoToReturn;
            }
            
            foreach(var q in itemdetails)
            {
                //add the record for cVRef 
                var cvref =new CVRef{
                    CandidateAssessmentId = q.CandidateAssessment.Id,
                    OrderItemId=q.OrderItemId, CandidateId = q.CandidateId, 
                    CustomerId =  q.CustomerId, ReferredOn = dateTimeNow, 
                    HRExecUsername = q.HRExecUsername, RefStatus = "Referred",
                    RefStatusDate = DateOnly.FromDateTime(DateTime.UtcNow)
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
        
        //1 - When the candidate wa shortlisted in CandidateAssessment, a task was created in the name of
        //    DocControllerAdmin to refer it to the client - CVRef.  Mark this task as completed, no
        //    that the CV Referral has been done

        //2 - create task in the name of DocControllerAdmin to email the CVs 
        //3 - Create task in the name of DocControllerAdmin to follow up with customers for selection
        //4 - Update CandidateAssessment.CVRefId
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
            var dateTimeNow = DateTime.UtcNow;
            string ErrorString ="";

        
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
                            task.TaskItems.Add(new TaskItem{AppTaskId=id, 
                                TransactionDate=DateTime.UtcNow,
                                TaskItemDescription="Task Completed", UserName=Username});
                            _context.Entry(task).State = EntityState.Modified;
                        }
                    }
                }
                
            //create various tasks
            string categoryDescription="";
            foreach(var item in candidatesNotRefDto) {
                //3 - create cvfwdtask - DocController to register CVRef in the system
                categoryDescription ="Candidate-" + await _context.GetCandidateDescriptionFromCandidateId(item.CandidateId) + 
                        " refer to " + await _context.GetOrderItemDescriptionFromOrderItemId(item.OrderItemId);
                var cvrefid=query.Where(x => x.candidateassessment.Id == item.CandidateAssessment.Id).FirstOrDefault().cvrefid;

                var cvfwdTask = new AppTask{
                    TaskDate=DateTime.UtcNow, 
                    CVRefId = cvrefid,
                    AssignedToUsername=_docControllerAdminAppUsername, 
                    TaskOwnerUsername = Username, 
                    CompleteBy=DateTime.UtcNow.AddDays(5), 
                    TaskDescription= "Forward CV to customer through the system - " + categoryDescription, 
                    TaskType="CVFwdTask", 
                    TaskStatus="Not Started", 
                    OrderId=item.OrderId, OrderItemId=item.OrderItemId,
                    CandidateId = item.CandidateId, CandidateAssessmentId = item.CandidateAssessment.Id,
                    OrderNo=item.OrderNo, ApplicationNo=item.ApplicationNo, PostTaskAction="Do not auto-send message",
                    TaskItems = new List<TaskItem>(){
                        new() { TaskItemDescription="Refer CVs to customer in the system", NextFollowupByName = _docControllerAdminAppUsername }
                     }
                };
                _context.Entry(cvfwdTask).State = EntityState.Added;

                //4 - 
                var cvrefTask = new AppTask{TaskDate=DateTime.UtcNow, 
                    AssignedToUsername=_docControllerAdminAppUsername,
                    TaskOwnerUsername = Username, CompleteBy=DateTime.UtcNow.AddDays(5), 
                    TaskDescription= "Send CVs to clients by email -" + categoryDescription, 
                    CandidateAssessmentId = item.CandidateAssessment.Id,CVRefId=cvrefid,
                    TaskType="CVRefByMail", TaskStatus="Not Started", ApplicationNo=item.ApplicationNo,
                    CandidateId=item.CandidateId, OrderItemId=item.OrderItemId, OrderId = item.OrderId, 
                    OrderNo=item.OrderNo, PostTaskAction="Do not auto-send message",
                    TaskItems = new List<TaskItem>(){
                        new() { TaskItemDescription="Refer CVs to customer by email", NextFollowupByName = _docControllerAdminAppUsername }
                    }};
                _context.Entry(cvrefTask).State = EntityState.Added;

                 //5 - create selectionTasks
                  var selTask = new AppTask{TaskDate=DateTime.UtcNow, 
                    AssignedToUsername=_docControllerAdminAppUsername, CVRefId = cvrefid,
                    TaskOwnerUsername = Username, CompleteBy= DateTime.UtcNow.AddDays(5), 
                    TaskDescription= "Follow up with clients for selection-" + categoryDescription, 
                    CandidateAssessmentId = item.CandidateAssessment.Id, OrderId = item.OrderId,
                    TaskType="SelectionFollowupWithClient", TaskStatus="Not Started", 
                    CandidateId = item.CandidateId, OrderItemId=item.OrderItemId, OrderNo=item.OrderNo,
                    ApplicationNo=item.ApplicationNo, PostTaskAction="Do not auto-send message",
                    TaskItems = new List<TaskItem>(){
                    new() { TaskItemDescription="Follow up with clients for selection", 
                        NextFollowupByName = _docControllerAdminAppUsername }
                    }};
                _context.Entry(selTask).State = EntityState.Added;

                //**todo - update the CVRef object with actual date cv sent - call back from email sent 
            }

            try {
                await _context.SaveChangesAsync();  //this is required, as flg function reads from tasks generated
            } catch (Exception ex) {
                ErrorString += ex.Message;
            }
            if(!string.IsNullOrEmpty(ErrorString)) return ErrorString;
                        
            //6 - compose cv forward message
            var msgWithErr = await _msgRepo.GenerateMessageForCVForward(candidatesNotRefDto, "CVFwdToCustomer", Username);
            //above function also saves the msg object to database.
            if(!string.IsNullOrEmpty(msgWithErr.ErrorString)) {
                ErrorString += msgWithErr.ErrorString;
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
                    CategoryRef = o.OrderNo + "-" + item.SrNo,
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

    }
}

