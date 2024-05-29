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
        private readonly DateOnly _todaydate;
        public OrderAssessmentRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
            _todaydate = DateOnly.FromDateTime(DateTime.UtcNow);
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

        public async Task<bool> EditOrderAssessment(OrderAssessment newObject, string Username)
        {
             var existingObject = _context.OrderAssessments
                .Where(x => x.Id == newObject.Id)
                .Include(x => x.OrderAssessmentItems)
                .ThenInclude(x => x.OrderAssessmentItemQs)
                .AsNoTracking()
                .SingleOrDefault();
            
            if (existingObject == null) return false;

            
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

                _context.Entry(existingObject).State = EntityState.Modified;
            }

            try{
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                throw new Exception(ex.Message, ex);
            }
            
            return true;
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
        
                _context.Entry(existingObject).State = EntityState.Modified;
            }

            try{
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                throw new Exception(ex.Message, ex);
            }
            
            return true;
        }

        public async Task<OrderAssessmentItem> GenerateOrderAssessmentItemFromStddQ(int orderItemId, string loggedInUserName)
        {   
            //verify orderItemId is valid
            var exists = await _context.OrderItems.FindAsync(orderItemId);
            if (exists == null) return null;
            
            var OrderNumber = await _context.GetOrderNoFromOrderItemId(orderItemId);
            var stdd = await _context.AssessmentQStdds.OrderBy(x => x.QuestionNo).ToListAsync();

            var ListQ=new List<OrderAssessmentItemQ>();
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

            var newAssessment = new OrderAssessmentItem{
                OrderItemId = orderItemId,
                CustomerName = await _context.GetCustomerNameFromOrderItemId(orderItemId),
                OrderNo = OrderNumber,
                AssessmentRef = OrderNumber + "-" + _context.GetSrNoFromOrderItemId(orderItemId),
                DateDesigned = _todaydate,
                DesignedBy = loggedInUserName,
                OrderAssessmentItemQs = ListQ
            };

            return newAssessment;
        }

        public async Task<ICollection<AssessmentQStdd>> GetAssessmentQStdds()
        {
            return await _context.AssessmentQStdds.OrderBy(x => x.QuestionNo).ToListAsync();
        }

        public async Task<OrderAssessment> GetOrderAssessment(int orderId)
        {
            return await _context.OrderAssessments
                .Include(x => x.OrderAssessmentItems)
                .ThenInclude(x => x.OrderAssessmentItemQs)
                .Where(x => x.OrderId == orderId)
                .FirstOrDefaultAsync();
        }
    
        public async Task<OrderAssessmentItem> GetOrderAssessmentItem(int orderItemId)
        {
            var assessmt = await _context.OrderAssessmentItems
                .Include(x => x.OrderAssessmentItemQs)
                .Where(x => x.OrderItemId == orderItemId)
                .FirstOrDefaultAsync();
            
            return assessmt;
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
            var obj = await (from assessment in  _context.orderItemAssessments 
                    where assessment.OrderItemId == orderitemid
                join Q in _context.OrderAssessmentItemQs on assessment.Id equals Q.OrderAssessmentItemId
                select Q).ToListAsync();
            
            return obj;

        }

    }
}
