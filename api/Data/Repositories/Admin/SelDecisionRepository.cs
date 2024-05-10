using api.DTOs.Admin;
using api.Entities.Finance;
using api.Entities.HR;
using api.Entities.Messages;
using api.Helpers;
using api.Interfaces.Admin;
using api.Interfaces.Finance;
using api.Interfaces.Messages;
using api.Params.Admin;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Repositories.Admin
{
    public class SelDecisionRepository : ISelDecisionRepository
    {
        private readonly DataContext _context;
        private readonly IComposeMessagesAdminRepository _msgAdmRepo;
        private readonly IFinanceRepository _finRepo;
        private readonly Mapper _mapper;
        public SelDecisionRepository(DataContext context, IComposeMessagesAdminRepository msgAdmRepo, 
            IFinanceRepository finRepo, Mapper mapper)
        {
            _mapper = mapper;
            _finRepo = finRepo;
            _msgAdmRepo = msgAdmRepo;
            _context = context;
        }

        public async Task<ICollection<Message>> ComposeRejMessagesToCandidates(List<int> cvrefids, string Username)
        {
            var selectedDetails = await (from cvref in _context.CVRefs 
                    where cvrefids.Contains(cvref.Id) 
                join sel in _context.SelectionDecisions on cvref.Id equals sel.CVRefId 
                    where sel.SelectionStatus == "Rejected"
                join item in _context.OrderItems on cvref.OrderItemId equals item.Id 
                join order in _context.Orders on item.OrderId equals order.Id
                join cv in _context.Candidates on cvref.CandidateId equals cv.Id
                
                select new SelectionMessageDto {
                    CustomerName = order.Customer.CustomerName,
                    CustomerCity = order.Customer.City,
                    OrderNo = order.OrderNo,
                    ProfessionName = item.Profession.ProfessionName,
                    SelectionStatus = sel.SelectionStatus,
                    RejectionReason = sel.RejectionReason,
                    ApplicationNo = cv.ApplicationNo,
                    CandidateId = cv.Id,
                    CandidateTitle = cv.Gender == "M" ? "Mr." : "Ms.",
                    CandidateName = cv.FullName,
                    CandidateGender = cv.Gender,
                    CandidateAppUserId = cv.AppUserId,
                    
                }).ToListAsync();

            var msgs = await _msgAdmRepo.AdviseRejectionStatusToCandidateByEmail(selectedDetails, Username);

            foreach(var msg in msgs) { _context.Entry(msg).State = EntityState.Added;}
            
            return msgs;
        }

        public async Task<ICollection<Message>> ComposeSelMessagesToCandidates(List<int> cvrefids, string Username)
        {
            var selectedDetails = await (from cvref in _context.CVRefs where cvrefids.Contains(cvref.Id)
                join sel in _context.SelectionDecisions on cvref.Id equals sel.CVRefId 
                join item in _context.OrderItems on cvref.OrderItemId equals item.Id 
                join order in _context.Orders on item.OrderId equals order.Id
                join cv in _context.Candidates on cvref.CandidateId equals cv.Id
                join emp in _context.Employments on sel.CVRefId equals emp.CVRefId
                
                select new SelectionMessageDto {
                    CustomerName = order.Customer.CustomerName,
                    CustomerCity = order.Customer.City,
                    OrderNo = order.OrderNo,
                    ProfessionName = item.Profession.ProfessionName,
                    SelectionStatus = sel.SelectionStatus,
                    RejectionReason = sel.RejectionReason,
                    ApplicationNo = cv.ApplicationNo,
                    CandidateId = cv.Id,
                    CandidateTitle = cv.Gender == "M" ? "Mr." : "Ms.",
                    CandidateName = cv.FullName,
                    CandidateGender = cv.Gender,
                    CandidateAppUserId = cv.AppUserId,
                    Employment = emp
                }).ToListAsync();

             var msgs = await _msgAdmRepo.ComposeSelectionStatusMessagesForCandidate(
                selectedDetails, Username);

                foreach(var msg in msgs) { _context.Entry(msg).State = EntityState.Added;}
            return msgs;
        }

        public async Task<bool> DeleteSelection(int selectionId)
        {
           var obj = await _context.SelectionDecisions.FindAsync(selectionId);
           if(obj == null) return false;

           _context.Entry(obj).State = EntityState.Deleted;
           return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> EditEmployment(Employment model, string Username )
        {
            var existingObj = await _context.Employments
               .Where(p => p.Id == model.Id)
               .AsNoTracking()
               .SingleOrDefaultAsync() ?? throw new Exception("The Selection object does not exist in the database");
               
            if (!existingObj.OfferAccepted && model.OfferAccepted ) {
                var candidateid = await _context.CVRefs.Where(x => x.Id == model.CVRefId)
                    .Select(x => x.CandidateId).FirstOrDefaultAsync();
                var appno = await _context.Candidates.Where(x => x.Id==candidateid)
                    .Select(x => x.ApplicationNo).FirstOrDefaultAsync();

                var coaCandidate = await _finRepo.CreateCoaForCandidate(appno, true);
                var items = new List<VoucherEntry>
                {
                    new() {TransDate = DateOnly.FromDateTime(DateTime.UtcNow),
                            COAId = coaCandidate.Id,
                            AccountName = coaCandidate.AccountName,
                            Dr = model.Charges},
                    new() {TransDate = DateOnly.FromDateTime(DateTime.UtcNow),
                            COAId = 11,
                            AccountName = "Sales Recruitment",
                            Cr = model.Charges}
                };
                await _finRepo.AddNewVoucher(new FinanceVoucher{
                    Divn="R", COAId=11,AccountName = "Sales - Recruitment",
                    Amount = model.Charges, 
                    Narration="Employment accepted by candidate",
                    VoucherEntries = items
                }, Username);
            }

            _context.Entry(existingObj).CurrentValues.SetValues(model);   //saves only the parent, not children
            _context.Entry(existingObj).State = EntityState.Modified; 

            try {
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                throw new Exception(ex.Message, ex);
            }

            return true;
        }


        public async Task<bool> EditSelection(SelectionDecision model)
        {
            // thanks to @slauma of stackoverflow
               var existingObj = await _context.SelectionDecisions
               .Where(p => p.Id == model.Id)
               .AsNoTracking()
               .SingleOrDefaultAsync() ?? throw new Exception("The Selection object does not exist in the database");
               
               _context.Entry(existingObj).CurrentValues.SetValues(model);   //saves only the parent, not children
               _context.Entry(existingObj).State = EntityState.Modified;        //this is doubling the contractreviewitems

               try {
                    await _context.SaveChangesAsync();
               } catch (Exception ex) {
                    throw new Exception(ex.Message, ex);
               }

               return true;
               
        }

        public async Task<PagedList<SelDecisionDto>> GetSelectionDecisions(SelDecisionParams selParams)
        {
            var query = _context.SelectionDecisions.AsQueryable();
            if(selParams.CVRefId > 0) query = query.Where(x => x.CVRefId == selParams.CVRefId);
            if(selParams.OrderItemId > 0) query = query.Where(x => x.OrderItemId == selParams.OrderItemId);
            if(selParams.ProfessionId > 0) query = query.Where(x => x.ProfessionId == selParams.ProfessionId);
            if(selParams.SelectedOn.Year > 2000) query = query.Where(x => x.SelectedOn == selParams.SelectedOn);

            var paged = await PagedList<SelDecisionDto>.CreateAsync(
                query.AsNoTracking()
                .ProjectTo<SelDecisionDto>(_mapper.ConfigurationProvider),
                selParams.PageNumber, selParams.PageSize);
    
          return paged;

        }

        public async Task<MessageWithError> RegisterSelections(ICollection<CreateSelDecisionDto> selDtos, string Username)
        {
            var dtoToReturn = new MessageWithError();

            var selDetails = await (from cvref in _context.CVRefs
                where selDtos.Select(x => x.CVRefId).ToList().Contains(cvref.Id)
                join dtos in selDtos on cvref.Id equals dtos.CVRefId
                join item in _context.OrderItems on cvref.OrderItemId equals item.Id
                join checklist in _context.ChecklistHRs on new { cvref.CandidateId, cvref.OrderItemId }
                    equals new { checklist.CandidateId, checklist.OrderItemId }
                select new SelectionDecision
                {
                    CVRefId = cvref.Id,
                    OrderItemId = cvref.OrderItemId,
                    ProfessionName = item.Profession.ProfessionName,
                    SelectedOn = dtos.DecisionDate,
                    SelectionStatus = dtos.SelectionStatus,
                    Charges = checklist.ChargesAgreed,
                    Remarks = dtos.Remarks,
                    CVRef = cvref
                }).ToListAsync();

            foreach (var sel in selDetails) { _context.Entry(sel).State = EntityState.Added;}       //selection/rejection detisions saved

            var cvrefids = selDtos.Select(x => x.CVRefId).ToList();
            if(cvrefids.Count == 0) return null;

            foreach(var item in selDetails) {
                item.CVRef.SelectionStatus = item.SelectionStatus;
                item.CVRef.SelectionStatusDate = DateOnly.FromDateTime(DateTime.UtcNow);
                _context.Entry(item.CVRef).State = EntityState.Modified;
            }
            
            var selectedRejectedMsgDto = await (from cvref in _context.CVRefs where cvrefids.Contains(cvref.Id)
                join sel in selDtos on cvref.Id equals sel.CVRefId 
                join item in _context.OrderItems on cvref.OrderItemId equals item.Id 
                join order in _context.Orders on item.OrderId equals order.Id
                join cv in _context.Candidates on cvref.CandidateId equals cv.Id
                join emp in _context.Employments on sel.CVRefId equals emp.CVRefId
                
                select new SelectionMessageDto {
                    CustomerName = order.Customer.CustomerName,
                    CustomerCity = order.Customer.City,
                    OrderNo = order.OrderNo,
                    ProfessionName = item.Profession.ProfessionName,
                    SelectionStatus = sel.SelectionStatus,
                    RejectionReason = sel.RejectionReason,
                    ApplicationNo = cv.ApplicationNo,
                    CandidateId = cv.Id,
                    CandidateTitle = cv.Gender == "M" ? "Mr." : "Ms.",
                    CandidateName = cv.FullName,
                    CandidateGender = cv.Gender,
                    CandidateAppUserId = cv.AppUserId,
                    Employment = emp
                }).ToListAsync();
            
            var employments = new List<Employment>();

            var selectionDetails = selDtos.Where(x => x.SelectionStatus == "Selected");

            if(selectionDetails.Any()) 
            {
                employments = await (from cvref in _context.CVRefs
                    where selDtos.Select(x => x.CVRefId).ToList().Contains(cvref.Id) 
                    join dto in selDtos on cvref.Id equals dto.CVRefId
                    where dto.SelectionStatus == "Selected"
                    join item in _context.OrderItems on cvref.OrderItemId equals item.Id
                    join remun in _context.Remunerations on item.Id equals remun.OrderItemId
                    join checklist in _context.ChecklistHRs on new { cvref.CandidateId, cvref.OrderItemId }
                        equals new { checklist.CandidateId, checklist.OrderItemId }
                    select new Employment
                    {
                        CVRefId = dto.CVRefId,
                        SelectionDecision = dto.SelectionStatus,
                        SelectedOn = dto.DecisionDate,
                        ChargesFixed = checklist.Charges,
                        Charges = checklist.ChargesAgreed,
                        SalaryCurrency = remun.SalaryCurrency,
                        Salary = remun.SalaryMin,
                        ContractPeriodInMonths = remun.ContractPeriodInMonths,
                        WeeklyHours = remun.WorkHours * 6,
                        HousingProvidedFree = remun.HousingProvidedFree,
                        HousingAllowance = remun.HousingAllowance,
                        HousingNotProvided = remun.HousingNotProvided,
                        FoodProvidedFree = remun.FoodProvidedFree,
                        FoodAllowance = remun.FoodAllowance,
                        FoodNotProvided = remun.FoodNotProvided,
                        OtherAllowance = remun.OtherAllowance,
                        LeavePerYearInDays = remun.LeavePerYearInDays,
                        LeaveAirfareEntitlementAfterMonths = remun.LeaveAirfareEntitlementAfterMonths
                    }).ToListAsync();
                
                foreach (var rem in employments) { _context.Entry(rem).State = EntityState.Added; }

                bool isSaved = false;
                do {
                    try {
                        await _context.SaveChangesAsync();
                        isSaved = true;
                    } catch (DbUpdateException ex) {
                        foreach (var entry in ex.Entries) {
                            entry.State = EntityState.Detached; // Remove from context so won't try saving again.
                            dtoToReturn.ErrorString += ex.Message;
                        }
                    }
                }
                while (!isSaved);

                if (!isSaved) return dtoToReturn;

                //update relevant tasks

                var selectedDetails = selectedRejectedMsgDto.Where(x => x.SelectionStatus.ToLower() == "selected").ToList();
                
                var msgs = await _msgAdmRepo.ComposeSelectionStatusMessagesForCandidate(selectedDetails, Username);
                foreach(var msg in msgs) { _context.Entry(msg).State = EntityState.Added;}
            }
            
            var rejectionDetails = selectedRejectedMsgDto.Where(x => x.SelectionStatus.ToLower() != "selected").ToList();

            if(rejectionDetails.Any()) {
                var msgs = await _msgAdmRepo.AdviseRejectionStatusToCandidateByEmail(rejectionDetails, Username);
                foreach(var msg in msgs) { _context.Entry(msg).State = EntityState.Added;}
            }

            //update relevant tasks
            var cvrefs = await _context.CVRefs.Where(x => cvrefids.Contains(x.Id)).ToListAsync();
            if(cvrefs == null) return null;
                
            var tasks = await _context.Tasks.Where(x => cvrefids.Contains(x.CVRefId)).ToListAsync();
            if(tasks != null) {
                foreach(var task in tasks) {
                    task.TaskStatus = "Completed";
                    task.CompletedOn = DateTime.UtcNow;
                }
            }

            try {await _context.SaveChangesAsync();} catch (DbUpdateException ex) {dtoToReturn.ErrorString += ex.Message;}

            if (string.IsNullOrEmpty(dtoToReturn.ErrorString)) return dtoToReturn;

            if(!string.IsNullOrEmpty(dtoToReturn.ErrorString)) dtoToReturn.ErrorString = "Selection/Rejection decisions registered, but failed to create Tasks and compose selection messages";
            return dtoToReturn;

        }
      
        
    }
}