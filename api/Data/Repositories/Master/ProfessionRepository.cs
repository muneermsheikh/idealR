using api.Entities.Master;
using api.Helpers;
using api.Interfaces.Masters;
using api.Params.Masters;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Repositories.Master
{
    public class ProfessionRepository : IProfessionRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public ProfessionRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }


        public async Task<string> AddProfession(string ProfessionName)
        {
            var q = await _context.Professions
                .Where(x => x.ProfessionName.ToLower() == ProfessionName.ToLower())
                .FirstOrDefaultAsync();

            if(q != null) return "That Profession already exists";


            var obj = new Profession{ProfessionName = ProfessionName};

            _context.Entry(obj).State = EntityState.Added;

            return await _context.SaveChangesAsync() > 0 ? "" : "Failed to add the Profession";
        }

        public async Task<string> DeleteProfession(string ProfessionName)
        {
             var q = await _context.Professions
                .Where(x => x.ProfessionName.ToLower() == ProfessionName.ToLower())
                .FirstOrDefaultAsync();

            if(q == null) return "That Profession does not exist";

            _context.Professions.Remove(q);
            _context.Entry(q).State = EntityState.Deleted;

            return await _context.SaveChangesAsync() > 0
                ? "" : "Failed to delete the Profession";
        }

        public async Task<string> DeleteProfessionById(int professionid)
        {
            var q = await _context.Professions.FindAsync(professionid);
                

            if(q == null) return "That Profession does not exist";

            _context.Professions.Remove(q);
            _context.Entry(q).State = EntityState.Deleted;

            return await _context.SaveChangesAsync() > 0
                ? "" : "Failed to delete the Profession";
        }


        public async Task<string> EditProfession(Profession profession)
        {
            var q = await _context.Professions
                .Where(x => x.ProfessionName.ToLower() == profession.ProfessionName.ToLower())
                .AsNoTracking()
                .FirstOrDefaultAsync();
            
            if(q == null) return "No such Profession exists in the database";

            _context.Entry(q).CurrentValues.SetValues(profession);

            return await _context.SaveChangesAsync() > 0
                ? "" : "Failed to update the Profession";
        }

        public async Task<Profession> GetProfessionById(int professionid)
        {
            return await _context.Professions.FindAsync(professionid);
        }


        public async Task<ICollection<Profession>> GetProfessionList()
        {
            return await _context.Professions.OrderBy(x => x.ProfessionName).ToListAsync();
        }


        public async Task<PagedList<Profession>> GetProfessions(ProfessionParams pParams)
        {
            var obj = _context.Professions.AsQueryable();

            if(!string.IsNullOrEmpty(pParams.ProfessionName)) 
                obj = obj.Where(x => x.ProfessionName.ToLower() == pParams.ProfessionName.ToLower());

            if(pParams.Id != 0) obj = obj.Where(x => x.Id == pParams.Id);

            var paged = await PagedList<Profession>.CreateAsync(obj.AsNoTracking()
                .ProjectTo<Profession>(_mapper.ConfigurationProvider),
                pParams.PageNumber, pParams.PageSize);
            
            return paged;
        }

    }
}