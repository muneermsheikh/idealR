using System.Data.Common;
using api.DTOs.Admin.Orders;
using api.DTOs.HR;
using api.Entities.Admin.Order;
using api.Entities.HR;
using api.Entities.Tasks;
using api.Extensions;
using api.Helpers;
using api.Interfaces;
using api.Interfaces.Admin;
using api.Interfaces.HR;
using api.Params.HR;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style.XmlAccess;

namespace api.Data.Repositories.HR
{
    public class CandidateAssessmentRepository : ICandidateAssessentRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IOrderAssessmentRepository _assessRepo;
        private readonly ITaskRepository _taskRepo;
        readonly int _docControllerAdminAppUserId=0;
        readonly string _docControllerAdminAppUsername= "";
        readonly string _docControllerAdminAppUserEmail= "";
        private readonly IChecklistRepository _checkRepo;
        private readonly DateTime _today = DateTime.UtcNow;

        public CandidateAssessmentRepository(DataContext context, IMapper mapper, IChecklistRepository checkRepo,
            IOrderAssessmentRepository assessRepo, ITaskRepository taskRepo, IConfiguration config)
        {
            _checkRepo = checkRepo;
            _taskRepo = taskRepo;
            _assessRepo = assessRepo;
            _mapper = mapper;
            _context = context;
            _docControllerAdminAppUserId=  Convert.ToInt32(config["DocControllerAdminAppUserId"] ?? "0");
            _docControllerAdminAppUsername=  config["DocControllerAdminAppUsername"] ?? "";
            _docControllerAdminAppUserEmail= config["DocControllerAdminAppUserEmail"] ?? "";
        }
        
        public async Task<CandidateAssessmentWithErrDto> GenerateCandidateAssessment(int candidateid, int orderItemId, string Username)
        {
            //Checklist is a mandatory process to be completed. If it is not, no need to generate the assessment
            var obj = new CandidateAssessmentWithErrDto();
            var checklistId = await _context.ChecklistHRs
                .Where(x => x.CandidateId == candidateid && x.OrderItemId == orderItemId)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();
            
            if(checklistId == 0) {
                obj.ErrorString = "Candidate not checklisted";
                return obj;
            }

            var assessed = await _context.CandidateAssessments.Include(x => x.CandidateAssessmentItems)
                .Where(x => x.CandidateId == candidateid && x.OrderItemId == orderItemId)
                .FirstOrDefaultAsync();
            if(assessed != null) {
                obj.candidateAssessment = assessed;
                return obj;
            }
            
            var RequireAssessment = await _context.RequireAssessment(orderItemId);
            assessed = new CandidateAssessment{
                CandidateId = candidateid,
                OrderItemId = orderItemId,
                AssessedOn = _today,
                AssessedByEmployeeName = Username,
                ChecklistHRId = checklistId,            //this field is not saved
                RequireInternalReview = RequireAssessment == "1",
                AssessResult = "Not Assessed",
                CategoryRefAndName = await _context.GetCategoryRefFromOrderItemId(orderItemId),
                CustomerName = await _context.GetCustomerNameFromOrderItemId(orderItemId),      //this field is not saved
                
                //CandidateAssessmentItems = RequireAssessment?.ToLower() == "y" 
                        //? new List<CandidateAssessmentItem>(){citems} //}await CreateCandidateAssessmentItems(orderItemId, candidateid)
                        //: null
            };
            
            if(RequireAssessment=="1") {
                var citems = await CreateCandidateAssessmentItems(orderItemId, candidateid);
                
                foreach(var itm in citems) {
                    var item = new CandidateAssessmentItem {AssessedOnTheParameter=itm.AssessedOnTheParameter, 
                        AssessmentGroup = itm.AssessmentGroup, CandidateAssessmentId=itm.CandidateAssessmentId,
                        IsMandatory=itm.IsMandatory, MaxPoints=itm.MaxPoints, Points=itm.Points, QuestionNo = itm.QuestionNo,
                        Question=itm.Question, Remarks=""};

                    if(assessed.CandidateAssessmentItems==null) {
                        assessed.CandidateAssessmentItems = new List<CandidateAssessmentItem>{item};
                    } else {
                        assessed.CandidateAssessmentItems.Add(item);
                    }
                }
            }

            obj.candidateAssessment = assessed;
            return obj;
        }

