using System.Runtime.CompilerServices;
using api.Entities.Master;
using api.Helpers;
using api.Interfaces.Masters;
using api.Params.Masters;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Repositories.Master
{
    public class IndustryRepository : IIndustryRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public IndustryRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<string> AddIndustry(string IndustryName)
        {
            var q = await _context.Industries
                .Where(x => x.IndustryName.ToLower() == IndustryName.ToLower())
                .FirstOrDefaultAsync();

            if(q != null) return "That industry name already exists";


            var obj = new Industry{IndustryName = IndustryName};

            _context.Entry(obj).State = EntityState.Added;

            return await _context.SaveChangesAsync() > 0 ? "" : "Failed to add the industry";
        }

        public async Task<string> DeleteIndustry(string IndustryName)
        {
            var q = await _context.Industries
                .Where(x => x.IndustryName.ToLower() == IndustryName.ToLower())
                .FirstOrDefaultAsync();

            if(q == null) return "That Industry does not exist";
            
            _context.Industries.Remove(q);
            _context.Entry(q).State = EntityState.Deleted;

            return await _context.SaveChangesAsync() > 0
                ? "" : "Failed to delete the Industry";
        }

        public async Task<string> EditIndustry(Industry industry)
        {
            var q = await _context.Industries
                .Where(x => x.IndustryName.ToLower() == industry.IndustryName.ToLower())
                .AsNoTracking()
                .FirstOrDefaultAsync();
            
            if(q == null) return "No such Industry name exists in the database";

            _context.Entry(q).CurrentValues.SetValues(industry);

            return await _context.SaveChangesAsync() > 0
                ? "" : "Failed to update the Industry";
        }

        public async Task<PagedList<Industry>> GetIndustries(IndustryParams indParams)
        {
            var obj = _context.Industries.AsQueryable();

            if(!string.IsNullOrEmpty(indParams.IndustryName)) obj = obj.Where(x => x.IndustryName.ToLower() == indParams.IndustryName.ToLower());

            if(indParams.Id != 0) obj = obj.Where(x => x.Id == indParams.Id);

            var paged = await PagedList<Industry>.CreateAsync(obj.AsNoTracking()
                .ProjectTo<Industry>(_mapper.ConfigurationProvider),
                indParams.PageNumber, indParams.PageSize);
            
            return paged;
        }

        public async Task<Industry> GetIndustryFromId(int id)
        {
            return await _context.Industries.FindAsync(id);
        }

        public async Task<ICollection<Industry>> GetIndustriesList()
        {
            return await _context.Industries.OrderBy(x => x.IndustryName).ToListAsync();
        }

        public async Task<string> DeleteIndustryById(int id)
        {
            var ind = await _context.Industries.FindAsync(id);
            if(ind == null) return "The industry does not exist";

            _context.Industries.Remove(ind);
            _context.Entry(ind).State = EntityState.Deleted;

            return await _context.SaveChangesAsync() > 0 ? "" : "Failed to delete the industry from database";
        }

    }
}