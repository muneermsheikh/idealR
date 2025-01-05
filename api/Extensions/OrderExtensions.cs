using api.Data;
using api.DTOs.Admin;
using api.DTOs.Admin.Orders;
using api.DTOs.Process;
using api.Entities.Admin.Order;
using api.Entities.HR;
using DocumentFormat.OpenXml.Office.CustomUI;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace api.Extensions
{
    public static class OrderExtensions
    {
        public async static Task<int> GetMaxOrderNo(this DataContext context)
        {
            var orderNo = await context.Orders.MaxAsync(x => (int?)x.OrderNo) ?? 1000;

            return orderNo;
        }

        public async static Task<int> GetOrderNoFromOrderItemId(this DataContext context, int orderItemId)
        {
            var orderno = await (from Orders in context.Orders
                join items in context.OrderItems on Orders.Id equals items.OrderId
                where items.Id == orderItemId
                select Orders.OrderNo)
                .FirstOrDefaultAsync();


            return orderno;
        }
        
        public async static Task<int> GetSrNoFromOrderItemId(this DataContext context, int orderItemId)
        {
            var SrNo = await context.OrderItems.Where(x => x.Id == orderItemId)
                .Select(x => x.SrNo).FirstOrDefaultAsync();
            return SrNo;
        }
        public async static Task<string> GetCustomerNameFromOrderItemId(this DataContext context, int orderItemId)
        {
            var CustomerName = await (from Orders in context.Orders
                join items in context.OrderItems on Orders.Id equals items.OrderId
                where items.Id == orderItemId
                select Orders.Customer.CustomerName)
                .FirstOrDefaultAsync();


            return CustomerName;
        }
        
        public async static Task<string> GetOrderItemDescriptionFromOrderItemId(this DataContext context, int orderItemId)
        {
            var CustomerName = await (from Orders in context.Orders
                join items in context.OrderItems on Orders.Id equals items.OrderId
                where items.Id == orderItemId
                select Orders.OrderNo + "-" + items.SrNo + "-" + items.Profession.ProfessionName +  " for " + 
                    Orders.Customer.KnownAs
            )
            .FirstOrDefaultAsync();


            return CustomerName;
        }

        public async static Task<string> GetCategoryRefFromOrderItemId(this DataContext context, int orderItemId)
        {
            var CatRef = await (from Orders in context.Orders
                join item in context.OrderItems on Orders.Id equals item.OrderId
                    where item.Id == orderItemId
                join cat in context.Professions on item.ProfessionId equals cat.Id
                
                select Orders.OrderNo + "-" + item.SrNo + "-" + cat.ProfessionName 
            )
            .FirstOrDefaultAsync();

            return CatRef;
        }

        public async static Task<string> GetHRExecUsernameFromOrderItemId(this DataContext context, int orderItemId)
        {
            var username = await context.ContractReviewItems.Where(x => x.OrderItemId == orderItemId)
                .Select(x => x.HrExecUsername).FirstOrDefaultAsync();
            
            return username ?? "";
        }

        public async static Task<string> GetHRUsernameFromOrderItemId(this DataContext context, int orderItemId)
        {
            var username = await context.ContractReviewItems.Where(x => x.OrderItemId == orderItemId)
                .Select(x => x.HrExecUsername).FirstOrDefaultAsync();
            
            return username;
        }


        public async static Task<int> GetServiceChargesFromOrderItemId(this DataContext context, int orderItemId)
        {
            var charges = await (from rvw in context.ContractReviewItems 
                where rvw.OrderItemId==orderItemId
                select rvw.Charges
            ).FirstOrDefaultAsync();

            return charges;
        }
        public async static Task<string> RequireAssessment(this DataContext context, int orderItemId)
        {
            var assess = await (from rvw in context.ContractReviewItems
                where rvw.OrderItemId==orderItemId
                select rvw.RequireAssess
            ).FirstOrDefaultAsync();

            return assess;
        }

        public async static Task<List<int>> GetOrderItemIdAndCustomerId(this DataContext context, int cvrefid)
        {
            var obj = await (from cvref in context.CVRefs where cvref.Id == cvrefid
                join item in context.OrderItems on cvref.OrderItemId equals item.Id
                join order in context.Orders on item.OrderId equals order.Id
                select new {item.Id, order.CustomerId}
            ).FirstOrDefaultAsync();

            if (obj == null) return null;

            var intList = new List<int> {obj.Id, obj.CustomerId};
            
            return intList;
        }

        public async static Task<string> GetCustomerIdAndNameFromOrderId(this DataContext context, int OrderId)
        {
            var obj = await context.Orders.Where(x => x.Id == OrderId)
                .Select(x => x.CustomerId + "|" + x.Customer.CustomerName).FirstOrDefaultAsync();
            
            return obj;
        }

        public async static Task<string> GetCustomerShortNameFromOrderId(this DataContext context, int OrderId)
        {
            var obj = await context.Orders.Where(x => x.Id == OrderId)
                .Select(x => x.Customer.KnownAs).FirstOrDefaultAsync();
            
            return obj;
        }


        public async static Task<string> GetProfessionNameFromOrderItemId(this DataContext context, int OrderItemId)
        {
            var obj = await (from item in context.OrderItems where item.Id == OrderItemId
                join cat in context.Professions on item.ProfessionId equals cat.Id
                select cat.ProfessionName).FirstOrDefaultAsync();
            
            return obj;
        }

        public async static Task<OrdIdProfNmCustNmCatRefDto> GetDetailsFromOrderItemId(this DataContext context, int OrderItemId, int CandidateId)
        {
            var obj = await (from item in context.OrderItems where item.Id == OrderItemId
                join rvwitem in context.ContractReviewItems on item.Id equals rvwitem.OrderItemId
                join cat in context.Professions on item.ProfessionId equals cat.Id
                join ord in context.Orders on item.OrderId equals ord.Id
                join remun in context.Remunerations on item.Id equals remun.OrderItemId into remuneration
                from salary in remuneration.DefaultIfEmpty()
                select new  OrdIdProfNmCustNmCatRefDto{ ProfessionName=cat.ProfessionName, 
                    CategoryRef = ord.OrderNo + "-" + item.SrNo + "-" + cat.ProfessionName,
                    CustomerName=ord.Customer.CustomerName, SalaryRange= string.Join(salary.SalaryCurrency,
                        " ", salary.SalaryMin, "-", salary.SalaryMax)})
                .FirstOrDefaultAsync();
            
            if(obj==null) return null;

            var cand = await context.Candidates.Where(x => x.Id == CandidateId)
                .Select(x => new {x.ApplicationNo, x.FullName}).FirstOrDefaultAsync();
    
            var dto = new OrdIdProfNmCustNmCatRefDto
            {
                OrderId = obj.OrderId, CategoryRef = obj.CategoryRef, CustomerName=obj.CustomerName, 
                ProfessionName=obj.ProfessionName, ApplicationNo = (int)(cand?.ApplicationNo), 
                CandidateName = cand?.FullName, HrExecUsername = obj.HrExecUsername,
                SalaryRange = obj.SalaryRange
            };

            return dto;

        }

        public async static Task<ICollection<DeploymentPendingBriefDto>> GetDepPendingBriefDtoFromDepItemIds(this DataContext context, ICollection<int> DepItemIds)
        {
            var query = await (from depItem in context.DepItems  where DepItemIds.Contains(depItem.Id)  
                join depStatus in context.DeployStatuses on depItem.Sequence equals depStatus.Sequence
                join dep in context.Deps on depItem.DepId equals dep.Id
                join cvref in context.CVRefs on dep.CvRefId equals cvref.Id
                join orderitem in context.OrderItems on cvref.OrderItemId equals orderitem.Id
                join order in context.Orders on orderitem.OrderId equals order.Id
                join cand in context.Candidates on cvref.CandidateId equals cand.Id
                select new DeploymentPendingBriefDto {
                    DepId=dep.Id, ReferredOn=cvref.ReferredOn, SelectedOn=dep.SelectedOn, 
                    ApplicationNo=cand.ApplicationNo, CandidateName=cand.FullName, OrderNo=order.OrderNo, 
                    CustomerName=order.Customer.KnownAs, CityOfWorking=order.CityOfWorking, 
                    CategoryName=orderitem.Profession.ProfessionName, DeploySequence = depItem.Sequence, 
                    NextSequence=depItem.NextSequence, NextStageDate=depItem.NextSequenceDate,
                    Ecnr = cand.Ecnr == "true", CurrentStatus = depStatus.StatusName
                }).ToListAsync();
    
            return query;
        }

        public async static Task<DeploymentPendingBriefDto> GetDepPendingBriefDtoFromDepId(this DataContext context, int depId)
        {
            var query = await (from dep in context.Deps where dep.Id==depId
                join depitem in context.DepItems on dep.Id equals depitem.DepId orderby depitem.Sequence descending
                join cvref in context.CVRefs on dep.CvRefId equals cvref.Id
                join orderitem in context.OrderItems on cvref.OrderItemId equals orderitem.Id
                join order in context.Orders on orderitem.OrderId equals order.Id
                join cand in context.Candidates on cvref.CandidateId equals cand.Id
                select new DeploymentPendingBriefDto {
                    DepId=dep.Id, ReferredOn=cvref.ReferredOn, SelectedOn=dep.SelectedOn, ApplicationNo=cand.ApplicationNo,
                    CandidateName=cand.FullName, OrderNo=order.OrderNo, CustomerName=order.Customer.KnownAs, 
                    CityOfWorking=order.CityOfWorking, CategoryName=orderitem.Profession.ProfessionName,
                    DeploySequence = depitem.Sequence, NextSequence=depitem.NextSequence, NextStageDate=depitem.NextSequenceDate,
                    Ecnr = cand.Ecnr == "true"
                }).FirstOrDefaultAsync();
    
            return query;
        }

        public async static Task<int> GetProfessionIdFromOrderItemId(this DataContext context, int OrderItemId)
        {
            var query = await (from item in context.OrderItems  where item.Id == OrderItemId 
                select item.ProfessionId).FirstOrDefaultAsync();
    
            return query;
        }

    }
    
}