using System.Data.Common;
using api.DTOs.Admin;
using api.DTOs.Admin.Orders;
using api.DTOs.Customer;
using api.Entities.Admin.Order;
using api.Extensions;
using api.Helpers;
using api.Interfaces;
using api.Interfaces.Admin;
using api.Interfaces.Orders;
using api.Params.Orders;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DocumentFormat.OpenXml;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace api.Data.Repositories.Orders
{
    public class OrderForwardRepository : IOrderForwardRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;
        private readonly IComposeMessagesHRRepository _msgRep;
        private readonly IMapper _mapper;
        private readonly ICustomerRepository _custRepo;
        public OrderForwardRepository(DataContext context, ICustomerRepository custRepo, 
            IConfiguration config, IComposeMessagesHRRepository msgRep, IMapper mapper)
        {
            _custRepo = custRepo;
            _mapper = mapper;
            _msgRep = msgRep;
            _config = config;
            _context = context;
        }

        public async Task<PagedList<OrderForwardCategory>> GetPagedList(OrderFwdParams fParams)
        {
            var query = _context.OrderForwardCategories
                .Include(x => x.OrderForwardCategoryOfficials)
                .AsQueryable();

            var temp = await query.ToListAsync();
            
            if(fParams.ProfessionId != 0) query = query.Where(x => x.ProfessionId == fParams.ProfessionId).Where(x => x.ProfessionId == fParams.ProfessionId);
            if(fParams.OrderId != 0) query = query.Where(x => x.OrderId == fParams.OrderId);
            
            var paged = await PagedList<OrderForwardCategory>.CreateAsync(query.AsNoTracking()
                    //.ProjectTo<OrderForwardToAgentDto>(_mapper.ConfigurationProvider)
                    , fParams.PageNumber, fParams.PageSize);
            
            
            return paged;

        }


        public async Task<string> InsertOrUpdateOrderForwardToAgents(ICollection<OfficialAndCustomerNameDto> officialIds, int orderid, string username)
        {
            var strErr="";
            
            var officialids = officialIds.Select(x => x.Id).Distinct().ToList();
            var orderitems = await _context.OrderItems.Where(x => x.OrderId == orderid)
                //.Select(x => x.ProfessionId)
                .ToListAsync();
            
            var officials =await(from off in _context.CustomerOfficials where officialids.Contains(off.Id)
                join cust in _context.Customers on off.CustomerId equals cust.Id
                select  new OrderForwardCategoryOfficial {
                    AgentName = cust.CustomerName, 
                    CustomerOfficialId = off.Id,
                    OfficialName = off.OfficialName,
                    DateForwarded = DateTime.Now,
                    EmailIdForwardedTo = off.Email,
                    PhoneNoForwardedTo = off.PhoneNo, Username = username
                }).ToListAsync();

            var fwdCats = await _context.OrderForwardCategories.Where(x => orderitems.Select(y => y.Id).ToList().Contains(x.OrderItemId)).ToListAsync();
            if(fwdCats.Count == 0) {
                fwdCats = await (from item in _context.OrderItems where orderitems.Select(x=>x.Id).ToList().Contains(item.Id)
                    join order in _context.Orders on item.OrderId equals order.Id 
                    join cust in _context.Customers on order.CustomerId equals cust.Id
                    join cat in _context.Professions on item.ProfessionId equals cat.Id
                    join rvw in _context.ContractReviewItems on item.Id equals rvw.OrderItemId into rvwitems 
                        from rvwitem in rvwitems.DefaultIfEmpty()
                    select new OrderForwardCategory {
                        OrderId=orderid,
                        OrderNo=order.OrderNo, 
                        OrderDate = order.OrderDate,
                        CustomerName=cust.CustomerName,
                        CustomerCity = cust.City,
                        OrderItemId = item.Id,
                        ProfessionId = item.ProfessionId,
                        ProfessionName = cat.ProfessionName,
                        Charges = rvwitem.Charges,
                        OrderForwardCategoryOfficials = officials
                }).ToListAsync();
                
                foreach(var cat in fwdCats) {
                    _context.OrderForwardCategories.Add(cat); 
                }
            } else {
                foreach(var fwd in fwdCats) {
                    if(fwd.OrderForwardCategoryOfficials==null || fwd.OrderForwardCategoryOfficials.Count == 0) {
                        fwd.OrderForwardCategoryOfficials=officials;
                    }
                    await UpdateOrderForwardCategory(fwd);
                }
            }

            var ids = officials.Select(x => x.CustomerOfficialId).Distinct().ToList();

            var msgs = await _msgRep.ComposeMsgsToForwardOrdersToAgents(fwdCats, ids, username);
            if(msgs.Count > 0) foreach(var msg in msgs){_context.Messages.Add(msg);}

            try {
                await _context.SaveChangesAsync();
             } catch (Exception ex) {
                if(ex.InnerException.Message.Contains("IX_OrderForwardCategoryOfficials_OrderForwardCategoryId_CustomerOfficialId")) {
                    strErr = "Unique Index Violation of CategoryId and OfficialId";
                } else if(ex.InnerException.Data.Contains("IX_OrderForwardCategoryOfficials_OrderForwardCategoryId_CustomerOfficialId")) {
                    strErr = "Unique Index Violation of CategoryId and OfficialId";
                } else {
                    strErr = ex.Message;
                }
            }

            return strErr;
        }

        public async Task<string> EditOrderForwardCategories(ICollection<OrderForwardCategory> models, string username)
        {
            var cats = new List<OrderForwardCategory>();

            foreach(var model in models)
            {
                var mdl =await UpdateOrderForwardCategory(model);     //the _context is modified/added with objects
                if(mdl != null) cats.Add(model);
            }

            var orderid = models.Select(x => x.OrderId).FirstOrDefault();

            var fwdToAgent = await _context.Orders.Where(x => x.Id == orderid)
                .Include(x => x.Customer)
                .Select(x => new OrderForwardToAgentDto{
                    CustomerCity = x.Customer.City, CustomerId=x.CustomerId, Id=0,
                        CustomerName=x.Customer.CustomerName, OrderDate = x.OrderDate, OrderId=x.Id,
                        OrderNo = x.OrderNo, ProjectManagerId = x.ProjectManagerId,
                        OrderForwardCategories = cats        
                    }).FirstOrDefaultAsync();
                

            //var msgs = await _msgRep.ComposeMsgsToForwardOrdersToAgents(fwdToAgent, username);
            
            //if(msgs.Count > 0) foreach(var msg in msgs) {_context.Entry(msg).State = EntityState.Added;}
  
            try {
                await _context.SaveChangesAsync();
            } catch (DbException ex) {
                return "Database Error" + ex.Message;
            } catch (Exception ex) {
                return ex.Message;
            }

            return "";
            
        }

        private async Task<OrderForwardCategory> UpdateOrderForwardCategory(OrderForwardCategory model)
        {

             var existing = await _context.OrderForwardCategories
                .Include(x => x.OrderForwardCategoryOfficials)
                .Where(x => x.Id == model.Id)
                .FirstOrDefaultAsync();
            if(existing != null) model.Id=existing.Id;
            _context.Entry(existing).CurrentValues.SetValues(model);

            //the model may not contain other valid entries of customer officials, so such entries not to be deleted

            foreach(var newItem in model.OrderForwardCategoryOfficials.ToList())
            {
                var itemToInsert = new OrderForwardCategoryOfficial
                {
                    AgentName=newItem.AgentName,
                    CustomerOfficialId = newItem.CustomerOfficialId,
                    DateForwarded = newItem.DateForwarded,
                    EmailIdForwardedTo = newItem.EmailIdForwardedTo,
                    OfficialName = newItem.OfficialName,
                    //OrderForwardCategoryId = newItem.OrderForwardCategoryId,
                    PhoneNoForwardedTo = newItem.PhoneNoForwardedTo,
                    Username = newItem.Username,
                    WhatsAppNoForwardedTo = newItem.WhatsAppNoForwardedTo
                };

                existing.OrderForwardCategoryOfficials.Add(itemToInsert);
                _context.Entry(itemToInsert).State = EntityState.Added;
            
            }
        
            _context.Entry(existing).State = existing.Id == 0 ? EntityState.Added : EntityState.Modified;
            return existing;
        }

        public async Task<ICollection<OrderForwardCategoryDto>> AssociatesOfOrderForwardsOfAnOrder(int orderid, string Username)
        {
            var obj = await _context.OrderForwardCategories
                .Include(x => x.OrderForwardCategoryOfficials)
                .Where(x => x.OrderId == orderid)
                .ToListAsync();
            
            var categoriesDto = new List<OrderForwardCategoryDto>();

            foreach(var cat in obj) {

                var officials = new List<OrderForwardToOfficialDto>();
                foreach(var off in cat.OrderForwardCategoryOfficials) {
                    var offs = new OrderForwardToOfficialDto {
                        AgentName = off.AgentName, CustomerOfficialId = off.CustomerOfficialId,
                        CustomerOfficialName = await _context.OfficialNameFromOfficialId(off.CustomerOfficialId),
                        DateForwarded = off.DateForwarded, 
                        EmailIdForwardedTo = off.EmailIdForwardedTo, 
                        OrderForwardCategoryId = off.OrderForwardCategoryId, 
                        PhoneNoForwardedTo = off.PhoneNoForwardedTo, Username = Username
                    };
                    officials.Add(offs);
                }
                
                var catDto = new OrderForwardCategoryDto {
                    Charges  = cat.Charges, OrderItemId = cat.OrderItemId, 
                    OrderId = cat.OrderId, OrderNo = cat.OrderNo, OrderDate=cat.OrderDate,
                    CustomerCity=cat.CustomerCity, CustomerName=cat.CustomerName,
                    ProfessionId = cat.ProfessionId, ProfessionName = cat.ProfessionName,
                    OfficialsDto = officials
                };
                categoriesDto.Add(catDto);
            }
            
            return categoriesDto;
        }

        public async Task<bool> DeleteOrderForwardCategory(int orderforwardcategoryid)
        {
            var obj = await _context.OrderForwardCategories.Where(x => x.Id == orderforwardcategoryid)
                .Include(x => x.OrderForwardCategoryOfficials)
                .ToListAsync();
            
            if (obj == null) return false;

            foreach(var dto in obj) {
                _context.OrderForwardCategories.Remove(dto);
                _context.Entry(dto).State = EntityState.Deleted;
            }
            
            try {
                await _context.SaveChangesAsync();
            } catch {
                return false;
            }

            return true;
        }


        public async Task<bool> DeleteOrderForwardCategoryOfficial(int orderFwdOfficialId)
        {
            var obj = await _context.OrderForwardCategoryOfficials
                .FirstOrDefaultAsync();
            
            if (obj == null) return false;

            _context.OrderForwardCategoryOfficials.Remove(obj);
            _context.Entry(obj).State = EntityState.Deleted;

            try {
                await _context.SaveChangesAsync();
            } catch {
                return false;
            }

            return true;
        }

        //fwd to HR
        public async Task<OrderForwardToHR> GenerateObjToForwardOrderToHR(int orderid)
    {
        var query = await _context.Orders.Include(x => x.OrderItems)
            .Where(x => x.Id == orderid).FirstOrDefaultAsync();
        
        var obj = new OrderForwardToHR {
            OrderId = orderid, DateOnlyForwarded = DateTime.UtcNow,
            RecipientUsername = _config["HRSupUsername"]
        };

        return obj;
    }

        public async Task<string> UpdateForwardOrderToHR(int orderid, string Username)
        {
            var fwdToHR = await _context.OrderForwardToHRs.Where(x => x.OrderId == orderid).FirstOrDefaultAsync();

            if(fwdToHR != null) return "Order forwarded to HR Dept on " + fwdToHR.DateOnlyForwarded;
            
            var recipientUsername = _config["HRSupUsername"];

            var fwd = await _context.Orders.Where(x => x.Id == orderid)
                .Select(x => new OrderForwardToHR {
                    OrderId = x.Id, DateOnlyForwarded=DateTime.Now,RecipientUsername = recipientUsername})
                .FirstOrDefaultAsync();
            if(fwd == null) return "Order Id submitted is invalid";

            _context.Entry(fwd).State = EntityState.Added;

            //ICollection<OrderItemBriefDto> OrderItems, int senderAppUserId, int recipientAppUserId
            var dataToComposeMsg = await (from order in _context.Orders where order.Id == orderid
                    join item in _context.OrderItems on order.Id equals item.OrderId
                select new OrderItemBriefDto {
                    OrderId = order.Id, CustomerId = order.CustomerId, CustomerName=order.Customer.CustomerName,
                    AboutEmployer=order.Customer.Introduction, OrderNo=order.OrderNo, OrderDate=order.OrderDate,
                    SrNo = item.SrNo, ProfessionId = item.ProfessionId, ProfessionName = item.Profession.ProfessionName,
                    Quantity = item.Quantity, Ecnr = item.Ecnr, CompleteBefore=item.CompleteBefore,
                    JobDescription = item.JobDescription, Remuneration=item.Remuneration,Status=item.Status
                }).ToListAsync();

            var msgs = await _msgRep.ComposeEmailMsgForDLForwardToHRHead(dataToComposeMsg, Username, recipientUsername);
            
            if(string.IsNullOrEmpty(msgs.ErrorString)) {
                foreach(var msg in msgs.Messages) { _context.Messages.Add(msg);}
            } else {
                return msgs.ErrorString;
            }

            try {
                await _context.SaveChangesAsync();
            } catch (DbException ex) {
                return ex.Message;
            } catch (Exception ex){
                return ex.Message;
            }

            return "";
        }
 }
}

