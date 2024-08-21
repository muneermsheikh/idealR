using System.Data.Common;
using System.Reflection.Metadata.Ecma335;
using api.DTOs.HR;
using api.Entities.Admin;
using api.Entities.HR;
using api.Errors;
using api.Helpers;
using api.Interfaces.HR;
using api.Params.HR;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Drawing.Controls;

namespace api.Data.Repositories.HR
{
    public class InterviewRepository : IInterviewRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public InterviewRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public Task<bool> DeleteInterview(int InterviewId)
        {
            throw new NotImplementedException();
        }

        public async Task<Intervw> EditInterview(Intervw model)
        {
            var existing = _context.Intervws
                .Where(x => x.OrderId == model.OrderId)
                .Include(x => x.InterviewItems)
                .ThenInclude(x => x.InterviewItemCandidates)
                .AsNoTracking()
                .SingleOrDefault();
            
            if (existing == null) return null;

            _context.Entry(existing).CurrentValues.SetValues(model);

            foreach(var existingitem in existing.InterviewItems.ToList()) {
                if(!model.InterviewItems.Any(c => c.Id == existingitem.Id && c.Id != default(int)))
                {
                    _context.IntervwItems.Remove(existingitem);
                    _context.Entry(existingitem).State = EntityState.Deleted;
                }
            }

            foreach(var newitem in model.InterviewItems) {
                var existingitem = existing.InterviewItems.Where(c => c.Id == newitem.Id && c.Id != default(int)).SingleOrDefault();

                if(existingitem !=null) {
                    _context.Entry(existingitem).CurrentValues.SetValues(newitem);
                    _context.Entry(existingitem).State = EntityState.Modified;
                } else {
                    var itemtoinsert = new IntervwItem {
                        IntervwId = existing.Id,
                        InterviewVenue = newitem.InterviewVenue,
                        OrderItemId = newitem.OrderItemId,
                        ProfessionId = newitem.ProfessionId,
                        ProfessionName = newitem.ProfessionName,
                        InterviewMode = newitem.InterviewMode,
                        InterviewerName = newitem.InterviewerName,
                        EstimatedMinsToInterviewEachCandidate = newitem.EstimatedMinsToInterviewEachCandidate 
                    };

                    existing.InterviewItems.Add(itemtoinsert);
                    _context.Entry(itemtoinsert).State = EntityState.Added;
                }

                //candidates in each interviewitme
                foreach(var existingCandidate in existingitem.InterviewItemCandidates.ToList()) {
                    if(!newitem.InterviewItemCandidates.Any(c => c.Id == existingCandidate.Id && c.Id != default(int))) {
                        _context.IntervwItemCandidates.Remove(existingCandidate);
                        _context.Entry(existingCandidate).State = EntityState.Deleted;
                    }
                }

                foreach(var newcandidate in newitem.InterviewItemCandidates) {
                    var existingcand = existingitem.InterviewItemCandidates
                        .Where(c=>c.Id == newcandidate.Id && c.Id != default(int)).SingleOrDefault();
                    
                    if(existingcand != null) {
                        _context.Entry(existingcand).CurrentValues.SetValues(newcandidate);
                        _context.Entry(existingcand).State = EntityState.Modified;
                    } else {
                        var candtoinsert=new IntervwItemCandidate {
                            InterviewItemId = newitem.Id,
                            ScheduledFrom = newcandidate.ScheduledFrom,
                            ReportedAt = newcandidate.ReportedAt,
                            InterviewedAt = newcandidate.InterviewedAt,
                            CandidateId = newcandidate.CandidateId,
                            ApplicationNo = newcandidate.ApplicationNo,
                            CandidateName = newcandidate.CandidateName,
                            InterviewerRemarks = newcandidate.InterviewerRemarks,
                            InterviewStatus = newcandidate.InterviewStatus
                        };
                        existingitem.InterviewItemCandidates.Add(candtoinsert);
                        _context.Entry(candtoinsert).State=EntityState.Added;
                    }

                }
            }

            _context.Entry(existing).State=EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch (DbException ex) {
                throw new Exception(ex.Message,ex);
            } catch (Exception ex) {
                throw new Exception(ex.Message,ex);
            }

            return existing;
        }