        public async Task<bool> AddCandidateAssessmentItems(int candidateAssessmentId)
        {
            var assessment = await _context.CandidateAssessments.Include(x => x.CandidateAssessmentItems)
                .Where(x => x.Id == candidateAssessmentId).AsNoTracking().FirstOrDefaultAsync();

           
            var data = await _assessRepo.GetAssessmentQStdds();
            var items = _mapper.Map<ICollection<CandidateAssessmentItem>>(data);
            assessment.CandidateAssessmentItems=items;

            foreach(var item in items) {
                item.CandidateAssessmentId=candidateAssessmentId;
                assessment.CandidateAssessmentItems.Add(item);
                _context.Entry(item).State=EntityState.Added;
            }
        
            //assessment.CandidateAssessmentItems = items;
            _context.Entry(assessment).State = EntityState.Modified;

            return await _context.SaveChangesAsync() > 0;

        }

        public async Task<CandidateAssessmentWithErrDto> SaveCandidateAssessment(CandidateAssessment model, string Username)
        {
            var dto = new CandidateAssessmentWithErrDto();

            var existing = await _context.CandidateAssessments.Where(
                x => x.CandidateId == model.CandidateId && x.OrderItemId == model.OrderItemId
            ).FirstOrDefaultAsync();
            
            if(existing != null) {
                return  await EditCandidateAssessment(model, Username);
            }
            
            _context.CandidateAssessments.Add(model);
            _context.Entry(model).State = EntityState.Added;
        
            try {
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                throw new Exception(ex.Message, ex);
            }

            var orderno = await _context.GetOrderNoFromOrderItemId(model.OrderItemId);
            var appno = await _context.GetApplicationNoFromCandidateId(model.CandidateId);

            //create task for DoCntrollerAdmin for refer the CVs to the client
            var taskObj = new AppTask{TaskType="CVFwdToCustomer", CVRefId=0,
                CandidateAssessmentId=model.Id, AssignedByUsername = Username,
                AssignedToUsername=_docControllerAdminAppUsername, 
                OrderItemId=model.OrderItemId,
                OrderNo = await _context.GetOrderNoFromOrderItemId(model.OrderItemId),
                ApplicationNo = await  _context.GetApplicationNoFromCandidateId(model.CandidateId),
                CandidateId = model.CandidateId, CompleteBy = _today.AddDays(5), 
                TaskDate = _today, 
                TaskDescription = await _context.GetCandidateDescriptionFromCandidateId(model.CandidateId)
            };

            /* var tassk = _taskRepo.GenerateAppTask("CVFwdToCustomer", model.Id, DateTime.UtcNow, Username, _docControllerAdminAppUsername, 0,
                model.OrderItemId, orderno,appno,model.CandidateId, 
                await _context.GetCandidateDescriptionFromCandidateId(model.CandidateId),
                DateTime.UtcNow.AddDays(5), "Not Started", Username);
            */
            _context.Entry(taskObj).State = EntityState.Added;

            await _context.SaveChangesAsync();

            model.TaskIdDocControllerAdmin = taskObj.Id;
            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            
            dto.candidateAssessment = model;
            return dto;
        }

