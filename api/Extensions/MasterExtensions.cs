using System.Reflection.Metadata.Ecma335;
using api.Data;
using api.DTOs.Process;
using api.Entities.Deployments;
using api.Entities.Master;
using Microsoft.EntityFrameworkCore;

namespace api.Extensions
{
    public static class MasterExtensions
    {
        public async static Task<string> GetProfessionNameFromId(this DataContext context, int professionId)
        {
            var profName = await context.Professions.Where(x => x.Id == professionId)
                .Select(x => x.ProfessionName).FirstOrDefaultAsync();

            return profName;
        }

        public async static Task<string> GetProfessionAndIndustryNameFromId(this DataContext context, int professionId, int industryId)
        {
            var profName = await context.Professions.Where(x => x.Id == professionId)
                .Select(x => x.ProfessionName).FirstOrDefaultAsync();
            var indName = await context.Industries.Where(x => x.Id == industryId)
                .Select(x => x.IndustryName).FirstOrDefaultAsync();
            
            return string.IsNullOrEmpty(indName) ? profName : profName + "-" + indName;
        }

        public async static Task<string> GetEmployeeNameFromId(this DataContext context, int employeeId)
        {
            var empName = await context.Employees.Where(x => x.Id == employeeId)
                .Select(x => x.FirstName + ' ' + x.FamilyName).FirstOrDefaultAsync();

            return empName;
        }

        
        public async static Task<DeployStatus> GetNextDepStatus(this DataContext context, int sequence)
        {
            var nextSeq = await context.DeployStatuses.Where(x => x.Sequence == sequence).FirstOrDefaultAsync();

            return nextSeq;
        }

        public async static Task<string> GetNextDepStatusName(this DataContext context, int sequence)
        {
            return await context.DeployStatuses.Where(x => x.Sequence == sequence).Select(x => x.StatusName).FirstOrDefaultAsync();
        }

         public async static Task<Referral> GetNextDepStatusNameAndPeriod(this DataContext context, int NextSequence)
        {
            return await context.DeployStatuses.Where(x => x.Sequence == NextSequence)
                .Select(x => new Referral {
                    Sequence = x.Sequence,
                    SequenceName = x.StatusName,
                    Period = x.WorkingDaysReqdForNextStage
                }).FirstOrDefaultAsync();
        }
        
    }
}