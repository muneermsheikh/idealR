using api.DTOs.HR;
using api.Entities.Admin.Order;
using api.Entities.HR;
using api.Entities.Master;
using api.Helpers;
using api.Interfaces.HR;
using api.Params.HR;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Repositories.HR
{
    public class AssessmentQBankRepository : IAssessmentQBankRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public AssessmentQBankRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<ICollection<OrderItemAssessmentQ>> GetAssessmentQBankByOrderItemId(int orderitemid)
        {
            var query = await (from assessment in _context.orderItemAssessments 
                    where assessment.OrderItemId == orderitemid 
                join qBank in _context.OrderItemAssessmentQs 
                    on assessment.Id equals qBank.OrderItemAssessmentId
                select new OrderItemAssessmentQ {
                    Id = assessment.Id,  MaxPoints = qBank.MaxPoints, IsMandatory = qBank.IsMandatory,
                    OrderItemAssessmentId = qBank.OrderItemAssessmentId, Question = qBank.Question,
                    QuestionNo = qBank.QuestionNo, Subject = qBank.Subject
                }).ToListAsync();

                return query;   
        }

        public async Task<PagedList<AssessmentQBankDto>> GetAssessmentQBanks(AssessmentQBankParams qParams)
        {
            var q =  _context.AssessmentQBanks.Include(x => x.AssessmentStddQs)
                .AsQueryable();
            
            if(!string.IsNullOrEmpty(qParams.ProfessionName)) 
                q = q.Where(x => x.ProfessionName.ToLower() == qParams.ProfessionName.ToLower());
            
            var paged = await PagedList<AssessmentQBankDto>.CreateAsync(q.AsNoTracking()
                    .ProjectTo<AssessmentQBankDto>(_mapper.ConfigurationProvider),
                    qParams.PageNumber, qParams.PageSize);
            
            return paged;
            
        }

        public async Task<ICollection<AssessmentQBankDto>> GetAssessmentStddQList(AssessmentQBankParams qParams) {
            
            
            var query = (from prof in _context.AssessmentQBanks orderby prof.ProfessionName
                join qs in _context.AssessmentStddQs on prof.Id equals qs.AssessmentQBankId
                select new AssessmentQBankDto {
                    Id = prof.Id, ProfessionId = prof.ProfessionId, 
                    ProfessionName = prof.ProfessionName, QNo = qs.QNo,
                    Question = qs.Question, AssessmentParameter = qs.AssessmentParameter,
                    IsMandatory = qs.IsMandatory, IsStandardQ = qs.IsStandardQ,
                    MaxPoints = qs.MaxPoints
                }).AsQueryable();
            
            
            if(!string.IsNullOrEmpty(qParams.ProfessionName)) 
                query = query.Where(x => x.ProfessionName.ToLower() == qParams.ProfessionName.ToLower());
            
            return await query.ToListAsync();
        }
        public async Task<AssessmentQBank> GetAssessmentQsOfACategoryByName(string categoryName)
        {
            var q =  await _context.AssessmentQBanks.Include(x => x.AssessmentStddQs)
                .Where(x => x.ProfessionName.ToLower() == categoryName.ToLower())
                .FirstOrDefaultAsync();

            return q;            
        }

        public async Task<ICollection<Profession>> GetExistingCategoriesInAssessmentQBank()
        {
            var prof = await (from ass in _context.AssessmentQBanks
                join cat in _context.Professions on ass.ProfessionId equals cat.Id
                orderby cat.ProfessionName
                select cat).ToListAsync();
            
            return prof;

        }

        public async Task<AssessmentQBank> InsertAssessmentQBank(AssessmentQBank model)
        {
            _context.Entry(model).State = EntityState.Added;

            try {
                await _context.SaveChangesAsync();
            } catch {
                return null;
            }
            
            return model;
        }

        public async Task<AssessmentQBank> UpdateAssessmentQBank(AssessmentQBank model)
        {
            var existing = await _context.AssessmentQBanks
                .Include(x => x.AssessmentStddQs)
                .Where(x => x.Id == model.Id)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            
            if(existing == null) return null;

            _context.Entry(model).CurrentValues.SetValues(model);

            foreach (var existingItem in existing.AssessmentStddQs.ToList())
            {
                if(!model.AssessmentStddQs.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                {
                    _context.AssessmentStddQs.Remove(existingItem);
                    _context.Entry(existingItem).State = EntityState.Deleted; 
                }
            }
            
            foreach(var newItem in model.AssessmentStddQs)
            {
                var existingItem = existing.AssessmentStddQs
                    .Where(c => c.Id == newItem.Id && c.Id != default(int)).SingleOrDefault();
                if(existingItem != null)    //update navigation record
                {
                    _context.Entry(existingItem).CurrentValues.SetValues(newItem);
                    _context.Entry(existingItem).State = EntityState.Modified;
                } else {    //insert new navigation record
                        
                    var itemToInsert = new AssessmentStddQ
                    {
                        AssessmentParameter = newItem.AssessmentParameter,
                        AssessmentQBankId = newItem.AssessmentQBankId,
                        IsMandatory = newItem.IsMandatory,
                        IsStandardQ = newItem.IsStandardQ,
                        MaxPoints = newItem.MaxPoints,
                        QNo = newItem.QNo,
                        Question = newItem.Question
                
                    };

                    existing.AssessmentStddQs.Add(itemToInsert);
                    _context.Entry(itemToInsert).State = EntityState.Added;
                }
            }

            _context.Entry(existing).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch {
                return null;
            }

            return existing;

        }

        public async Task<bool> DeleteAssessmentQBank(int questionId)
        {
            var q = await _context.AssessmentStddQs.FindAsync(questionId);
            if (q == null) return false;

            _context.AssessmentStddQs.Remove(q);
            _context.Entry(q).State = EntityState.Deleted;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<AssessmentStddQ> InsertStddQ(AssessmentStddQ stddQ)
        {
            if(stddQ.AssessmentQBankId == 0) return null;

            _context.Entry(stddQ).State = EntityState.Added;

            try {
                await _context.SaveChangesAsync();
            } catch {
                return null;
            }

            return stddQ;
        }

        
        public async Task<AssessmentStddQ> UpdateStddQ(AssessmentStddQ stddQ)
        {
            var existing = await _context.AssessmentStddQs.FindAsync(stddQ.Id);
            if (existing == null) return null;

            _context.Entry(stddQ).CurrentValues.SetValues(stddQ);

            try {
                await _context.SaveChangesAsync();
            } catch {
                return null;
            }

            return stddQ;
        }
    }
}