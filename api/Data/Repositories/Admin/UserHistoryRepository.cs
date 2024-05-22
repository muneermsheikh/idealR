using api.DTOs.Admin;
using api.Entities.Admin;
using api.Helpers;
using api.Interfaces.Admin;
using api.Params.Admin;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DocumentFormat.OpenXml.Office2019.Drawing.Model3D;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Repositories.Admin
{
    public class UserHistoryRepository : IUserHistoryRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly DateOnly _today = DateOnly.FromDateTime(DateTime.UtcNow);
        public UserHistoryRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }


        public async Task<UserHistory> AddNewUserHistory(UserHistory userHistory)
        {
            _context.Entry(userHistory).State = EntityState.Added;

            try {
                await _context.SaveChangesAsync();
            } catch {
                return null;
            }

            return userHistory;
        }

        public async Task<UserHistoryItem> AddNewHistoryItem(UserHistoryItem item, string Username)
        {
            item.Username = Username;            

            _context.Entry(item).State = EntityState.Added;

            try {
                await _context.SaveChangesAsync();
            } catch {
                return null;
            }

            return item;
        }

        public async Task<bool> DeleteUserHistory(int historyId)
        {
            var existing = await _context.UserHistories.FindAsync(historyId);

            if (existing == null) return false;

            _context.UserHistories.Remove(existing);

            return await _context.SaveChangesAsync() > 0;
        }


        public async Task<bool> DeleteUserHistoryItem(int historyitemid)
        {
             var existing = await _context.UserHistoryItems.FindAsync(historyitemid);
            if (existing == null) return false;

            _context.UserHistoryItems.Remove(existing);
            _context.Entry(existing).State = EntityState.Deleted;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<UserHistoryReturnDto> EditContactHistory(UserHistory model, string Username)
        {
            var dtoToRetun = new UserHistoryReturnDto();
            
            var existing = await _context.UserHistories.Include(x => x.UserHistoryItems)
                .Where(x => x.Id == model.Id).AsNoTracking().FirstOrDefaultAsync();
            
            if (existing == null) return null;

            _context.Entry(existing).CurrentValues.SetValues(model);

            foreach (var existingItem in existing.UserHistoryItems.ToList())
            {
                if(!model.UserHistoryItems.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                {
                    _context.UserHistoryItems.Remove(existingItem);
                    _context.Entry(existingItem).State = EntityState.Deleted; 
                }
            }
            
            foreach(var newItem in model.UserHistoryItems)
            {
                var existingItem = existing.UserHistoryItems
                    .Where(c => c.Id == newItem.Id && c.Id != default(int)).SingleOrDefault();
                if(existingItem != null)    //update navigation record
                {
                    _context.Entry(existingItem).CurrentValues.SetValues(newItem);
                    _context.Entry(existingItem).State = EntityState.Modified;
                } else {    //insert new navigation record
                        
                    var itemToInsert = new UserHistoryItem
                    {
                        ContactResult = newItem.ContactResult,
                        DateOfContact = newItem.DateOfContact,
                        GistOfDiscussions = newItem.GistOfDiscussions,
                        IncomingOutgoing = newItem.IncomingOutgoing,
                        PhoneNo = newItem.PhoneNo,
                        UserHistoryId = model.Id,
                        Username = Username
                    };

                    existing.UserHistoryItems.Add(itemToInsert);
                    _context.Entry(itemToInsert).State = EntityState.Added;
                }
            }

            _context.Entry(existing).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch {
                dtoToRetun.ErrorString = "Failed to update the object";
                return dtoToRetun;
            }

            dtoToRetun.UserHistory = model;
            return dtoToRetun;
        }

        public async Task<ICollection<UserHistoryItem>> EditHistoryItems(ICollection<UserHistoryItem> items, string Username)
        {
            var itemList = new List<UserHistoryItem>();

            foreach(var item in items) {
                if(item.UserHistoryId == 0) continue;
                var existing = await _context.UserHistoryItems.FindAsync(item.Id);
                if(existing != null) continue;
                _context.Entry(existing).CurrentValues.SetValues(item);
                itemList.Add(existing);
            }
            
            try {
                await _context.SaveChangesAsync();
            } catch {
                return null;
            }
            
            return itemList;


        }

        public async Task<UserHistory> GetHistoryByPersonId(UserHistoryParams hParams)
        {
            var query = _context.UserHistories.Include(x => x.UserHistoryItems)
                .AsQueryable();
            
            if(hParams.CandidateId !=0) {
                query = query.Where(x => x.CandidateId == hParams.CandidateId);
            } else if(!string.IsNullOrEmpty(hParams.ResumeId)) {
                query = query.Where(x => x.ResumeId == hParams.ResumeId);
            } else {
                return null;
            }
            
            return await query.SingleOrDefaultAsync();

        }

        public async Task<UserHistoryDto> GetHistoryWithItemsFromHistoryId(int historyId)
        {
            var obj = await (from hist in _context.UserHistories where hist.Id == historyId
                join item in _context.UserHistoryItems on hist.Id equals item.UserHistoryId
                select new UserHistoryDto {
                    ResumeId = hist.ResumeId, AlternateEmailId = hist.AlternateEmailId, AlternatePhoneNo= hist.AlternatePhoneNo,
                    ApplicationNo=hist.ApplicationNo, CandidateId=(int)hist.CandidateId, Id=hist.Id, CandidateName = hist.CandidateName,
                    CategoryRef=hist.CategoryRef, MobileNo = hist.MobileNo, CreatedOn = hist.CreatedOn, Concluded = hist.Concluded,
                    ConcludedByName = hist.ConcludedByUsername, EmailId = hist.EmailId, Gender = hist.Gender, Status = hist.Status,
                    UserName = hist.UserName,  UserHistoryItems = (ICollection<UserHistoryItemDto>)hist.UserHistoryItems
                }).FirstOrDefaultAsync();
            
            return obj;
        }


        public async Task<UserHistory> GetOrAddUserHistoryByCandidateId(int candidateId)
        {
            var obj = await _context.UserHistories.Where(x => x.CandidateId == candidateId).FirstOrDefaultAsync();
            if(obj==null) {
                obj = await (from cand in _context.Candidates where cand.Id == candidateId
                        select new UserHistory {
                            ApplicationNo = cand.ApplicationNo, CandidateId = candidateId,
                            CandidateName=cand.FullName, City = cand.City, CreatedOn = _today,
                            EmailId = cand.Email, Gender=cand.Gender, 
                            MobileNo = cand.UserPhones.Where(x => x.IsMain && x.IsValid)
                            .Select(x => x.MobileNo).FirstOrDefault(),
                            Status = cand.Status, UserName = cand.UserName
                    }).FirstOrDefaultAsync();
                
                _context.UserHistories.Add(obj);

                await _context.SaveChangesAsync();
            }

            return obj;
            
        }

        public async Task<PagedList<UserHistoryBriefDto>> GetUserHistoryPaginated(UserHistoryParams hParams)
        {
        
            var query = _context.UserHistories.AsQueryable();

            if(hParams.UserHistoryId !=0) {
                query = query.Where(x => x.Id == hParams.UserHistoryId);
            } else if(!string.IsNullOrEmpty(hParams.CategoryRef)) {
                query = query.Where(x => x.CategoryRef.ToLower() == hParams.CategoryRef.ToLower());
            } else if(hParams.ApplicationNo != 0) {
                query = query.Where(x => x.ApplicationNo == hParams.ApplicationNo);
            } else {
                if(!string.IsNullOrEmpty(hParams.UserName)) 
                    query = query.Where(x => x.UserName.ToLower() == hParams.UserName.ToLower());
                if(hParams.Concluded.HasValue) 
                    query = query.Where(x => x.Concluded == hParams.Concluded);
                if(!string.IsNullOrEmpty(hParams.Status)) 
                    query = query.Where(x => x.Status == hParams.Status);
            }    

             var paged = await PagedList<UserHistoryBriefDto>.CreateAsync(query.AsNoTracking()
                    .ProjectTo<UserHistoryBriefDto>(_mapper.ConfigurationProvider),
                    hParams.PageNumber, hParams.PageSize);
               
               return paged;
        }

        public async Task<UserHistoryItem> UpdateHistoryItem(UserHistoryItem userhistoryitem, string UserDisplayName)
        {
            var existing = await _context.UserHistoryItems.FindAsync(userhistoryitem.Id);
            if(existing == null) return null;

            _context.Entry(existing).CurrentValues.SetValues(userhistoryitem);

            return await _context.SaveChangesAsync() > 0 ? existing : null;
        }

    }
}