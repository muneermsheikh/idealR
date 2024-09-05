using System.Data.Common;
using api.DTOs.Admin;
using api.DTOs.Admin.Orders;
using api.DTOs.Orders;
using api.Entities.Admin.Order;
using api.Entities.HR;
using api.Extensions;
using api.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Repositories
{
    public class OrderAssessmentRepository : IOrderAssessmentRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly DateTime _todaydate;
        public OrderAssessmentRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
            _todaydate = DateTime.UtcNow;
        }

        public async Task<OrderAssessment> SaveNewOrderAssessment(OrderAssessment orderAssessment)
        {
            _context.OrderAssessments.Add(orderAssessment);

            return await _context.SaveChangesAsync() > 0 ? orderAssessment : null;
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
                        var itemToInsert = new OrderAssessmentItem
                        {
                            CustomerName = newItem.CustomerName,
                            DateDesigned = _todaydate,
                            DesignedBy = Username,
                            OrderItemId = newItem.OrderItemId,
                            OrderNo = newItem.OrderNo, 
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

        public async Task<bool> EditOrderAssessmentItem(OrderAssessmentItem newObject)
        {
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
            }
            
            _context.Entry(existingObject).State = EntityState.Modified;
            try{
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                throw new Exception(ex.Message, ex);
            }
            
            return true;
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
        private async Task<OrderAssessmentItemWithErrDto> GenerateOrderAssessmentItemFromStddQ(int orderItemId, string loggedInUserName)
        {   
            var dtoErr = new OrderAssessmentItemWithErrDto();

            //ascertain the record does not exist;
            var assessmtItem = await _context.OrderAssessmentItems.Where(x => x.OrderItemId==orderItemId).FirstOrDefaultAsync();
            if(assessmtItem != null) {
                dtoErr.orderAssessmentItem = assessmtItem;
                return dtoErr;
            }

            //verify orderItemId is valid
            var orderitem = await _context.OrderItems.FindAsync(orderItemId);
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
                CustomerName = await _context.GetCustomerNameFromOrderItemId(orderItemId),
                OrderNo = order.OrderNo,
                AssessmentRef = order.OrderNo + "-" + _context.GetSrNoFromOrderItemId(orderItemId),
                DateDesigned = _todaydate,
                DesignedBy = loggedInUserName,
                OrderAssessmentItemQs = ListQ
            };

            //if ordderassessment exists, get the id to use in orderassessmentitem
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

        public async Task<ICollection<OrderAssessmentItemQ>> GetCustomAssessmentQsForAProfession(int professionid)
        {
            var qs = await _context.AssessmentQBanks
                .Include(x => x.AssessmentStddQs.OrderBy(x => x.QNo))
                .Where(x => x.ProfessionId == professionid)
                .Select(x => x.AssessmentStddQs)
                .ToListAsync();

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
            
            var dtoErr = new OrderAssessmentItemWithErrDto();

            var assessmtItem = await _context.OrderAssessmentItems
                .Include(x => x.OrderAssessmentItemQs.OrderBy(x => x.QuestionNo))
                .Where(x => x.OrderItemId == orderItemId)
                .FirstOrDefaultAsync();   // ?? await GenerateOrderAssessmentItemFromStddQ(orderItemId, loggedInUserName);
            if(assessmtItem == null) {
                var assitemDto = await GenerateOrderAssessmentItemFromStddQ(orderItemId, loggedInUserName);
                if(string.IsNullOrEmpty(assitemDto.Error)) {
                    dtoErr.orderAssessmentItem = assitemDto.orderAssessmentItem;
                }
            }
            
            if(string.IsNullOrEmpty(assessmtItem.CustomerName)) 
                assessmtItem.CustomerName = await _context.GetCustomerIdAndNameFromOrderId(assessmtItem.OrderId);
            if(string.IsNullOrEmpty(assessmtItem.DesignedBy)) assessmtItem.DesignedBy=loggedInUserName;
            if(assessmtItem.OrderNo==0) assessmtItem.OrderNo=await _context.GetOrderNoFromOrderItemId(assessmtItem.OrderItemId);
            if(assessmtItem.ProfessionId==0) assessmtItem.ProfessionId=await _context.GetProfessionIdFromOrderItemId(assessmtItem.OrderItemId);
            if(string.IsNullOrEmpty(assessmtItem.ProfessionName)) assessmtItem.ProfessionName=await _context.GetProfessionNameFromId(assessmtItem.ProfessionId);

            if(assessmtItem.OrderAssessmentItemQs.Count==0) {
                var stddQs = await _context.AssessmentQStdds.OrderBy(x => x.QuestionNo).ToListAsync();

                var assessmentItemQs =new List<OrderAssessmentItemQ>();     //part of OrderAssessmentItem
                foreach (var q in stddQs)
                {
                    assessmentItemQs.Add(new OrderAssessmentItemQ{
                        OrderAssessmentItemId = assessmtItem.Id,
                        QuestionNo = q.QuestionNo,
                        Question=q.Question,
                        Subject=q.Subject,
                        MaxPoints = q.MaxPoints,
                        IsMandatory=q.IsMandatory
                    });
                }

                assessmtItem.OrderAssessmentItemQs = assessmentItemQs;
            }

            dtoErr.orderAssessmentItem = assessmtItem;
            return dtoErr;
        }

        public async Task<OrderAssessmentItem> SaveOrderAssessmentItem(OrderAssessmentItem orderItemAssessment)
        {
            if(orderItemAssessment.OrderAssessmentItemQs.Count == 0) throw new Exception("Assessment Questions not defined");

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

        public async Task<ICollection<OrderAssessmentItemQ>> GetOrderAssessmentItemQs(int orderitemid)
        {
            var obj = await (from assessment in  _context.OrderAssessmentItems 
                    where assessment.OrderItemId == orderitemid
                join Q in _context.OrderAssessmentItemQs on assessment.Id equals Q.OrderAssessmentItemId
                select Q).ToListAsync();
            
            return obj;

        }

    }
}