        public async Task<InterviewItemWithErrDto> EditInterviewItem(IntervwItem model)
        {
            var dtoErr=new InterviewItemWithErrDto();

            var existing = _context.IntervwItems
                .Where(x => x.OrderItemId == model.OrderItemId)
                .Include(x => x.InterviewItemCandidates)
                .AsNoTracking()
                .SingleOrDefault();
            
            if (existing == null) {
                dtoErr.Error = "Interview Item does not exist";
                return dtoErr;
            }

            //ascertain IntervwId field is not zero
            if(model.IntervwId == 0) {
                var orderid = await _context.OrderItems.Where(x => x.Id==model.OrderItemId).Select(x => x.OrderId).FirstOrDefaultAsync();
                if(orderid != 0) {
                    var intervw = await _context.Intervws.Where(x => x.OrderId == orderid).FirstOrDefaultAsync();
                    if(intervw == null) {
                        dtoErr.Error = "Invalid OrderItemId";
                        return dtoErr;
                    }
                    model.IntervwId = intervw.Id;
                }
            }
            _context.Entry(existing).CurrentValues.SetValues(model);

            foreach(var existingCand in existing.InterviewItemCandidates.ToList()) {
                if(!model.InterviewItemCandidates.Any(c => c.Id == existingCand.Id && c.Id != default(int)))
                {
                    _context.IntervwItemCandidates.Remove(existingCand);
                    _context.Entry(existingCand).State = EntityState.Deleted;
                }
            }

              
            foreach(var newcandidate in model.InterviewItemCandidates) {
                var existingcand = existing.InterviewItemCandidates
                    .Where(c=>c.Id == newcandidate.Id && c.Id != default(int)).SingleOrDefault();
                
                if(existingcand != null) {
                    _context.Entry(existingcand).CurrentValues.SetValues(newcandidate);
                    _context.Entry(existingcand).State = EntityState.Modified;
                } else {
                    
                    var candtoinsert=new IntervwItemCandidate {
                        InterviewItemId = existing.Id,
                        ScheduledFrom = newcandidate.ScheduledFrom,
                        ReportedAt = newcandidate.ReportedAt,
                        InterviewedAt = newcandidate.InterviewedAt,
                        CandidateId = newcandidate.CandidateId,
                        ApplicationNo = newcandidate.ApplicationNo,
                        CandidateName = newcandidate.CandidateName,
                        InterviewerRemarks = newcandidate.InterviewerRemarks,
                        InterviewStatus = newcandidate.InterviewStatus,
                        AttachmentFileNameWithPath = newcandidate.AttachmentFileNameWithPath
                    };
                    existing.InterviewItemCandidates.Add(candtoinsert);
                    _context.Entry(candtoinsert).State=EntityState.Added;
                }

            }
            

            _context.Entry(existing).State=EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
                dtoErr.intervwItem = existing;
            } catch (DbException ex) {
                throw new Exception(ex.Message,ex);
            } catch (Exception ex) {
                throw new Exception(ex.Message,ex);
            }

