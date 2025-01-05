using System.Data.Common;
using api.DTOs.Admin;
using api.DTOs.HR;
using api.Entities.Admin;
using api.Entities.HR;
using api.Helpers;
using api.Interfaces.Admin;
using api.Interfaces.HR;
using api.Params.Admin;
using api.Params.HR;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Repositories.HR
{
    public class InterviewRepository : IInterviewRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IProspectiveCandidatesRepository _prosRepo;
        private readonly IComposeMsgForIntrviews _composeInvitation;
        public InterviewRepository(DataContext context, IMapper mapper, IProspectiveCandidatesRepository prosRepo, 
            IComposeMsgForIntrviews composeInvitation)
        {
            _composeInvitation = composeInvitation;
            _prosRepo = prosRepo;
            _mapper = mapper;
            _context = context;
        }

        public async Task<bool> DeleteInterview(int InterviewId)
        {
            var interview = await _context.Intervws.Include(x => x.InterviewItems).ThenInclude(x => x.InterviewItemCandidates)
                .Where(x => x.Id==InterviewId).FirstOrDefaultAsync();
            if (interview == null) return false;

            _context.Entry(interview).State = EntityState.Deleted;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Intervw> UpdateInterviewHeader(Intervw model)
        {
            var existing = await _context.Intervws.FindAsync(model.Id);
            
            if(existing != null) {
                _context.Entry(existing).CurrentValues.SetValues(model);
                _context.Entry(existing).State  = EntityState.Modified;
            } else {
                if(model.InterviewVenues=="" || model.CustomerId==0 || string.IsNullOrEmpty(model.CustomerName)
                    || model.InterviewDateFrom.Year < 2000 || model.InterviewDateUpto.Year < 2000
                    || model.OrderId==0 || model.OrderNo == 0 || model.OrderDate.Year < 2000) {
                        return null;
                    }
            
                _context.Entry(model).State = EntityState.Added;
            }

            try {
                await _context.SaveChangesAsync();
                return model;
            } catch (Exception ex) {
                throw new Exception(ex.Message);
            }
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
                        CategoryRef = newitem.CategoryRef ?? existing.OrderNo + "-" ,
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

        public async Task<InterviewItemWithErrDto> EditInterviewItem(IntervwItem model, string Username)
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
                    if(intervw == null) {   // ";}
                        var order = await _context.Orders.Where(x => x.Id == orderid)
                            .Select(x => new {x.OrderDate, x.CustomerId, x.Customer.CustomerName }).FirstOrDefaultAsync();
                        intervw = new Intervw{OrderId = orderid, CustomerId=order.CustomerId, CustomerName=order.CustomerName, OrderDate=order.OrderDate};
                        _context.Entry(intervw).State=EntityState.Added;
                        await _context.SaveChangesAsync();
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

            DateTime maxScheduledAt = new();

            if(existing.InterviewItemCandidates.Count > 0) {
                maxScheduledAt = existing.InterviewItemCandidates.Max(x => x.ScheduledFrom);
            } 

            var prospectiveIds = model.InterviewItemCandidates.Where(m => m.ProspectiveCandidateId > 0)
                .Select(m => m.ProspectiveCandidateId).ToList();
            var candidatesInserted = new List<IntervwItemCandidate>();
            var prospectives = await _context.ProspectiveCandidates.Where(x => prospectiveIds
                .Contains(x.Id)).ToListAsync();

            foreach(var newcandidate in model.InterviewItemCandidates) {
                var existingcand = existing.InterviewItemCandidates
                    .Where(c=>c.Id == newcandidate.Id && c.Id != default(int)).SingleOrDefault();
                
                if(existingcand != null) {
                    _context.Entry(existingcand).CurrentValues.SetValues(newcandidate);
                    _context.Entry(existingcand).State = EntityState.Modified;
                } else {
                    
                    var prospective = new ProspectiveCandidate();
                    if(prospectives != null && prospectives.Count > 0) {
                        prospective=prospectives.FirstOrDefault(x => x.Id == newcandidate.ProspectiveCandidateId);
                    }
                    var candtoinsert=new IntervwItemCandidate {
                        InterviewItemId = existing.Id,
                        ScheduledFrom = newcandidate.ScheduledFrom.Year > 2000 ? newcandidate.ScheduledFrom : maxScheduledAt.AddMinutes(25),
                        ReportedAt = newcandidate.ReportedAt,
                        InterviewedAt = newcandidate.InterviewedAt,
                        CandidateId = newcandidate.CandidateId,
                        PersonId  = newcandidate.PersonId ?? prospective.PersonId,
                        ApplicationNo = newcandidate.ApplicationNo,
                        CandidateName = newcandidate.CandidateName,
                        InterviewerRemarks = newcandidate.InterviewerRemarks,
                        InterviewStatus = newcandidate.InterviewStatus,
                        ProspectiveCandidateId = newcandidate.ProspectiveCandidateId == 0 ? prospective.Id : newcandidate.ProspectiveCandidateId,
                        AttachmentFileNameWithPath = newcandidate.AttachmentFileNameWithPath,
                    };

                    existing.InterviewItemCandidates.Add(candtoinsert);
                    _context.Entry(candtoinsert).State=EntityState.Added;
                    candidatesInserted.Add(candtoinsert);
                }

            }
            

            _context.Entry(existing).State=EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
                var prospReturnDtos = await _prosRepo.ConvertProspectiveToCandidates(prospectiveIds, Username);
                foreach(var dto in prospReturnDtos) {
                    var cand = candidatesInserted.FirstOrDefault(x => x.ProspectiveCandidateId == dto.ProspectiveCandidateId);
                    if(cand != null) {
                        cand.ApplicationNo=dto.ApplicationNo;
                        cand.CandidateId = dto.CandidateId;
                        _context.Entry(cand).State=EntityState.Modified;
                    }
                }
                if(_context.ChangeTracker.HasChanges()) await _context.SaveChangesAsync();
                
                dtoErr.Error="";
                dtoErr.intervwItem = existing;
            } catch (DbException ex) {
                throw new Exception(ex.Message,ex);
            } catch (Exception ex) {
                throw new Exception(ex.Message,ex);
            }

            return dtoErr;
        }

        public async Task<ICollection<InterviewMatchingCategoryDto>> GetInterviewBriefMatching(string categoryName)
        {
            var copyCatName = categoryName;

            var strCat = new List<string>();
            do {
                int pos = categoryName.IndexOf(",");
                if(pos > -1) {
                    strCat.Add(categoryName[..pos]);
                } else {
                    strCat.Add(categoryName);
                    categoryName="";
                    continue;
                }
                
                categoryName = categoryName[(pos + 1)..];
            } while (categoryName.Length > 0);            
            
            do {
                int pos = copyCatName.IndexOf(" ");
                if(pos > -1) {
                    strCat.Add(copyCatName[..pos]);
                    
                } else {
                    strCat.Add(copyCatName);
                    copyCatName="";
                    continue;
                }
                
                copyCatName =copyCatName[(pos + 1)..];
            } while (copyCatName.Length > 0);          

            var dto = new List<InterviewMatchingCategoryDto>();
            foreach(var cat in strCat) {
                var query = await (from item in _context.IntervwItems where item.ProfessionName.Contains(cat)
                    join interview in _context.Intervws on item.IntervwId equals interview.Id
                    select new InterviewMatchingCategoryDto {Id = item.Id, Checked=false, CategoryName=item.ProfessionName,
                        CustomerName = interview.CustomerName, InterviewDateFrom = interview.InterviewDateFrom}
                    ).FirstOrDefaultAsync();
                if(query != null) dto.Add(query);
            }

            return dto;        
        }
        
        public async Task<PagedList<InterviewBriefDto>> GetInterviewPagedList(InterviewParams iParams)
        {
            var query = _context.Intervws.OrderByDescending(x => x.InterviewDateFrom).AsQueryable();

            if(iParams.OrderId != 0) query = query.Where(x => x.OrderId == iParams.OrderId);
            if(iParams.OrderNo != 0) query = query.Where(x => x.OrderNo == iParams.OrderNo);
            if(iParams.CustomerId != 0) query = query.Where(x => x.CustomerId == iParams.CustomerId);
            if(!string.IsNullOrEmpty(iParams.InterviewStatus)) query = query.Where(x => x.InterviewStatus.ToLower() == iParams.InterviewStatus.ToLower());

            var paged = await PagedList<InterviewBriefDto>.CreateAsync(query.AsNoTracking()
                    .ProjectTo<InterviewBriefDto>(_mapper.ConfigurationProvider),
                    iParams.PageNumber, iParams.PageSize);
            
            return paged;
        }

        public async Task<InterviewWithErrDto> GetOrGenerateInterviewR(int OrderNo)
        {
            var dtoErr = new InterviewWithErrDto();

            var interview = await _context.Intervws.Include(x => x.InterviewItems).ThenInclude(x => x.InterviewItemCandidates)
                .Where(x => x.OrderNo == OrderNo).FirstOrDefaultAsync();
            //insert any intrview items not present above.
            if(interview != null) {
                var itemIdsIncluded=interview.InterviewItems.Select(x => new {x.OrderItemId, x.ProfessionId, x.ProfessionName})
                    .ToList();
                var existingOrderItemIds = itemIdsIncluded.Select(x => x.OrderItemId).ToList();
                var orderitemsMissing = await _context.OrderItems
                    .Where(x => x.OrderId==interview.OrderId && !existingOrderItemIds.Contains(x.Id))
                    .Select(x => new {x.Id, x.ProfessionId, x.Profession.ProfessionName, 
                    CategoryRef = OrderNo + "-" + x.SrNo}).ToListAsync();
                
                //insert above missing orderitems
                if(orderitemsMissing.Count > 0) {
                    foreach(var item in orderitemsMissing) {
                        var itemToInsert = new IntervwItem{EstimatedMinsToInterviewEachCandidate=25, 
                            InterviewerName="To Be Announced", InterviewItemCandidates=new List<IntervwItemCandidate>(), 
                            InterviewMode="Personal", InterviewVenue="To Be Announced", IntervwId=interview.Id, 
                            OrderItemId=item.Id, ProfessionId=item.ProfessionId, ProfessionName=item.ProfessionName,
                            CategoryRef = item.CategoryRef};
                        _context.Entry(itemToInsert).State = EntityState.Added;
                        interview.InterviewItems.Add(itemToInsert);
                    }
                    await _context.SaveChangesAsync();
                }
                dtoErr.intervw=interview;
                return dtoErr;
            }

            //generate new interview object
            var dow = DateTime.Now.DayOfWeek.ToString();
            var interwFrom = dow == "Friday" ? 4 : 2;
            var candidates = new List<IntervwItemCandidate>();
            
            interview = await (from order in _context.Orders where order.OrderNo==OrderNo
                select new Intervw{OrderDate=order.OrderDate, OrderId=order.Id,
                    OrderNo=order.OrderNo, CustomerId=order.CustomerId, CustomerName=order.Customer.CustomerName,
                    InterviewDateFrom=DateTime.UtcNow, InterviewDateUpto=DateTime.UtcNow, InterviewStatus="Not Started",
                    InterviewVenues="To Be Announced"})
            .FirstOrDefaultAsync();
            
            var items = await(from item in _context.OrderItems 
                where item.OrderId == interview.OrderId 
            join cat in _context.Professions on item.ProfessionId equals cat.Id
            select new IntervwItem {                
                OrderItemId = item.Id, ProfessionId = item.ProfessionId, 
                IntervwId = interview.Id, ProfessionName = cat.ProfessionName,
                InterviewMode = "Personal", InterviewVenue="To be announced", 
                InterviewerName="To be announced", CategoryRef = OrderNo + "-" + item.SrNo, 
                InterviewItemCandidates =candidates
            }).ToListAsync();

            if(items.Count > 0) {
                var interviewitems = new List<IntervwItem>();
                foreach(var item in items) {interviewitems.Add(item);}

                interview.InterviewItems=interviewitems;
            }

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

        public async Task<InterviewItemWithErrDto> SaveNewInterviewItem(IntervwItem interviewItem, string loggedInUsername)
        {
            var ErrDto = new InterviewItemWithErrDto();

            var exists = await _context.IntervwItems.Where(x => x.OrderItemId == interviewItem.OrderItemId).FirstOrDefaultAsync();
            
            if(exists != null ) {
                _context.Entry(exists).State = EntityState.Modified;
                if(await _context.SaveChangesAsync() >  0) {
                    ErrDto.intervwItem=exists;
                } else {
                    ErrDto.Error = "Failed to update the inteview item";
                }
                return ErrDto;
            }

            //if prospective candidate, convert to candidates
            foreach(var cand in interviewItem.InterviewItemCandidates) {
                if(cand.CandidateId == 0) {
                    await _prosRepo.ConvertProspectiveToCandidate(cand.ProspectiveCandidateId, loggedInUsername);
                }
            }
            
            //if Intervw.Id is not found, create new Intervw
            var interviewid = interviewItem.IntervwId;
            if(interviewid == 0) {      //check if the Intervw record exists
                var qry = await (from item in _context.OrderItems where item.Id==interviewItem.OrderItemId
                    join intervw in _context.Intervws on item.OrderId equals intervw.OrderId
                    select new {interviewid=intervw.Id}).FirstOrDefaultAsync();
                interviewid = qry == null ? 0 : qry.interviewid;
            } 
            if(interviewid==0) {      //parent object not available, create one
                var orderid = await _context.OrderItems.Where(x => x.Id == interviewItem.OrderItemId)
                    .Select(x => x.OrderId).FirstOrDefaultAsync();
                if(orderid==0) {
                    ErrDto.Error = "Invalid order id";
                    return ErrDto;}
                
                //create parent object and attach interviewItem to it.
                var intervw = await (from order in _context.Orders where order.Id == orderid 
                    select new Intervw {
                        OrderId = orderid, CustomerId = order.CustomerId, 
                        CustomerName= order.Customer.CustomerName, OrderDate = order.OrderDate,
                        InterviewStatus = "Not Started", OrderNo = order.OrderNo,
                        InterviewItems = new List<IntervwItem>{interviewItem}
                    }).FirstOrDefaultAsync();
                _context.Intervws.Add(intervw);

             } else {
                interviewItem.IntervwId=interviewid;
                _context.Entry(interviewItem).State = EntityState.Added;
             }

            try {
                await _context.SaveChangesAsync();
                ErrDto.intervwItem = interviewItem;
            } catch (DbException ex) {
                ErrDto.Error = ex.Message;
            } catch (Exception ex) {
                ErrDto.Error = ex.Message;
            }

            return ErrDto;
        }

        public async Task<PagedList<InterviewAttendanceDto>> GetInterviewAttendancePagedList(AttendanceParams aParams)
        {
            var query = (from interview in _context.Intervws where interview.OrderId == aParams.OrderId
                    join interviewItem in _context.IntervwItems on interview.Id equals interviewItem.IntervwId
                    join cand in _context.IntervwItemCandidates on interviewItem.Id equals cand.InterviewItemId
                    orderby interviewItem.OrderItemId, cand.ApplicationNo
                    select new InterviewAttendanceDto {
                        OrderNo = interview.OrderNo, CustomerName = interview.CustomerName,
                        ApplicationNo = cand.ApplicationNo, CandidateName = cand.CandidateName, 
                        InterviewId=interviewItem.IntervwId, InterviewMode = interviewItem.InterviewMode, 
                        OrderItemId = interviewItem.OrderItemId, InterviewVenue = interviewItem.InterviewVenue,
                        PersonId = cand.PersonId, ProfessionName = interviewItem.ProfessionName, 
                        ScheduledFrom = cand.ScheduledFrom, ReportedAt = Convert.ToDateTime(cand.ReportedAt),
                        InterviewedAt = Convert.ToDateTime(cand.InterviewedAt),
                        InterviewStatus = cand.InterviewStatus, 
                        InterviewerRemarks = cand.InterviewerRemarks, InterviewItemCandidateId = cand.Id,
                        AttachmentFileNameWithPath = cand.AttachmentFileNameWithPath
                    }).AsQueryable();
            
            var paged = await PagedList<InterviewAttendanceDto>.CreateAsync(query.AsNoTracking()
                    .ProjectTo<InterviewAttendanceDto>(_mapper.ConfigurationProvider),
                    aParams.PageNumber, aParams.PageSize);
            
            return paged;
        }

        public async Task<ICollection<InterviewAttendanceToUpdateDto>> UpdateInterviewAttendance(ICollection<InterviewAttendanceToUpdateDto> attendanceDtos) {
            
            var attendances = new List<InterviewAttendanceToUpdateDto>();
            foreach(var dto in attendanceDtos) {
                var attendance = await _context.IntervwItemCandidates.Where(x => x.Id == dto.InterviewCandidateId).FirstOrDefaultAsync();
                if(attendance != null) {
                    attendance.InterviewerRemarks = dto.InterviewerRemarks;
                    attendance.InterviewedAt = !dto.InterviewedAt.HasValue || dto.InterviewedAt.Value.Year < 2000 
                        ? DateTime.UtcNow : dto.InterviewedAt;
                    attendance.ReportedAt = !dto.ReportedAt.HasValue || dto.ReportedAt.Value.Year < 2000 
                        ? DateTime.UtcNow : dto.ReportedAt;
                    attendance.InterviewStatus = dto.InterviewStatus;

                    _context.Entry(attendance).State = EntityState.Modified;

                    dto.InterviewedAt = attendance.InterviewedAt;
                    dto.ReportedAt = attendance.ReportedAt;

                    //insert into intervwattendances
                    await UpdateAttendanceStatusWOSave(dto.InterviewCandidateId, dto.InterviewStatus, dto.InterviewStatusDate);
                    if(await _context.SaveChangesAsync() > 0) attendances.Add(dto);
                } else {
                    return null;
                }
            }

            
            return attendances;
            
        }

        public async Task<ICollection<IntervwAttendance>> GetAttendanceOfACandidate(int interviewCandidateId) {
            
            var attendances = await _context.IntervwAttendances
                .Where(x => x.IntervwItemCandidateId == interviewCandidateId)
                .OrderBy(x => x.AttendanceStatusId)
                .ToListAsync();
            return attendances;
        }

        private async Task<bool> UpdateAttendanceStatusWOSave(int interviewCandidateId, string status, DateTime statusDate) {
            
            var exists = await _context.IntervwAttendances.Where(x => 
                x.IntervwItemCandidateId== interviewCandidateId && x.Status==status)
            .FirstOrDefaultAsync();
            
            if(exists==null) {
                var insertAttend = new IntervwAttendance {
                    IntervwItemCandidateId=interviewCandidateId, Status = status,
                    StatusDate = statusDate};
                _context.Entry(insertAttend).State = EntityState.Added;
            } else {
                exists.StatusDate = statusDate;
                _context.Entry(exists).State = EntityState.Modified;
            }
            return true;
        }

        public async Task<ICollection<AttendanceStatus>> GetAttendanceStatusData()
        {
            return await _context.AttendanceStatuses.OrderBy(x => x.StatusId).ToListAsync();
        }

        public async Task<MessagesWithErrDto> ComposeInterviewInvitationMessages(ICollection<int> InterviewCandidateIds, string Username)
        {
            var returnDto = new MessagesWithErrDto();

            returnDto = await _composeInvitation.InviteCandidatesForInterviews(InterviewCandidateIds, Username);
            if(returnDto.Messages.Count > 0) {
                foreach(var msg in returnDto.Messages) {_context.Entry(msg).State = EntityState.Added;}
                await _context.SaveChangesAsync();
            }

            return returnDto;
        }

        public async Task<bool> DeleteInterviewItemCandidate(int InterviewCandidateId)
        {
            var obj = await _context.IntervwItemCandidates.FindAsync(InterviewCandidateId);
            if(obj == null) return false;

            _context.Entry(obj).State=EntityState.Deleted;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<string> UpdateInterviewCandidateAttachmentFileName(IntervwItemCandidate interviewCandidate)
        {
            var cand = await _context.IntervwItemCandidates.Where(x =>
                x.Id == interviewCandidate.Id).FirstOrDefaultAsync();
            
            if(cand == null) return "Failed to retrieve the Interview Candidate Entity from InterviewCandidateId";

            cand.InterviewedAt = interviewCandidate.InterviewedAt;
            cand.ReportedAt = interviewCandidate.ReportedAt;
            cand.InterviewStatus  = interviewCandidate.InterviewStatus;
            cand.InterviewerRemarks = interviewCandidate.InterviewerRemarks;
            cand.AttachmentFileNameWithPath = interviewCandidate.AttachmentFileNameWithPath;
                
            _context.Entry(cand).State = EntityState.Modified;
            
            return await _context.SaveChangesAsync() > 0 ? "" : "Failed to update the filename attachment";
        }

        public async Task<bool> SaveNewInterviewItemCandidate(IntervwItemCandidate itemCandidate)
        {
            _context.IntervwItemCandidates.Add(itemCandidate);

            return await _context.SaveChangesAsync() > 0;
        }
    }
}