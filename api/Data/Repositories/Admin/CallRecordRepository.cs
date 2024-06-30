using System.Data.Common;
using api.DTOs.Admin;
using api.DTOs.HR;
using api.Entities.Admin;
using api.Helpers;
using api.Interfaces.Admin;
using api.Params.Admin;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Repositories.Admin
{
    public class CallRecordRepository : ICallRecordRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly DateTime _today = DateTime.UtcNow;
        public CallRecordRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }


        public async Task<CallRecord> AddNewCallRecord(CallRecord CallRecord)
        {
            _context.Entry(CallRecord).State = EntityState.Added;

            try {
                await _context.SaveChangesAsync();
            } catch {
                return null;
            }

            return CallRecord;
        }

        public async Task<CallRecordItem> AddNewHistoryItem(CallRecordItem item, string Username)
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

        public async Task<bool> DeleteCallRecord(int historyId)
        {
            var existing = await _context.CallRecords.FindAsync(historyId);

            if (existing == null) return false;

            _context.CallRecords.Remove(existing);

            return await _context.SaveChangesAsync() > 0;
        }


        public async Task<bool> DeleteCallRecordItem(int historyitemid)
        {
             var existing = await _context.CallRecordItems.FindAsync(historyitemid);
            if (existing == null) return false;

            _context.CallRecordItems.Remove(existing);
            _context.Entry(existing).State = EntityState.Deleted;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<CallRecordReturnDto> EditCallRecordWithSingleItem(CallRecord model, string Username)
        {
            var dtoToRetun = new CallRecordReturnDto();
            if(model.CallRecordItems.Count > 1) {
                dtoToRetun.ErrorString="invalid model - the model is expected to be having just one record, the one to add";
                return dtoToRetun;
             }
      
            var existing = await _context.CallRecords.Include(x => x.CallRecordItems)
                .Where(x => x.PersonId == model.PersonId).AsNoTracking().FirstOrDefaultAsync();
            
            if (existing == null) { //the whole object is to be inserted
                model.CallRecordItems.FirstOrDefault().Username=Username;
                _context.CallRecords.Add(model);
                try {
                    var ct = await _context.SaveChangesAsync();
                    dtoToRetun.CallRecord=model;
                } catch (DbException ex) {
                    dtoToRetun.ErrorString = ex.Message;
                } catch (Exception ex) {
                    dtoToRetun.ErrorString = ex.Message;
                }
                return dtoToRetun;
            }
            
            var itemToInsert=model.CallRecordItems.FirstOrDefault();
           
            itemToInsert.CallRecordId = existing.Id;
            itemToInsert.Username = Username;
            
            //_context.Entry(itemToInsert).State=EntityState.Added;
            //existing.CallRecordItems.Add(itemToInsert);
            _context.CallRecordItems.Add(itemToInsert);
            //_context.Entry(existing).State = EntityState.Modified;
         
            try {
                await _context.SaveChangesAsync();
                dtoToRetun.CallRecord = existing;
            } catch (DbException ex) {
                dtoToRetun.ErrorString = ex.Message;
                return dtoToRetun;
            } catch (Exception ex) {
                dtoToRetun.ErrorString = ex.Message;
            }

            if(string.IsNullOrEmpty(dtoToRetun.ErrorString)) {
             
                existing.Status=itemToInsert.ContactResult;
                existing.StatusDate=DateTime.Now;
                _context.Entry(existing).CurrentValues.SetValues(model);
                
                try {
                    await _context.SaveChangesAsync();
                    dtoToRetun.CallRecord = existing;
                } catch (DbException ex) {
                    dtoToRetun.ErrorString = ex.Message;
                    return dtoToRetun;
                } catch (Exception ex) {
                    dtoToRetun.ErrorString = ex.Message;
                }
            }

            return dtoToRetun;
        }

        public async Task<CallRecordReturnDto> EditCallRecord(CallRecord model, string Username)
        {
            var dtoToRetun = new CallRecordReturnDto();
            
            var existing = await _context.CallRecords.Include(x => x.CallRecordItems)
                .Where(x => x.PersonId == model.PersonId).AsNoTracking().FirstOrDefaultAsync();
            
            if (existing == null) { //the whole object is to be inserted
                model.CallRecordItems.FirstOrDefault().Username=Username;
                model.Status=model.CallRecordItems.FirstOrDefault().ContactResult;
                model.StatusDate = model.CallRecordItems.FirstOrDefault().DateOfContact;

                _context.CallRecords.Add(model);

            } else {
                _context.Entry(existing).CurrentValues.SetValues(model);

                foreach (var existingItem in existing.CallRecordItems.ToList())
                {
                    if(!model.CallRecordItems.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                    {
                        _context.CallRecordItems.Remove(existingItem);
                        _context.Entry(existingItem).State = EntityState.Deleted; 
                    }
                }
            
                foreach(var newItem in model.CallRecordItems)
                {
                    var existingItem = existing.CallRecordItems
                        .Where(c => c.Id == newItem.Id && c.Id != default(int)).SingleOrDefault();
                    if(existingItem != null)    //update navigation record
                    {
                        _context.Entry(existingItem).CurrentValues.SetValues(newItem);
                        _context.Entry(existingItem).State = EntityState.Modified;
                    } else {    //insert new navigation record
                            
                        var itemToInsert = new CallRecordItem
                        {
                            CallRecordId = model.Id,
                            DateOfContact = newItem.DateOfContact,
                            PhoneNo = newItem.PhoneNo,
                            IncomingOutgoing = newItem.IncomingOutgoing,
                            GistOfDiscussions = newItem.GistOfDiscussions,
                            ContactResult = newItem.ContactResult,
                            NextAction = newItem.NextAction,
                            NextActionOn = new DateTime(),                        
                            Username = Username
                        };

                        existing.CallRecordItems.Add(itemToInsert);
                        _context.Entry(itemToInsert).State = EntityState.Added;
                    }
                }
            }

            try {
                await _context.SaveChangesAsync();
                dtoToRetun.CallRecord = existing;
            } catch (DbException ex) {
                dtoToRetun.ErrorString = ex.Message;
                return dtoToRetun;
            } catch (Exception ex) {
                dtoToRetun.ErrorString = ex.Message;
            }

            if(!string.IsNullOrEmpty(dtoToRetun.ErrorString)) return dtoToRetun;

            //modify callREcords.Status and Date
            var item = await _context.CallRecordItems.OrderByDescending(x => x.DateOfContact).Take(1).FirstOrDefaultAsync();
            string personid;
            if(existing == null) {
                model.Status=item.ContactResult;
                model.StatusDate = item.DateOfContact;
                _context.Entry(model).State = EntityState.Modified;
                personid=model.PersonId;
            } else {
                existing.Status=item.ContactResult;
                existing.StatusDate = item.DateOfContact;
                _context.Entry(existing).State = EntityState.Modified;
                personid = existing.PersonId;
            }

            var prospective = await _context.ProspectiveCandidates.Where(x => x.PersonId==personid).FirstOrDefaultAsync();
            if (prospective!=null) {
                prospective.Status = item.ContactResult;
                prospective.StatusDate = item.DateOfContact;
                _context.Entry(prospective).State = EntityState.Modified;
            }
            
            try {
                await _context.SaveChangesAsync();
                dtoToRetun.CallRecord = existing;
            } catch (DbException ex) {
                dtoToRetun.ErrorString = ex.Message;
                return dtoToRetun;
            } catch (Exception ex) {
                dtoToRetun.ErrorString = ex.Message;
            }

            return dtoToRetun;
        }

        public async Task<ICollection<CallRecordItem>> EditHistoryItems(ICollection<CallRecordItem> items, string Username)
        {
            var itemList = new List<CallRecordItem>();

            foreach(var item in items) {
                if(item.CallRecordId == 0) continue;
                var existing = await _context.CallRecordItems.FindAsync(item.Id);
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


        public async Task<PagedList<CallRecordBriefDto>> GetCallRecordPaginated(CallRecordParams hParams, string Username)
        {
            var query = _context.CallRecords.AsQueryable();
        
            query = query.Where(x => x.PersonId == hParams.PersonId);
            if (!string.IsNullOrEmpty(hParams.Username)) query = query.Where(x => x.Username == Username);
            if(!string.IsNullOrEmpty(hParams.Subject)) query = query.Where(x => x.Subject == hParams.Subject);
            if(!string.IsNullOrEmpty(hParams.Status)) query = query.Where(x => x.Status == hParams.Status);
            if(!string.IsNullOrEmpty(hParams.CategoryRef)) query = query.Where(x => x.CategoryRef == hParams.CategoryRef);
            if(!string.IsNullOrEmpty(hParams.MobileNo)) query = query.Where(x => x.PhoneNo == hParams.MobileNo);
            if(!string.IsNullOrEmpty(hParams.Email)) query = query.Where(x => x.Email == hParams.Email);

            var paged = await PagedList<CallRecordBriefDto>.CreateAsync(query.AsNoTracking()
                .ProjectTo<CallRecordBriefDto>(_mapper.ConfigurationProvider)
                , hParams.PageNumber, hParams.PageSize);

            return paged;

        }

        //creates a blank RecordItem and appends to existing/newly created CallRecord.  Does not save it.
        public async Task<CallRecord> GetOrAddCallRecordWithItemsByParams(CallRecordItemToCreateDto dto, string username)
        {
            var obj = new CallRecord(); 
            if(dto.CallRecordId > 0) obj = await _context.CallRecords.Include(x => x.CallRecordItems)
                .Where(x => x.PersonId == dto.PersonId).FirstOrDefaultAsync();
            
            if(obj != null && obj.Id > 0) {
                var callRecordItem = new CallRecordItem{
                    Id=0, CallRecordId=obj.Id, AdvisoryBy="", ContactResult=dto.Status, DateOfContact= DateTime.Now, 
                    GistOfDiscussions="", NextAction="", NextActionOn=DateTime.Now, IncomingOutgoing="out", PhoneNo=dto.PhoneNo, 
                    Username = username
                };
                obj.CallRecordItems.Add(callRecordItem);
            } else {

                obj = await GenerateCallRecord(dto.CallRecordId, dto.PersonType, dto.PersonId, dto.CategoryRef, username);

                /* var callRecordItem = new CallRecordItem{
                    Id=0, CallRecordId=obj.Id, AdvisoryBy="", ContactResult=dto.Status, DateOfContact= DateTime.Now, GistOfDiscussions="", 
                    NextAction="", NextActionOn=DateTime.Now, IncomingOutgoing="out", PhoneNo="", Username = username
                };
                var recordItems = new List<CallRecordItem>();
                
                switch (dto.PersonType.ToLower()) {
                    case "candidate" : 
                        obj = await (from cand in _context.Candidates where cand.Id == Convert.ToInt32(dto.PersonId)
                            select new CallRecord {
                                Id=0, CategoryRef=dto.CategoryRef, PersonType = "Candidate",
                                PersonId = dto.PersonId, PersonName = cand.FullName,
                                Subject = dto.Subject, PhoneNo = cand.UserPhones.Where(x => x.IsMain && x.IsValid)
                                    .Select(x => x.MobileNo).FirstOrDefault(),
                                Email = cand.Email, Status = dto.Status, StatusDate = _today, CreatedOn = _today,
                                Username = cand.UserName, ConcludedOn = null
                        }).FirstOrDefaultAsync();
                        
                        callRecordItem.PhoneNo=obj.PhoneNo;
                        callRecordItem.Email=obj.Email;
                        recordItems.Add(callRecordItem);
                        obj.CallRecordItems=recordItems;

                        break;
                    
                    case "official": 
                        obj = await _context.CustomerOfficials.Where(x => x.Id == Convert.ToInt32(dto.PersonId))
                            .Select(x => new CallRecord {
                                    PersonId = dto.PersonId, CreatedOn = _today, PersonType = "Official", Email = x.Email, 
                                    PhoneNo = x.PhoneNo, Status = dto.Status, Username = x.UserName, Subject = dto.Subject, 
                                    CategoryRef=dto.CategoryRef, StatusDate = _today, PersonName = x.OfficialName
                            })
                            .FirstOrDefaultAsync();

                        callRecordItem.PhoneNo=obj.PhoneNo;
                        callRecordItem.Email=obj.Email;
                        recordItems.Add(callRecordItem);
                        obj.CallRecordItems=recordItems;
                        
                        break;
                    case "prospective":
                        obj = await _context.ProspectiveCandidates.Where(x => x.PersonId == dto.PersonId)
                            .Select(x => new CallRecord {
                                PersonId = dto.PersonId, CreatedOn = _today, PersonType = "Prospective", Email = x.Email,
                                PhoneNo = x.PhoneNo, Status = dto.Status, Username = "", Subject = dto.Subject ?? "", 
                                CategoryRef=dto.CategoryRef, StatusDate = _today, PersonName = x.CandidateName
                            }).FirstOrDefaultAsync();
                            
                        callRecordItem.PhoneNo=obj.PhoneNo;
                        callRecordItem.Email=obj.Email;
                        recordItems.Add(callRecordItem);
                        obj.CallRecordItems=recordItems;
                        
                        break;
                    case "employee":
                    default:
                        break;
                }                
                */
            }
            
            return obj;
            
        }

        private async Task<CallRecord> GenerateCallRecord(int callRecordId, string personType, string personId, 
            string categoryRef, string username) {

            var obj = new CallRecord();

            var callRecordItem = new CallRecordItem{
                    Id=0, CallRecordId=callRecordId, AdvisoryBy="", ContactResult="Not Started", 
                    DateOfContact= DateTime.Now, GistOfDiscussions="", NextAction="", 
                    NextActionOn=DateTime.Now, IncomingOutgoing="out", PhoneNo="", Username = username
                };
            var recordItems = new List<CallRecordItem>();

            switch (personType.ToLower()) {
                case "candidate" : 
                    obj = await (from cand in _context.Candidates where cand.Id == Convert.ToInt32(personId)
                        select new CallRecord {
                            Id=0, CategoryRef=categoryRef, PersonType = "Candidate",
                            PersonId = personId, PersonName = cand.FullName,
                            Subject = "followup acceptance", PhoneNo = cand.UserPhones.Where(x => x.IsMain && x.IsValid)
                                .Select(x => x.MobileNo).FirstOrDefault(),
                            Email = cand.Email, Status = "Not Started", StatusDate = _today, CreatedOn = _today,
                            Username = cand.UserName, ConcludedOn = null
                    }).FirstOrDefaultAsync();
                    
                    callRecordItem.PhoneNo=obj.PhoneNo;
                    callRecordItem.Email=obj.Email;
                    recordItems.Add(callRecordItem);
                    obj.CallRecordItems=recordItems;

                    break;
                    
                case "official": 
                    obj = await _context.CustomerOfficials.Where(x => x.Id == Convert.ToInt32(personId))
                        .Select(x => new CallRecord {
                                PersonId = personId, CreatedOn = _today, PersonType = "Official", Email = x.Email, 
                                PhoneNo = x.PhoneNo, Status = "Not Startedd", Username = x.UserName, 
                                Subject = "followup with Client", CategoryRef=categoryRef, 
                                StatusDate = _today, PersonName = x.OfficialName
                        })
                        .FirstOrDefaultAsync();

                    callRecordItem.PhoneNo=obj.PhoneNo;
                    callRecordItem.Email=obj.Email;
                    recordItems.Add(callRecordItem);
                    obj.CallRecordItems=recordItems;
                    
                    break;
                case "prospective":
                    obj = await _context.ProspectiveCandidates.Where(x => x.PersonId == personId)
                        .Select(x => new CallRecord {
                            PersonId = personId, CreatedOn = _today, PersonType = "Prospective", Email = x.Email,
                            PhoneNo = x.PhoneNo, Status = "Not Started", Username = "", Subject = "acceptance followup", 
                            CategoryRef=categoryRef, StatusDate = _today, PersonName = x.CandidateName
                        }).FirstOrDefaultAsync();
                        
                    callRecordItem.PhoneNo=obj.PhoneNo;
                    callRecordItem.Email=obj.Email;
                    recordItems.Add(callRecordItem);
                    obj.CallRecordItems=recordItems;
                    
                    break;
                case "employee":
                default:

                    break;
            }

            return obj;
            
        }
        public async Task<CallRecordDto> GetCallRecordDtoByParams(CallRecordParams hParams) {

            var query = _context.CallRecords.Include(x => x.CallRecordItems.OrderByDescending(x => x.DateOfContact)) .AsQueryable();
        
            query = query.Where(x => x.PersonId == hParams.PersonId);
            if(!string.IsNullOrEmpty(hParams.Subject)) query = query.Where(x => x.Subject == hParams.Subject);
            if(!string.IsNullOrEmpty(hParams.Status)) query = query.Where(x => x.Status == hParams.Status);
            if(!string.IsNullOrEmpty(hParams.CategoryRef)) query = query.Where(x => x.CategoryRef == hParams.CategoryRef);
            if(!string.IsNullOrEmpty(hParams.MobileNo)) query = query.Where(x => x.PhoneNo == hParams.MobileNo);
            if(!string.IsNullOrEmpty(hParams.Email)) query = query.Where(x => x.Email == hParams.Email);

            var obj = await query.Select(x => new CallRecordDto{PersonId=x.PersonId, CategoryRef=x.CategoryRef, 
                CreatedOn=x.CreatedOn, Email=x.Email, MobileNo = x.PhoneNo, Id=x.Id, 
                Status=x.Status, Username=x.Username, ConcludedOn=Convert.ToDateTime(x.ConcludedOn)}).FirstOrDefaultAsync();

            return obj;
        }
       
        public async Task<CallRecordItem> UpdateHistoryItem(CallRecordItem CallRecorditem, string UserDisplayName)
        {
            var existing = await _context.CallRecordItems.FindAsync(CallRecorditem.Id);
            if(existing == null) return null;

            _context.Entry(existing).CurrentValues.SetValues(CallRecorditem);

            return await _context.SaveChangesAsync() > 0 ? existing : null;
        }

        public async Task<CallRecord> GetCallRecordWithItems(int callRecordId, string personType, string personId,
            string categoryRef, string Username)
        {
            var query = await _context.CallRecords.Include(x => x.CallRecordItems.OrderByDescending(x => x.DateOfContact))
                .Where(x => x.PersonId == personId).FirstOrDefaultAsync();

            query ??= await GenerateCallRecord(callRecordId, personType, personId, categoryRef, Username);
            
            return query;
        }
    }
}