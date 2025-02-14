using api.Data;
using api.DTOs.Admin;
using api.Entities.Identity;
using api.Interfaces.Admin;
using Microsoft.EntityFrameworkCore;

namespace api.Extensions
{
    public static class EmployeeExtensions
    {
        public static async Task<int> GetAppUserIdOfEmployee (this DataContext context, int employeeId)
        {
            var appuserid = await context.Employees.Where(x => x.Id == employeeId)
                .Select(x => x.AppUserId).FirstOrDefaultAsync();
            
            return appuserid;
        }

        public static async Task<int> GetAppUserIdOfCandidate (this DataContext context, int candidateId)
        {
            var appuserid = await context.Candidates.Where(x => x.Id == candidateId)
                .Select(x => x.AppUserId).FirstOrDefaultAsync();
            
            return appuserid;
        }

        public static async Task<string> GetAppUsernameOfCandidate (this DataContext context, int candidateId)
        {
            var appusername = await context.Candidates.Where(x => x.Id == candidateId)
                .Select(x => x.Username).FirstOrDefaultAsync();
            
            return appusername;
        }
        
       
        public static async Task<ReturnStringsDto> GetAppUsernameFromDepId (this DataContext context, ISelDecisionRepository selRepo, int depId)
        {
            var dto = new ReturnStringsDto();

            var obj = await (from dep in context.Deps where dep.Id==depId
                join cvref in context.CVRefs on dep.CvRefId equals cvref.Id
                join cand in context.Candidates on cvref.CandidateId equals cand.Id
                select new {UserName = cand.Username, CandidateId=cand.Id}).FirstOrDefaultAsync();
            
            if(string.IsNullOrEmpty(obj.UserName) && obj.CandidateId != 0) {
                var appUser = await selRepo.AppUserFromCandidateId(obj.CandidateId);
                if (appUser != null) {
                    dto.SuccessString = appUser.UserName;
                } else {
                    dto.ErrorString = "Failed to retrieve/create Username"; 
                }
            } else {
                dto.SuccessString = obj.UserName;
            }

            return dto;
            
        }


    }
}