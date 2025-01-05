using System.Data.Common;
using System.Drawing.Text;
using api.DTOs.Admin;
using api.DTOs.Admin.Orders;
using api.Entities.Identity;
using api.Entities.Tasks;
using api.Helpers;
using api.Interfaces.Orders;
using api.Interfaces.Quality;
using api.Params.Objectives;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Repositories.Quality
{
    public class QualityRepository : IQualityRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly DateTime _today = DateTime.UtcNow;
        private readonly UserManager<AppUser> _userManager;
        private readonly IComposeMessagesHRRepository _msgHRRepo;
        public QualityRepository(DataContext context, IMapper mapper, UserManager<AppUser> userManager, IComposeMessagesHRRepository msgHRRepo)
        {
            _msgHRRepo = msgHRRepo;
            _userManager = userManager;
            _mapper = mapper;
            _context = context;
        }


        public async Task<PagedList<HRObjective>> GetHRObjectives(MedicalParams medParams)
        {
            var query = _context.HRTasks.AsQueryable();

            query = query.Where(x => 
                x.TaskDate >= medParams.FromDate 
                && x.TaskStatusDate <= medParams.UptoDate
                );

            if(!string.IsNullOrEmpty(medParams.EmployeeUsername)) 
                query = query.Where(x => x.AssignedToUsername.ToLower() == medParams.EmployeeUsername.ToLower());
            
            if(medParams.OrderItemId != 0) query = query.Where(x => x.OrderItemId==medParams.OrderItemId);
            
            query.OrderBy(x => new {x.AssignedToUsername, x.TaskDate});

            var paged = await PagedList<HRObjective>.CreateAsync(query.AsNoTracking()
                .ProjectTo<HRObjective>(_mapper.ConfigurationProvider)
                , medParams.PageNumber, medParams.PageSize);
            
            return paged;
        }

        public async Task<PagedList<MedicalObjective>> GetMedicalObjectives(MedicalParams medParams)
        {
            var seqs = new List<int>{ 300, 400, 500, 100};
          
            var query = _context.DepItems.AsQueryable();

            query = query.Where(x => seqs.Contains(x.Sequence));
            query = query.Where(x => x.TransactionDate >= medParams.FromDate);
            query = query.Where(x => x.TransactionDate <= medParams.UptoDate);
            query.GroupBy(x => x.DepId);

            var paged = await PagedList<MedicalObjective>.CreateAsync(query.AsNoTracking()
                .ProjectTo<MedicalObjective>(_mapper.ConfigurationProvider)
                , medParams.PageNumber, medParams.PageSize);
            //var test = await query.Take(medParams.PageSize).Skip(medParams.PageSize*(medParams.PageNumber-1)).ToListAsync();

            var depids = paged.Select(x => x.DepId).ToList();
            var datas = await (from depitem in _context.DepItems 
                    where depids.Contains(depitem.DepId) && seqs.Contains(depitem.Sequence)
                join dep in _context.Deps on depitem.DepId equals dep.Id
                join cvref in _context.CVRefs on dep.CvRefId equals cvref.Id
                join cand in _context.Candidates on cvref.CandidateId equals cand.Id 
                join item in _context.OrderItems on dep.OrderItemId equals item.Id 
                join order in _context.Orders on item.OrderId equals order.Id 
                select new {
                    CandidateName = cand.FullName, DepId = dep.Id, DateSelected = cvref.SelectionStatusDate,
                    ApplicationNo = cand.ApplicationNo, CustomerName = order.Customer.CustomerName,
                    Seq = depitem.Sequence, TransDate = depitem.TransactionDate
                }).ToListAsync();

            foreach(var pg in paged) {
                var data = datas.Where(x => x.DepId == pg.DepId).ToList();
                if(data != null) {
                    pg.ApplicationNo = data.Select(x => x.ApplicationNo).FirstOrDefault();
                    pg.CustomerName = data.Select(x => x.CustomerName).FirstOrDefault();
                    pg.CandidateName = data.Select(x => x.CandidateName).FirstOrDefault();
                    pg.DateSelected =data.Select(x => x.DateSelected).FirstOrDefault();
                    pg.RefForMedicals = (DateTime)(data.Where(x => x.Seq == 300)?.FirstOrDefault()?.TransDate);
                    pg.MedicalResult = (DateTime)(data.Where(x => x.Seq == 400 || x.Seq == 500)?.FirstOrDefault()?.TransDate);
                }
            }
           return paged;
            
        }

        
        public async Task<string> AssignTasksToHRExecs(ICollection<int> orderItemIds, string Username)
        {
            var err = "";

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
                    OrderDate = order.OrderDate, OrderItemId = item.Id, RequireInternalReview = rvw.RequireAssess == "Y",
                    SrNo = item.SrNo, ProfessionId = item.ProfessionId,
                    ProfessionName = item.Profession.ProfessionName, Quantity = item.Quantity, Ecnr = item.Ecnr,
                    CompleteBefore = item.CompleteBefore, Status = item.Status, 
                    HrExecUsername = rvw.HrExecUsername, JobDescription=jd, Remuneration = remun
                    
                }).ToListAsync();    
            
            if(assignments.Count==0) return "Failed to retrieve any records";

            //OrderItemId + HRExecUsername combination is unique.
            var HRExecsDefinedInContractReviews = await _context.ContractReviewItems
                .Where(x => orderItemIds.Contains(x.OrderItemId)).Select(x => new {x.HrExecUsername, x.OrderItemId})
                .ToListAsync();
            
            var HRTaskRecords = await _context.HRTasks.Where(x => orderItemIds
                .Contains(x.OrderItemId)).ToListAsync();
            
            foreach(var hrtask in HRTaskRecords) {
                var matching = HRExecsDefinedInContractReviews
                    .Where(x => x.OrderItemId == hrtask.OrderItemId && x.HrExecUsername == hrtask.AssignedToUsername)
                    .FirstOrDefault();
                if(matching!=null) {
                    var item = orderItemIds.Where(x => x == matching.OrderItemId);
                    if(item!=null) {
                        orderItemIds.Remove(matching.OrderItemId);
                        err += "Order Item Id excluded " + matching.OrderItemId;
                    }
                } 
            }

            if(orderItemIds.Count == 0) return "All the Item Ids selected have been assigned already";

            //create task in the name of HRExecUsername
            var tasks = new List<HRTask>();
            var task = new HRTask();

            foreach(var t in assignments)
            {
                var recipientObj= await _userManager.FindByNameAsync(t.HrExecUsername);
                if(recipientObj == null) continue;

                if (t.CompleteBefore.Year < 2000) t.CompleteBefore = _today.AddDays(7);

                var hrTask = new HRTask{TaskDate=_today, AssignedToUsername = recipientObj.UserName,
                    CompleteBy=_today.AddDays(4), OrderId=t.OrderId, OrderItemId=t.OrderItemId,
                    TaskDescription="Assignment to source suitable CVs: Category Ref: " + 
                    t.OrderNo + "-" + t.SrNo + "-" + t.ProfessionName +
                    " for " + t.CustomerName, AssignedByUsername = recipientObj.UserName, 
                    TaskStatus = "Not started", QntyAssigned = t.Quantity, OrderNo = t.OrderNo};

                _context.Entry(hrTask).State = EntityState.Added;
            }
                
            var msgs = await _msgHRRepo.ComposeMessagesToHRExecToSourceCVs(assignments, Username);

            foreach(var msg in msgs.Messages) {
                _context.Entry(msg).State = EntityState.Added;
            }

            try{
                await _context.SaveChangesAsync();
                // await _context.SaveChangesAsync() > 0 ? "" : "Failed to save the tasks and message to the database";
            } catch (DbException ex) {
                err += ex.Data.ToString();
            } catch (Exception ex) {
                err +=ex.InnerException.Message;
            }

            return err;

        }


        public async Task<PagedList<HRObjective>> GetPendingHRTasks(MedicalParams medParams)
        {
            var query = _context.HRTasks.AsQueryable();

            query = query.Where(x => x.TaskStatus.ToLower() != "completed");

            var paged = await PagedList<HRObjective>.CreateAsync(query.AsNoTracking()
                .ProjectTo<HRObjective>(_mapper.ConfigurationProvider)
                , medParams.PageNumber, medParams.PageSize);
            
            return paged;
        }


        public async Task<bool> SetHRTasksAsCompleted(ICollection<int> hrTaskIds, string Username)
        {
            var tasks = await _context.HRTasks.Include(x => x.HRTaskItems).Where(x => hrTaskIds.Contains(x.Id)).ToListAsync();

            foreach(var t in tasks) {
                t.TaskStatus = "Completed";
                t.TaskStatusDate = DateTime.UtcNow;
                t.QntyDelivered = t.HRTaskItems.Count;
                t.StatusUpdatedBy = Username;

                _context.Entry(t).State = EntityState.Modified;
            }

            return await _context.SaveChangesAsync() > 0;
        }

    }
}