        public async Task<string> UpdateCandidateAssessmentStatus(int candidateAssessmentId, string username)
        {
            var existing = await _context.CandidateAssessments
                .Include(x => x.CandidateAssessmentItems)
                .Where(x => x.Id == candidateAssessmentId)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if(existing == null) return "The Candidate Assessment Object was not found";
            if(existing.AssessResult.ToLower() != "not assessed") return "candidate assessment result is: " + existing.AssessResult;

            var grade = GetGradeFromAssessmentItems(existing.CandidateAssessmentItems);
            existing.AssessResult = grade;
            existing.AssessedOn = _today;
            existing.AssessedByEmployeeName = username;
            _context.Entry(existing).CurrentValues.SetValues(existing);   //saves only the parent, not children

            _context.Entry(existing).State = EntityState.Modified;

            return await _context.SaveChangesAsync() > 0 ? "Updated" : "failed to update";
        }
        public async Task<CandidateAssessmentWithErrDto> EditCandidateAssessment(CandidateAssessment model, string Username )
        {
            var dto = new CandidateAssessmentWithErrDto();
            
            var existing = await _context.CandidateAssessments
                .Include(x => x.CandidateAssessmentItems)
                .Where(x => x.Id == model.Id)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if(existing == null) {
                dto.ErrorString = "The record does not exist to edit";
                return dto;
            }
            var grade = GetGradeFromAssessmentItems(model.CandidateAssessmentItems);
            if(grade != "Assessment Items not properly defined") {
                model.AssessResult = grade;
                model.AssessedOn = _today;
                model.AssessedByEmployeeName = Username;
            }

            _context.Entry(existing).CurrentValues.SetValues(model);   //saves only the parent, not children

            //the children 
            //Delete children that exist in existing record, but not in the new model order
            foreach (var existingItem in existing.CandidateAssessmentItems.ToList())
            {
                if (!model.CandidateAssessmentItems.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                {
                    _context.CandidatesAssessmentItems.Remove(existingItem);
                    _context.Entry(existingItem).State = EntityState.Deleted;
                }
            }

            //children that are not deleted, are either updated or new ones to be added
            foreach (var modelItem in model.CandidateAssessmentItems)
            {
                var existingItem = existing.CandidateAssessmentItems.Where(c => c.Id == modelItem.Id && c.Id != default(int)).SingleOrDefault();
                if (existingItem != null)       // Update child
                {
                    _context.Entry(existingItem).CurrentValues.SetValues(modelItem);
                    _context.Entry(existingItem).State = EntityState.Modified;
                }
                else            //insert children as new record
                {
                    var newItem = new CandidateAssessmentItem{
                        CandidateAssessmentId = existing.Id,
                        AssessmentGroup = modelItem.AssessmentGroup,
                        QuestionNo = modelItem.QuestionNo, 
                        Question=modelItem.Question, 
                        IsMandatory=modelItem.IsMandatory, 
                        AssessedOnTheParameter = modelItem.AssessedOnTheParameter,
                        MaxPoints = modelItem.MaxPoints,
                        Points= modelItem.Points
                    };
                    existing.CandidateAssessmentItems.Add(newItem);
                    _context.Entry(newItem).State = EntityState.Added;
                }

            }
        
            _context.Entry(existing).State = EntityState.Modified;
            
            try{
               await _context.SaveChangesAsync() ;
               dto.candidateAssessment=existing;
            } catch (Exception ex) {
                dto.ErrorString = ex.Message;
            }

            return dto;
            
        }

        public async Task<bool> DeleteCandidateAssessment(int id)
        {
            var assessment = await _context.CandidateAssessments
                .Include(x => x.CandidateAssessmentItems)
                .Where(x => x.Id==id)
                .FirstOrDefaultAsync();

           if (assessment == null) return false;

            _context.CandidateAssessments.Remove(assessment);
           _context.Entry(assessment).State = EntityState.Deleted;

           try {
                await _context.SaveChangesAsync();
           } catch (Exception ex) {
                throw new Exception(ex.Message, ex);
           }

           return true;
        }

        public async Task<bool> DeleteCandidateAssessmentItem(int id)
        {
            var assessment = await _context.CandidatesAssessmentItems.FindAsync(id);
            
           if (assessment == null) return false;

            _context.CandidatesAssessmentItems.Remove(assessment);
           _context.Entry(assessment).State = EntityState.Deleted;

           try {
                await _context.SaveChangesAsync();
           } catch (Exception ex) {
                throw new Exception(ex.Message, ex);
           }

           return true;
        }

        public async Task<PagedList<CandidateAssessedDto>> GetCandidateAssessments(CandidateAssessmentParams assessParams)
        {
            var query = _context.CandidateAssessments.Include(x => x.CandidateAssessmentItems)
                .AsQueryable();

            if(assessParams.CandidateAssessmentId > 0) {
                query = query.Where(x => x.Id == assessParams.CandidateAssessmentId);
            } else {
                if(assessParams.CandidateId > 0) query = query.Where(x => x.CandidateId == assessParams.CandidateId);
                
                if(assessParams.OrderItemId > 0) query = query.Where(x => x.OrderItemId == assessParams.OrderItemId);
                
                query = query.Where(x => x.CVRefId == assessParams.CVRefId);
                
                if(!string.IsNullOrEmpty(assessParams.AssessResult)) {
                    if(assessParams.AssessResult.ToLower()=="pass") {
                        query = query.Where(x => "ABC".Contains(x.AssessResult));
                    } else {
                        query = query.Where(x => assessParams.AssessResult.Contains(x.AssessResult));
                    }
                }
                
                if(!string.IsNullOrEmpty(assessParams.AssessedByEmployeeName)) query = query.Where(x => x.AssessedByEmployeeName.ToLower() == assessParams.AssessedByEmployeeName.ToLower());
                
                if(assessParams.AssessedOn.Year > 2000) query = query.Where(x => DateOnly.FromDateTime(x.AssessedOn) == DateOnly.FromDateTime(assessParams.AssessedOn));
            }
            
            var paged = await PagedList<CandidateAssessedDto>.CreateAsync(query.AsNoTracking()
                    .ProjectTo<CandidateAssessedDto>(_mapper.ConfigurationProvider),
                    assessParams.PageNumber, assessParams.PageSize);
            
            return paged;
            
        }

        public async Task<CandidateAssessmentAndChecklistDto> GetChecklistAndAssessment(int candidateid, int orderitemid, string Username)
        {
            var dto = new CandidateAssessmentAndChecklistDto();

            var checklist = new ChecklistHR();
            var checklistdto = new ChecklistHRDto();

            var checklistWithErr = await _checkRepo.GetOrGenerateChecklist(candidateid, orderitemid, Username);
            
            if(!string.IsNullOrEmpty(checklistWithErr.ErrorString)) {
                dto.ErrorString = checklistWithErr.ErrorString;
                return dto;
            }
            checklistdto = checklistWithErr.checklistDto;
            
            //assessment
            var assessWithErr = await GenerateCandidateAssessment(candidateid, orderitemid, Username);
            checklistdto.AssessmentIsNull = assessWithErr.candidateAssessment?.CandidateAssessmentItems?.Count ==0;
            dto.ChecklistHRDto = checklistdto;

            dto.Assessed = assessWithErr.candidateAssessment;
            dto.ErrorString = assessWithErr.ErrorString;
            return dto;
        }

        //*todo* USE parameters to commbine this and above
        public async Task<CandidateAssessmentAndChecklistDto> GetCandidateAssessmentWithChecklistByAssessmentId(int candidateAssessmentId, string Username) {
            //this will not be an error, as candidateassessmentid already exists
            var assessment = await _context.CandidateAssessments.Include(x => x.CandidateAssessmentItems)
                .Where(x => x.Id == candidateAssessmentId).FirstOrDefaultAsync();
            
            var checklistWithErr = await _checkRepo.GetOrGenerateChecklist(assessment.CandidateId, assessment.OrderItemId, Username);

            var dto = new CandidateAssessmentAndChecklistDto{Assessed = assessment};
            
            dto.ChecklistHRDto=dto.ChecklistHRDto;

            return dto;
        }

        public async Task<ChecklistAndCandidateAssessmentDto> SaveNewChecklist (ChecklistHR checklisthr, string Username)
        {
            var checkobj = new ChecklistAndCandidateAssessmentDto();

            var obj = await _checkRepo.GetChecklist(checklisthr.CandidateId, checklisthr.OrderItemId);
            
            if(obj != null) {
                var errString = await _checkRepo.EditChecklistHR(checklisthr, Username);
                    if(string.IsNullOrEmpty(errString)) checkobj.ChecklistHR = checklisthr; //no error
            } else {
            
                var errStr = await _checkRepo.VerifyChecklist(checklisthr);
                if(!string.IsNullOrEmpty(errStr)) {
                    checkobj.ErrorString = errStr;
                    return checkobj;
                }
            
                _context.ChecklistHRs.Add(checklisthr);
                _context.Entry(checklisthr).State = EntityState.Added;
                await _context.SaveChangesAsync();
            }

            checkobj.ChecklistHR  = checklisthr;
            //new checklist, so candidateassessment does not exist. create it
            var errDto = await GenerateCandidateAssessment(
                checkobj.ChecklistHR.CandidateId, checkobj.ChecklistHR.OrderItemId, Username);
            if(!string.IsNullOrEmpty(errDto.ErrorString)) {
                checkobj.ErrorString=errDto.ErrorString;
            } else {
                checkobj.Assessed=errDto.candidateAssessment;
            }

            
            try {
                await _context.SaveChangesAsync();
                checkobj.ChecklistHR = checklisthr;
            } catch (DbException ex) {
                checkobj.ErrorString = ex.Message;
            } catch (Exception ex) {
                if(ex.Message.Contains("IX_ChecklistHRs_OrderItemId_CandidateId")) {
                    checkobj.ErrorString = "Unique Index violation - OrderItem and Candidate" +
                    "- the candidate has already been assessed for the same requirement earlier";
                } else {
                checkobj.ErrorString = ex.Message;
                }
            }

            return checkobj;
            
        }
        public async  Task<CandidateAssessment> GetCandidateAssessment(CandidateAssessmentParams assessParams)
        {
            var query = _context.CandidateAssessments.AsQueryable();

            if(assessParams.CandidateAssessmentId > 0) {
                query = query.Where(x => x.Id == assessParams.CandidateAssessmentId);
            } else {
                if(assessParams.CandidateId > 0) query = query.Where(x => x.CandidateId == assessParams.CandidateId);
                if(assessParams.OrderItemId > 0) query = query.Where(x => x.OrderItemId == assessParams.OrderItemId);
                if(assessParams.CVRefId > 0) query = query.Where(x => x.CVRefId == assessParams.CVRefId);
                if(!string.IsNullOrEmpty(assessParams.AssessResult)) query = query.Where(x => x.AssessResult == assessParams.AssessResult);
                if(!string.IsNullOrEmpty(assessParams.AssessedByEmployeeName)) query = query.Where(x => x.AssessedByEmployeeName.ToLower() == assessParams.AssessedByEmployeeName.ToLower());
                if(assessParams.AssessedOn.Year > 2000) query = query.Where(x => DateOnly.FromDateTime(x.AssessedOn) == DateOnly.FromDateTime(assessParams.AssessedOn));
            }
            
            return await query.FirstOrDefaultAsync();
        }

        public async  Task<CandidateAssessment> GetCandidateAssessmentWithItems(CandidateAssessmentParams assessParams)
        {
            var query = _context.CandidateAssessments.Include(x => x.CandidateAssessmentItems)
                .AsQueryable();

            if(assessParams.CandidateAssessmentId > 0) {
                query = query.Where(x => x.Id == assessParams.CandidateAssessmentId);
            } else {
                if(assessParams.CandidateId > 0) query = query.Where(x => x.CandidateId == assessParams.CandidateId);
                if(assessParams.OrderItemId > 0) query = query.Where(x => x.OrderItemId == assessParams.OrderItemId);
                if(assessParams.CVRefId > 0) query = query.Where(x => x.CVRefId == assessParams.CVRefId);
                if(!string.IsNullOrEmpty(assessParams.AssessResult)) query = query.Where(x => x.AssessResult == assessParams.AssessResult);
                if(!string.IsNullOrEmpty(assessParams.AssessedByEmployeeName)) query = query.Where(x => x.AssessedByEmployeeName.ToLower() == assessParams.AssessedByEmployeeName.ToLower());
                if(assessParams.AssessedOn.Year > 2000) query = query.Where(x => DateOnly.FromDateTime(x.AssessedOn) == DateOnly.FromDateTime(assessParams.AssessedOn));
            }
            
            return await query.FirstOrDefaultAsync();
        }

        public async Task<ICollection<CandidateAssessedShortDto>> GetCandidateAssessmentsByCandidateId(int candidateId)
        {
            var dtos = await (from assess in _context.CandidateAssessments where assess.CandidateId == candidateId
                join item in _context.OrderItems on assess.OrderItemId equals item.Id
                join cat in _context.Professions on item.ProfessionId equals cat.Id
                join order in _context.Orders on item.OrderId equals order.Id
                join cv in _context.Candidates on assess.CandidateId equals cv.Id
                select new CandidateAssessedShortDto {
                    AssessedByUsername = assess.AssessedByEmployeeName,
                    AssessedOn = assess.AssessedOn, AssessResult = assess.AssessResult,
                    CandidateId = assess.CandidateId, CandidateName = cv.FullName,
                    CustomerName = order.Customer.KnownAs, CandidateAssessmentId = assess.Id,
                    OrderItemId = item.Id, CategoryRef = order.OrderNo + "-" + item.SrNo + "-" + cat.ProfessionName,
                    RequireInternalReview = assess.RequireInternalReview,
                    orderItemBriefDto = new OrderItemBriefDto {
                        CustomerName = order.Customer.KnownAs,
                    }
                }).ToListAsync();
            
            return dtos;
        }

        public async Task<CandidateAssessmentDto> GetCandidateAssessmentDtoWithItems(int candidateid, int orderitemid)
        {
            var assItems=new List<AssessmentItemDto>();
           
            var dto = await _context.CandidateAssessments.Include(x => x.CandidateAssessmentItems)
                .Where(x => x.CandidateId == candidateid && x.OrderItemId == orderitemid).FirstOrDefaultAsync();
            
            if(dto == null) return null;

            foreach(var item in dto.CandidateAssessmentItems) {
                var assItem = new AssessmentItemDto {
                    CandidateAssessmentId=dto.Id, Id=item.Id,
                    IsMandatory=item.IsMandatory, MaxPoints=item.MaxPoints,
                    Points=item.Points, Question=item.Question, QuestionNo=item.QuestionNo,
                };
                assItems.Add(assItem);
            }
            
            var orderiddetails = await _context.GetDetailsFromOrderItemId(orderitemid,candidateid);

            var categorydto = new CandidateAssessmentDto{
                ApplicationNo=orderiddetails.ApplicationNo, CandidateId=candidateid,
                CandidateName=orderiddetails.CandidateName, Id=candidateid, 

                AssessedByUsername=dto.AssessedByEmployeeName,
                AssessedOn = dto.AssessedOn,  AssessResult = dto.AssessResult,
                ChecklistHRId=dto.ChecklistHRId, OrderItemId=dto.OrderItemId,
                RequireInternalReview=dto.RequireInternalReview,
                ProfessionName = orderiddetails.ProfessionName,
                CategoryRef = orderiddetails.CategoryRef,
                CustomerName = orderiddetails.CustomerName,
                OrderId = orderiddetails.OrderId, 
                AssessmentItemsDto=assItems
            };

            return categorydto;
        }

        private async Task<ICollection<CandidateAssessmentItem>> CreateCandidateAssessmentItems(int orderItemId, int candidateId)
        {
            var Qs = await _context.OrderAssessmentItems.Include(x => x.OrderAssessmentItemQs)
                .Where(x => x.OrderItemId==orderItemId)
            .FirstOrDefaultAsync();

            if (Qs.OrderAssessmentItemQs.Count == 0) return null;

            var items = new List<CandidateAssessmentItem>();

            foreach(var q in Qs.OrderAssessmentItemQs) {
                var item = new CandidateAssessmentItem{    
                    AssessedOnTheParameter=false,
                    AssessmentGroup = q.Subject,
                    IsMandatory=q.IsMandatory,
                    MaxPoints=q.MaxPoints,
                    Points=0,
                    Question=q.Question,
                    QuestionNo=q.QuestionNo,
                    Remarks=""
                };
                items.Add(item);
            }

            return items;
        }

        private static string GetGradeFromAssessmentItems(ICollection<CandidateAssessmentItem> assessmentItems)
        {
            var sumOfPoints = assessmentItems.Sum(x => x.Points);
            var totalOfPoints = assessmentItems.Sum(x => x.MaxPoints);
            
            if(sumOfPoints ==0 || totalOfPoints == 0) return "";

            float pct = sumOfPoints*100/totalOfPoints;

            var grade = pct > 90 ? "A+" : pct > 80 ? "A" : pct > 70 ? "B+" : pct > 60 ? "B"  : pct > 50 ? "C" : "D";
            
            return grade;
        }
        
        public async Task<ICollection<cvsAvailableDto>> GetAvailableCandidates()
        {
            var qry = await (from assesmt in _context.CandidateAssessments 
                    where assesmt.CVRefId==0 && assesmt.AssessResult != "Not Assessed"
                join cv in _context.Candidates on assesmt .CandidateId equals cv.Id  
                join item in _context.OrderItems on assesmt.OrderItemId equals item.Id
                join cat in _context.Professions on item.ProfessionId equals cat.Id
                join order in _context.Orders on item.OrderId equals order.Id
                orderby cv.ApplicationNo
                select new cvsAvailableDto {
                    CandAssessmentId = assesmt.Id, ApplicationNo = cv.ApplicationNo, City = cv.City, FullName = cv.FullName, 
                    CandidateId = cv.Id, AssessedOn = assesmt.AssessedOn, 
                    Gender=  (cv.Gender == "female" ? "F": "M").ToUpper(),
                    GradeAssessed=assesmt.AssessResult,  Checked = false, OrderItemId = item.Id,
                    OrderCategoryRef=cat.ProfessionName + " (" + order.OrderNo + "-" + item.SrNo + ") - " + order.Customer.KnownAs,
                }).ToListAsync();
           
                foreach(var item in qry) {
                    item.userProfessions = await _context.UserProfessions.Where(x => x.CandidateId == item.CandidateId).ToListAsync();
                    foreach(var prof in item.userProfessions) {
                        if(string.IsNullOrEmpty(prof.ProfessionName)) {
                            prof.ProfessionName = await _context.GetProfessionNameFromId(prof.ProfessionId);
                        }
                        _context.Entry(prof).State = EntityState.Modified;
                    }
                }
                
                if(_context.ChangeTracker.HasChanges()) await _context.SaveChangesAsync();
                return qry;
        }

        public async Task<ICollection<ProspectiveHeaderDto>> CategoriesFromCVAvailableToRefer()
        {
            var query = await _context.CandidateAssessments
                .Where(x => x.AssessResult != "Not Assessed" && x.CVRefId == 0 )
                .Select(x => x.CategoryRefAndName)
                .Distinct()
                .ToListAsync();
            return (ICollection<ProspectiveHeaderDto>)query;
        }

    }
}