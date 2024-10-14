using api.Data;
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

        public static async Task<string> GetAppUsernameFromDepId (this DataContext context, int depId)
        {
           var appusername = await (from dep in context.Deps where dep.Id==depId
                join cvref in context.CVRefs on dep.CvRefId equals cvref.Id
                join cand in context.Candidates on cvref.CandidateId equals cand.Id
                select cand.Username).FirstOrDefaultAsync();
            
            return appusername;
        }


    }
}