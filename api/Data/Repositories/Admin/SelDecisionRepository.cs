//using System.Diagnostics;
using System.Data.Common;
using api.DTOs.Admin;
using api.Entities.Deployments;
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
        private readonly DateTime _today = DateTime.UtcNow;
        private readonly IConfiguration _config;
        private readonly string _tempPassword;
        public SelDecisionRepository(DataContext context, IComposeMessagesAdminRepository msgAdmRepo, 
           IFinanceRepository finRepo , IMapper mapper, UserManager<AppUser> userManager, IConfiguration config)
        {
            _config = config;
            _userManager = userManager;
            _mapper = mapper;
            _finRepo = finRepo;
            _msgAdmRepo = msgAdmRepo;
            _context = context;
            _tempPassword = _config["tempPassword"];
        }

        public async Task<MessageWithError> ComposeRejMessagesToCandidates(List<int> cvrefids, string Username)
        {
            var msgErr = new MessageWithError();

            if(cvrefids.Count == 0) return null;

            var rejectedDetails = await (from cvref in _context.CVRefs 
                where cvrefids.Contains(cvref.Id) 
                join sel in _context.SelectionDecisions on cvref.Id equals sel.CvRefId 
                    where sel.SelectionStatus.Contains("Rejected")  
                join item in _context.OrderItems on cvref.OrderItemId equals item.Id 
                join rvwitem in _context.ContractReviewItems on item.Id equals rvwitem.OrderItemId
                join order in _context.Orders on item.OrderId equals order.Id
                join cv in _context.Candidates on cvref.CandidateId equals cv.Id
                
                select new SelectionMessageDto {
                    CustomerName = order.Customer.CustomerName,
                    CustomerCity = order.Customer.City,
                    OrderNo = order.OrderNo,
                    ProfessionName = item.Profession.ProfessionName,
                    SelectionStatus = sel.SelectionStatus,
                    //RejectionReason = sel.RejectionReason,
                    ApplicationNo = cv.ApplicationNo,
                    CandidateId = cv.Id,
                    CandidateTitle = cv.Gender == "M" ? "Mr." : "Ms.",
                    CandidateName = cv.FullName,
                    CandidateGender = cv.Gender,
                    CandidateAppUserId = cv.AppUserId,

                    HrExecUsername = rvwitem.HrExecUsername
                    
                }).ToListAsync();

            string ErrorString="";
            ErrorString = await VerifyDataAvailableForSelMessages(rejectedDetails);
            if(!string.IsNullOrEmpty(ErrorString)) {
                msgErr.ErrorString = ErrorString;
                return msgErr;
            }
            var msgs = await _msgAdmRepo.AdviseRejectionStatusToCandidateByEmail(rejectedDetails, Username);
            
            msgErr.Messages = msgs.Messages;

            return msgErr;
        }

        public async Task<ICollection<Message>> ComposeSelMessagesToCandidates(List<int> cvrefids, string Username)
        {
            if(cvrefids.Count == 0) return null;

            var selectedDetails = await (from cvref in _context.CVRefs where cvrefids.Contains(cvref.Id)
                join sel in _context.SelectionDecisions on cvref.Id equals sel.CvRefId 
                join item in _context.OrderItems on cvref.OrderItemId equals item.Id 
                join order in _context.Orders on item.OrderId equals order.Id
                join cv in _context.Candidates on cvref.CandidateId equals cv.Id
                join emp in _context.Employments on sel.CvRefId equals emp.CvRefId
                
                select new SelectionMessageDto {
                    CustomerName = order.Customer.CustomerName,
                    CustomerCity = order.Customer.City,
                    OrderNo = order.OrderNo,
                    ProfessionName = item.Profession.ProfessionName,
                    SelectionStatus = sel.SelectionStatus,
                    //RejectionReason = sel.RejectionReason,
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

        
        public async Task<string> ComposeAcceptanceReminderToCandidates(List<int> cvrefids, string Username)
        {
            if(cvrefids.Count == 0) return null;

            var selectedDetails = await (from cvref in _context.CVRefs where cvrefids.Contains(cvref.Id)
                join sel in _context.SelectionDecisions on cvref.Id equals sel.CvRefId 
                join item in _context.OrderItems on cvref.OrderItemId equals item.Id 
                join order in _context.Orders on item.OrderId equals order.Id
                join cv in _context.Candidates on cvref.CandidateId equals cv.Id
                join emp in _context.Employments on sel.CvRefId equals emp.CvRefId
                
                select new SelectionMessageDto {
                    CustomerName = order.Customer.CustomerName,
                    CustomerCity = order.Customer.City,
                    OrderNo = order.OrderNo,
                    ProfessionName = item.Profession.ProfessionName,
                    SelectionStatus = sel.SelectionStatus,
                    //RejectionReason = sel.RejectionReason,
                    ApplicationNo = cv.ApplicationNo,
                    CandidateId = cv.Id,
                    CandidateTitle = cv.Gender == "M" ? "Mr." : "Ms.",
                    CandidateName = cv.FullName,
                    CandidateGender = cv.Gender,
                    CandidateAppUserId = cv.AppUserId,
                    Employment = emp
            }).ToListAsync();
            
            string ErrorString="";
            ErrorString = await VerifyDataAvailableForSelMessages(selectedDetails);
            if(!string.IsNullOrEmpty(ErrorString)) throw new Exception(ErrorString) ;

            var msgErr = await _msgAdmRepo.ComposeAcceptanceReminderToCandidates(selectedDetails, Username);

            if(!string.IsNullOrEmpty(msgErr.ErrorString)) {
                return msgErr.ErrorString;
            }

            foreach(var msg in msgErr.Messages) {
                _context.Messages.Add(msg);
            }

            return await _context.SaveChangesAsync() > 0 ? "" : "Failed to save messages";

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
                
                await _userManager.CreateAsync(newAppUser, _tempPassword);
            }

            if(newAppUser != null) {
                empObj.AppUserId = newAppUser.Id;
                _context.Entry(empObj).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            return newAppUser;

        }

        private async Task<AppUser> AppUserFromCandidateId(int CandidateId) {
            var candidateObj = await _context.Candidates.FindAsync(CandidateId);
            if(candidateObj == null) return null;

            if (candidateObj.AppUserId  != 0) return await _userManager.FindByIdAsync(candidateObj.AppUserId.ToString());
            
            //user does not have appuser object
            
            //there might be an AppUser record for the candidate, but the candidate object does not have the AppUserId info
            var newAppUser = new AppUser();

            if(!string.IsNullOrEmpty(candidateObj.UserName)) {
                newAppUser = await _userManager.FindByNameAsync(candidateObj.UserName);
            }
            
            if(newAppUser.Id == 0) {        //appuser not found
                newAppUser = new AppUser{
                    Gender = candidateObj.Gender ?? "Male",
                    KnownAs=candidateObj.KnownAs ?? "",
                    //if(candidateObj.DOB != null) newAppUser.DateOfBirth= (DateTime)candidateObj.DOB; 
                    Created =_today,
                    City = candidateObj.City ?? "",
                    Country = candidateObj.Country ?? "India",
                    Email = candidateObj.Email ?? "",
                    UserName = candidateObj.Email,
                    PhoneNumber = candidateObj.UserPhones == null ? "" : candidateObj.UserPhones.Where(x => x.IsMain).Select(x => x.MobileNo).FirstOrDefault()
                };
                
                var result = await _userManager.CreateAsync(newAppUser, _tempPassword);
                if(result.Succeeded) newAppUser = await _userManager.FindByEmailAsync(candidateObj.Email);
            }

            if(newAppUser.Id != 0) {
                candidateObj.AppUserId = newAppUser.Id;
                _context.Entry(candidateObj).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            
            return newAppUser.Id == 0 ? null : newAppUser;

        }

        private async Task<string> VerifyDataAvailableForSelMessages(ICollection<SelectionMessageDto> selMessages)
        {
            string ErrString="";
            //check if senderObj and RecipientObj have valid data
            foreach(var item in selMessages) {
                var recipientObj = await AppUserFromCandidateId(item.CandidateId);
                if(recipientObj == null) ErrString += ", Failed to get recipient AppUser object";
                if(string.IsNullOrEmpty(item.SelectionStatus)) ErrString += ", " + "Selection Status not defined for " + item.CandidateKnownAs;

                /*var empObj = item.HRSupId == 0 
                    ? await AppUserFromEmployeeId(0, item.HRExecEmail)
                    : await AppUserFromEmployeeId(item.HRSupId, "");
                */
             }
             return ErrString;
        }

        public async Task<string> DeleteSelection(int selectionId)
        {
            var selections = await _context.SelectionDecisions.ToListAsync();
            var employments = await _context.Employments.ToListAsync();
            var vouchers = await _context.FinanceVouchers.ToListAsync();
            var deps = await _context.Deps.ToListAsync();

            //Deletebehavior.Cascade for Selections and other related tables cd not be set, so all related
            //records are deleted manually
            var obj = await _context.SelectionDecisions.FindAsync(selectionId);
            if(obj == null) return "no selection object by that Selection Id";
            var cvrefid = obj.CvRefId;
            DateTime offerAcceptedOn;
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
            var emp = await _context.Employments.Where(x => x.CvRefId==cvrefid).FirstOrDefaultAsync();
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
            var dep = await _context.Deps.Where(x => x.CvRefId == cvrefid).Include(x => x.DepItems).FirstOrDefaultAsync();
            _context.Deps.Remove(dep);
            _context.Entry(dep).State = EntityState.Deleted;
            
            //4 = DELETE VOUCHERS
            if(offerAcceptedOn.Year > 2000) {
                var voucher = await _context.FinanceVouchers
                    .Where(x => x.COAId == coaRecruitmentSales
                        &&  x.VoucherDated == DateOnly.FromDateTime(offerAcceptedOn)).FirstOrDefaultAsync();
                if(voucher != null) {
                    _context.FinanceVouchers.Remove(voucher);
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
        public async Task<bool> EditSelection(SelectionDecision model)
        {
            // thanks to @slauma of stackoverflow
            //model.ProfessionId = await _context.prof

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
            var query = (from sel in _context.SelectionDecisions 
                join item in _context.OrderItems on sel.OrderItemId equals item.Id
                join ord in _context.Orders on item.OrderId equals ord.Id
                join cvref in _context.CVRefs on sel.CvRefId equals cvref.Id
                join cv in _context.Candidates on cvref.CandidateId equals cv.Id
                select new SelDecisionDto {
                    CvRefId = cvref.Id,
                    ApplicationNo = cv.ApplicationNo, 
                    ProfessionId = item.ProfessionId,
                    CandidateId = cv.Id,
                    CandidateName = cv.FullName,
                    CategoryRef = ord.OrderNo + "-" + item.SrNo + "-" + sel.SelectedAs,
                    CustomerName = ord.Customer.CustomerName, 
                    ReferredOn = cvref.ReferredOn,
                    Id = sel.Id,
                    SelectedOn = sel.SelectedOn, 
                    SelectionStatus = sel.SelectionStatus
                }).AsQueryable();

            if(selParams.CVRefId > 0) query = query.Where(x => x.CvRefId == selParams.CVRefId);
            if(selParams.OrderItemId > 0) query = query.Where(x => x.OrderItemId == selParams.OrderItemId);
            //if(selParams.ProfessionId > 0) query = query.Where(x => x.ProfessionId == selParams.ProfessionId);
            if(selParams.SelectedOn.Year > 2000) query = query.Where(x => DateOnly.FromDateTime(x.SelectedOn) == DateOnly.FromDateTime(selParams.SelectedOn));

            var paged = await PagedList<SelDecisionDto>.CreateAsync(
                query.AsNoTracking(), selParams.PageNumber, selParams.PageSize);
    
          return paged;

        }
        

        public async Task<MessageWithError> RegisterSelections(ICollection<CreateSelDecisionDto> selDtos, string Username)
        {
            //1 - saves selection/rejections to SelectionDecision table
            //2 - if selected, create employment and deployment records
            //3 - compose email messages to all candidates - selected or rejected, advising them

            //selections as well as rejections are rgistered here
            var dtoToReturn = new MessageWithError();
            
            var cvrefids = selDtos.Select(x => x.CVRefId).ToList();
            //verify the CVRefIds are not already selected
            var excludeCVRefsAlreadySelected = await _context.CVRefs
                .Where(x => cvrefids.Contains(x.Id) && x.SelectionStatus == "Selected").Select(x => x.Id).ToListAsync();
            //exclude already selected records from cvrefids
            if(excludeCVRefsAlreadySelected.Count > 0) cvrefids = cvrefids
                .Where(x => !excludeCVRefsAlreadySelected.Contains(x)).ToList();

            if(cvrefids.Count ==0) {
                dtoToReturn.ErrorString = "All the candidates are already selected";
                return dtoToReturn;
            }

            //create selectiondecision records
            var selDetails = await  (from cvref in _context.CVRefs 
                    where cvrefids.Contains(cvref.Id) && 
                    (cvref.SelectionStatus == null || cvref.SelectionStatus == "")
                //join dtos in selDtos on cvref.Id equals dtos.CVRefId
                join cv in _context.Candidates on cvref.CandidateId equals cv.Id
                join item in _context.OrderItems on cvref.OrderItemId equals item.Id
                join order in _context.Orders on item.OrderId equals order.Id
                join checklist in _context.ChecklistHRs on new { cvref.CandidateId, cvref.OrderItemId }
                    equals new { checklist.CandidateId, checklist.OrderItemId }
                select new SelectionDecision
                {
                    CvRefId = cvref.Id,
                    OrderItemId = cvref.OrderItemId,
                    SelectedAs = item.Profession.ProfessionName,
                    ProfessionId = item.ProfessionId,
                    CustomerId = order.CustomerId,
                    CustomerName = order.Customer.CustomerName,
                    CityOfWorking = order.CityOfWorking,
                    Charges = checklist.ChargesAgreed,
                    CandidateId = cvref.CandidateId,
                    ApplicationNo = cv.ApplicationNo,
                    Gender = cv.Gender,
                    //Remarks = dtos.Remarks,
                    CVRef = cvref
                }).ToListAsync();

            if(selDetails.Count == 0) {
                dtoToReturn.ErrorString = "No Valid Referral Data available";
                return dtoToReturn;
            }

            //above query does not exclude selDtos, as that wd cause error at client processing; once the query is
            //retrieved, update selectedOn and selectionStatus
            foreach(var sel in selDetails) {
                    
                sel.SelectedOn = selDtos.Where(x => x.CVRefId == sel.CvRefId).Select(x => Convert.ToDateTime(x.DecisionDate)).FirstOrDefault();
                sel.SelectionStatus = selDtos.Where(x => x.CVRefId == sel.CvRefId).Select(x => x.SelectionStatus).FirstOrDefault();
                sel.Remarks =selDtos.Where(x => x.CVRefId == sel.CvRefId).Select(x => x.Remarks).FirstOrDefault();
                
                _context.Entry(sel).State = EntityState.Added;

                //update cvref fields 
                sel.CVRef.SelectionStatus = sel.SelectionStatus;
                sel.CVRef.SelectionStatusDate = DateTime.UtcNow;
                _context.Entry(sel.CVRef).State = EntityState.Modified;

                var depItems = new List<DepItem>();
                var depItem = new DepItem{TransactionDate=_today, Sequence=100,NextSequence=300, NextSequenceDate=_today.AddDays(3)};
                depItems.Add(depItem);

                //create deployment record
                var dep = new Dep
                { CvRefId = sel.CvRefId, OrderItemId = sel.OrderItemId, CustomerId = sel.CustomerId,
                    SelectedOn = sel.SelectedOn, CurrentStatus = "Selected", CurrentStatusDate = _today,
                    CustomerName = sel.CustomerName, CityOfWorking = sel.CityOfWorking, DepItems=depItems
                };
                
                _context.Deps.Add(dep);
             }
         
            try {
                await _context.SaveChangesAsync();          //required before creating employment records
            } catch (DbException ex) {
                    dtoToReturn.ErrorString += ex.Message;
            } catch (Exception ex){
                    dtoToReturn.ErrorString +=ex.Message;
            }

            if(!string.IsNullOrEmpty(dtoToReturn.ErrorString)) return dtoToReturn;

            dtoToReturn.CVRefIdsInserted=cvrefids;

            //2 - create employment records
            var orderItemIdsForSelected = selDetails.Where(x => x.SelectionStatus=="Selected").Distinct().Select(x => x.OrderItemId).ToList();
            
            if(orderItemIdsForSelected.Count > 0) {

                var remunerations = await _context.Remunerations
                    .Where(x => orderItemIdsForSelected.Contains(x.OrderItemId))
                    .ToListAsync();

                if(remunerations.Count > 0) 
                {
                    var employments = (from emp in selDetails where emp.SelectionStatus=="Selected" 
                        join item in _context.OrderItems on emp.OrderItemId equals item.Id
                        join remun in _context.Remunerations on item.Id equals remun.OrderItemId
                        join sel in _context.SelectionDecisions on emp.CvRefId equals sel.CvRefId
                        select new Employment
                        {
                            SelectionDecisionId = sel.Id,
                            CvRefId = emp.CvRefId,
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

            var selectedCVRefIds = selDetails.Where(x => x.SelectionStatus == "Selected").Select(x => x.CvRefId).ToList();

            if(selectedCVRefIds.Count > 0) {
                var selMessages = await ComposeSelMessagesToCandidates(selectedCVRefIds, Username);
                foreach(var msg in selMessages) { _context.Entry(msg).State = EntityState.Added;}
            } 
            
            var rejCVRefIds = selDetails.Where(x => x.SelectionStatus != "Selected").Select(x => x.CvRefId).ToList();
            if(rejCVRefIds.Count > 0) {
                var msgs = await ComposeRejMessagesToCandidates(rejCVRefIds, Username);
                if(msgs.Messages.Count > 0) {
                    foreach(var msg in msgs.Messages) { _context.Entry(msg).State = EntityState.Added;}
                } else {
                    dtoToReturn.ErrorString = msgs.ErrorString;
                    return dtoToReturn;
                }
            } 

            //update relevant tasks
            var tasks = await _context.Tasks.Where(x => 
                x.TaskType=="SelectionFollowupWithClient" 
                && cvrefids.Contains((int)x.CVRefId)).ToListAsync();
            
            if(tasks != null) {
                foreach(var task in tasks) {
                    task.TaskStatus = "Completed";
                    task.CompletedOn = _today;
                    _context.Entry(task).State = EntityState.Modified;
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
                query = query.Where(x => empParams.CVRefIds.Contains(x.CvRefId));
            
            if(empParams.SelectedOn.Year > 2000) query = query
                .Where(x => DateOnly.FromDateTime(x.SelectedOn) == DateOnly.FromDateTime(empParams.SelectedOn));

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
        
        public async Task<Employment> GetEmploymentFromSelDecId(int selDecisionId)
        {
            var query = await (from emp in _context.Employments where emp.SelectionDecisionId == selDecisionId
                join cvref in _context.CVRefs on emp.CvRefId equals cvref.Id
                join item in _context.OrderItems on cvref.OrderItemId equals item.Id
                join cat in _context.Professions on item.ProfessionId equals cat.Id
                join order in _context.Orders on item.OrderId equals order.Id
                join cand in _context.Candidates on cvref.CandidateId equals cand.Id
                join chklst in _context.ChecklistHRs 
                    on new {cvref.CandidateId, cvref.OrderItemId} equals new {chklst.CandidateId, chklst.OrderItemId}
                select new Employment {
                    Id = emp.Id, SelectionDecisionId = selDecisionId, CvRefId = cvref.Id,
                    //ApplicationNo = cand.ApplicationNo, CandidateName=cand.FullName, 
                    //CompanyName=order.Customer.CustomerName, 
                    Charges=chklst.ChargesAgreed, 
                    ChargesFixed=chklst.Charges, 
                    ContractPeriodInMonths=emp.ContractPeriodInMonths, FoodAllowance=emp.FoodAllowance, 
                    FoodNotProvided=emp.FoodNotProvided, FoodProvidedFree=emp.FoodProvidedFree,
                    HousingAllowance=emp.HousingAllowance, HousingNotProvided=emp.HousingNotProvided,
                    HousingProvidedFree=emp.HousingProvidedFree, LeavePerYearInDays=emp.LeavePerYearInDays,
                    LeaveAirfareEntitlementAfterMonths=emp.LeaveAirfareEntitlementAfterMonths,
                    OfferAccepted=emp.OfferAccepted, 
                    OfferAcceptanceConcludedOn=emp.OfferAcceptanceConcludedOn,
                    OtherAllowance=emp.OtherAllowance, 
                    //ProfessionName=cat.ProfessionName,
                    Salary=emp.Salary, SalaryCurrency=emp.SalaryCurrency, SelectedOn=emp.SelectedOn,
                    TransportAllowance=emp.TransportAllowance,
                    TransportProvidedFree = emp.TransportProvidedFree, WeeklyHours = emp.WeeklyHours
                }).FirstOrDefaultAsync();

            return query;
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
            return await _context.SelectionDecisions.Where(x => x.CvRefId == cvrefid).FirstOrDefaultAsync();
        }

        public async Task<SelDecisionDto> GetSelDecisionDtoFromId(int selDecisionId)
        {
            var query = await (from sel in _context.SelectionDecisions where sel.Id == selDecisionId
                    join cvref in _context.CVRefs on sel.CvRefId equals cvref.Id
                    join item in _context.OrderItems on sel.OrderItemId equals item.Id
                    join order in _context.Orders on item.OrderId equals order.Id
                    join cand in _context.Candidates on cvref.CandidateId equals cand.Id
                    
                select new SelDecisionDto {
                    ApplicationNo = cand.ApplicationNo, CandidateName=cand.FullName, 
                    CandidateId = cand.Id, ProfessionId = item.ProfessionId,
                    CategoryRef= order.OrderNo + "-" + item.SrNo + "-" +  sel.SelectedAs,
                    Charges = sel.Charges, CustomerName=order.Customer.CustomerName, CvRefId=sel.CvRefId,
                    OrderItemId=sel.OrderItemId, SelectedAs=sel.SelectedAs,
                    ReferredOn = cvref.ReferredOn, Id = sel.Id, SelectedOn = sel.SelectedOn,
                    SelectionStatus = sel.SelectionStatus
                }).FirstOrDefaultAsync();

            return query;
        }

    }
}