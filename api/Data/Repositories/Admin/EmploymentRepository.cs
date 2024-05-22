using api.Entities.HR;
using api.Helpers;
using api.Interfaces.Admin;
using api.Params.Admin;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Repositories.Admin
{
    public class EmploymentRepository : IEmploymentRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public EmploymentRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
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

        public async Task<string> EditEmployment(Employment employment)
        {
            string ErrString = "";

            var existing = await _context.Employments.Where(x => x.Id == employment.Id).AsNoTracking().FirstOrDefaultAsync();

            if (existing == null) return "the employment does not exist";

            ErrString = VerifyEmploymentObject(employment);

            if(!string.IsNullOrEmpty(ErrString)) return ErrString;

            _context.Entry(existing).CurrentValues.SetValues(employment);

            try {
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                ErrString = ex.Message;
            }
            
            return ErrString;

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

        public async Task<Employment> GenerateEmploymentObject(int selectionId)
        {
            var obj = await _context.Employments.Where(x => x.SelectionDecisionId == selectionId).FirstOrDefaultAsync();
            if(obj != null)  return null;

            var query = await (from sel in _context.SelectionDecisions where sel.Id == selectionId
                join cvref in _context.CVRefs on sel.CVRefId equals cvref.Id
                join item in _context.OrderItems on sel.OrderItemId equals item.Id
                join checklist in _context.ChecklistHRs on 
                    new {cvref.OrderItemId, cvref.CandidateId} equals 
                    new {checklist.OrderItemId, checklist.CandidateId}
                join itemAssess in _context.orderItemAssessments on item.Id equals itemAssess.OrderItemId
                    select new Employment {
                        SalaryCurrency = item.Remuneration.SalaryCurrency,
                        Salary = item.Remuneration.SalaryMin, SelectedOn = sel.SelectedOn,
                        SelectionDecisionId = sel.Id,
                        ContractPeriodInMonths = item.Remuneration.ContractPeriodInMonths,
                        WeeklyHours = item.Remuneration.WorkHours,
                        HousingAllowance=item.Remuneration.HousingAllowance,
                        HousingProvidedFree = item.Remuneration.HousingProvidedFree,
                        HousingNotProvided = item.Remuneration.HousingNotProvided,
                        FoodProvidedFree = item.Remuneration.FoodProvidedFree,
                        FoodAllowance = item.Remuneration.FoodAllowance,
                        FoodNotProvided = item.Remuneration.FoodNotProvided,
                        TransportAllowance = item.Remuneration.TransportAllowance,
                        TransportProvidedFree = item.Remuneration.TransportProvidedFree,
                        TransportNotProvided = item.Remuneration.TransportNotProvided,
                        OtherAllowance = item.Remuneration.OtherAllowance,
                        LeavePerYearInDays = item.Remuneration.LeavePerYearInDays,
                        LeaveAirfareEntitlementAfterMonths = item.Remuneration.LeaveAirfareEntitlementAfterMonths,
                        Charges = checklist.ChargesAgreed, ChargesFixed = checklist.Charges,
                        CVRefId = cvref.Id
                    }).FirstOrDefaultAsync();
            
            return query;
                
        }  

        public async Task<PagedList<Employment>> GetEmployments(EmploymentParams empParams)
        {
            var query = _context.Employments.AsQueryable();

            if(empParams.CVRefIds.Count > 0) query = query.Where(x => empParams.CVRefIds.Contains(x.CVRefId));

            if(empParams.SelectedOn.Year > 2000) query = query.Where(x => x.SelectedOn == empParams.SelectedOn);
            if(empParams.SelectionDecisionIds.Count > 0) query = query.Where(x => empParams.SelectionDecisionIds.Contains(x.SelectionDecisionId));

            var paged = await PagedList<Employment>.CreateAsync(query.AsNoTracking()
                .ProjectTo<Employment>(_mapper.ConfigurationProvider),
                empParams.PageNumber, empParams.PageSize);
            
            return paged;
        }

        public async Task<string> SaveNewEmployment(Employment employment)
        {
            var strError = VerifyEmploymentObject(employment);

            if(!string.IsNullOrEmpty(strError)) return strError;

            try {
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                strError = ex.Message;
            }

            return strError;
        }
    }
}