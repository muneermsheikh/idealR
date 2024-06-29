using System.Runtime.CompilerServices;
using api.DTOs.Admin;
using api.Entities.Admin;
using api.Entities.HR;
using api.Entities.Identity;
using api.Helpers;
using api.Interfaces.Admin;
using api.Params.Admin;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DocumentFormat.OpenXml.EMMA;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Repositories.Master
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        public EmployeeRepository(DataContext context, IMapper mapper, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _mapper = mapper;
            _context = context;
        }


        public async Task<Employee> AddNewEmployee(EmployeeToAddDto model)
        {
          
            var user = new AppUser{
                City=model.City, Country=model.Country, Created = model.DOJ,
                DateOfBirth = model.DOB, Email = model.Email, KnownAs = model.KnownAs,
                Gender = model.Gender, Position = model.Position, 
                PhoneNumber = model.OfficialMobileNo, UserName = model.Email
            };

            var result = await _userManager.CreateAsync(user, "Pa$$w0rd");

            if(!result.Succeeded) return null;

            model.AppUserId = user.Id;

            var emp = _mapper.Map<Employee>(model);
            _context.Employees.Add(emp);

            return await _context.SaveChangesAsync() > 0 ? emp : null;
        }

        public async Task<bool> DeleteEmployee(int employeeid)
        {
            var obj = await _context.Employees.FindAsync(employeeid);

            if(obj == null) return false;

            _context.Employees.Remove(obj);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Employee> EditEmployee(Employee model )
        {
            var existing = await _context.Employees.Include(x => x.HRSkills).Include(x => x.OtherSkills)
                .Where(x => x.Id == model.Id).FirstOrDefaultAsync();
            
            if(existing == null) return null;

            _context.Entry(existing).CurrentValues.SetValues(model);

            //HRSkills
            foreach (var existingItem in existing.HRSkills.ToList())
            {
                if (!model.HRSkills.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                {
                    _context.HRSkills.Remove(existingItem);
                    _context.Entry(existingItem).State = EntityState.Deleted;
                }
            }

            foreach (var modelItem in model.HRSkills)
            {
                var existingItem = existing.HRSkills.Where(c => c.Id == modelItem.Id && c.Id != default(int)).SingleOrDefault();
                if (existingItem != null)       // Update child
                {
                    _context.Entry(existingItem).CurrentValues.SetValues(modelItem);
                    _context.Entry(existingItem).State = EntityState.Modified;
                }
                else            //insert children as new record
                {
                    var newItem = new HRSkill{ EmployeeId=model.Id, IndustryId= modelItem.IndustryId, 
                        ProfessionId = modelItem.ProfessionId, IsMain = modelItem.IsMain, 
                        SkillLevel = modelItem.SkillLevel};
                    
                    existing.HRSkills.Add(newItem);
                    _context.Entry(newItem).State = EntityState.Added;
                }
            }

            //OtherSkills
            foreach (var existingItem in existing.OtherSkills.ToList())
            {
                if (!model.HRSkills.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                {
                    _context.OtherSkills.Remove(existingItem);
                    _context.Entry(existingItem).State = EntityState.Deleted;
                }
            }

            foreach (var modelItem in model.OtherSkills)
            {
                var existingItem = existing.OtherSkills.Where(c => c.Id == modelItem.Id && c.Id != default(int)).SingleOrDefault();
                if (existingItem != null)       // Update child
                {
                    _context.Entry(existingItem).CurrentValues.SetValues(modelItem);
                    _context.Entry(existingItem).State = EntityState.Modified;
                }
                else            //insert children as new record
                {
                    var newItem = new OtherSkill{ EmployeeId=model.Id, SkillDataId = modelItem.SkillDataId,
                        IsMain = modelItem.IsMain, SkillLevel = modelItem.SkillLevel};
                    
                    existing.OtherSkills.Add(newItem);
                    _context.Entry(newItem).State = EntityState.Added;
                }
            }

            var user = await _userManager.FindByEmailAsync(model.OfficialEmail);
            if (user == null)
            {
                user = new AppUser{
                    City=model.City, Country=model.Country, Created = model.DateOfJoining,
                    DateOfBirth = model.DateOfBirth, Email = model.OfficialEmail, KnownAs = model.KnownAs,
                    Gender = model.Gender, Position = model.Position, PhoneNumber = model.EmployeePhone,
                    UserName = model.OfficialEmail
                };

                var result = await _userManager.CreateAsync(user, "Pa$$w0rd");

                if(!result.Succeeded) return null;
            } else {
                //email or phone changed? update ApopUser
                 var appuserchanged=false;
                if(existing.OfficialEmail != model.OfficialEmail) {
                    user.Email = model.OfficialEmail;
                    appuserchanged = true;
                }

                if(existing.OfficialPhoneNo != model.OfficialPhoneNo) {
                    user.PhoneNumber = model.OfficialPhoneNo;
                    appuserchanged = true;
                }

                if(appuserchanged) await _userManager.UpdateAsync(user);
            }

            model.AppUserId = user.Id;

            _context.Entry(existing).State = EntityState.Modified;

            return await _context.SaveChangesAsync() > 0 ? existing : null;

        }

        public async Task<Employee> GetEmployeeFromEmpId(int empId)
        {
            return await _context.Employees.FindAsync(empId);
        }

        public async Task<ICollection<EmployeeIdAndKnownAsDto>> GetEmployeeIdAndKnownAs()
        {
            var obj = await _context.Employees.Include(x => x.HRSkills).OrderBy(x => x.KnownAs)
                .Select(x => new EmployeeIdAndKnownAsDto {
                     Id = x.Id, KnownAs = x.KnownAs, Username=x.UserName, HrSkills=x.HRSkills
                })
                .ToListAsync();
   
            return obj;
        }


        public async Task<PagedList<EmployeeBriefDto>> GetEmployeePaginated(EmployeeParams empParams)
        {
            var query = _context.Employees.AsQueryable();

            if(empParams.Id != 0) {
                query = query.Where(x => x.Id == empParams.Id);
            } else {
                if(!string.IsNullOrEmpty(empParams.Status)) query = query.Where(x => x.Status.ToLower() == empParams.Status.ToLower());
                if(!string.IsNullOrEmpty(empParams.Position)) query = query.Where(x => x.Position.ToLower() == empParams.Position.ToLower());
                if(!string.IsNullOrEmpty(empParams.UserName)) query = query.Where(x => x.UserName.ToLower() == empParams.UserName.ToLower());
                if(!string.IsNullOrEmpty(empParams.FirstName)) query = query.Where(x => x.FirstName.ToLower() == empParams.FirstName.ToLower());
                if(!string.IsNullOrEmpty(empParams.SecondName)) query = query.Where(x => x.SecondName.ToLower() == empParams.SecondName.ToLower());
                if(!string.IsNullOrEmpty(empParams.FamilyName)) query = query.Where(x => x.FamilyName.ToLower() == empParams.FamilyName.ToLower());
                if(!string.IsNullOrEmpty(empParams.Department)) query = query.Where(x => x.Department.ToLower() == empParams.Status.ToLower());
                if(!string.IsNullOrEmpty(empParams.Email)) query = query.Where(x => x.OfficialEmail.ToLower() == empParams.Email.ToLower());
                if(!string.IsNullOrEmpty(empParams.Gender)) query = query.Where(x => x.Gender.ToLower() == empParams.Gender.ToLower());
            }

            var paged = await PagedList<EmployeeBriefDto>.CreateAsync(query.AsNoTracking()
                    .ProjectTo<EmployeeBriefDto>(_mapper.ConfigurationProvider),
                    empParams.PageNumber, empParams.PageSize);
            
            return paged;
        }

    }
}