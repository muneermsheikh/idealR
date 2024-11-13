using System.Reflection.Metadata.Ecma335;
using api.DTOs.Admin;
using api.DTOs.Admin.Orders;
using api.Entities.Admin.Order;
using api.Interfaces.Orders;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Repositories.Orders
{
    public class JDAndRemunRepository : IJDAndRemunRepository
    {
        private readonly DataContext _context;
        public JDAndRemunRepository(DataContext context)
        {
            _context = context;
        }
        
        public async Task<JobDescription> AddJobDescription(JobDescription jobDescription)
        {
            if(jobDescription.OrderItemId==0) return null;
            if(jobDescription.Id !=0) {
                var jd = await _context.JobDescriptions.FindAsync(jobDescription.Id);
                if(jd != null) return null;
            } 

            _context.Entry(jobDescription).State=EntityState.Added;

            if(await _context.SaveChangesAsync() > 0) return jobDescription;

            return null;
        }

        public async Task<bool> EditJobDescription(JobDescription jobDescription)
        {
            if(jobDescription.Id == 0) {
                _context.JobDescriptions.Add(jobDescription);
            } else {
                var existingObject = await _context.JobDescriptions.FindAsync(jobDescription.Id);
                if (existingObject == null) return false;
                _context.Entry(existingObject).CurrentValues.SetValues(jobDescription);
            }
            
            
            try{
                await _context.SaveChangesAsync();
                return true;
            } catch (Exception ex) {
                throw new Exception(ex.Message, ex);
            }

        }


        public async Task<bool> DeleteJobDescription(int jobDescriptionId)
        {
            var jd = await _context.JobDescriptions.FindAsync(jobDescriptionId);
            if (jd == null) return false;
            _context.JobDescriptions.Remove(jd);
            _context.Entry(jd).State = EntityState.Deleted;
            return await _context.SaveChangesAsync() > 0;
        }


        public async Task<Remuneration> AddRemuneration(Remuneration remuneration)
        {
            if(remuneration.OrderItemId == 0) return null;

            _context.Entry(remuneration).State=EntityState.Added;   

            if(await _context.SaveChangesAsync() > 0) return remuneration;

            return null;
        }
        
        public JobDescription CreateNewJobDescription(JobDescription newJD, int OrderItemId)
        {
            var itemToInsert = new JobDescription
                {
                    OrderItemId = OrderItemId,
                    JobDescInBrief = newJD.JobDescInBrief,
                    MaxAge=newJD.MaxAge,
                    MinAge=newJD.MinAge,
                    ExpDesiredMin = newJD.ExpDesiredMin,
                    ExpDesiredMax = newJD.ExpDesiredMax,
                    QualificationDesired = newJD.QualificationDesired,
                };
            return itemToInsert;
        }
        
        public async Task<JDDto> GetJDOfOrderItem(int OrderItemId)
        {
            var orderdata = await (from item in _context.OrderItems where item.Id == OrderItemId
                join cat in _context.Professions on item.ProfessionId equals cat.Id
                join order in _context.Orders on item.OrderId equals order.Id
                select new {OrderNo = order.OrderNo, 
                OrderDate = order.OrderDate, CategoryName = cat.ProfessionName,
                CustomerName = order.Customer.CustomerName}).FirstOrDefaultAsync();
            
            var jobDesc = await _context.JobDescriptions.Where(x => x.OrderItemId == OrderItemId)
                .Select (x => new JDDto {
                    Id = x.Id, CompanyName = orderdata.CustomerName, OrderNo = orderdata.OrderNo,
                    CategoryName = orderdata.CategoryName, OrderItemId = x.OrderItemId, JobDescInBrief = x.JobDescInBrief,
                    QualificationDesired = x.QualificationDesired, ExpDesiredMax = x.ExpDesiredMax,
                    ExpDesiredMin = x.ExpDesiredMin, MinAge = x.MinAge, MaxAge = x.MaxAge}).FirstOrDefaultAsync()
                    ?? new JDDto {
                        CompanyName = orderdata.CustomerName, OrderNo = orderdata.OrderNo, 
                        OrderDate = orderdata.OrderDate,
                        CategoryName = orderdata.CategoryName, OrderItemId = OrderItemId};
            
            return jobDesc;
        }

        public async Task<Remuneration> GetRemuneratinOfOrderItem(int OrderItemId)
        {
            return await _context.Remunerations.Where(x => x.OrderItemId == OrderItemId).FirstOrDefaultAsync();
        }


        public async Task<bool> DeleteRemuneration(int remunerationId)
        {
           var item = await _context.Remunerations.FindAsync(remunerationId);
            if (item == null) return false;
            
            _context.Remunerations.Remove(item);
            _context.Entry(item).State = EntityState.Deleted;
            
            try{
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                throw new Exception(ex.Message, ex);
            }
            
            return  true;
        }

        public Remuneration CreateNewRemuneration(Remuneration remun, int orderItemId)
        {
            var itemToInsert2 = new Remuneration
            {
                OrderItemId = orderItemId,
                WorkHours = remun.WorkHours,
                SalaryCurrency = remun.SalaryCurrency,
                SalaryMin = remun.SalaryMin,
                SalaryMax = remun.SalaryMax,
                ContractPeriodInMonths = remun.ContractPeriodInMonths,
                HousingProvidedFree = remun.HousingProvidedFree,
                HousingAllowance = remun.HousingAllowance,
                HousingNotProvided = remun.HousingNotProvided,
                FoodProvidedFree = remun.FoodProvidedFree,
                FoodAllowance = remun.FoodAllowance,
                FoodNotProvided = remun.FoodNotProvided,
                TransportProvidedFree = remun.TransportProvidedFree,
                TransportAllowance = remun.TransportAllowance,
                TransportNotProvided = remun.TransportNotProvided,
                OtherAllowance = remun.OtherAllowance,
                LeavePerYearInDays = remun.LeavePerYearInDays,
                LeaveAirfareEntitlementAfterMonths = remun.LeaveAirfareEntitlementAfterMonths
            };
            return itemToInsert2;
        }
       
       public async Task<bool> EditRemuneration(Remuneration remuneration)
        {
            var existingObject = await _context.Remunerations.FindAsync(remuneration.Id);
            if (existingObject == null) return false;

            _context.Entry(existingObject).CurrentValues.SetValues(remuneration);

            try{
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                throw new Exception(ex.Message, ex);
            }

            return true;
        }

        public async Task<RemunerationDto> GetRemunerationDtoOfOrderItem(int OrderItemId)
        {
            var query = await (from remun in _context.Remunerations 
                    where remun.OrderItemId == OrderItemId
                join item in _context.OrderItems on remun.OrderItemId equals item.Id
                join order in _context.Orders on item.OrderId equals order.Id
                select new RemunerationDto {
                    Id = remun.Id,
                    CategoryName = item.Profession.ProfessionName,
                    CustomerName = order.Customer.CustomerName,
                    OrderDate = order.OrderDate,
                    OrderNo = order.OrderNo,
                    OrderItemId= item.Id,
                    WorkHours = remun.WorkHours,
                    SalaryCurrency = remun.SalaryCurrency,
                    SalaryMin = remun.SalaryMin,
                    SalaryMax = remun.SalaryMax,
                    ContractPeriodInMonths = remun.ContractPeriodInMonths,
                    HousingAllowance = remun.HousingAllowance,
                    HousingNotProvided = remun.HousingNotProvided,
                    HousingProvidedFree = remun.HousingProvidedFree,
                    FoodAllowance = remun.FoodAllowance,
                    FoodNotProvided = remun.FoodNotProvided,
                    FoodProvidedFree = remun.FoodProvidedFree,
                    TransportAllowance = remun.TransportAllowance,
                    TransportNotProvided = remun.TransportNotProvided,
                    TransportProvidedFree = remun.TransportProvidedFree,
                    OtherAllowance = remun.OtherAllowance,
                    LeaveAirfareEntitlementAfterMonths = remun.LeaveAirfareEntitlementAfterMonths,
                    LeavePerYearInDays = remun.LeavePerYearInDays
                }).FirstOrDefaultAsync();
            
            query ??= await (from item in _context.OrderItems where item.Id == OrderItemId
                join order in _context.Orders on item.OrderId equals order.Id
                select new RemunerationDto {
                    Id = 0,
                    CategoryName = item.Profession.ProfessionName,
                    CustomerName = order.Customer.CustomerName,
                    OrderDate = order.OrderDate,
                    OrderNo = order.OrderNo,
                    OrderItemId= item.Id,
                    WorkHours = 48
                }).FirstOrDefaultAsync();
            
            return query;
        }
    }
}