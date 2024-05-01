using api.Entities.Admin;
using api.Extensions;
using api.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Repositories
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public FeedbackRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<bool> DeleteFeedback(int id)
        {
            var feedbk = await _context.Feedbacks.Include(x => x.FeedbackItems).FirstOrDefaultAsync();
            if(feedbk == null) return false;

            _context.Feedbacks.Remove(feedbk);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> EditFeedback(Feedback feedback)
        {
            var existingObject = _context.Feedbacks
                .Where(x => x.Id == feedback.Id)
                .Include(x => x.FeedbackItems)
                .AsNoTracking()
                .SingleOrDefault();
            
            if (existingObject == null) return false;

            _context.Entry(existingObject).CurrentValues.SetValues(feedback);

            //delete records in existingObject that are not present in new object
            foreach (var existingItem in existingObject.FeedbackItems.ToList())
            {
                if(!feedback.FeedbackItems.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                {
                    _context.FeedbackItems.Remove(existingItem);
                    _context.Entry(existingItem).State = EntityState.Deleted; 
                }
            }

            //items in current object - either updated or new items
            foreach(var newItem in feedback.FeedbackItems)
            {
                var existingItem = existingObject.FeedbackItems
                    .Where(c => c.Id == newItem.Id && c.Id != default(int)).SingleOrDefault();
                if(existingItem != null)    //update navigation record
                {
                    _context.Entry(existingItem).CurrentValues.SetValues(newItem);
                    _context.Entry(existingItem).State = EntityState.Modified;
                } else {    //insert new navigation record
                    var itemToInsert = new FeedbackItem
                    {
                        FeedbackId = existingObject.Id,
                        FeedbackGroup = newItem.FeedbackGroup,
                        FeedbackQNo = newItem.FeedbackQNo,
                        FeedbackQuestion = newItem.FeedbackQuestion,
                        IsMandatory = newItem.IsMandatory,
                        Response = newItem.Response,
                        Remarks = newItem.Remarks
                    };

                    existingObject.FeedbackItems.Add(itemToInsert);
                    _context.Entry(itemToInsert).State = EntityState.Added;
                }
            }

            _context.Entry(existingObject).State = EntityState.Modified;

            return await _context.SaveChangesAsync() > 0;

        }

        public async Task<Feedback> GetFeedbackFromId(int id)
        {
            var obj = await _context.Feedbacks.Include(x => x.FeedbackItems.OrderBy(x => x.FeedbackQNo))
                .FirstOrDefaultAsync();
                
            return obj;
        }

        public async Task<ICollection<Feedback>> GetFeedbackFromCustomerName(string customerName)
        {
            var obj = await _context.Feedbacks.Include(x => x.FeedbackItems.OrderBy(x => x.FeedbackQNo))
                .Where(x => x.CustomerName.ToLower() == customerName.ToLower()).ToListAsync();

            return obj;
        }

        public async Task<Feedback> InsertFeedback(Feedback feedback)
        {
            _context.Feedbacks.Add(feedback);
            if (await _context.SaveChangesAsync() >0) return feedback;
            return null;
        }


        public async Task<FeedbackItem> InsertFeedbackItem(FeedbackItem feedbackItem)
        {
            _context.FeedbackItems.Add(feedbackItem);
            if (await _context.SaveChangesAsync() > 0) return feedbackItem;
            return null;
        }


        public async Task<ICollection<FeedbackItem>> InsertFeedbackItems(ICollection<FeedbackItem> feedbackItems)
        {
            foreach(var item in feedbackItems)
            {
                _context.FeedbackItems.Add(item);
            }

            if (await _context.SaveChangesAsync() > 0) return feedbackItems;

            return null;
            
        }

        public async Task<ICollection<FeedbackStddQ>> InsertFeedbackStddQs(ICollection<FeedbackStddQ> feedbackStddQs)
        {
            var qList = new List<FeedbackStddQ>();

            foreach(var q in feedbackStddQs) {
                var stddQ = _context.feedbackStddQs.Where(x => x.FeedbackQuestion == q.FeedbackQuestion).FirstOrDefault();
                if (stddQ == null) qList.Add(q);
            }
            
            if(qList.Count == 0) return null;

            foreach(var feedback in qList) {
                _context.Entry(feedback).State = EntityState.Added;
            }
            
            if(await _context.SaveChangesAsync() >0) {
                return qList;
            } else {
                return null;
            }
        }

        public async Task<Feedback> GenerateNewFeedback(int customerId)
        {
            var StddQs = await _context.feedbackStddQs.OrderBy(x => x.FeedbackQNo).ToListAsync();
            if(StddQs.Count == 0) return null;

            var feedbackItems = _mapper.Map<List<FeedbackItem>>(StddQs);

            var feedback = new Feedback{
                CustomerName = await _context.CustomerNameFromId(customerId),
                IssuedOn = DateOnly.FromDateTime(DateTime.UtcNow),
                FeedbackItems = feedbackItems
            };

            return feedback;
        }

        public async Task<ICollection<FeedbackStddQ>> GetFeedbackStddQs()
        {
            return await _context.feedbackStddQs.OrderBy(x => x.FeedbackQNo).ToListAsync();
            
        }

    }
}