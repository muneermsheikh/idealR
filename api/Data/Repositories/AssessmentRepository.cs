using api.Entities.Admin.Order;
using api.Entities.HR;
using api.Extensions;
using api.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Repositories
{
    public class AssessmentRepository : IAssessmentRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public AssessmentRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<bool> DeleteOrderItemAssessment(int orderItemId)
        {
            var obj = await _context.orderItemAssessments.Include(x => x.OrderItemAssessmentQs)
                .Where(x => x.OrderItemId == orderItemId).FirstOrDefaultAsync();
            
            if(obj == null) return false;

            _context.Entry(obj).State = EntityState.Deleted;

            try {
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                throw new Exception(ex.Message, ex);
            }

            return true;

        }

        public async Task<bool> DeleteOrderItemAssessmentQ(int AssessmentQId)
        {
            var obj = await _context.OrderItemAssessmentQs.FindAsync(AssessmentQId);
            if(obj == null) return false;
            _context.Entry(obj).State = EntityState.Deleted;

            try{
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                throw new Exception(ex.Message, ex);
            }

            return true;
        }

        public async Task<bool> EditOrderItemAssessment(OrderItemAssessment newObject)
        {
             var existingObject = _context.orderItemAssessments
                .Where(x => x.Id == newObject.Id)
                .Include(x => x.OrderItemAssessmentQs)
                .AsNoTracking()
                .SingleOrDefault();
            
            if (existingObject == null) return false;

            _context.Entry(existingObject).CurrentValues.SetValues(newObject);

            //delete records in existingObject that are not present in new object
            foreach (var existingItem in existingObject.OrderItemAssessmentQs.ToList())
            {
                if(!newObject.OrderItemAssessmentQs.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                {
                    _context.OrderItemAssessmentQs.Remove(existingItem);
                    _context.Entry(existingItem).State = EntityState.Deleted; 
                }

                 //items in current object - either updated or new items
                foreach(var newItem in newObject.OrderItemAssessmentQs)
                {
                    var itemExisting = existingObject.OrderItemAssessmentQs
                        .Where(c => c.Id == newItem.Id && c.Id != default(int)).SingleOrDefault();
                    
                    if(itemExisting != null)    //update navigation record
                    {
                        _context.Entry(itemExisting).CurrentValues.SetValues(newItem);
                        _context.Entry(itemExisting).State = EntityState.Modified;
                    } else {    //insert new navigation record
                        var itemToInsert = new OrderItemAssessmentQ
                        {
                            OrderItemAssessmentId = existingObject.Id,
                            QuestionNo = newItem.QuestionNo,
                            Subject = newItem.Subject,
                            Question = newItem.Question,
                            MaxPoints = newItem.MaxPoints,
                            IsMandatory = newItem.IsMandatory
                        };

                        existingObject.OrderItemAssessmentQs.Add(itemToInsert);
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

        public async Task<OrderItemAssessment> GenerateOrderItemAssessmentFromStddQ(int orderItemId, string loggedInUserName)
        {   
            //verify orderItemId is valid
            var exists = await _context.OrderItems.FindAsync(orderItemId);
            if (exists == null) return null;
            
            var OrderNumber = await _context.GetOrderNoFromOrderItemId(orderItemId);
            var stdd = await _context.AssessmentQStdds.OrderBy(x => x.QuestionNo).ToListAsync();

            var ListQ=new List<OrderItemAssessmentQ>();
            foreach (var q in stdd)
            {
                ListQ.Add(new OrderItemAssessmentQ{
                    QuestionNo = q.QuestionNo,
                    Question=q.Question,
                    Subject=q.Subject,
                    MaxPoints = q.MaxPoints,
                    IsMandatory=q.IsMandatory
                });
            }

            var newAssessment = new OrderItemAssessment{
                OrderItemId = orderItemId,
                CustomerName = await _context.GetCustomerNameFromOrderItemId(orderItemId),
                OrderNo = OrderNumber,
                AssessmentRef = OrderNumber + "-" + _context.GetSrNoFromOrderItemId(orderItemId),
                DateDesigned = DateOnly.FromDateTime(DateTime.UtcNow),
                DesignedBy = loggedInUserName,
                OrderItemAssessmentQs = ListQ
            };

            return newAssessment;
        }

        public async Task<ICollection<AssessmentQStdd>> GetAssessmentQStdds()
        {
            return await _context.AssessmentQStdds.OrderBy(x => x.QuestionNo).ToListAsync();
        }

        public async Task<ICollection<OrderItemAssessment>> GetOrderAssessments(int orderId)
        {
            var orderItemIds = await _context.OrderItems.Where(x => x.OrderId == orderId)
                .OrderBy(x => x.Id).Select(x => x.Id).ToListAsync();
            
            if(orderItemIds.Count()==0) return null;

             return await _context.orderItemAssessments
                .Include(x => x.OrderItemAssessmentQs)
                .Where(x => orderItemIds.Contains(x.OrderItemId))
                .ToListAsync();
        }
    
        public async Task<OrderItemAssessment> GetOrderItemAssessment(int orderItemId)
        {
            var assessmt = await _context.orderItemAssessments
                .Include(x => x.OrderItemAssessmentQs)
                .Where(x => x.OrderItemId == orderItemId)
                .FirstOrDefaultAsync();
            
            return assessmt;
        }

        public async Task<OrderItemAssessment> SaveOrderItemAssessment(OrderItemAssessment orderItemAssessment)
        {
            if(orderItemAssessment.OrderItemAssessmentQs.Count == 0) throw new Exception("Assessment Questions not defined");

            orderItemAssessment.CustomerName = await _context.GetCustomerNameFromOrderItemId(orderItemAssessment.OrderItemId);

            var orderNo = await _context.GetOrderNoFromOrderItemId(orderItemAssessment.OrderItemId);
            orderItemAssessment.OrderNo = orderNo;
            orderItemAssessment.AssessmentRef = orderNo + "-"; // + _context.GetSrNoFromOrderItemId(orderItemAssessment.OrderItemId);

            //_context.Entry(orderItemAssessment).State = EntityState.Added;
            _context.orderItemAssessments.Add(orderItemAssessment);
            //_context.Entry(orderItemAssessment.OrderItemAssessmentQs).State = EntityState.Added;

            try {
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                throw new Exception(ex.Message, ex);
            }
            return orderItemAssessment;
        }
    }
}
