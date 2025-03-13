using api.DTOs.Admin;
using api.DTOs.Customer;
using api.DTOs.HR;
using api.DTOs.Process;
using api.Entities.HR;
using api.Entities.Identity;
using api.Extensions;
using api.Interfaces;
using api.Interfaces.Finance;
using api.Interfaces.HR;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        //private readonly IProspectiveCandidatesRepository _prospRepo;
        private readonly IFinanceRepository _finRepo;
        public UserRepository(DataContext context, IFinanceRepository finRepo,//IProspectiveCandidatesRepository prospRepo, 
            UserManager<AppUser> userManager, IMapper mapper)
        {
            _finRepo = finRepo;
            //_prospRepo = prospRepo;
            _userManager = userManager;
            _mapper = mapper;
            _context = context;
        }

        
        public async Task<AppUser> GetCandidateAsync(string username)
        {
            return await _context.Users.Where(x => x.UserName == username)
                //.ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<ICollection<CVsMatchingProfAvailableDto>> GetMatchingCandidatesAvailable(int professionid)
        {
            var query2 = await (from prof in _context.UserProfessions where prof.ProfessionId == professionid
                    join cv in _context.Candidates on prof.CandidateId equals cv.Id where
                        "!traveledcanceledselectedblacklisted".Contains(cv.Status.ToLower())
                    join ph in _context.UserPhones on cv.Id equals ph.CandidateId where ph.IsValid
                    orderby cv.ApplicationNo
                    select new CVsMatchingProfAvailableDto {
                        ApplicationNo = cv.ApplicationNo, City = cv.City, FullName = cv.FullName, 
                        PersonId = "", CandidateId = cv.Id, Gender=  cv.Gender, Checked = false, 
                        ProfessionName = prof.ProfessionName, Source="Candidates", MobileNo=ph.MobileNo,
                        ProspectiveCandidateId = 0
                    }).ToListAsync();

            var query = await (from prosp in _context.ProspectiveCandidates where prosp.ProfessionId==professionid
                select new CVsMatchingProfAvailableDto {
                    PersonId = prosp.PersonId, City = prosp.CurrentLocation, FullName = prosp.CandidateName,
                    CandidateId = 0, Gender = prosp.Gender, Checked=false, Source = "Prospectives", 
                    ProfessionName=prosp.ProfessionName, MobileNo = prosp.PhoneNo,
                    ProspectiveCandidateId = prosp.Id
                }).ToListAsync();
            
            foreach(var q in query) {
                query2.Add(q);
            }

            return query2;
        }
        public async Task<AppUser> GetUserByIdAsync(int id)
        {
                return await _context.Users.Where(x => x.Id == id)
                //.ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<AppUser> GetUserByUserNameAsync(string username)
        {
            var dto = await _context.Users
                .Where(x => x.UserName == username)
                //.ProjectTo<AppUser>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();

            return dto;
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;

        }

        public async Task<bool> DeleteMember(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Remove(user);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<AppUserReturnDto> CreateAppUser(string userType, int userTypeValue, string loggedInUsername)
        {
            var dtoErr = new AppUserReturnDto();

            var appuser = new AppUser();
            var role = "";
     
            switch (userType.ToLower()) {
                case "employee":
                    var emp = await _context.Employees.FindAsync(userTypeValue);
                    if(emp == null) {
                        dtoErr.Error = "Employee Not on record";
                        return dtoErr;
                    }
                    if(string.IsNullOrEmpty(emp.Email)) {
                        dtoErr.Error = "Email not available";
                        return dtoErr;
                    }
                    var app = await _userManager.FindByNameAsync(emp.UserName);
                    if(app != null)  {
                        dtoErr.AppUserId=app.Id;
                        dtoErr.Username = app.UserName;
                        return dtoErr;
                    }

                    appuser = new AppUser{Gender=emp.Gender, KnownAs = emp.KnownAs, UserName = emp.UserName ?? emp.Email,
                        DateOfBirth = emp.DateOfBirth, Created = DateTime.UtcNow, City = emp.City, Position = emp.Position};

                    role="Employee";
                    break;
                
                case "official":
                    var off = await _context.CustomerOfficials.FindAsync(userTypeValue);
                    if(string.IsNullOrEmpty(off.Email)) {
                        dtoErr.Error = "official not on record";
                        return dtoErr;
                    }
                    appuser = await _context.Users.Where(x => x.Email == off.Email).FirstOrDefaultAsync();
                    if(appuser != null) {
                        dtoErr.AppUserId = appuser.Id;
                        dtoErr.Username = appuser.UserName;
                        return dtoErr;
                    }

                    appuser = new AppUser{Gender=off.Gender, KnownAs=off.OfficialName, PhoneNumber = off.PhoneNo,
                        Email = off.Email, Created = DateTime.UtcNow, UserName=off.Email};
                    role = "Official";

                    break;

                case "candidate":
                    var candd =  await _context.Candidates.FindAsync(userTypeValue);
                    if(candd == null) {
                        dtoErr.Error = "Candidate Not on record";
                        return dtoErr;
                    }
                    if(string.IsNullOrEmpty(candd.Email)) {
                        dtoErr.Error = "Candidate not on record";
                        return dtoErr;
                    } 
                    
                    appuser = await _context.Users.Where(x => x.Email == candd.Email).FirstOrDefaultAsync();
                    if(appuser != null) {
                        dtoErr.AppUserId = appuser.Id;
                        dtoErr.Username = appuser.UserName;
                        return dtoErr;
                    }

                    appuser = new AppUser{Gender = candd.Gender, KnownAs = candd.KnownAs, Email = candd.Email,
                    PhoneNumber = candd.UserPhones.Where(x => x.IsMain && x.IsValid).Select(x => x.MobileNo).FirstOrDefault(),
                    City = candd.City, Country = candd.Country, //DateOfBirth = Convert.ToDateTime(cand.DOB),
                    Created = DateTime.UtcNow, UserName = candd.Email};

                    role = "Candidate";
                    break;

                default:
                    break;
            }

            var created = await _userManager.CreateAsync(appuser, "Pa$$w0rd");
            if(created.Succeeded) {
                var roleUpdated = await _userManager.AddToRoleAsync(appuser, role);
            } else if(!created.Succeeded) {
                dtoErr.Error = created.Errors.FirstOrDefault().Description;
                return dtoErr;
            }

            dtoErr.AppUserId = appuser.Id;
            dtoErr.Username = appuser.UserName;

            return dtoErr;
        }

        public async Task<NextDepDataDto> GetNextRecruitmentProcess(string PPNo) 
        {
            var dtoRet = new NextDepDataDto();
            
            var candExists = await _context.Candidates.Where(x => x.PpNo.ToLower() == PPNo.ToLower())
                .Select(x => new {FullName=x.FullName, ApplicationNo=x.ApplicationNo, Id=x.Id, PpNo=x.PpNo})
                .FirstOrDefaultAsync();
            
            if(candExists == null) {
                dtoRet.ErrorString = "Invalid Passport No";
                return dtoRet;
            }  

            var refs = new List<Referral>();
            
            var cvrefs = await _context.CVRefs.Where(x => x.CandidateId==candExists.Id && x.RefStatus.ToLower() != "concluded").ToListAsync();
            if(cvrefs.Count == 0) {
                dtoRet.ErrorString = "Mr. " + candExists.FullName + ", PP No." + candExists.PpNo + ", application " + candExists.ApplicationNo +
                    " is not yet referred.  Please shortlist the candidate for referral to customer, and then use the CV Referral " +
                    "command to refer the candidate";
                return dtoRet; 
            }

            foreach(var cvreferred in cvrefs) {
                var query = await(from cvref in _context.CVRefs where cvref.Id == cvreferred.Id
                    join orderitem in _context.OrderItems on cvref.OrderItemId equals orderitem.Id
                    join order in _context.Orders on orderitem.OrderId equals order.Id
                    join dep in _context.Deps on cvref.Id equals dep.CvRefId
                    join item in _context.DepItems on dep.Id equals item.DepId
                    orderby item.TransactionDate descending
                    select new {ApplicationNo=candExists.ApplicationNo, CandidateName = candExists.FullName,
                        CustomerName=order.Customer.CustomerName, CategoryRef = cvref.CategoryRef, 
                        ReferredOn = cvref.ReferredOn, DepId = dep.Id, 
                        NextSequence = item.NextSequence }
                ).Take(1).FirstOrDefaultAsync();

                if(query != null) {
                    var nextProcessName = await _context.DeployStatuses.Where(x => x.Sequence == query.NextSequence)
                        .Select(x => new {StatusName=x.StatusName, Period=x.WorkingDaysReqdForNextStage}).FirstOrDefaultAsync();

                    var referr = new Referral{CategoryRef=query.CategoryRef,
                        CustomerName = query.CustomerName, DepId=query.DepId, 
                        Period=nextProcessName.Period, ReferredOn=query.ReferredOn, Sequence=query.NextSequence,
                        SequenceName=nextProcessName.StatusName};

                    refs.Add(referr);
                }
            }
            
            return new NextDepDataDto{ApplicationNo=candExists.ApplicationNo, CandidateName=candExists.FullName,
                Referrals = refs};
        }

        public async Task<CandidateDto> GetUserHistory(int CandidateId)
        {
            //verify candidateId is valid
            var Candidate = await _context.Candidates.Where(x => x.Id==CandidateId).Include(x => x.UserProfessions).FirstOrDefaultAsync();
            
            if(Candidate == null) return null;

            var histories = new List<CandidateHistoryDto>();

            var bal = await _finRepo.CandidateBalance(Candidate.ApplicationNo);

            var refs = await (from cvref in _context.CVRefs where cvref.CandidateId == CandidateId
                join dep in _context.Deps on cvref.Id equals dep.CvRefId
                select new CandidateHistoryDto {
                    CategoryRef=cvref.CategoryRef, ReferredOn=cvref.ReferredOn, SelectionStatus=cvref.RefStatus,
                    OrderItemId = cvref.OrderItemId, SelectionStatusDate = cvref.RefStatusDate,
                    CustomerName= dep.CustomerName, DeploymentStatus = dep.CurrentStatus,
                    DeploymentStatusDate = dep.CurrentStatusDate,  }).ToListAsync();

            var candidateDto = new CandidateDto {ApplicationNo = Candidate.ApplicationNo,
                AmountDue=bal, CandidateId=Candidate.Id, CandidateName=Candidate.FullName, 
                CoreProfession=Candidate.UserProfessions.Where(x => x.IsMain==true)
                    .Select(x => x.ProfessionName).FirstOrDefault(), CandidateHistoriesDto=refs};
            
            return candidateDto;

        }
    }
}