            return dtoErr;
        }

        public async Task<PagedList<InterviewBriefDto>> GetInterviewPagedList(InterviewParams iParams)
        {
            var query = _context.Intervws.AsQueryable();

            if(iParams.OrderId != 0) query = query.Where(x => x.OrderId == iParams.OrderId);
            if(iParams.OrderNo != 0) query = query.Where(x => x.OrderNo == iParams.OrderNo);
            if(iParams.CustomerId != 0) query = query.Where(x => x.CustomerId == iParams.CustomerId);
            if(!string.IsNullOrEmpty(iParams.InterviewStatus)) query = query.Where(x => x.InterviewStatus.ToLower() == iParams.InterviewStatus.ToLower());

            var temp = await query.ToListAsync();
            var paged = await PagedList<InterviewBriefDto>.CreateAsync(query.AsNoTracking()
                    .ProjectTo<InterviewBriefDto>(_mapper.ConfigurationProvider),
                    iParams.PageNumber, iParams.PageSize);
            
            return paged;
        }

        public async Task<InterviewWithErrDto> GetOrGenerateInterviewR(int OrderNo)
        {
            var dtoErr = new InterviewWithErrDto();

            var dow = DateTime.Now.DayOfWeek.ToString();
            var interwFrom = dow == "Friday" ? 4 : 2;
            var candidates = new List<IntervwItemCandidate>{
                new() {CandidateName="Test Name",
                ScheduledFrom=DateTime.Now.AddDays(interwFrom), 
                InterviewStatus="Not Started"}
            };

            var interview = await _context.Intervws.Where(x => x.OrderNo == OrderNo)
                .Include(x => x.InterviewItems)
                .ThenInclude(x => x.InterviewItemCandidates)
                .FirstOrDefaultAsync();
            
            if (interview != null) {
                
                var itemIdsIncluded=interview.InterviewItems.Select(x => x.OrderItemId).ToList();

                var items = await(from item in _context.OrderItems 
                    where item.OrderId == interview.OrderId 
                        && !itemIdsIncluded.Contains(item.Id)
                join cat in _context.Professions on item.ProfessionId equals cat.Id
                select new IntervwItem {
                    OrderItemId = item.Id, ProfessionId = item.ProfessionId, 
                    IntervwId = interview.Id, ProfessionName = cat.ProfessionName,
                    InterviewMode = "Personal", InterviewVenue="To be announced", 
                    InterviewerName="To be announced",
                    InterviewItemCandidates =candidates
                }).ToListAsync();

                if(items.Count > 0) {
                    foreach(var item in items) {interview.InterviewItems.Add(item);}
                }

                dtoErr.intervw = interview;

                return dtoErr;
            }

            //var itemCandidates = new List<IntervwItemCandidate>();  //{new() {CandidateName="", InterviewStatus="Not Interviewed" }};


            interview = await _context.Orders.Where(x => x.OrderNo == OrderNo)
                .Select(x => new Intervw {
                OrderId = x.Id, OrderNo=OrderNo, OrderDate = x.OrderDate, 
                CustomerId = x.CustomerId, CustomerName = x.Customer.CustomerName,
                InterviewDateFrom = DateTime.Now.AddDays(interwFrom),
                InterviewDateUpto = DateTime.Now.AddDays(interwFrom +1),
                InterviewStatus = "Planned"
            }).FirstOrDefaultAsync();

             if (interview == null) {
                dtoErr.Error = "OrderNo + " + OrderNo + " is Invalid";
                return dtoErr;
            }
            
            //var today = DateTime.Now;
      
            var interviewitems = await(from item in _context.OrderItems where item.OrderId == interview.OrderId
                join cat in _context.Professions on item.ProfessionId equals cat.Id
                select new IntervwItem {
                    OrderItemId = item.Id, ProfessionId = item.ProfessionId, 
                    IntervwId = interview.Id, ProfessionName = cat.ProfessionName,
                    InterviewMode = "Personal", InterviewVenue="To be announced", 
                    InterviewerName="To be announced",
                    InterviewItemCandidates =candidates
            }).ToListAsync();

            interview.InterviewItems = interviewitems;
            dtoErr.intervw = interview;
            return dtoErr;
        }
        

        public async Task<InterviewWithErrDto> SaveNewInterview(Intervw interview)
        {
            var ErrDto = new InterviewWithErrDto();

            var exists = await _context.Intervws.Where(x => x.OrderNo == interview.OrderNo).FirstOrDefaultAsync();

            if(exists != null) return null;

            _context.Intervws.Add(interview);
            _context.Entry(interview).State = EntityState.Added;

            try {
                await _context.SaveChangesAsync();
                ErrDto.intervw = interview;
            } catch (DbException ex) {
                ErrDto.Error = ex.Message;
            } catch (Exception ex) {
                ErrDto.Error = ex.Message;
            }

            return ErrDto;
        }

      
        public async Task<InterviewItemWithErrDto> SaveNewInterviewItem(IntervwItem interview)
        {
            var ErrDto = new InterviewItemWithErrDto();

            var exists = await _context.IntervwItems.Where(x => x.OrderItemId == interview.OrderItemId).FirstOrDefaultAsync();
            
            if(exists != null ) {
                ErrDto.Error="The Interview category already exists.";
                return ErrDto;
            }

            var intervwid = interview.IntervwId;
            if(intervwid == 0) {
                var qry = await (from items in _context.OrderItems where items.Id==interview.OrderItemId
                    join intervw in _context.Intervws on items.OrderId equals intervw.OrderId
                    select new {interviewid=intervw.Id}).FirstOrDefaultAsync();
            }
            if(intervwid==0) {      //parent object id not available, create one
                var orderid = await _context.OrderItems.Where(x => x.Id == interview.OrderItemId).Select(x => x.OrderId).FirstOrDefaultAsync();
                if(orderid==0) {
                    ErrDto.Error = "Invalid order id";
                    return ErrDto;}

                intervwid = await _context.Intervws.Where(x => x.OrderId==orderid).Select(x => x.Id).FirstOrDefaultAsync();
                if(intervwid != 0) interview.IntervwId=intervwid;

                //creaate parent object intervw and attach intervwitem to it.
                
                var intervw = await (from order in _context.Orders where order.Id == orderid 
                    select new Intervw {
                        OrderId = interview.OrderItemId, CustomerId = order.CustomerId, 
                        CustomerName= order.Customer.CustomerName, OrderDate = order.OrderDate,
                        InterviewStatus = "Not Started", OrderNo = order.OrderNo,
                        InterviewItems = new List<IntervwItem>{interview}
                    }).FirstOrDefaultAsync();
                _context.Intervws.Add(intervw);

             } else {
                if(interview.IntervwId==0) interview.IntervwId=intervwid;

                _context.IntervwItems.Add(interview);
                _context.Entry(interview).State = EntityState.Added;
             }

            try {
                await _context.SaveChangesAsync();
                ErrDto.intervwItem = interview;
            } catch (DbException ex) {
                ErrDto.Error = ex.Message;
            } catch (Exception ex) {
                ErrDto.Error = ex.Message;
            }

            return ErrDto;
        }

       
    }
}