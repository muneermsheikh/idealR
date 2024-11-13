using api.Data;
using api.DTOs.Admin;
using api.Entities.Admin;
using api.Entities.Admin.Client;
using api.Entities.HR;
using api.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.Extensions
{
    public static class UpdateAppUserIdExtensions
    {
       
        public static async Task<int> UpdateCustomerOfficialAppUserId(this CustomerOfficial official,
            UserManager<AppUser> userManager, DataContext context, string cityName)
        {
            var off = await userManager.FindByEmailAsync(official.Email);

            if(off == null) {
                off = new AppUser{
                    Gender = official.Gender,
                    KnownAs = official.KnownAs,
                    City = cityName,
                    UserName = string.IsNullOrEmpty(official.UserName) ? official.Email : official.UserName,
                    Created = DateTime.UtcNow,
                    Email = official.Email
                };
                
                var result = await userManager.CreateAsync(off, "Pa$$w0rd");
                official.UserName=off.UserName;
            }

            var roleresult = await userManager.GetRolesAsync(off);
            if(!roleresult.Contains("Client")) await userManager.AddToRoleAsync(off, "Client");            
            return off.Id;
        }

        public static async Task<int> UpdateEmployeeAppUserId(this Employee employee,
            UserManager<AppUser> userManager, string cityName)
        {
            var off =await userManager.FindByEmailAsync(employee.Email);
            if(off == null){
                off = new AppUser{
                    Gender = employee.Gender,
                    KnownAs = employee.KnownAs,
                    City = cityName,
                    UserName = employee.UserName,
                    Created = DateTime.UtcNow
                };
                await userManager.CreateAsync(off, "Pa$$w0rd");
                await userManager.AddToRoleAsync(off, "Client");
            }          
        
            return off.Id;  
        }

        public static async Task<int> UpdateCandidateAppUserId(this Candidate candidate,
            UserManager<AppUser> userManager, DataContext context)
        {
            var off = await userManager.FindByEmailAsync(candidate.Email);
            if(off==null) {
                off = new AppUser{
                    Gender = candidate.Gender,
                    KnownAs = candidate.KnownAs,
                    City = candidate.City,
                    UserName = candidate.Username,
                    Created = DateTime.UtcNow
                };

                await userManager.CreateAsync(off, "Pa$$w0rd");
                await userManager.AddToRoleAsync(off, "Client");
            }
            
            return off.Id;
        }

        public static async Task<string> GetAppUserEmail(this UserManager<AppUser> userManager, int appUserId)
        {
            var user = await userManager.FindByIdAsync(appUserId.ToString());
            if(user==null) return "";
            return user.Email;

        }

         public static async Task<UsernameAndEmailDto> AppUserEmailAndUsernameFromAppUserId(this UserManager<AppUser> userManager, int appUserId)
        {
            var obj = await userManager.FindByIdAsync(appUserId.ToString());
            if(obj==null) return null;
            var dto = new UsernameAndEmailDto
            {
                Username = obj.UserName,
                KnownAs = obj.KnownAs,
                Email = obj.Email,
                Position = obj.Position,
                AppUserId = obj.Id
            };

            return dto;
        }

        public static async Task<AppUserBriefDto> GetAppUserObjFromEmployeeId(this UserManager<AppUser> userManager,
            DataContext context, int EmployeeId)
        {
            var email = await context.Employees.Where(x => x.Id == EmployeeId)
                .Select(x => new {appuserid=x.AppUserId, AppUserEmail=x.Email, Name=x.FirstName, Position=x.Position})
                .FirstOrDefaultAsync();

            if(email == null) return null;

            var obj = email.appuserid == 0 
                ? await userManager.FindByIdAsync(email.appuserid.ToString())         
                : await userManager.FindByEmailAsync(email.AppUserEmail);
            if(obj == null) return null;
            return  new AppUserBriefDto { KnownAs = obj.KnownAs, AppUserId=EmployeeId, Username= obj.UserName, AppUserEmail=obj.Email,
                Position=email.Position, Name=email.Name};
        }

        public static async Task<AppUserBriefDto> GetAppUserObjFromCustomerOfficial(this UserManager<AppUser> userManager,
            DataContext context, int OfficialId)
        {
            var email = await context.CustomerOfficials.Where(x => x.Id == OfficialId)
                .Select(x => new {appuserid=x.AppUserId, Email=x.Email, Name=x.OfficialName, Position=x.Designation})
                .FirstOrDefaultAsync();

            if(email == null) return null;

            var obj = email.appuserid == 0 
                ? await userManager.FindByIdAsync(email.appuserid.ToString())         
                : await userManager.FindByEmailAsync(email.Email);
            if(obj == null) return null;
            return  new AppUserBriefDto { KnownAs = obj.KnownAs, AppUserId=OfficialId, Username= obj.UserName, 
                AppUserEmail=obj.Email, Name=email.Name, Position=email.Position};
        }

        public static async Task<UsernameAndEmailDto> AppUserEmailAndUsernameFromAppUsername(this UserManager<AppUser> userManager, string username)
        {
            var obj = await userManager.FindByNameAsync(username);
            if(obj==null) return null;
            var dto = new UsernameAndEmailDto
            {
                Username = obj.UserName,
                KnownAs = obj.KnownAs,
                Email = obj.Email,
                Position = obj.Position,
                AppUserId = obj.Id
            };

            return dto;
        }
    }
}