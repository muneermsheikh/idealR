using api.DTOs.Admin;
using api.Interfaces.Admin;
using api.Params.Admin;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Repositories.Admin
{
    public class QueryableRepository: IQueryableRepository
    {
        private readonly DataContext _context;
        public QueryableRepository(DataContext context)
        {
            _context = context;
        }


        public async Task<IQueryable<CVRefDto>> GetCVReDtoQueryable(CVRefParams refParams)
        {
            var query =(from cvref in _context.CVRefs 
                join item in _context.OrderItems on cvref.OrderItemId equals item.Id 
                join o in _context.Orders on item.OrderId equals o.Id
                join cv in _context.Candidates on cvref.CandidateId equals cv.Id 
        
                select new CVRefDto{
                    CVRefId = cvref.Id,
                    Checked = false,
                    CustomerId = o.CustomerId,
                    CustomerName = o.Customer.CustomerName,
                    CandidateId = cvref.CandidateId,
                    CandidateName = cv.FullName,
                    ApplicationNo = cv.ApplicationNo,
                    OrderId = item.OrderId,
                    OrderNo = o.OrderNo,
                    OrderDate = o.OrderDate,
                    OrderItemId = cvref.OrderItemId,
                    ProfessionName = item.Profession.ProfessionName,
                    CategoryRef = o.OrderNo + "-" + item.SrNo,
                    PPNo = cv.PpNo,
                    ReferredOn = cvref.ReferredOn,
                    RefStatus = cvref.RefStatus
                })
                .AsQueryable();
    
            if(refParams.OrderItemId  > 0) query = query.Where(x => x.OrderItemId == refParams.OrderItemId);
            if(refParams.CustomerId > 0) query = query.Where(x => x.CustomerId == refParams.CustomerId);
            if(refParams.CandidateId != 0) query = query.Where(x => x.CandidateId == refParams.CandidateId);
            if(!string.IsNullOrEmpty(refParams.RefStatus)) query = query.Where(x => x.RefStatus.ToLower() == refParams.RefStatus.ToLower());

            if(refParams.ProfessionId !=0) {
                var orderItemIds = await _context.OrderItems.
                    Where(x => x.ProfessionId == refParams.ProfessionId)
                    .Select(x => x.Id).ToListAsync();
                if(orderItemIds != null && orderItemIds.Count > 0) {
                    query = query.Where(x => orderItemIds.Contains(x.OrderItemId));
                }
            }

            if(refParams.AgentId != 0) {
                var candidateIds = await _context.Candidates.Where(x => x.CustomerId == refParams.AgentId).Select(x => x.Id).ToListAsync();
                if(candidateIds != null && candidateIds.Count > 0) {
                    query = query.Where(x => candidateIds.Contains(x.CandidateId));
                }
            }

            return query;
        }
    
        public IQueryable<CustomerAndOfficialsDto> GetCustomerAndOfficialQueryable(int customerId, string OfficialDivn)
        {
            
             var customerNOfficials = (from cust in _context.Customers where cust.Id == customerId
                join off in _context.CustomerOfficials on cust.Id equals off.CustomerId 
                    where off.Divn==OfficialDivn
                select new CustomerAndOfficialsDto {
                    CustomerId = cust.Id, OfficialId = off.Id, AppUserId = off.AppUserId,
                    CustomerName = cust.CustomerName, City=cust.City, Country=cust.Country,
                    OfficialDesignation = off.Designation
                }).AsQueryable();
            
            return customerNOfficials;

        }
    }
}