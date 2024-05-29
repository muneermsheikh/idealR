using api.Data;
using api.DTOs.HR;
using api.Entities.Identity;
using DocumentFormat.OpenXml.Presentation;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace api.Extensions
{
    public static class CandidateExtensions
    {
        public static async Task<int> NextCandidateApplicationNo(this DataContext context)
        {
            var appno = await context.Candidates.MaxAsync(x => x.ApplicationNo);
            return ++appno;
        }

        public static async Task<int> GetApplicationNoFromCandidateId(this DataContext context, int candidateId)
        {
            var appno = await context.Candidates.Where(x => x.Id == candidateId)
                .Select(x => x.ApplicationNo).FirstOrDefaultAsync();
            return appno;
        }
        
        
        public static async Task<string> GetCandidateNameFromCandidateId(this DataContext context, int candidateId)
        {
            var candName = await context.Candidates.Where(x => x.Id == candidateId)
                .Select(x => x.FullName).FirstOrDefaultAsync();
            return candName;
        }

        public static async Task<string> GetCandidateDescriptionFromCandidateId(this DataContext context, int candidateId)
        {
            var appno = await context.Candidates.Where(x => x.Id == candidateId)
                .Select(x => x.FullName + ", " +  x.ApplicationNo).FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(appno)) return null;
            
            return appno;
        }
    }
}