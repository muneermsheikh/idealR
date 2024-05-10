using api.DTOs.HR;
using api.Entities.HR;
using api.Entities.Identity;
using api.Entities.Master;
using api.Extensions;
using api.Helpers;
using api.Interfaces;
using api.Params.HR;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Repositories
{
    public class CandidatesRepository : ICandidateRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        public CandidatesRepository(DataContext context, UserManager<AppUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
            _context = context;
        }


        public async Task<bool> DeleteCandidate(int Id)
        {
            var candidate = await _context.Candidates.FindAsync(Id);
            if (candidate == null) return false;
            _context.Entry(candidate).State = EntityState.Deleted;

            try{
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                throw new Exception(ex.Message, ex);
            }

            return true;
        
        }

        public async Task<Candidate> GetCandidate(CandidateParams candidateParams)
        {
            var query = _context.Candidates.Include(x => x.UserProfessions).AsQueryable();
            if(candidateParams.Id > 0) {
                query = query.Where(x => x.Id == candidateParams.Id);}
                else {
                    if(!string.IsNullOrEmpty(candidateParams.CandidateName)) 
                        query = query.Where(x => x.FullName.ToLower().Contains(candidateParams.CandidateName.ToLower()));
                    if(!string.IsNullOrEmpty(candidateParams.PassportNo))
                        query = query.Where(x => x.PpNo == candidateParams.PassportNo);
                    /*if(candidateParams.ProfessionId > 0)
                        query = query.Where(x => x.UserProfessions.Select(x => x.ProfessionId).ToList()
                        ).Include(candidateParams.ProfessionId);
                    */
                }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<PagedList<CandidateBriefDto>> GetCandidates(CandidateParams candidateParams)
        {
            var query = _context.Candidates.AsQueryable();
            if(candidateParams.Id > 0) {
                query = query.Where(x => x.Id == candidateParams.Id);}
                else {
                    if(!string.IsNullOrEmpty(candidateParams.CandidateName)) 
                        query = query.Where(x => x.FullName.ToLower().Contains(candidateParams.CandidateName.ToLower()));
                    
                    if(!string.IsNullOrEmpty(candidateParams.PassportNo))
                        query = query.Where(x => x.PpNo == candidateParams.PassportNo);
                    
                    if(candidateParams.ProfessionId != 0) {
                        var candidateids = await _context.UserProfessions.Where(x => x.ProfessionId == candidateParams.ProfessionId)
                            .Select(x => x.CandidateId).ToListAsync();
                        if(candidateids.Count > 0) {
                            query = query.Where(x => candidateids.Contains(x.Id));
                        }
                    }

                    if(candidateParams.OrderItemId !=0) {
                        var professionid = await _context.OrderItems.Where(x => x.Id == candidateParams.OrderItemId)
                            .Select(x => x.ProfessionId).FirstOrDefaultAsync();
                        
                        var candidateids = await (from orderitem in _context.OrderItems where orderitem.Id==candidateParams.OrderItemId
                            join userprof in _context.UserProfessions on orderitem.Id equals userprof.ProfessionId
                            select userprof.CandidateId).ToListAsync();
                        if(candidateids.Count > 0) query = query.Where(x => candidateids.Contains(x.Id));
                    }

                    if(candidateParams.AgentId > 0) query = query.Where(x => x.CustomerId == candidateParams.AgentId);
                }
            
            var paged = await PagedList<CandidateBriefDto>.CreateAsync(query.AsNoTracking()
                    .ProjectTo<CandidateBriefDto>(_mapper.ConfigurationProvider),
                    candidateParams.PageNumber, candidateParams.PageSize);
            
            return paged;
        }

        public async Task<bool> InsertCandidate(Candidate candidate)
        {
            candidate.AppUserId = await UpdateAppUserIdExtensions.UpdateCandidateAppUserId(candidate, 
                    _userManager, _context);
          
            _context.Candidates.Add(candidate);
            
            return await _context.SaveChangesAsync() > 0;
            
        }

        public async Task<bool> UpdateCandidate(Candidate newObject)
        {
            var existingObject = _context.Candidates
                .Where(x => x.Id == newObject.Id)
                .Include(x => x.UserProfessions)
                .Include(x => x.UserExperiences)
                .Include(x => x.UserPhones)
                .AsNoTracking()
                .SingleOrDefault();
            
            if (existingObject == null) return false;

            _context.Entry(existingObject).CurrentValues.SetValues(newObject);

            //delete records in existingObject that are not present in new object
            foreach (var existingItem in existingObject.UserProfessions.ToList())
            {
                if(!newObject.UserProfessions.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                {
                    _context.UserProfessions.Remove(existingItem);
                    _context.Entry(existingItem).State = EntityState.Deleted; 
                }
            }

            //items in current object - either updated or new items
            foreach(var newItem in newObject.UserProfessions)
            {
                var existingItem = existingObject.UserProfessions
                    .Where(c => c.Id == newItem.Id && c.Id != default(int)).SingleOrDefault();
                if(existingItem != null)    //update navigation record
                {
                    _context.Entry(existingItem).CurrentValues.SetValues(newItem);
                    _context.Entry(existingItem).State = EntityState.Modified;
                } else {    //insert new navigation record
                    var itemToInsert = new UserProfession
                    {
                        CandidateId = existingObject.Id,
                        ProfessionId = newItem.ProfessionId,
                        IsMain = newItem.IsMain,
                        IndustryId = newItem.IndustryId
                    };

                    existingObject.UserProfessions.Add(itemToInsert);
                    _context.Entry(itemToInsert).State = EntityState.Added;
                }
            }

            //edit UserPhones
            foreach (var existingItem in existingObject.UserPhones.ToList())
            {
                if(!newObject.UserPhones.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                {
                    _context.UserPhones.Remove(existingItem);
                    _context.Entry(existingItem).State = EntityState.Deleted; 
                }
            }

            //items in current object - either updated or new items
            foreach(var newItem in newObject.UserPhones)
            {
                var existingItem = existingObject.UserPhones
                    .Where(c => c.Id == newItem.Id && c.Id != default(int)).SingleOrDefault();
                if(existingItem != null)    //update navigation record
                {
                    _context.Entry(existingItem).CurrentValues.SetValues(newItem);
                    _context.Entry(existingItem).State = EntityState.Modified;
                } else {    //insert new navigation record
                    var itemToInsert = new UserPhone
                    {
                        CandidateId = existingObject.Id,
                        MobileNo = newItem.MobileNo,
                        IsMain = newItem.IsMain,
                        IsValid = true,
                        Remarks = newItem.Remarks
                    };

                    existingObject.UserPhones.Add(itemToInsert);
                    _context.Entry(itemToInsert).State = EntityState.Added;
                }
            }

            //edit UserExp
            foreach (var existingItem in existingObject.UserExperiences.ToList())
            {
                if(!newObject.UserExperiences.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                {
                    _context.UserExps.Remove(existingItem);
                    _context.Entry(existingItem).State = EntityState.Deleted; 
                }
            }

            //items in current object - either updated or new items
            foreach(var newItem in newObject.UserExperiences)
            {
                var existingItem = existingObject.UserExperiences
                    .Where(c => c.Id == newItem.Id && c.Id != default(int)).SingleOrDefault();
                if(existingItem != null)    //update navigation record
                {
                    _context.Entry(existingItem).CurrentValues.SetValues(newItem);
                    _context.Entry(existingItem).State = EntityState.Modified;
                } else {    //insert new navigation record
                    var itemToInsert = new UserExp
                    {
                        CandidateId = existingObject.Id,
                        SrNo = newItem.SrNo,
                        Employer = newItem.Employer,
                        Position = newItem.Position,
                        CurrentJob = newItem.CurrentJob,
                        WorkedFrom = newItem.WorkedFrom,
                        WorkedUpto = newItem.WorkedUpto,
                        SalaryCurrency = newItem.SalaryCurrency,
                        MonthlySalaryDrawn = newItem.MonthlySalaryDrawn
                    };

                    existingObject.UserExperiences.Add(itemToInsert);
                    _context.Entry(itemToInsert).State = EntityState.Added;
                }
            }

            _context.Entry(existingObject).State = EntityState.Modified;

            return await _context.SaveChangesAsync() > 0;
        }
    

    }
}