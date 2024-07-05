using System.Data.Common;
using System.Runtime.InteropServices.Marshalling;
using api.DTOs.Admin.Orders;
using api.Entities.Admin.Order;
using api.Extensions;
using api.Helpers;
using api.Interfaces;
using api.Interfaces.Admin;
using api.Interfaces.Orders;
using api.Params.Orders;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Repositories.Orders
{
    public class OrderForwardRepository : IOrderForwardRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;
        private readonly IComposeMessagesHRRepository _msgRep;
        private readonly IMapper _mapper;
        private readonly int _defaultProjectManagerId = 0;
        private readonly int _defaultProjManagerAppUserId=0;
        private readonly ICustomerRepository _custRepo;
        public OrderForwardRepository(DataContext context, ICustomerRepository custRepo,
            IConfiguration config, IComposeMessagesHRRepository msgRep, IMapper mapper)
        {
            _custRepo = custRepo;
            _mapper = mapper;
            _msgRep = msgRep;
            _config = config;
            _context = context;
            _defaultProjectManagerId = Convert.ToInt32(_config["EmpHRSupervisorId"] ?? "0");
            _defaultProjManagerAppUserId = Convert.ToInt32(_config["HRSupAppuserId"] ?? "0");
        }

        public async Task<OrderForwardToAgent> GenerateOrderForwardToAgent(int orderid)
        {
            var orderFwd = await _context.OrderForwardToAgents
                .Include(x => x.OrderForwardCategories)
                .ThenInclude(x => x.OrderForwardCategoryOfficials)
                .Where(x => x.OrderId == orderid)
                .FirstOrDefaultAsync();
            
            if(orderFwd != null) return orderFwd;

            //generate a new object

            var offList = new List<OrderForwardCategoryOfficial>
            {
                new() {
                    //CustomerOfficialId = 5, OfficialName = "VK Patel",
                    //AgentName = "Binladen Industrial Co Ltd", 
                    DateForwarded = DateTime.Now
                }
            };
            

            var itemsWithCharges = await (from item in _context.OrderItems where item.OrderId==orderid
                //join ord in _context.Orders on item.OrderId equals ord.Id
                join cat in _context.Professions on item.ProfessionId equals cat.Id
                join rvw in _context.ContractReviewItems on item.Id equals rvw.OrderItemId
                select new {rvw.Charges, cat.ProfessionName, item})
                .ToListAsync();
            //var items = await _context.OrderItems.Where(x => x.OrderId == orderid).ToListAsync();
            var categories = new List<OrderForwardCategory>();
            
            foreach(var orderitem in itemsWithCharges) {
                    categories.Add(new OrderForwardCategory{
                        OrderItemId = orderitem.item.Id, ProfessionId=orderitem.item.ProfessionId, 
                        Charges=orderitem.Charges, OrderId=orderitem.item.OrderId, ProfessionName=orderitem.ProfessionName,
                        OrderForwardCategoryOfficials = offList
                });
                
                var order = await _context.Orders.Where(x => x.Id == orderid)
                    .Select(x => new {CustomerName=x.Customer.CustomerName, OrderNo=x.OrderNo,
                    OrderDate = x.OrderDate}).FirstOrDefaultAsync(); 

                orderFwd = new OrderForwardToAgent{customerName = order.CustomerName,
                    OrderDate= order.OrderDate, OrderId = orderid, OrderNo = order.OrderNo,
                    OrderForwardCategories = categories};
            }
            
            return orderFwd;
        }
        public async Task<PagedList<OrderForwardToAgentDto>> GetPagedListOfOrderFwds(OrderFwdParams fParams)
        {
            var query = _context.OrderForwardToAgents
                .Include(x => x.OrderForwardCategories)
                .ThenInclude(x => x.OrderForwardCategoryOfficials)
                .AsQueryable();
            
            //if(fParams.ProfessionId != 0) qyery = query.Where(x => x.OrderForwardCategories.Select(y => y.ProfessionId).ToList().Contains(fParams.ProfessionId).Where(x => x.ProfessionId == fParams.ProfessionId));
            if(fParams.OrderId != 0) query = query.Where(x => x.OrderId == fParams.OrderId);
            
            var paged = await PagedList<OrderForwardToAgentDto>.CreateAsync(query.AsNoTracking()
                    .ProjectTo<OrderForwardToAgentDto>(_mapper.ConfigurationProvider),
                    fParams.PageNumber, fParams.PageSize);
            
            return paged;

        }

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

            try {
                await _context.SaveChangesAsync();
            } catch (DbException ex) {
                return ex.Message;
            } catch (Exception ex){
                return ex.Message;
            }
            
            
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

            var msg = await _msgRep.ComposeEmailMsgForDLForwardToHRHead(dataToComposeMsg, Username, recipientUsername);
            
            return "";
        }

        public async Task<string> InsertOrderForwardedToAgents(OrderForwardToAgent fwd, string username)
        {
            //if the header exists, get it
            foreach(var cat in fwd.OrderForwardCategories) {
                foreach(var off in cat.OrderForwardCategoryOfficials) {
                    if(string.IsNullOrEmpty(off.AgentName)) {
                        var custAndOff = await _custRepo.GetCustomerOfficialDto(off.CustomerOfficialId);
                        if(custAndOff != null) {
                            off.AgentName = custAndOff.CustomerName;
                            off.EmailIdForwardedTo = custAndOff.OfficialEmailId;
                            off.Username = username;
                        }
                    }
                }
            }
            
            if (fwd.ProjectManagerId == 0) fwd.ProjectManagerId = _defaultProjectManagerId;
            
            var orderFwd = await _context.OrderForwardToAgents.Include(x => x.OrderForwardCategories)
                .ThenInclude(x => x.OrderForwardCategoryOfficials).FirstOrDefaultAsync();

            if(orderFwd != null) {  //add the categories+officials to the existing header
                
                foreach(var cat in fwd.OrderForwardCategories) {
                    var existingCatItem = orderFwd.OrderForwardCategories.Where(c => c.OrderItemId == cat.OrderItemId).SingleOrDefault();
                    
                    if(existingCatItem == null) {       //OrderForwardCategory does not exist for the OrderItemId, which is unique index
                            orderFwd.OrderForwardCategories.Add(new OrderForwardCategory{
                            Charges = cat.Charges, OrderForwardToAgentId = cat.OrderForwardToAgentId,
                            OrderId = cat.OrderId, OrderItemId=cat.OrderItemId, ProfessionId = cat.ProfessionId,
                            ProfessionName = cat.ProfessionName, OrderForwardCategoryOfficials = cat.OrderForwardCategoryOfficials
                        });
                    } else {        //OrderForwardCategory exists, so append the officials to this record
                        existingCatItem.OrderForwardCategoryOfficials = cat.OrderForwardCategoryOfficials;
                    }
                    
                }
                _context.Entry(orderFwd).State = EntityState.Modified;

            } else {        //create all objects including the header
                if(fwd.CustomerId == 0) {
                    var st = await _context.GetCustomerIdAndNameFromOrderId(fwd.OrderId);
                    var index=st.IndexOf("|");
                    fwd.CustomerId = Convert.ToInt32(st.Substring(0, index));
                    fwd.customerName = st.Substring(index+1);
                }
                _context.OrderForwardToAgents.Add(fwd);
                _context.Entry(fwd).State = EntityState.Added;
            }
            
            var msgs = await _msgRep.ComposeMsgsToForwardOrdersToAgents(fwd, username);

            if(msgs != null && msgs.Count > 0) foreach(var msg in msgs) {_context.Entry(msg).State = EntityState.Added;}
            
            try {
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                return ex.Message;
            }

            return "";

        }
        
        public async Task<string> UpdateOrderForwardedToAgents(OrderForwardToAgent model, string username)
        {
            //variables to store new objects, for composing msgs
            var OrderFwdCategories = new List<OrderForwardCategory>();
            var OrderFwdCategory = new OrderForwardCategory();
            var OrderFwdOfficials = new List<OrderForwardCategoryOfficial>();
            var OrderFwdToAgent = new OrderForwardToAgent();

            var existing = await _context.OrderForwardToAgents
                .Include(x => x.OrderForwardCategories)
                .ThenInclude(x => x.OrderForwardCategoryOfficials)
                .Where(x => x.Id == model.Id)
                .FirstOrDefaultAsync();
            
            if(existing == null) return "No order forwarding object exists to edit";

            _context.Entry(existing).CurrentValues.SetValues(model);

            foreach(var existingItem in existing.OrderForwardCategories.ToList())
            {
                if(!model.OrderForwardCategories.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                {
                    _context.OrderForwardCategories.Remove(existingItem);
                    _context.Entry(existingItem).State = EntityState.Deleted;
                }
            }

            foreach(var newItem in model.OrderForwardCategories)
            {
                OrderFwdCategories = new List<OrderForwardCategory>();
                OrderFwdCategory = new OrderForwardCategory();

                var existingItem = existing.OrderForwardCategories
                    .Where(c => c.Id == newItem.Id && c.Id != default(int)).SingleOrDefault();
                
                if(existingItem != null) {
                    _context.Entry(existingItem).CurrentValues.SetValues(newItem);
                    _context.Entry(existingItem).State = EntityState.Modified;
                } else {
                    var itemToInsert = new OrderForwardCategory 
                    {
                        Charges = newItem.Charges,
                        OrderForwardToAgentId = newItem.OrderForwardToAgentId,
                        OrderId =  newItem.OrderId,
                        OrderItemId = newItem.OrderItemId,
                        ProfessionId = newItem.ProfessionId,
                        ProfessionName = newItem.ProfessionName,
                        //OrderForwardCategoryOfficials = newItem.OrderForwardCategoryOfficials
                    };
                    
                    existing.OrderForwardCategories.Add(itemToInsert);
                    _context.Entry(itemToInsert).State = EntityState.Added;

                    OrderFwdCategory = itemToInsert;
                }

                foreach(var existingSubItem in existingItem?.OrderForwardCategoryOfficials.ToList())
                {
                    if(!existingItem.OrderForwardCategoryOfficials.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                    {
                        _context.OrderForwardCategoryOfficials.Remove(existingSubItem);
                        _context.Entry(existingSubItem).State = EntityState.Deleted;
                    }
                }
                
                OrderFwdOfficials = new List<OrderForwardCategoryOfficial>();
                foreach(var newSubItem in newItem.OrderForwardCategoryOfficials)
                {
                    var subItemExisting = existingItem.OrderForwardCategoryOfficials
                        .Where(c => c.Id == newSubItem.Id && c.Id != default(int)).SingleOrDefault();
                    
                    if(subItemExisting != null)
                    {
                        _context.Entry(subItemExisting).CurrentValues.SetValues(newSubItem);
                        _context.Entry(subItemExisting).State = EntityState.Modified;
                    } else {
                        var subItemToInsert = new OrderForwardCategoryOfficial
                        {
                            AgentName=newSubItem.AgentName,
                            CustomerOfficialId = newSubItem.CustomerOfficialId,
                            DateForwarded = newSubItem.DateForwarded,
                            EmailIdForwardedTo = newSubItem.EmailIdForwardedTo,
                            OfficialName = newSubItem.OfficialName,
                            OrderForwardCategoryId = newSubItem.OrderForwardCategoryId,
                            PhoneNoForwardedTo = newSubItem.PhoneNoForwardedTo,
                            Username = newSubItem.Username,
                            WhatsAppNoForwardedTo = newSubItem.WhatsAppNoForwardedTo
                        };

                        existingItem.OrderForwardCategoryOfficials.Add(subItemToInsert);
                        _context.Entry(subItemToInsert).State = EntityState.Added;

                        OrderFwdOfficials.Add(subItemToInsert);
                    }
                }

                OrderFwdCategory.OrderForwardCategoryOfficials = OrderFwdOfficials;
                OrderFwdCategories.Add(OrderFwdCategory);

                if(OrderFwdCategory != null) {
                    OrderFwdCategory.OrderForwardCategoryOfficials = OrderFwdOfficials;
                    OrderFwdToAgent = new OrderForwardToAgent {
                        CustomerCity = model.CustomerCity, CustomerId=model.CustomerId, Id=model.Id,
                        customerName=model.customerName, OrderDate = model.OrderDate, OrderId=model.OrderId,
                        OrderNo = model.OrderNo, ProjectManagerId = model.ProjectManagerId,
                        OrderForwardCategories = OrderFwdCategories
                    };
                }
                
                _context.Entry(existing).State = EntityState.Modified;
            }

            var msgs = await _msgRep.ComposeMsgsToForwardOrdersToAgents(OrderFwdToAgent, username);
            if(msgs.Count > 0) foreach(var msg in msgs) {_context.Entry(msg).State = EntityState.Added;}
  
            try {
                await _context.SaveChangesAsync();
            } catch (DbException ex) {
                return "Database Error" + ex.Message;
            } catch (Exception ex) {
                return ex.Message;
            }

            //compose email messages
          return "";
        }

        public async Task<OrderForwardToAgent> OrderFowardsOfAnOrder(int orderid)
        {
            var obj = await _context.OrderForwardToAgents
                .Include(x => x.OrderForwardCategories).ThenInclude(x => x.OrderForwardCategoryOfficials)
                .Where(x => x.OrderId == orderid).FirstOrDefaultAsync();
            
            return obj;
        }

        public async Task<OrderForwardToAgentDto> AssociatesOfOrderForwardsOfAnOrder(int orderid, string Username)
        {
            var obj = await _context.OrderForwardToAgents
                .Include(x => x.OrderForwardCategories).ThenInclude(x => x.OrderForwardCategoryOfficials)
                .Where(x => x.OrderId == orderid)
                .FirstOrDefaultAsync();
            
            var categoriesDto = new List<OrderForwardCategoryDto>();

            foreach(var cat in obj.OrderForwardCategories) {

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
                    ProfessionId = cat.ProfessionId, ProfessionName = cat.ProfessionName,
                    OfficialsDto = officials
                };
                categoriesDto.Add(catDto);
            }
            
            var dto = new OrderForwardToAgentDto {
                OrderId = orderid,OrderDate = obj.OrderDate,
                CustomerName = obj.customerName, OrderNo=obj.OrderNo,
                OrderForwardCategoriesDto = categoriesDto

            };

            return dto;
        }

        public async Task<bool> DeleteOrderForward(int orderid)
        {
            var obj = await _context.OrderForwardToAgents
                .Include(x => x.OrderForwardCategories)
                .ThenInclude(x => x.OrderForwardCategoryOfficials)
                .FirstOrDefaultAsync();
            
            if (obj == null) return false;

            _context.OrderForwardToAgents.Remove(obj);
            _context.Entry(obj).State = EntityState.Deleted;

            try {
                await _context.SaveChangesAsync();
            } catch {
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteOrderForwardCategory(int orderitemid)
        {
            var obj = await _context.OrderForwardCategories
                .Include(x => x.OrderForwardCategoryOfficials)
                .FirstOrDefaultAsync();
            
            if (obj == null) return false;

            _context.OrderForwardCategories.Remove(obj);
            _context.Entry(obj).State = EntityState.Deleted;

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

    }
}
