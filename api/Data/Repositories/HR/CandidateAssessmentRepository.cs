using api.Data.Migrations;
using api.DTOs.HR;
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
using OrderItemAssessment = api.Entities.HR.OrderItemAssessment;

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
        private readonly DateOnly _today = DateOnly.FromDateTime(DateTime.UtcNow);

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
        
        public async Task<CandidateAssessment> GenerateCandidateAssessment(int candidateid, int orderItemId, string Username)
        {
            
            var checklistId = await _context.ChecklistHRs
                .Where(x => x.CandidateId == candidateid && x.OrderItemId == orderItemId)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();
            
            if(checklistId == 0) throw new Exception("Candidate has not been checklisted");

            var dateAssessed = await _context.CandidateAssessments
                .Where(x => x.CandidateId == candidateid && x.OrderItemId == orderItemId)
                .Select(x => x.AssessedOn).FirstOrDefaultAsync();

            if (dateAssessed.Year > 2000) throw new Exception("Candidate already assessed for the same position on " + dateAssessed);
 
            var orderItemAssessment = await _context.orderItemAssessments
                .Include(x => x.OrderItemAssessmentQs.OrderBy(x => x.QuestionNo))
                .Where(x => x.OrderItemId == orderItemId && !string.IsNullOrEmpty(x.ApprovedBy))
                //.Select(x =>  x.OrderItemAssessmentQs )
                .FirstOrDefaultAsync() ?? throw new Exception("Assessment Questions for this category have not been defined" +
                " or not approved");

            var requireAssess = await _context.RequireAssessment(orderItemId);

            if(requireAssess && orderItemAssessment.OrderItemAssessmentQs.Count ==0 ||
                orderItemAssessment?.OrderItemAssessmentQs == null) 
                throw new Exception("The Order Category is defined as requiring Candidate assessment, " + 
                    "but no Order Assessment Questions defined. Or if the assessment Questions are defined, it has not been approved.");
    
            var assessment = new CandidateAssessment{
                CandidateId = candidateid,
                OrderItemId = orderItemId,
                AssessedOn = _today,
                AssessedByEmployeeName = Username,
                ChecklistHRId = checklistId,
                RequireInternalReview = requireAssess,
                AssessResult = "Not Assessed",
                CandidateAssessmentItems = requireAssess ? 
                    _mapper.Map<ICollection<CandidateAssessmentItem>>(orderItemAssessment.OrderItemAssessmentQs) : null
            };

            return assessment;
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

        public async Task<CandidateAssessment> SaveCandidateAssessment(CandidateAssessment model, string Username)
        {
            string grade = GetGradeFromAssessmentItems(model.CandidateAssessmentItems);
            
            model.AssessResult = grade;
            model.AssessedOn = _today;
            model.AssessedByEmployeeName = Username;

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
                CandidateAssessmentId=model.Id, TaskOwnerUsername = Username,
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
            
            return model;
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
        public async Task<string> EditCandidateAssessment(CandidateAssessment model, string Username )
        {
            var existing = await _context.CandidateAssessments
                .Include(x => x.CandidateAssessmentItems)
                .Where(x => x.Id == model.Id)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if(existing == null) return "The Candidate Assessment Object was not found";
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
                    _context.CandidatesItemAssessments.Remove(existingItem);
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
            } catch (Exception ex) {
                return ex.Message;
            }
            
            return "";
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
            var assessment = await _context.CandidatesItemAssessments.FindAsync(id);
            
           if (assessment == null) return false;

            _context.CandidatesItemAssessments.Remove(assessment);
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
            if(assessParams.Id > 0) {
                query = query.Where(x => x.Id == assessParams.Id);
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
                
                if(assessParams.AssessedOn.Year > 2000) query = query.Where(x => x.AssessedOn == assessParams.AssessedOn);
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

            var assess = await GenerateCandidateAssessment(candidateid, orderitemid, Username);

            var checkobj = await _checkRepo.GetOrGenerateChecklist(candidateid, orderitemid, Username);
            if(!string.IsNullOrEmpty(checkobj.ErrorString)) {
                checklist = await _checkRepo.GetChecklist(candidateid, orderitemid);
                checklistdto = _mapper.Map<ChecklistHRDto>(checklist);
            }

            var cand = await _context.Candidates.Where(x => x.Id == candidateid).Select(x => new {x.FullName, x.ApplicationNo }).FirstOrDefaultAsync();
            checklistdto.ApplicationNo = cand.ApplicationNo;
            checklistdto.CandidateName = cand.FullName;
            checklistdto.AssessmentIsNull = assess.CandidateAssessmentItems.Count ==0;

            if(assess != null && checklist != null) {
                dto.Assessed = assess;
                dto.ChecklistHRDto = checklistdto;
            }

            return dto;
        }

        public async  Task<CandidateAssessment> GetCandidateAssessment(CandidateAssessmentParams assessParams)
        {
            var query = _context.CandidateAssessments.AsQueryable();

            if(assessParams.Id > 0) {
                query = query.Where(x => x.Id == assessParams.Id);
            } else {
                if(assessParams.CandidateId > 0) query = query.Where(x => x.CandidateId == assessParams.CandidateId);
                if(assessParams.OrderItemId > 0) query = query.Where(x => x.OrderItemId == assessParams.OrderItemId);
                if(assessParams.CVRefId > 0) query = query.Where(x => x.CVRefId == assessParams.CVRefId);
                if(!string.IsNullOrEmpty(assessParams.AssessResult)) query = query.Where(x => x.AssessResult == assessParams.AssessResult);
                if(!string.IsNullOrEmpty(assessParams.AssessedByEmployeeName)) query = query.Where(x => x.AssessedByEmployeeName.ToLower() == assessParams.AssessedByEmployeeName.ToLower());
                if(assessParams.AssessedOn.Year > 2000) query = query.Where(x =>x.AssessedOn == assessParams.AssessedOn);
            }
            
            return await query.FirstOrDefaultAsync();
        }

        public async  Task<CandidateAssessment> GetCandidateAssessmentWithItems(CandidateAssessmentParams assessParams)
        {
            var query = _context.CandidateAssessments.Include(x => x.CandidateAssessmentItems)
                .AsQueryable();

            if(assessParams.Id > 0) {
                query = query.Where(x => x.Id == assessParams.Id);
            } else {
                if(assessParams.CandidateId > 0) query = query.Where(x => x.CandidateId == assessParams.CandidateId);
                if(assessParams.OrderItemId > 0) query = query.Where(x => x.OrderItemId == assessParams.OrderItemId);
                if(assessParams.CVRefId > 0) query = query.Where(x => x.CVRefId == assessParams.CVRefId);
                if(!string.IsNullOrEmpty(assessParams.AssessResult)) query = query.Where(x => x.AssessResult == assessParams.AssessResult);
                if(!string.IsNullOrEmpty(assessParams.AssessedByEmployeeName)) query = query.Where(x => x.AssessedByEmployeeName.ToLower() == assessParams.AssessedByEmployeeName.ToLower());
                if(assessParams.AssessedOn.Year > 2000) query = query.Where(x => x.AssessedOn == assessParams.AssessedOn);
            }
            
            return await query.FirstOrDefaultAsync();
        }

    }
}