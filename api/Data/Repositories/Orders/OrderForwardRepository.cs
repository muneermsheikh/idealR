using System.Data.Common;
using api.DTOs.Admin.Orders;
using api.Entities.Admin.Order;
using api.Entities.Messages;
using api.Extensions;
using api.Interfaces.Admin;
using api.Interfaces.Orders;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Office.CustomUI;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Repositories.Orders
{
    public class OrderForwardRepository : IOrderForwardRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;
        private readonly IComposeMessagesHRRepository _msgRep;
        public OrderForwardRepository(DataContext context, IConfiguration config, IComposeMessagesHRRepository msgRep)
        {
            _msgRep = msgRep;
            _config = config;
            _context = context;
        }

        public async Task<OrderForwardToAgent> GenerateOrderForwardObjForAgentByOrderId(int orderid)
        {
            var order = await _context.Orders.Include(x => x.OrderItems)
                .Where(x => x.Id == orderid).FirstOrDefaultAsync();
            
            var itemList = new List<OrderForwardCategory>();
            foreach(var item in order.OrderItems) {
                itemList.Add(new OrderForwardCategory() { 
                    OrderId = item.OrderId, ProfessionId = item.ProfessionId,
                    OrderItemId = item.Id, ProfessionName = item.Profession.ProfessionName,
                });
            }

            var obj = new OrderForwardToAgent{
                OrderId = orderid, customerName = order.Customer.CustomerName,
                CustomerCity = order.Customer.City, CustomerId = order.Customer.Id,
                OrderNo = order.OrderNo, OrderDate = order.OrderDate,
                ProjectManagerId = order.ProjectManagerId,
                OrderForwardCategories = itemList
            };

            return obj;
        }

        public async Task<OrderForwardToHR> GenerateObjToForwardOrderToHR(int orderid)
        {
            var query = await _context.Orders.Include(x => x.OrderItems)
                .Where(x => x.Id == orderid).FirstOrDefaultAsync();
            
            var obj = new OrderForwardToHR {
                OrderId = orderid, DateOnlyForwarded = DateOnly.FromDateTime(DateTime.UtcNow),
                RecipientUsername = _config["HRSupUsername"]
            };

            return obj;
        }

        public async Task<string> UpdateForwardOrderToHR(OrderForwardToHR obj, string Username)
        {
            _context.Entry(obj).State = EntityState.Added;

            try {
                await _context.SaveChangesAsync();
            } catch (DbException ex) {
                return ex.Message;
            } catch (Exception ex){
                return ex.Message;
            }
            
            var recipientUsername = _config["HRSupUsername"];
            //ICollection<OrderItemBriefDto> OrderItems, int senderAppUserId, int recipientAppUserId
            var dataToComposeMsg = await (from order in _context.Orders where order.Id == obj.OrderId
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

        public async Task<string> UpdateOrderForwardedToAgents(OrderForwardToAgent fwd)
        {
            //verify the object
            foreach(var item in fwd.OrderForwardCategories) {
                if(item.OrderForwardToAgentId == 0 || 
                    item.Charges == 0) return "Agent Id Or Charges not defined";
                foreach(var item2 in item.OrderForwardCategoryOfficials) {
                    if(item2.CustomerOfficialId == 0 || 
                        item2.OrderForwardCategoryId == 0) return "customer official or category Id not defined";
                }
            }

            _context.Entry(fwd).State = EntityState.Added;
            
            return await _context.SaveChangesAsync() > 0 ? "" : "failed to save the Order Forward object";
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

                var officials = new List<AssociatesOrderForwardedToDto>();
                foreach(var off in cat.OrderForwardCategoryOfficials) {
                    var offs = new AssociatesOrderForwardedToDto {
                        AgentName = off.AgentName, CustomerOfficialId = off.CustomerOfficialId,
                        CustomerOfficialName = await _context.OfficialNameFromOfficialId(off.CustomerOfficialId),
                        DateOnlyForwarded = off.DateOnlyForwarded, 
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


    }
}
