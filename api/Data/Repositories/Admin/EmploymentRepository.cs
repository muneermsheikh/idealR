using api.DTOs.Admin;
using api.Entities.Finance;
using api.Entities.HR;
using api.Helpers;
using api.Interfaces.Admin;
using api.Interfaces.Finance;
using api.Params.Admin;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using api.Extensions;
using api.Entities.Deployments;
using System.Data.Common;

namespace api.Data.Repositories.Admin
{
    public class EmploymentRepository : IEmploymentRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IFinanceRepository _finRepo;
        public EmploymentRepository(DataContext context, IMapper mapper, IFinanceRepository finRepo)
        {
            _finRepo = finRepo;
            _mapper = mapper;
            _context = context;
        }


        public async Task<Employment> GetOrGenerateEmploymentFromSelDecId(int selDecisionId)
        {
            var employmt = await _context.Employments.Where(x => x.SelectionDecisionId==selDecisionId).FirstOrDefaultAsync();

            if (employmt != null) return employmt;

            var query = await (from sel in _context.SelectionDecisions 
                    where sel.Id==selDecisionId
                join remun in _context.Remunerations on sel.OrderItemId equals remun.OrderItemId
                join chklst in _context.ChecklistHRs 
                    on new {sel.CandidateId, sel.OrderItemId} equals 
                    new {chklst.CandidateId, chklst.OrderItemId}
                select new Employment {
                    SelectionDecisionId = selDecisionId, CvRefId = sel.CvRefId,
                    Charges=chklst.ChargesAgreed, 
                    ChargesFixed=chklst.Charges, 
                    ContractPeriodInMonths=remun.ContractPeriodInMonths, FoodAllowance=remun.FoodAllowance, 
                    FoodNotProvided=remun.FoodNotProvided, FoodProvidedFree=remun.FoodProvidedFree,
                    HousingAllowance=remun.HousingAllowance, HousingNotProvided=remun.HousingNotProvided,
                    HousingProvidedFree=remun.HousingProvidedFree, LeavePerYearInDays=remun.LeavePerYearInDays,
                    LeaveAirfareEntitlementAfterMonths=remun.LeaveAirfareEntitlementAfterMonths,
                    OtherAllowance=remun.OtherAllowance, 
                    Salary=remun.SalaryMin, SalaryCurrency=remun.SalaryCurrency, SelectedOn=sel.SelectedOn,
                    TransportAllowance=remun.TransportAllowance,
                    TransportProvidedFree = remun.TransportProvidedFree, WeeklyHours = remun.WorkHours * 6
                }).FirstOrDefaultAsync();

            return query;
        }


        public async Task<string> DeleteEmployment(int employmentId)
        {
            var obj = await _context.Employments.FindAsync(employmentId);

            if (obj == null) return "No such employment object exists";

            _context.Employments.Remove(obj);
            _context.Entry(obj).State = EntityState.Deleted;

            try {
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                return ex.Message;
            }

            return "";
        }


        public async Task<bool> PostOfferAcceptanceWithNoSave(ICollection<PostOfferAcceptanceDto> dtos, string Username)
        {
            //dtos.does not contain these values: coaDR, COACR, CandidateId.  These valuess are to be populated below.
            //does not save to database
            
            //consider this to shift to HttpContext.AfterObjectUpdated filter

            //1 - debit candidate account if selected
            //2 - create new record in Deployment if selected
            //3 - update SelectionDecision.SelectionnStatus in all cases
            var cvrefidsSELECTED = dtos.Where(x => x.ConclusionStatus == "Accepted").Select(x => x.CVRefId).ToList();

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
                        var applicationno = candidates.Where(x => x.Id == dto.CandidateId).Select(x => x.ApplicationNo).FirstOrDefault();
                        dto.coaDR = await _finRepo.GetOrCreateCoaForCandidateWithNoSave(applicationno, true);  
                        if(dto.coaDR == null) continue;
                        if(dto.coaDR.Id ==0) _context.Entry(dto.coaDR).State = EntityState.Added;  //IF dto.coaDR.Id==0, it is a new object, to be saved
                    } else {
                        continue;       //this shd never happen
                    }

                    dto.coaCR = coaSalesRecruitment;   
                    var candidate = candidates.Where(x => x.Id==dto.CandidateId).FirstOrDefault();
 
                    var voucheritems = new List<VoucherEntry>
                    {
                        new() {TransDate = DateTime.Now,   //  dto.ConclusionDate, 
                            CoaId = dto.coaDR.Id, AccountName = dto.coaDR.AccountName,
                            Narration = "Service charges applied, post offer acceptance by candidate",
                            Dr = dto.Charges
                        },
                        new() {TransDate =DateTime.Now,     //VoucherId = voucher.Id,
                            CoaId = dto.coaCR.Id, AccountName = dto.coaCR.AccountName, 
                            Narration = "Sales towards recruitment of the candidate",
                            Cr = dto.Charges
                        }                    
                    };

                    var voucher = new FinanceVoucher {     //FinanceVoucher {
                        Divn="R", CoaId= dto.coaCR.Id, AccountName = dto.coaCR.AccountName, 
                        Amount = dto.Charges, 
                        VoucherNo = await _finRepo.GetNextVoucherNo(),
                        VoucherDated = DateTime.Now, //Iddto.ConclusionDate,
                        //CVRefId = dto.CVRefId,
                        Narration="Offer accepted by " + candidate.FullName + ", App# " + candidate.ApplicationNo,
                        VoucherEntries = voucheritems
                    };
                    _context.FinanceVouchers.Add(voucher);
           
