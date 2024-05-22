//using System.Diagnostics;
using System.Data.Common;
using System.Linq;
using api.DTOs.Admin;
using api.Entities.Deployments;
using api.Entities.Finance;
using api.Entities.HR;
using api.Entities.Identity;
using api.Entities.Messages;
using api.Extensions;
using api.Helpers;
using api.Interfaces.Admin;
using api.Interfaces.Finance;
using api.Interfaces.Messages;
using api.Params.Admin;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Repositories.Admin
{
    public class SelDecisionRepository : ISelDecisionRepository
    {
        private readonly DataContext _context;
        private readonly IComposeMessagesAdminRepository _msgAdmRepo;
        private readonly IFinanceRepository _finRepo;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly DateOnly _today = DateOnly.FromDateTime(DateTime.UtcNow);
        public SelDecisionRepository(DataContext context, IComposeMessagesAdminRepository msgAdmRepo, 
               IFinanceRepository finRepo , IMapper mapper, UserManager<AppUser> userManager            )
        {
            _userManager = userManager;
            _mapper = mapper;
            _finRepo = finRepo;
            _msgAdmRepo = msgAdmRepo;
            _context = context;
        }

        public async Task<ICollection<Message>> ComposeRejMessagesToCandidates(List<int> cvrefids, string Username)
        {
            if(cvrefids.Count == 0) return null;

            var rejectedDetails = await (from cvref in _context.CVRefs 
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

            string ErrorString="";
            ErrorString = await VerifyDataAvailableForSelMessages(rejectedDetails);
            if(string.IsNullOrEmpty(ErrorString)) throw new Exception(ErrorString) ;
            var msgs = await _msgAdmRepo.AdviseRejectionStatusToCandidateByEmail(rejectedDetails, Username);

            foreach(var msg in msgs) { _context.Entry(msg).State = EntityState.Added;}
            
            return msgs;
        }

        public async Task<ICollection<Message>> ComposeSelMessagesToCandidates(List<int> cvrefids, string Username)
        {
            if(cvrefids.Count == 0) return null;

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
            }).ToListAsync();;
            
            string ErrorString="";
            ErrorString = await VerifyDataAvailableForSelMessages(selectedDetails);
            if(!string.IsNullOrEmpty(ErrorString)) throw new Exception(ErrorString) ;

            var msgs = await _msgAdmRepo.ComposeSelectionStatusMessagesForCandidate(selectedDetails, Username);

            return msgs;
        }

        private async Task<AppUser> AppUserFromEmployeeId(int EmployeeId, string email) {
            
            var empObj = EmployeeId == 0 
                ? string.IsNullOrEmpty(email) ? null : await _context.Employees.Where(x => x.OfficialEmail == email).FirstOrDefaultAsync()
                : await _context.Employees.FindAsync(EmployeeId);

            if(empObj == null) return null;

            if (empObj.AppUserId  != 0) return await _userManager.FindByIdAsync(empObj.AppUserId.ToString());
            
            AppUser newAppUser;
            newAppUser = await _userManager.FindByNameAsync(empObj.UserName);

            if(newAppUser == null) {
                newAppUser = new AppUser{
                    Gender = empObj.Gender, KnownAs=empObj.KnownAs,
                    DateOfBirth= empObj.DateOfBirth,
                    Created = _today,
                    City = empObj.City, Country = empObj.Country
                };
                
                await _userManager.CreateAsync(newAppUser, "Pa$$w0rd");
            }

            if(newAppUser != null) {
                empObj.AppUserId = newAppUser.Id;
                _context.Entry(empObj).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            return newAppUser;

        }

        private async Task<AppUser> AppUserFromCandidateId(int CandidateId) {
            var candidateObj = await _context.Candidates.Where(x => x.Id == CandidateId)
                .FirstOrDefaultAsync();
            if(candidateObj == null) return null;

            if (candidateObj.AppUserId  != 0) return await _userManager
                .FindByIdAsync(candidateObj.AppUserId.ToString());
            
            //check if AppUser has the record
            AppUser newAppUser;
            newAppUser = await _userManager.FindByNameAsync(candidateObj.UserName);

            if(newAppUser == null) {
                await _context.Candidates.Where(x => x.Id == x.Id)
                .Select(x => new AppUser{
                    Gender = x.Gender, KnownAs=x.KnownAs,
                    DateOfBirth= (DateOnly)x.DOB,
                    Created =_today,
                    City = x.City, Country = x.Country
                }).FirstOrDefaultAsync();
                
                await _userManager.CreateAsync(newAppUser, "Pa$$w0rd");
            }

            if(newAppUser != null) {
                candidateObj.AppUserId = newAppUser.Id;
                _context.Entry(candidateObj).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            return newAppUser;

        }

        private async Task<string> VerifyDataAvailableForSelMessages(ICollection<SelectionMessageDto> selMessages)
        {
            string ErrString="";
            //check if senderObj and RecipientObj have valid data
            foreach(var item in selMessages) {
                var recipientObj = await AppUserFromCandidateId(item.CandidateId);
                if(recipientObj == null) ErrString += ", Failed to get recipient AppUser object";
                if(string.IsNullOrEmpty(item.SelectionStatus)) ErrString += ", " + "Selection Status not defined for " + item.CandidateKnownAs;

                var empObj = item.HRupId == 0 
                    ? await AppUserFromEmployeeId(0, item.HRExecEmail)
                    : await AppUserFromEmployeeId(item.HRupId, "");
                
             }
             return ErrString;
        }

        public async Task<string> DeleteSelection(int selectionId)
        {
            var selections = await _context.SelectionDecisions.ToListAsync();
            var employments = await _context.Employments.ToListAsync();
            var vouchers = await _context.Vouchers.ToListAsync();
            var deps = await _context.Deps.ToListAsync();

            //Deletebehavior.Cascade for Selections and other related tables cd not be set, so all related
            //records are deleted manually
            var obj = await _context.SelectionDecisions.FindAsync(selectionId);
            if(obj == null) return "no selection object by that Selection Id";
            var cvrefid = obj.CVRefId;
            DateOnly offerAcceptedOn;
            int coaRecruitmentSales=11;
            int appno=0;

            string ErrString = "";

            _context.SelectionDecisions.Remove(obj);
            _context.Entry(obj).State = EntityState.Deleted;

            //delete related records, as indexes for Deletebahvior.Cascade not working
            //1. delete Employment
            //2. update CVRef.SelectionStatus field
            //3. delete vouchers debiting the candidate
            //4. send message to concerned

            //1 . delete employment
            var emp = await _context.Employments.Where(x => x.CVRefId==cvrefid).FirstOrDefaultAsync();
            if (emp==null) return "Failed to retrieve Employment record.  Cannot proceed with the deletion of selection";
            _context.Employments.Remove(emp);
            _context.Entry(emp).State = EntityState.Deleted;
        
            //2 - update CVRef.SelectionStatus
            var CVREF = await _context.CVRefs.FindAsync(cvrefid);
            if(CVREF == null) return "Failed to retrieve CVRef record.  Cannot proceed with the deletion of selection";

            CVREF.SelectionStatus  = "";
            var candidateId = CVREF.CandidateId;
            offerAcceptedOn = emp.OfferAcceptanceConcludedOn;
            appno = await _context.GetApplicationNoFromCandidateId(candidateId);
        
            _context.Entry(CVREF).State = EntityState.Modified;

            //3 - Delete Deployments
            var dep = await _context.Deps.Where(x => x.CVRefId == cvrefid).Include(x => x.DepItems).FirstOrDefaultAsync();
            _context.Deps.Remove(dep);
            _context.Entry(dep).State = EntityState.Deleted;
            
            //4 = DELETE VOUCHERS
            if(offerAcceptedOn.Year > 2000) {
                var voucher = await _context.Vouchers
                    .Where(x => x.CVRefId == cvrefid && x.COAId == coaRecruitmentSales
                        && x.VoucherDated == offerAcceptedOn).FirstOrDefaultAsync();
                if(voucher != null) {
                    _context.Vouchers.Remove(voucher);
                    _context.Entry(voucher).State = EntityState.Deleted; 
                }
            }

            try {
                await _context.SaveChangesAsync();
            } catch (DbException ex) {
                ErrString = ex.Message;
            } catch (Exception ex) {
                ErrString = ex.Message;
            }
             
            return ErrString;
        }

        public async Task<bool> PostOfferAcceptanceWithNoSave(ICollection<PostOfferAcceptanceDto> dtos, string Username)
        {
            //dtos.does not contain these values: coaDR, COACR, CandidateId.  These valuess are to be populated below.
            //does not save to database
            
            //consider this to shift to HttpContext.AfterObjectUpdated filter
            var cvrefidsSELECTED = dtos.Where(x => x.ConclusionStatus.ToLower() == "accepted").Select(x => x.CVRefId).ToList();

            var cvrefidAndCandidateIdsSELECTED = await _context.CVRefs
                .Where(x => cvrefidsSELECTED.Contains(x.Id)).Select(x => new {cvrefid=x.Id, candidateid=x.CandidateId}).ToListAsync();
            
            var candidateids = cvrefidAndCandidateIdsSELECTED.Select(x => x.candidateid).ToList();
            var candidates = await _context.Candidates
                .Where(x => candidateids.Contains(x.Id)).Select(x => new{x.Id, x.ApplicationNo, x.FullName}).ToListAsync();
            var selectedDtos = dtos.Where(x => x.ConclusionStatus.ToLower()=="accepted").ToList();
            var coaSalesRecruitment = await _finRepo.GetSalesRecruitmentCOA() ?? throw new Exception("Recritment Sales Account not defined");
            foreach (var dto in selectedDtos) {
                //SELECTED candidates
                    var candidateid = cvrefidAndCandidateIdsSELECTED.Where(x => x.cvrefid==dto.CVRefId).Select(x => x.candidateid).FirstOrDefault();
                    if(candidateid != 0) {
                        dto.CandidateId = candidateid;
                        var applicationno = await _context.GetApplicationNoFromCandidateId(candidateid);
                        dto.coaDR = await _finRepo.CreateCoaForCandidateWithNoSave(applicationno, true);
                        if(dto.coaDR == null) continue;
                        if(dto.coaDR.Id ==0) _context.Entry(dto.coaDR).State = EntityState.Added;  //IF dto.coaDR.Id==0, it is a new object, to be saved
                    }
                    dto.coaCR = coaSalesRecruitment;   
                    var candidate = candidates.Where(x => x.Id==dto.CandidateId).FirstOrDefault();
                    var voucher = new Voucher {     //FinanceVoucher {
                        Divn="R", COAId= dto.coaCR.Id, AccountName = dto.coaCR.AccountName, 
                        Amount = dto.Charges, 
                        VoucherNo = await _finRepo.GetNextVoucherNo(),
                        VoucherDated = dto.ConclusionDate,
                        CVRefId = dto.CVRefId,
                        Narration="Offer accepted by " + candidate.FullName + ", App# " + candidate.ApplicationNo,
                        //VoucherEntries = voucherEntries
                        //VoucherItems = voucherEntries
                    };
                    _context.Entry(voucher).State =EntityState.Added;

                    var ids = await _context.GetOrderItemIdAndCustomerId(dto.CVRefId);
                    var process = new  Dep{     // Process{
                        CurrentStatus = "Selected", 
                        CVRefId=dto.CVRefId,
                        SelectedOn = dto.SelectedOn,
                        OrderItemId = ids.Count > 0 ? ids[0] : 0,
                        CustomerId = ids.Count > 0 ? ids[1] : 0
                        //ProcessItems = new List<ProcessItem>
                     };
                     _context.Entry(process).State = EntityState.Added;

                    await _context.SaveChangesAsync();

                    var voucherEntries = new List<VoucherItem>      // List<VoucherEntry>
                    {
                        new() { 
                                VoucherId = voucher.Id,
                                TransDate =  dto.ConclusionDate, 
                                COAId = dto.coaDR.Id,
                                AccountName = dto.coaDR.AccountName,
                                Narration = "Service charges applied, post offer acceptance by candidate",
                                Dr = dto.Charges},
                        new() {
                                VoucherId = voucher.Id,
                                TransDate = DateOnly.FromDateTime(DateTime.UtcNow), 
                                COAId = dto.coaCR.Id,
                                AccountName = dto.coaCR.AccountName, 
                                Narration = "Sales towards recruitment of the candidate",
                                Cr = dto.Charges}
                    };
                    foreach(var entry in voucherEntries) {_context.Entry(entry).State=EntityState.Added;}

                    //create Deployment record
                    var depItems = new List<DepItem>
                        {
                            new() {
                                DepId = process.Id,
                                TransactionDate = dto.ConclusionDate,
                                Sequence = 100,
                                NextSequence = 300,
                                NextSequenceDate = dto.ConclusionDate.AddDays(5)        
                            }
                        };

                     foreach(var depItem in depItems) { _context.Entry(depItem).State = EntityState.Added;}
            }  
            
            return true;
        }
        
        public async Task<bool> EditEmployment(Employment model, string Username )
        {
            var todayDateOnly = DateOnly.FromDateTime(DateTime.UtcNow);
         
            var existingObj = await _context.Employments
               .Where(p => p.Id == model.Id)
               .AsNoTracking()
               .SingleOrDefaultAsync() ?? throw new Exception("The Selection object does not exist in the database");
            
            _context.Entry(existingObj).CurrentValues.SetValues(model);   //saves only the parent, not children
            _context.Entry(existingObj).State = EntityState.Modified; 

            if (existingObj.OfferAccepted != model.OfferAccepted) {
                var postAcceptanceDto = new PostOfferAcceptanceDto
                {
                     ConclusionStatus = model.OfferAcceptanceUrl, Charges=model.Charges, SelectedOn=model.SelectedOn, 
                     ConclusionDate= model.OfferAcceptanceConcludedOn, CVRefId=model.CVRefId
                };
                var postAcceptanceDtos = new List<PostOfferAcceptanceDto>();
                postAcceptanceDtos.Add(postAcceptanceDto);

                var posted = await PostOfferAcceptanceWithNoSave(postAcceptanceDtos, Username);
                if(!posted) return false;
            }
            
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
            //selections as well as rejections are rgistered here
            var dtoToReturn = new MessageWithError();
            
            var cvrefids = selDtos.Select(x => x.CVRefId).ToList();
            //verify the CVRefIds are not already selected
            var cvrefsAlreadySelected = await _context.CVRefs
                .Where(x => cvrefids.Contains(x.Id) && x.SelectionStatus == "Selected").Select(x => x.Id).ToListAsync();
            if(cvrefsAlreadySelected.Count > 0) cvrefids = cvrefids
                .Where(x => !cvrefsAlreadySelected.Contains(x)).ToList();

            if(cvrefids.Count ==0) {
                dtoToReturn.ErrorString = "All the candidates are already selected";
                return dtoToReturn;
            }
            
            
            if(cvrefids.Count == 0) return null;

        //create selectiondecision records
            var selDetails = await  (from cvref in _context.CVRefs 
                    where cvrefids.Contains(cvref.Id)
                //join dtos in selDtos on cvref.Id equals dtos.CVRefId
                join item in _context.OrderItems on cvref.OrderItemId equals item.Id
                join checklist in _context.ChecklistHRs on new { cvref.CandidateId, cvref.OrderItemId }
                    equals new { checklist.CandidateId, checklist.OrderItemId }
                select new SelectionDecision
                {
                    CVRefId = cvref.Id,
                    OrderItemId = cvref.OrderItemId,
                    ProfessionName = item.Profession.ProfessionName,
                    ProfessionId = item.ProfessionId,
                    //SelectedOn = dtos.DecisionDate,
                    //SelectionStatus = dtos.SelectionStatus,
                    Charges = checklist.ChargesAgreed,
                    //Remarks = dtos.Remarks,
                    CVRef = cvref
                }).ToListAsync();

            foreach(var sel in selDetails) {
                sel.SelectedOn = selDtos.Where(x => x.CVRefId == sel.CVRefId).Select(x => x.DecisionDate).FirstOrDefault();
                sel.SelectionStatus =  selDtos.Where(x => x.CVRefId == sel.CVRefId).Select(x => x.SelectionStatus).FirstOrDefault();
                sel.Remarks = selDtos.Where(x => x.CVRefId == sel.CVRefId).Select(x => x.Remarks).FirstOrDefault();
                _context.Entry(sel).State = EntityState.Added;

                //update cvref fields 
                sel.CVRef.SelectionStatus = sel.SelectionStatus;
                sel.CVRef.SelectionStatusDate = DateOnly.FromDateTime(DateTime.UtcNow);
                _context.Entry(sel.CVRef).State = EntityState.Modified;
            }
         
           try {
            await _context.SaveChangesAsync();          //required before creating employment records
           } catch (DbException ex) {
                dtoToReturn.ErrorString += ex.Message;
           } catch (Exception ex){
                dtoToReturn.ErrorString +=ex.Message;
           }

           if(!string.IsNullOrEmpty(dtoToReturn.ErrorString)) return dtoToReturn;

        //create employment records
            var orderItemIdsForSelected = selDetails.Where(x => x.SelectionStatus=="Selected").Distinct().Select(x => x.OrderItemId).ToList();
            var remunerations = await _context.Remunerations
                .Where(x => orderItemIdsForSelected.Contains(x.OrderItemId))
                .ToListAsync();

            if(remunerations.Count > 0) {
                var employments = (from emp in selDetails where emp.SelectionStatus=="Selected" 
                    join item in _context.OrderItems on emp.OrderItemId equals item.Id
                    join remun in _context.Remunerations on item.Id equals remun.OrderItemId
                    join sel in _context.SelectionDecisions on emp.CVRefId equals sel.CVRefId
                    select new Employment
                    {
                            //SelectionDecisionId = sel.Id,
                            CVRefId = emp.CVRefId,
                            SelectedOn = emp.SelectedOn,
                            ChargesFixed = emp.Charges,
                            Charges = emp.Charges,
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
                    }).ToList();
            
                await Task.Delay(3000);     //hoping above synchronous function completes by then

                foreach(var emp in employments) {
                    _context.Entry(emp).State=EntityState.Added;
                }

            }
            
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
        
        //generate messages

            var selectedCVRefIds = selDetails.Where(x => x.SelectionStatus == "Selected").Select(x => x.CVRefId).ToList();

            if(selectedCVRefIds.Count > 0) {
                var selMessages = await ComposeSelMessagesToCandidates(selectedCVRefIds, Username);
                foreach(var msg in selMessages) { _context.Entry(msg).State = EntityState.Added;}
            } 
            
            var rejCVRefIds = selDetails.Where(x => x.SelectionStatus != "Selected").Select(x => x.CVRefId).ToList();
            if(rejCVRefIds.Count > 0) {
                var rejMessages = await ComposeRejMessagesToCandidates(rejCVRefIds, Username);
                foreach(var msg in rejMessages) { _context.Entry(msg).State = EntityState.Added;}
            }

            //update relevant tasks
            var tasks = await _context.Tasks.Where(x => cvrefids.Contains((int)x.CVRefId)).ToListAsync();
            if(tasks != null) {
                foreach(var task in tasks) {
                    task.TaskStatus = "Completed";
                    task.CompletedOn = _today;
                }
            }
        
            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateException ex) {
                dtoToReturn.ErrorString += ex.Message;
            } catch(Exception ex) {
                dtoToReturn.ErrorString += ", " + ex.Message;
            }

            if (string.IsNullOrEmpty(dtoToReturn.ErrorString)) return dtoToReturn;

            if(!string.IsNullOrEmpty(dtoToReturn.ErrorString)) dtoToReturn.ErrorString = "Selection/Rejection decisions registered, but failed to create Tasks and compose selection messages";
            return dtoToReturn;

        }

        public async Task<PagedList<EmploymentDto>> GetEmploymentsPaged(EmploymentParams empParams)
        {
            var query = _context.Employments.AsQueryable();

            /*if(empParams.SelectionDecisionIds.Count > 0) 
                query = query.Where(x => empParams.SelectionDecisionIds.Contains((int)x.SelectionDecisionId));
            */
            if(empParams.CVRefIds.Count > 0)
                query = query.Where(x => empParams.CVRefIds.Contains(x.CVRefId));
            
            if(empParams.SelectedOn.Year > 2000) query = query.Where(x => x.SelectedOn == empParams.SelectedOn);

             var paged = await PagedList<EmploymentDto>.CreateAsync(
                query.AsNoTracking()
                .ProjectTo<EmploymentDto>(_mapper.ConfigurationProvider)
                , empParams.PageNumber, empParams.PageSize);
    
          return paged;
        }

        public async Task<Employment> GetEmployment(int EmploymentId)
        {
            var employment = await _context.Employments.FindAsync(EmploymentId);

            return employment;
        }

        public async Task<string> RegisterOfferAcceptance(ICollection<OfferConclusionDto> dtos, string Username)
        {
            var ExistingEmps = await _context.Employments.Where(x => dtos.Select(x => x.EmploymentId).ToList().Contains(x.Id)).ToListAsync();
            
            if(ExistingEmps.Count == 0) return "employment data does not exist";

            var postOfferConclusionList = new List<PostOfferAcceptanceDto>();

            foreach(var emp in ExistingEmps) {
                var dto = dtos.FirstOrDefault(x => x.EmploymentId == emp.Id);
                emp.OfferAccepted = dto.acceptedString;
                emp.OfferAcceptanceConcludedOn = dto.ConclusionDate;
                emp.OfferConclusionRegisteredByUsername = Username;

                _context.Entry(emp).State = EntityState.Modified;

                postOfferConclusionList.Add(new PostOfferAcceptanceDto {
                    Charges = emp.Charges,
                    ConclusionDate = dto.ConclusionDate,
                    ConclusionStatus = dto.acceptedString,
                    CVRefId = emp.CVRefId,
                    SelectedOn = emp.SelectedOn
                });

            }

            var posted = await PostOfferAcceptanceWithNoSave(postOfferConclusionList, Username);
    
            return await _context.SaveChangesAsync() > 0 ? "" : "Failed to update database";
        }

        public async Task<PagedList<EmploymentsNotConcludedDto>> EmploymentsAwaitingConclusion(EmploymentParams empParams)
        {
             var query = _context.Employments.Where(x => x.OfferAccepted == null || x.OfferAccepted == "").AsQueryable();
             
             var paged = await PagedList<EmploymentsNotConcludedDto>.CreateAsync(
                query.AsNoTracking()
                .ProjectTo<EmploymentsNotConcludedDto>(_mapper.ConfigurationProvider),
                empParams.PageNumber, empParams.PageSize);

            return paged;
        }

        public async Task<SelectionDecision> GetSelectionDecisionFromCVRefId(int cvrefid)
        {
            return await _context.SelectionDecisions.Where(x => x.CVRefId == cvrefid).FirstOrDefaultAsync();
        }
    }
}