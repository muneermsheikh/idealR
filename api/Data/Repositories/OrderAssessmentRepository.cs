using System.Data.Common;
using api.DTOs.Admin;
using api.DTOs.Admin.Orders;
using api.DTOs.Orders;
using api.Entities.Admin.Order;
using api.Extensions;
using api.Interfaces;
using api.Interfaces.Admin;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Repositories
{
    public class OrderAssessmentRepository : IOrderAssessmentRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly DateTime _todaydate;
        private readonly IOrderForwardRepository _orderFwdRepo;
        public OrderAssessmentRepository(DataContext context, IOrderForwardRepository orderFwdRepo, IMapper mapper)
        {
            _orderFwdRepo = orderFwdRepo;
            _mapper = mapper;
            _context = context;
            _todaydate = DateTime.UtcNow;
        }

        public async Task<OrderAssessment> SaveNewOrderAssessment(OrderAssessment orderAssessment)
        {
            _context.OrderAssessments.Add(orderAssessment);

            var ct = await _context.SaveChangesAsync();
            
            if(ct==0) return null;

            await _orderFwdRepo.UpdateOrderExtn(orderAssessment.OrderId, "AssessmentDesigned", orderAssessment.DateDesigned.ToString() );
            
            return  orderAssessment;
        }

        public async Task<bool> DeleteOrderAssessmentItem(int orderItemId)
        {
            var obj = await _context.OrderAssessmentItems.Include(x => x.OrderAssessmentItemQs)
                .Where(x => x.OrderItemId == orderItemId).FirstOrDefaultAsync();
            
            if(obj == null) return false;
            _context.OrderAssessmentItems.Remove(obj);
            _context.Entry(obj).State = EntityState.Deleted;

            try {
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                throw new Exception(ex.Message, ex);
            }

            return true;

        }

        public async Task<bool> DeleteOrderAssessmentItemQ(int questionId)
        {
            var obj = await _context.OrderAssessmentItemQs.FindAsync(questionId);
            if(obj == null) return false;
            
            _context.OrderAssessmentItemQs.Remove(obj);
            _context.Entry(obj).State = EntityState.Deleted;

            try{
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                throw new Exception(ex.Message, ex);
            }

            return true;
        }

        public async Task<string> EditOrderAssessment(OrderAssessment newObject, string Username)
        {
             if(newObject.Id == 0) {
                _context.OrderAssessments.Add(newObject);
                try {
                    await _context.SaveChangesAsync();
                    return "";
                } catch (DbException ex) {
                    return ex.Message;
                } catch (Exception ex) {
                    return ex.Message;
                }
             }

             var existingObject = _context.OrderAssessments
                .Where(x => x.Id == newObject.Id)
                .Include(x => x.OrderAssessmentItems)
                .ThenInclude(x => x.OrderAssessmentItemQs)
                .AsNoTracking()
                .SingleOrDefault();
            
                if (existingObject == null) return "The Order has not been assessed, so it cannot be modified.  Try  to add new assessment questions";
            _context.Entry(existingObject).CurrentValues.SetValues(newObject);

            //delete records in existingObject that are not present in new object
            foreach (var existingItem in existingObject.OrderAssessmentItems.ToList())
            {
                if(!newObject.OrderAssessmentItems.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                {
                    _context.OrderAssessmentItems.Remove(existingItem);
                    _context.Entry(existingItem).State = EntityState.Deleted; 
                }

                 //items in current object - either updated or new items
                foreach(var newItem in newObject.OrderAssessmentItems)
                {
                    var itemExisting = existingObject.OrderAssessmentItems
                        .Where(c => c.Id == newItem.Id && c.Id != default(int)).SingleOrDefault();
                    
                    if(itemExisting != null)    //update navigation record
                    {
                        _context.Entry(itemExisting).CurrentValues.SetValues(newItem);
                        _context.Entry(itemExisting).State = EntityState.Modified;
                    } else {    //insert new navigation record
                        var profession = await (from item in _context.OrderItems where item.Id==newItem.OrderItemId
                            join prof in _context.Professions on item.ProfessionId equals prof.Id
                            select prof).FirstOrDefaultAsync();
                        var itemToInsert = new OrderAssessmentItem
                        {
                            CustomerName = newItem.CustomerName,
                            DateDesigned = _todaydate,
                            DesignedBy = Username,
                            OrderItemId = newItem.OrderItemId,
                            ProfessionId =profession.Id,
                            ProfessionName =profession.ProfessionName,
                            ProfessionGroup=profession.ProfessionGroup,
                            OrderNo = newItem.OrderNo, 
                            OrderId =newItem.OrderId,
                            RequireCandidateAssessment = itemExisting.RequireCandidateAssessment
                        };

                        existingObject.OrderAssessmentItems.Add(itemToInsert);
                        _context.Entry(itemToInsert).State = EntityState.Added;
                    }

                    foreach (var existingSubItem in existingItem.OrderAssessmentItemQs.ToList())
                    {
                        if(!existingItem.OrderAssessmentItemQs.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                        {
                            _context.OrderAssessmentItemQs.Remove(existingSubItem);
                            _context.Entry(existingSubItem).State = EntityState.Deleted; 
                        }

                        //items in current object - either updated or new items
                        foreach(var newSubItem in newItem.OrderAssessmentItemQs)
                        {
                            var subItemExisting = existingItem.OrderAssessmentItemQs
                                .Where(c => c.Id == newSubItem.Id && c.Id != default(int)).SingleOrDefault();
                            
                            if(subItemExisting != null)    //update navigation record
                            {
                                _context.Entry(subItemExisting).CurrentValues.SetValues(newSubItem);
                                _context.Entry(subItemExisting).State = EntityState.Modified;
                            } else {    //insert new navigation record
                                var subItemToInsert = new OrderAssessmentItemQ
                                {
                                    OrderAssessmentItemId = existingSubItem.Id,
                                    QuestionNo = newSubItem.QuestionNo,
                                    Subject = newSubItem.Subject,
                                    Question = newSubItem.Question,
                                    MaxPoints = newSubItem.MaxPoints,
                                    IsMandatory = newSubItem.IsMandatory
                                };

                                existingItem.OrderAssessmentItemQs.Add(subItemToInsert);
                                _context.Entry(subItemToInsert).State = EntityState.Added;
                            }
                        }
                    }
                }

            }

            _context.Entry(existingObject).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
                return "";
            } catch (DbException ex) {
                return ex.Message;
            } catch (Exception ex) {
                return ex.Message;
            }

        }

        public async Task<bool> EditOrderAssessmentItem(OrderAssessmentItem newObject, string username)
        {
             if(string.IsNullOrEmpty(newObject.ProfessionGroup)) {
                var prof = await _context.Professions.FindAsync(newObject.ProfessionId);
                newObject.ProfessionGroup = prof.ProfessionGroup;
             }

             if(newObject.Id == 0) {
                var obj = new OrderAssessmentItem {
                    OrderAssessmentId = newObject.OrderAssessmentId,
                    AssessmentRef = newObject.AssessmentRef, OrderItemId = newObject.OrderItemId,
                    CustomerName = newObject.CustomerName, ProfessionName = newObject.ProfessionName,
                    ProfessionId = newObject.ProfessionId, ProfessionGroup = newObject.ProfessionGroup,
                    OrderNo = newObject.OrderNo, OrderId=newObject.OrderId,
                    ApprovedBy = newObject.ApprovedBy, DateDesigned = newObject.DateDesigned,
                    RequireCandidateAssessment = newObject.RequireCandidateAssessment,
                    DesignedBy = newObject.DesignedBy};
                
                foreach(var newItem in newObject.OrderAssessmentItemQs) {
                     var itemToInsert = new OrderAssessmentItemQ {
                        QuestionNo = newItem.QuestionNo,
                        Subject = newItem.Subject,
                        Question = newItem.Question,
                        MaxPoints = newItem.MaxPoints,
                        IsMandatory = newItem.IsMandatory};

                    obj.OrderAssessmentItemQs.Add(itemToInsert);
                    _context.Entry(itemToInsert).State = EntityState.Added;
                }
                _context.Entry(obj).State = EntityState.Added;

                //if OrderAssessment does not exist, create one. to update OrderAssessmentItem.OrderAssessmentId
                var orderdata = await (from item in _context.OrderItems where item.Id == newObject.OrderItemId
                    join order in _context.Orders on item.OrderId equals order.Id 
                    select new {orderid=order.Id, OrderNo = order.OrderNo, OrderDate = order.OrderDate,
                    CustomerName = order.Customer.CustomerName, DesignedByUsername = username }).FirstOrDefaultAsync();
                
                var orderassessment = await _context.OrderAssessments.Where(x => x.OrderId==orderdata.orderid).FirstOrDefaultAsync();
                if(orderassessment != null) {
                    obj.OrderAssessmentId = orderassessment.Id;
                    _context.OrderAssessmentItems.Add(obj);
                } else {
                    var lst = new List<OrderAssessmentItem>{obj};
                    orderassessment = new OrderAssessment {OrderId = orderdata.orderid, OrderNo = orderdata.OrderNo,
                        OrderDate = orderdata.OrderDate, CustomerName = orderdata.CustomerName, DesignedByUsername = username,
                        DateDesigned = DateTime.UtcNow, OrderAssessmentItems = lst};
                    _context.Entry(orderassessment).State = EntityState.Added;
                }
                
                    
             } else {
                var existingObject = _context.OrderAssessmentItems
                    .Where(x => x.Id == newObject.Id)
                    .Include(x => x.OrderAssessmentItemQs)
                    .AsNoTracking()
                    .SingleOrDefault();
                
                if (existingObject == null) return false;

                _context.Entry(existingObject).CurrentValues.SetValues(newObject);

                //delete records in existingObject that are not present in new object
                foreach (var existingItem in existingObject.OrderAssessmentItemQs.ToList())
                {
                    if(!newObject.OrderAssessmentItemQs.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                    {
                        _context.OrderAssessmentItemQs.Remove(existingItem);
                        _context.Entry(existingItem).State = EntityState.Deleted; 
                    }
                }

                //items in current object - either updated or new items
                foreach(var newItem in newObject.OrderAssessmentItemQs)
                {
                    var itemExisting = existingObject.OrderAssessmentItemQs
                        .Where(c => c.Id == newItem.Id && c.Id != default(int)).SingleOrDefault();
                    
                    if(itemExisting != null)    //update navigation record
                    {
                        _context.Entry(itemExisting).CurrentValues.SetValues(newItem);
                        _context.Entry(itemExisting).State = EntityState.Modified;
                    } else {    //insert new navigation record
                        var itemToInsert = new OrderAssessmentItemQ
                        {
                            OrderAssessmentItemId = existingObject.Id,
                            QuestionNo = newItem.QuestionNo,
                            Subject = newItem.Subject,
                            Question = newItem.Question,
                            MaxPoints = newItem.MaxPoints,
                            IsMandatory = newItem.IsMandatory
                        };

                        existingObject.OrderAssessmentItemQs.Add(itemToInsert);
                        _context.Entry(itemToInsert).State = EntityState.Added;
                    }
                }
                

                if(existingObject.OrderAssessmentId==0) existingObject.OrderAssessmentId = 
                    await GetOrderAssessmentIdFromOrderItemId(newObject.OrderItemId);
                
                _context.Entry(existingObject).State = EntityState.Modified;
             }

            try{
                await _context.SaveChangesAsync();
                return true;
            } catch (Exception ex) {
                throw new Exception(ex.Message, ex);
            }
            
        }

        private async Task<int> GetOrderAssessmentIdFromOrderItemId(int orderitemid)
        {
            var order = await (from item in _context.OrderItems where item.Id==orderitemid
                join o in _context.Orders on item.OrderId equals o.Id
                select new {OrderId=o.Id, OrderNo = o.OrderNo, CustomerName=o.Customer.CustomerName, 
                OrderDate=o.OrderDate}).FirstOrDefaultAsync();
            
            var assessmt = await _context.OrderAssessments.Where(x => x.OrderId == order.OrderId).FirstOrDefaultAsync();
            if(assessmt == null) {
                assessmt = new OrderAssessment{ CustomerName=order.CustomerName, OrderDate = order.OrderDate,
                OrderId = order.OrderId, OrderNo = order.OrderNo};
                
                _context.Entry(assessmt).State = EntityState.Added;

                await _context.SaveChangesAsync();
            }

            return assessmt.Id;
            
        }
        private async Task<OrderAssessment> GenerateOrderAssessmentFromOrderId(int orderId, string username) {
            
            var orderassessment = await _context.OrderAssessments
                .Include(x => x.OrderAssessmentItems)
                .ThenInclude(x => x.OrderAssessmentItemQs)
                .Where(x => x.OrderId == orderId)
                .FirstOrDefaultAsync();
            
            if(orderassessment != null) return orderassessment;

            //since order assessment is not present, create one.
            
            var order = await _context.Orders.Include(x => x.OrderItems).Include(x => x.Customer).FirstOrDefaultAsync();
           
            var stddQs = await _context.AssessmentQStdds.OrderBy(x => x.QuestionNo).ToListAsync();

            var assessmentItemQs =new List<OrderAssessmentItemQ>();     //part of OrderAssessmentItem
            foreach (var q in stddQs)
            {
                assessmentItemQs.Add(new OrderAssessmentItemQ{
                    QuestionNo = q.QuestionNo,
                    Question=q.Question,
                    Subject=q.Subject,
                    MaxPoints = q.MaxPoints,
                    IsMandatory=q.IsMandatory
                });
            }

            var assessmentItems = new List<OrderAssessmentItem>();

            foreach(var item in order.OrderItems) {

                var newAssessmentItem = new OrderAssessmentItem{
                    OrderItemId = item.Id,
                    ProfessionGroup = item.Profession.ProfessionGroup,
                    ProfessionId = item.ProfessionId,
                    ProfessionName = item.Profession.ProfessionName,
                    CustomerName = order.Customer.CustomerName,
                    OrderNo = order.OrderNo,
                    AssessmentRef = order.OrderNo + "-" + item.SrNo,
                    DateDesigned = _todaydate,
                    DesignedBy = username,
                    OrderAssessmentItemQs = assessmentItemQs
                };
                assessmentItems.Add(newAssessmentItem);
            }

            orderassessment = new OrderAssessment{
                    OrderId = order.Id,
                    OrderNo = order.OrderNo,
                    CustomerName = order.Customer.CustomerName,
                    OrderDate = order.OrderDate,
                    DesignedByUsername = username,
                    OrderAssessmentItems = assessmentItems
                };
                
            _context.OrderAssessments.Add(orderassessment);

            return await _context.SaveChangesAsync() > 0 ? orderassessment : null;
        }
        private async Task<OrderAssessmentItemWithErr> GenerateOrderAssessmentItemFromStddQ(int orderItemId, string loggedInUserName)
        {   
            var dtoErr = new OrderAssessmentItemWithErr();

            //ascertain the record does not exist;
            var assessmtItem = await _context.OrderAssessmentItems.Where(x => x.OrderItemId==orderItemId).FirstOrDefaultAsync();
            if(assessmtItem != null) {
                dtoErr.orderAssessmentItem = assessmtItem;
                return dtoErr;
            }

            //verify orderItemId is valid
            var orderitem = await _context.OrderItems.Include(x => x.Profession).Where(x => x.Id==orderItemId).FirstOrDefaultAsync();

            if (orderitem == null) return null;
            
            var order = await _context.Orders.FindAsync(orderitem.OrderId);
           
            var stdd = await _context.AssessmentQStdds.OrderBy(x => x.QuestionNo).ToListAsync();

            var ListQ =new List<OrderAssessmentItemQ>();
            foreach (var q in stdd)
            {
                ListQ.Add(new OrderAssessmentItemQ{
                    QuestionNo = q.QuestionNo,
                    Question=q.Question,
                    Subject=q.Subject,
                    MaxPoints = q.MaxPoints,
                    IsMandatory=q.IsMandatory
                });
            }

            var newAssessmentItem = new OrderAssessmentItem{
                OrderItemId = orderItemId,
                ProfessionGroup=orderitem.Profession.ProfessionGroup,
                ProfessionId=orderitem.ProfessionId,
                ProfessionName = orderitem.Profession.ProfessionName,
                CustomerName = await _context.GetCustomerNameFromOrderItemId(orderItemId),
                OrderNo = order.OrderNo,
                AssessmentRef = order.OrderNo + "-" + _context.GetSrNoFromOrderItemId(orderItemId),
                DateDesigned = _todaydate,
                DesignedBy = loggedInUserName,
                OrderAssessmentItemQs = ListQ
            };

            //if orderassessment - the parent - exists, get the OrderAssessmentId
            Thread.Sleep(1000);     //without this, flg statement raises error A SECOND OPERATION WAS STARTED ON THIS CONTEXT INSTANCE BEFORE A PREVIOUS OPERATION COMPLETED
            var orderassessment = await _context.OrderAssessments.Where(x => x.OrderId==order.Id).FirstOrDefaultAsync();
            if(orderassessment != null) {
                newAssessmentItem.OrderAssessmentId=orderassessment.Id;
                _context.Entry(newAssessmentItem).State=EntityState.Added;
            } else {
                var orderassessmt = new OrderAssessment{
                    OrderId = order.Id,
                    OrderDate = order.OrderDate,
                    DesignedByUsername = loggedInUserName,
                    OrderAssessmentItems = new List<OrderAssessmentItem>{newAssessmentItem}
                };
                _context.Entry(orderassessmt).State=EntityState.Added;
                await _context.SaveChangesAsync();
                newAssessmentItem.OrderAssessmentId = orderassessmt.Id;
            }
            
            try {
                await _context.SaveChangesAsync();
                dtoErr.orderAssessmentItem = newAssessmentItem;
                return dtoErr;
            } catch(DbException ex) {
                dtoErr.Error = ex.Message;
                return dtoErr;
            } catch (Exception ex) {
                dtoErr.Error = ex.Message;
                return dtoErr;
            }
            
        }

        public async Task<ICollection<OrderAssessmentItemQ>> GetAssessmentQStdds()
        {
            var qs = await _context.AssessmentQStdds.OrderBy(x => x.QuestionNo).ToListAsync();
            
            var mapped = _mapper.Map<ICollection<OrderAssessmentItemQ>>(qs);

            return mapped;
            
        }

        public async Task<OrderAssessment> GetOrderAssessment(int orderId, string username)
        {
            var assessment = await _context.OrderAssessments
                .Include(x => x.OrderAssessmentItems.OrderBy(x => x.OrderItemId))
                .ThenInclude(x => x.OrderAssessmentItemQs.OrderBy(x => x.QuestionNo))
                .Where(x => x.OrderId == orderId)
                .FirstOrDefaultAsync() ?? await GenerateOrderAssessmentFromOrderId(orderId, username);
            
            return assessment;
        }
    
        public async Task<OrderAssessmentItemWithErrDto> GetOrCreateOrderAssessmentItem(int orderItemId, string loggedInUserName)
        {
            var errDto = new OrderAssessmentItemWithErrDto();

            var orderdata = await (from item in _context.OrderItems where item.Id == orderItemId
                join order in _context.Orders on item.OrderId equals order.Id 
                select new {OrderId=order.Id, OrderNo=order.OrderNo, 
                    OrderDate=order.OrderDate, CustomerName=order.Customer.CustomerName,
                    ProfessionGroup = item.Profession.ProfessionGroup,
                    ProfessionId = item.ProfessionId, ProfessionName = item.Profession.ProfessionName}
            ).FirstOrDefaultAsync();

            var assessmentItem = await _context.OrderAssessmentItems
                .Include(x => x.OrderAssessmentItemQs.OrderBy(m => m.QuestionNo))
                .Where(x => x.OrderItemId == orderItemId)
                .FirstOrDefaultAsync();
            var dto = new OrderAssessmentItemDto();

            if(assessmentItem != null) {
                dto = new OrderAssessmentItemDto {
                    Id = assessmentItem.Id,
                    OrderId = orderdata.OrderId, OrderDate = orderdata.OrderDate, OrderNo=orderdata.OrderNo,
                    CustomerName = orderdata.CustomerName, OrderItemId = orderItemId,
                    OrderAssessmentId=assessmentItem.OrderAssessmentId, 
                    ProfessionId=assessmentItem.ProfessionId == 0 ? orderdata.ProfessionId : assessmentItem.ProfessionId, 
                    ProfessionName=assessmentItem.ProfessionName ?? orderdata.ProfessionName, 
                    ProfessionGroup = orderdata.ProfessionGroup, DesignedBy = assessmentItem.DesignedBy, 
                    OrderAssessmentItemQs = assessmentItem.OrderAssessmentItemQs };
                errDto.orderAssessmentItemDto=dto;
                return errDto;                    
            }

            //CREATE NEW OBJECT
            var orderassessmentid = await _context.OrderAssessments
                .Where(x => x.OrderId==orderdata.OrderId).Select(x => x.Id).FirstOrDefaultAsync();
            
            dto = new OrderAssessmentItemDto {
                    OrderId=orderdata.OrderId, CustomerName = orderdata.CustomerName
                    , OrderDate = orderdata.OrderDate, OrderNo = orderdata.OrderNo
                    , ProfessionId = orderdata.ProfessionId
                    , ProfessionName = orderdata.ProfessionName
                    , ProfessionGroup = orderdata.ProfessionGroup
                    , OrderItemId = orderItemId
                    , DesignedBy = loggedInUserName
                    , OrderAssessmentItemQs = new List<OrderAssessmentItemQ>()
                    , OrderAssessmentId=0};
            
            if(orderassessmentid==0) {

                var orderassessment = new OrderAssessment {OrderId=orderdata.OrderId,
                    OrderDate=orderdata.OrderDate, OrderNo=orderdata.OrderNo, 
                    CustomerName=orderdata.CustomerName,DesignedByUsername=loggedInUserName,
                    DateDesigned=DateTime.UtcNow
                    //, OrderAssessmentItems = new List<OrderAssessmentItem> { assessmentItem }
                    };
                _context.OrderAssessments.Add(orderassessment);
                await _context.SaveChangesAsync();
                orderassessmentid=orderassessment.Id;
            } 
            
            dto.OrderAssessmentId=orderassessmentid;
            
            errDto.orderAssessmentItemDto = dto;
                
            return errDto;
            
        }

        public async Task<OrderAssessmentItem> SaveOrderAssessmentItem(OrderAssessmentItem orderItemAssessment)
        {
            if(orderItemAssessment.OrderAssessmentItemQs.Count == 0) return null;  // throw new Exception("Assessment Questions not defined");

            orderItemAssessment.CustomerName = await _context.GetCustomerNameFromOrderItemId(orderItemAssessment.OrderItemId);

            var orderNo = await _context.GetOrderNoFromOrderItemId(orderItemAssessment.OrderItemId);
            orderItemAssessment.OrderNo = orderNo;
            orderItemAssessment.AssessmentRef = orderNo + "-"; // + _context.GetSrNoFromOrderItemId(orderItemAssessment.OrderItemId);

            //_context.Entry(orderItemAssessment).State = EntityState.Added;
            _context.OrderAssessmentItems.Add(orderItemAssessment);
            //_context.Entry(orderItemAssessment.OrderAssessmentItemQs).State = EntityState.Added;

            try {
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                throw new Exception(ex.Message, ex);
            }
            return orderItemAssessment;
        }

        public async Task<ICollection<OrderAssessmentItemQ>> GetOrderAssessmentItemQsFromOrderItemId(int orderitemid)
        {
            var obj = await (from assessment in  _context.OrderAssessmentItems 
                    where assessment.OrderItemId == orderitemid
                join Q in _context.OrderAssessmentItemQs on assessment.Id equals Q.OrderAssessmentItemId
                select Q).ToListAsync();
            
            return obj;

        }

        public async Task<ICollection<OrderAssessmentItemHeaderDto>> GetOrderAssessmentHeaders(string ProfessionGroup)
        {
            var query = await (from assessmt in _context.OrderAssessmentItems where assessmt.ProfessionGroup==ProfessionGroup
                join item in _context.OrderItems on assessmt.OrderItemId equals item.Id 
                orderby assessmt.DateDesigned descending, assessmt.OrderItemId
            select new OrderAssessmentItemHeaderDto {
                Id = assessmt.Id, CategoryRef = assessmt.OrderNo + "-" + item.SrNo,
                CustomerName = assessmt.CustomerName, DateDesigned = assessmt.DateDesigned, 
                ProfessionName = assessmt.ProfessionName, OrderItemId=assessmt.OrderItemId}).ToListAsync();
            
            return query;
                        
        }

        public async Task<ICollection<OrderAssessmentItemQ>> GetOrderAssessmentItemQsFromId(int orderassessmentitemid)
        {
            var obj = await _context.OrderAssessmentItemQs.Where(x => x.OrderAssessmentItemId == orderassessmentitemid)
                .OrderBy(x => x.QuestionNo).ToListAsync();
                
            return obj;
        }

        public async Task<SingleStringDto> GetAndSetProfessionGroupFromProfessionId(int ProfessionId, int OrderAssessmentItemId)
        {
            var grp = await _context.Professions.Where(x => x.Id == ProfessionId).Select(x => x.ProfessionGroup).FirstOrDefaultAsync();
            var dto = new SingleStringDto {StringValue = grp};

            /*if(OrderAssessmentItemId != 0) {
                var OrderAssessmentItem = await _context.OrderAssessmentItems.FindAsync(OrderAssessmentItemId);
                if(OrderAssessmentItem == null) return dto;

                OrderAssessmentItem.ProfessionGroup = dto.StringValue;
                _context.Entry(OrderAssessmentItem).State = EntityState.Modified;
            } */

            await _context.SaveChangesAsync();

            return dto;
        }
    }
}