                //2 - create Dep record
                    var ids = await _context.GetOrderItemIdAndCustomerId(dto.CVRefId);
                    var depitem = new DepItem{
                        TransactionDate = DateTime.Now, // dto.ConclusionDate,
                        Sequence = 100,
                        NextSequence = 300,
                        NextSequenceDate = dto.ConclusionDate.AddDays(5)        
                    };

                    var process = new  Dep{     // Process{
                        CurrentStatus = "Selected", 
                        CvRefId=dto.CVRefId,
                        SelectedOn = dto.SelectedOn,
                        OrderItemId = ids.Count > 0 ? ids[0] : 0,
                        CustomerId = ids.Count > 0 ? ids[1] : 0,
                        DepItems = new List<DepItem> { depitem }
                     };
                    
                    _context.Deps.Add(process);
            }  
            
            //3 - Update Selections table with SelectionStatus value
            var cvrefids = dtos.Select(x => x.CVRefId).ToList();
            var sels = await _context.SelectionDecisions.Where(x => cvrefids.Contains(x.CvRefId)).ToListAsync();
                
            foreach(var sel in sels) {
                sel.SelectionStatus = dtos.Where(x => x.CVRefId == sel.CvRefId).Select(x => x.ConclusionStatus).FirstOrDefault();
                _context.Entry(sel).State = EntityState.Modified;
            }

            return true;
        }
        

        public async Task<string> EditEmployment(Employment model, string Username)
        {
             var existingObj = await _context.Employments
               .Where(p => p.Id == model.Id)
               .AsNoTracking()
               .SingleOrDefaultAsync() ?? throw new Exception("The Employment object does not exist in the database");
            
            var offerAcceptChanged = string.IsNullOrEmpty(existingObj.OfferAccepted) || existingObj.OfferAccepted != model.OfferAccepted;

            _context.Entry(existingObj).CurrentValues.SetValues(model);   //saves only the parent, not children
            _context.Entry(existingObj).State = EntityState.Modified; 

            if (offerAcceptChanged) {
                var postAcceptanceDto = new PostOfferAcceptanceDto
                {
                     ConclusionStatus = model.OfferAccepted ?? "", 
                     Charges=model.Charges, 
                     SelectedOn=model.SelectedOn, 
                     ConclusionDate= model.OfferAcceptanceConcludedOn, 
                     CVRefId=model.CvRefId
                };
                var postAcceptanceDtos = new List<PostOfferAcceptanceDto>{postAcceptanceDto};

                var posted = await PostOfferAcceptanceWithNoSave(postAcceptanceDtos, Username);
                if(!posted) return "Failed to execute Post Offer Acceptance tasks";
            }
            
            try {
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                throw new Exception(ex.Message, ex);
            }

            return "";
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
                    CVRefId = emp.CvRefId,
                    SelectedOn = emp.SelectedOn
                });

            }

            var posted = await PostOfferAcceptanceWithNoSave(postOfferConclusionList, Username);
    
            return await _context.SaveChangesAsync() > 0 ? "" : "Failed to update database";
        }


        private static string VerifyEmploymentObject(Employment emp)
        {
            string ErrString = "";

            if(emp.Salary == 0) ErrString = "Salary not defined";
            if(string.IsNullOrEmpty(emp.SalaryCurrency)) ErrString += "Salary Currency not defined";
            if(emp.TransportAllowance ==0 && !emp.TransportProvidedFree && !emp.TransportNotProvided)
                ErrString += ". You must define atleast one of the 3 Transport parameters";
            if(emp.FoodAllowance == 0 && !emp.FoodProvidedFree && !emp.FoodNotProvided)
                ErrString += ". You must define atleast one of the 3 Food parameters";
            if(emp.HousingAllowance == 0 && !emp.HousingProvidedFree && !emp.HousingNotProvided)
                ErrString += ". You must define atleast one of the 3 Housing parameters";
            if(emp.LeaveAirfareEntitlementAfterMonths == 0) ErrString += ". Leave Airfare entitlement period not defined";
            if(emp.LeavePerYearInDays == 0) ErrString +=". Annual Leave not defined";
            
            return ErrString;

            
        }
        
        public async Task<PagedList<Employment>> GetEmployments(EmploymentParams empParams)
        {
            var query = _context.Employments.AsQueryable();

            if(empParams.CVRefIds.Count > 0) query = query.Where(x => empParams.CVRefIds.Contains(x.CvRefId));

            if(empParams.SelectedOn.Year > 2000) query = query.Where(x => x.SelectedOn == empParams.SelectedOn);
            if(empParams.SelectionDecisionIds.Count > 0) query = query.Where(x => empParams.SelectionDecisionIds.Contains(x.SelectionDecisionId));

            var paged = await PagedList<Employment>.CreateAsync(query.AsNoTracking()
                .ProjectTo<Employment>(_mapper.ConfigurationProvider),
                empParams.PageNumber, empParams.PageSize);
            
            return paged;
        }

        public async Task<string> SaveNewEmployment(Employment newObject)
        {
            var emp = await _context.Employments.Where(x => x.SelectionDecisionId==newObject.SelectionDecisionId).FirstOrDefaultAsync();

            if(emp != null) return "Employment Object for selection decision Id already exists";

            var strError = VerifyEmploymentObject(newObject);

            if(!string.IsNullOrEmpty(strError)) return strError;

            _context.Employments.Add(newObject);

            try {
                await _context.SaveChangesAsync();
            } catch (DbException ex) {
                strError = ex.Message;
            } catch (Exception ex) {
                strError = ex.Message;
            }

            return strError;
        }
    }
}