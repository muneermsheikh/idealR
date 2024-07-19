using api.DTOs.Admin;
using api.DTOs.HR;
using api.Entities.Admin.Client;
using api.Helpers;
using api.Interfaces.Admin;
using api.Interfaces.HR;
using api.Params;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Repositories.HR
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IComposeMessagesAdminRepository _msgRep;
        public FeedbackRepository(DataContext context, IMapper mapper, IComposeMessagesAdminRepository msgRep)
        {
            _msgRep = msgRep;
            _mapper = mapper;
            _context = context;
        }


        public async Task<bool> DeleteFeedback(int id)
        {
            var obj = await _context.CustomerFeedbacks.Include(x => x.FeedbackItems).Where(x => x.Id==id).FirstOrDefaultAsync();
            if (obj == null) return false;

            _context.CustomerFeedbacks.Remove(obj);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteFeedbackItem(int FeedbackItemId)
        {
            var obj = await _context.FeedbackItems.FindAsync(FeedbackItemId);
            if (obj == null) return false;

            _context.FeedbackItems.Remove(obj);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<string> EditFeedback(CustomerFeedback model)
        {
            var existing = await _context.CustomerFeedbacks
                .Include(x => x.FeedbackItems)
                .Where(x => x.Id == model.Id)
                .FirstOrDefaultAsync();
            
            if(existing == null) return "No such record exists to edit";

            //coopy values of the model object to existing object
            _context.Entry(existing).CurrentValues.SetValues(model);

            //Delete any objects that are in the existing object, but not in the model
            foreach (var existingItem in existing.FeedbackItems.ToList())
            {
                if(!model.FeedbackItems.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                {
                    _context.FeedbackItems.Remove(existingItem);
                    _context.Entry(existingItem).State = EntityState.Deleted; 
                }
            }

            //iterate through each model item, compare their id values to the existing records
            //if id exists, then update, else insert new records
            foreach(var newItem in model.FeedbackItems)
            {
                var existingItem = existing.FeedbackItems.Where(c => 
                    c.Id == newItem.Id && c.Id != default(int)).SingleOrDefault();
                
                if(existingItem != null)    //update existing item, by copying model item
                {
                    _context.Entry(existingItem).CurrentValues.SetValues(newItem);
                    _context.Entry(existingItem).State = EntityState.Modified;
                } else {    //insert new navigation record
                    var itemToInsert = new FeedbackItem
                    {
                        CustomerFeedbackId=existing.Id,
                        QuestionNo=newItem.QuestionNo,
                        Question = newItem.Question,
                        Prompt1 = newItem.Prompt1,
                        Prompt2 = newItem.Prompt2,
                        Prompt3 = newItem.Prompt3,
                        Prompt4 = newItem.Prompt4,
                        Response = newItem.Response,
                        Remarks = newItem.Remarks
                    };

                    existing.FeedbackItems.Add(itemToInsert);
                    _context.Entry(itemToInsert).State = EntityState.Added;
                }
            }

            _context.Entry(existing).State = EntityState.Modified;  
            
            return await _context.SaveChangesAsync() > 0 ? "" : "Failed to update the Feedback object";

        }

        public async Task<CustomerFeedback> GenerateOrGetFeedbackFromId(int FeedbackId, int CustomerId)
        {
            //check if any Feedback exists n the month of feedbackDAte given
            /*var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;

            var fdbk = await _context.CustomerFeedbacks.Include(x => x.FeedbackItems.OrderBy(x => x.QuestionNo))
                .Where(x => x.DateIssued.Month == month && x.DateIssued.Year == year && x.CustomerId==CustomerId)
                .FirstOrDefaultAsync();
            */
            
            if(FeedbackId == 0) {   //generate new feedback for the customer
                var inputQs = await _context.FeedbackQs.OrderBy(x => x.QuestionNo)
                    .Select(x => new FeedbackItem {
                        FeedbackGroup=x.FeedbackGroup,
                        Question=x.Question, QuestionNo = x.QuestionNo,
                        Response="", Prompt1=x.Prompt1, Prompt2=x.Prompt2, 
                        Prompt3=x.Prompt3, Prompt4=x.Prompt4
                    }).ToListAsync();
                
                var fdbk = await (from cust in _context.Customers where cust.Id == CustomerId
                    join off in _context.CustomerOfficials on cust.Id equals off.CustomerId 
                    where off.Status == "Active" && off.Divn=="HR"
                    orderby off.PriorityHR
                    select new CustomerFeedback{
                        CustomerId = CustomerId, CustomerName = cust.CustomerName, City=cust.City,
                        OfficialName=off.OfficialName, Designation = off.Designation,
                        Email = off.Email, PhoneNo = off.PhoneNo, DateIssued = DateOnly.FromDateTime(DateTime.Now),
                        FeedbackItems=inputQs}
                ).FirstOrDefaultAsync();

                return fdbk;
            } else {
                var fdbk = await _context.CustomerFeedbacks.Where(x => x.Id == FeedbackId)
                    .Include(x => x.FeedbackItems).FirstOrDefaultAsync();
                return fdbk;
            }
                
        }

        public async Task<ICollection<FeedbackHistoryDto>> CustomerFeedbackHistory(int customerId) {
            
            var hist = await _context.CustomerFeedbacks.Where(x => x.CustomerId==customerId)
                .Select(x => new FeedbackHistoryDto{
                    FeedbackId=x.Id, FeedbackIssueDate=x.DateIssued, 
                    //FeedbackRecdDate= x.DateReceived
                })
                .OrderByDescending(x => x.FeedbackIssueDate)
                .ToListAsync();
            
            return hist;
            
        }

        public async Task<PagedList<FeedbackDto>> GetFeedbackList(FeedbackParams fParams)
        {
            var query = _context.CustomerFeedbacks.Include(x => x.FeedbackItems).AsQueryable();

            if(fParams.CustomerId > 0) query = query.Where(x => x.Id == fParams.CustomerId);
            
            if(fParams.DateIssued.Year > 2000) query = query.Where(x => 
                x.DateIssued == fParams.DateIssued);

            if(fParams.DateReceived.Year > 2000) query = query.Where(x => 
                    x.DateReceived == fParams.DateIssued);
            if(!string.IsNullOrEmpty(fParams.Email)) query = query.Where(x => x.Email == fParams.Email);
            if(!string.IsNullOrEmpty(fParams.PhoneNo)) query = query.Where(x => x.PhoneNo == fParams.PhoneNo);
            
            var paged = await PagedList<FeedbackDto>.CreateAsync(query.AsNoTracking()
                    .ProjectTo<FeedbackDto>(_mapper.ConfigurationProvider),
                    fParams.PageNumber, fParams.PageSize);
            
            return paged;
        }

        public async Task<ICollection<FeedbackQ>> GetFeedbackStddQs()
        {
            return await _context.FeedbackQs.OrderBy(x => x.QuestionNo).ToListAsync();
        }

        public async Task<CustomerFeedback> GetFeedbackWithItems(int feedbackId)
        {
             var feedbkinput = await _context.CustomerFeedbacks
                .Include(x => x.FeedbackItems.OrderBy(x => x.QuestionNo))                
                    .Select(x => new CustomerFeedback{
                        City=x.City, CustomerId=x.CustomerId, CustomerName=x.CustomerName, 
                        CustomerSuggestion=x.CustomerSuggestion,
                        DateIssued=x.DateIssued, Designation=x.Designation, Email=x.Email, PhoneNo=x.PhoneNo,
                        GradeAssessedByClient = x.GradeAssessedByClient, OfficialName=x.OfficialName,
                        FeedbackItems = x.FeedbackItems
                }).FirstOrDefaultAsync();
            
            return feedbkinput;
        }

        private async Task<int> NewFeedbackNo() {
            var newno = await _context.CustomerFeedbacks.MaxAsync(x => x.FeedbackNo);
            if(newno==0) newno=1001;
            return newno;
        }
        public async Task<CustomerFeedback> SaveNewFeedback(FeedbackInput fInput)
        {
            var responses = fInput.FeedbackInputItems;

            var items = new List<FeedbackItem>();

            foreach(var item in responses) {
                var feedbackitem = new FeedbackItem{
                    FeedbackGroup = item.FeedbackGroup, QuestionNo=item.QuestionNo, Question=item.Question,
                    Prompt1 = item.Prompt1, Prompt2=item.Prompt2, Prompt3=item.Prompt3, Prompt4=item.Prompt4,
                    Response=item.Response, Remarks=item.Remarks};
                items.Add(feedbackitem);
            }
            
            var feedback = new CustomerFeedback{
                FeedbackNo = await NewFeedbackNo(),
                CustomerId=fInput.CustomerId, CustomerName = fInput.CustomerName,
                City=fInput.City, OfficialName=fInput.OfficialName, 
                GradeAssessedByClient=fInput.GradeAssessedByClient,
                Designation = fInput.Designation, Email = fInput.Email,
                PhoneNo = fInput.PhoneNo, DateIssued = DateOnly.FromDateTime(fInput.DateIssued),
                DateReceived = DateOnly.FromDateTime(DateTime.Now),
                CustomerSuggestion = fInput.CustomerSuggestion,
                FeedbackItems = items
            };

            _context.CustomerFeedbacks.Add(feedback);

            return await _context.SaveChangesAsync() > 0 ? feedback : null;
        }

        public async Task<string> SendFeedbackEmailToCustomer(int feedbackId, string Url, string username)
        {
            var strErr="";

            var msgWithErr = await _msgRep.ComposeFeedbackMailToCustomer(feedbackId, Url, username);
            if(msgWithErr.Messages.Count > 0) {
                _context.Messages.Add(msgWithErr.Messages.FirstOrDefault());
                strErr = await _context.SaveChangesAsync() > 0 ? "" : msgWithErr.ErrorString;
            } else {
                strErr = msgWithErr.ErrorString;
            }

            return strErr;
        }
    }
}