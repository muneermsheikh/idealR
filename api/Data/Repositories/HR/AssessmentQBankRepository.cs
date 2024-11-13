using api.DTOs;
using api.DTOs.HR;
using api.Entities.Admin.Order;
using api.Entities.HR;
using api.Entities.Master;
using api.Extensions;
using api.Helpers;
using api.Interfaces.HR;
using api.Params.HR;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DocumentFormat.OpenXml.Drawing.Diagrams;
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

        public async Task<ICollection<OrderAssessmentItemQ>> GetAssessmentQBankByOrderItemId(int orderitemid)
        {
            var query = await (from assessment in _context.OrderAssessmentItems 
                    where assessment.OrderItemId == orderitemid 
                join qBank in _context.OrderAssessmentItemQs 
                    on assessment.Id equals qBank.OrderAssessmentItemId
                select new OrderAssessmentItemQ {
                    Id = assessment.Id,  MaxPoints = qBank.MaxPoints, IsMandatory = qBank.IsMandatory,
                    OrderAssessmentItemId = qBank.OrderAssessmentItemId, Question = qBank.Question,
                    QuestionNo = qBank.QuestionNo, Subject = qBank.Subject
                }).ToListAsync();

                return query;   
        }

        public async Task<PagedList<AssessmentQBankDto>> GetAssessmentQBanks(AssessmentQBankParams qParams)
        {
            var q =  _context.AssessmentBanks.AsQueryable();
            
            if(!string.IsNullOrEmpty(qParams.ProfessionName)) 
                q = q.Where(x => x.ProfessionName.ToLower() == qParams.ProfessionName.ToLower());
            
            var paged = await PagedList<AssessmentQBankDto>.CreateAsync(q.AsNoTracking()
                    .ProjectTo<AssessmentQBankDto>(_mapper.ConfigurationProvider),
                    qParams.PageNumber, qParams.PageSize);
            
            return paged;
            
        }

        public async Task<ICollection<AssessmentBankDto>> GetAssessmentStddQList(AssessmentQBankParams qParams) {
            
            
            var query = (from bk in _context.AssessmentBanks orderby bk.ProfessionName
                    join q in _context.AssessmentBankQs on bk.Id equals q.AssessmentBankId
                select new AssessmentBankDto {
                    Id = bk.Id, ProfessionId = bk.ProfessionId, 
                    ProfessionName = bk.ProfessionName, QNo = q.QNo,
                    Question = q.Question, AssessmentParameter = q.AssessmentParameter,
                    IsMandatory = q.IsMandatory, IsStandardQ = q.IsStandardQ,
                    MaxPoints = q.MaxPoints
                }).AsQueryable();
                       
            return await query.ToListAsync();
        }
        public async Task<ICollection<AssessmentBank>> GetAssessmentQsOfACategoryByName(string categoryName)
        {
            var qs =  await _context.AssessmentBanks
                .Include(x => x.AssessmentBankQs.OrderBy(m => m.QNo))
                .Where(x => x.ProfessionName.ToLower() == categoryName.ToLower())
                .ToListAsync();

            return qs;            
        }

        public async Task<ICollection<AssessmentBank>> GetExistingCategoriesInAssessmentBanks()
        {
            var prof = await (from ass in _context.AssessmentBanks
                orderby ass.ProfessionName
                select ass)
                .ToListAsync();
            
            return prof;
        }

        public async Task<bool> InsertAssessmentBank(AssessmentBank model)
        {
            var assessment = await _context.AssessmentBanks
                .Where(x => x.Id == model.Id).Include(x => x.AssessmentBankQs)
                .FirstOrDefaultAsync();
            
            if(assessment == null) {
                var qs = new List<AssessmentBankQ>();
                foreach(var q in model.AssessmentBankQs) {
                    qs.Add(new AssessmentBankQ{
                        QNo = q.QNo, AssessmentParameter = q.AssessmentParameter,
                        IsMandatory=q.IsMandatory, Question = q.Question, MaxPoints=q.MaxPoints});
                }
                assessment = new AssessmentBank {
                    ProfessionId = model.ProfessionId, ProfessionName = model.ProfessionName,
                    AssessmentBankQs = qs };

                _context.Entry(assessment).State = EntityState.Added;
            } else {
                _context.Entry(assessment).CurrentValues.SetValues(model);
                assessment.AssessmentBankQs = model.AssessmentBankQs;
            }

            try {
                await _context.SaveChangesAsync();
                return true;
            } catch {
                return false;
            }
            
        }

        public async Task<bool> UpdateAssessmentBank(AssessmentBank model)
        {
            var existing = await _context.AssessmentBanks
                .Include(x => x.AssessmentBankQs)
                .Where(x => x.Id == model.Id)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            
            if(existing == null) return false;

            _context.Entry(model).CurrentValues.SetValues(model);
            
            if(existing.AssessmentBankQs == null || existing.AssessmentBankQs.Count == 0) {
                foreach(var newItem in model.AssessmentBankQs)
                {
                    var itemToInsert = new AssessmentBankQ
                    {
                        AssessmentParameter = newItem.AssessmentParameter,
                        AssessmentBankId = newItem.AssessmentBankId,
                        IsMandatory = newItem.IsMandatory,
                        IsStandardQ = newItem.IsStandardQ,
                        MaxPoints = newItem.MaxPoints,
                        QNo = newItem.QNo,
                        Question = newItem.Question
                    };

                    existing.AssessmentBankQs.Add(itemToInsert);
                    _context.Entry(itemToInsert).State = EntityState.Added;
                }

            } else {

                foreach (var existingItem in existing.AssessmentBankQs.ToList())
                {
                    if(!model.AssessmentBankQs.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                    {
                        _context.AssessmentBankQs.Remove(existingItem);
                        _context.Entry(existingItem).State = EntityState.Deleted; 
                    }
                }

                foreach(var newItem in model.AssessmentBankQs)
                {
                    var existingItem = existing.AssessmentBankQs
                        .Where(c => c.Id == newItem.Id && c.Id != default(int)).SingleOrDefault();
                    if(existingItem != null)    //update navigation record
                    {
                        _context.Entry(existingItem).CurrentValues.SetValues(newItem);
                        _context.Entry(existingItem).State = EntityState.Modified;
                    } else {    //insert new navigation record
                            
                        var itemToInsert = new AssessmentBankQ
                        {
                            AssessmentParameter = newItem.AssessmentParameter,
                            AssessmentBankId = newItem.AssessmentBankId,
                            IsMandatory = newItem.IsMandatory,
                            IsStandardQ = newItem.IsStandardQ,
                            MaxPoints = newItem.MaxPoints,
                            QNo = newItem.QNo,
                            Question = newItem.Question
                    
                        };

                        existing.AssessmentBankQs.Add(itemToInsert);
                        _context.Entry(itemToInsert).State = EntityState.Added;
                    }
                }
            }


            _context.Entry(existing).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
                return true;
            } catch {
                return false;
            }

        }

        public async Task<bool> DeleteAssessmentBankQ(int questionId)
        {
            var q = await _context.AssessmentBankQs.FindAsync(questionId);
            if (q == null) return false;

            _context.AssessmentBankQs.Remove(q);
            _context.Entry(q).State = EntityState.Deleted;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<AssessmentBankQ> InsertStddQ(AssessmentBankQ stddQ)
        {
            if(stddQ.AssessmentBankId == 0) return null;

            _context.Entry(stddQ).State = EntityState.Added;

            try {
                await _context.SaveChangesAsync();
            } catch {
                return null;
            }

            return stddQ;
        }

        
        public async Task<AssessmentBankQ> UpdateStddQ(AssessmentBankQ stddQ)
        {
            var existing = await _context.AssessmentBankQs.FindAsync(stddQ.Id);
            if (existing == null) return null;

            _context.Entry(stddQ).CurrentValues.SetValues(stddQ);

            try {
                await _context.SaveChangesAsync();
            } catch {
                return null;
            }

            return stddQ;
        }
        
        public async Task<AssessmentBank> GetOrCreateCustomAssessmentQsForAProfession(int professionid)
        {
            var qs = await _context.AssessmentBanks
                .Include(x => x.AssessmentBankQs.OrderBy(x => x.QNo))
                .Where(x => x.ProfessionId == professionid)
                .FirstOrDefaultAsync() ?? new AssessmentBank{
                    Id=0, ProfessionId=professionid,
                    ProfessionName = await _context.GetProfessionNameFromId(professionid),
                    AssessmentBankQs = new List<AssessmentBankQ>()  {new () {IsStandardQ=false, AssessmentParameter="Test",
                         QNo=1, Question="Question", IsMandatory=false, MaxPoints=1 }}
                 };

            return qs;
            
        }

        public async Task<PagedList<AssessmentBank>> GetQBankPaginated(AssessmentQBankParams qParams)
        {
            var q =  _context.AssessmentBanks.AsQueryable();
            
            var paged = await PagedList<AssessmentBank>.CreateAsync(q.AsNoTracking()
                    .ProjectTo<AssessmentBank>(_mapper.ConfigurationProvider),
                    qParams.PageNumber, qParams.PageSize);
            
            return paged;
        }

    }
}