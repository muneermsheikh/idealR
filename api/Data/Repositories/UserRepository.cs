using api.DTOs.Admin;
using api.DTOs.HR;
using api.Entities.HR;
using api.Entities.Identity;
using api.Interfaces;
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
        public UserRepository(DataContext context, UserManager<AppUser> userManager, IMapper mapper)
        {
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
                        CandidateId = Convert.ToString(cv.Id), Gender=  cv.Gender, Checked = false, 
                        ProfessionName = prof.ProfessionName, Source="Candidates", MobileNo=ph.MobileNo
                    }).ToListAsync();

            var query = await (from prosp in _context.ProspectiveCandidates where prosp.ProfessionId==professionid
                select new CVsMatchingProfAvailableDto {
                    CandidateId = prosp.PersonId, City = prosp.CurrentLocation, FullName = prosp.CandidateName,
                    Gender = prosp.Gender, Checked=false, Source = "Prospectives", 
                    ProfessionName=prosp.ProfessionName, MobileNo = prosp.PhoneNo
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

        public async Task<AppUserReturnDto> CreateAppUser(string userType, int userTypeValue)
        {
            var dtoErr = new AppUserReturnDto();

            var appuser = new AppUser();
            string role="";

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
                    var cand =  await _context.Candidates.FindAsync(userTypeValue);
                    if(cand == null) {
                        dtoErr.Error = "Candidate Not on record";
                        return dtoErr;
                    }
                    if(string.IsNullOrEmpty(cand.Email)) {
                        dtoErr.Error = "Candidate not on record";
                        return dtoErr;
                    } 
                    appuser = await _context.Users.Where(x => x.Email == cand.Email).FirstOrDefaultAsync();
                    if(appuser != null) {
                        dtoErr.AppUserId = appuser.Id;
                        dtoErr.Username = appuser.UserName;
                        return dtoErr;
                    }

                    appuser = new AppUser{Gender = cand.Gender, KnownAs = cand.KnownAs, Email = cand.Email,
                    PhoneNumber = cand.UserPhones.Where(x => x.IsMain && x.IsValid).Select(x => x.MobileNo).FirstOrDefault(),
                    City = cand.City, Country = cand.Country, DateOfBirth = Convert.ToDateTime(cand.DOB),
                    Created = DateTime.UtcNow, UserName = cand.Email};

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
    }
}