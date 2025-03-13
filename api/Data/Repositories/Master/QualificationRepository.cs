using api.DTOs.Admin;
using api.Entities.Master;
using api.Helpers;
using api.Interfaces.Masters;
using api.Params.Masters;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Repositories.Master
{
    public class QualificationRepository: IQualificationRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public QualificationRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }


        public async Task<ReturnQualificationDto> AddQualification(string qualificationName)
        {
             var dto = new ReturnQualificationDto();

             var q = await _context.Qualifications
                .Where(x => x.QualificationName.ToLower() == qualificationName.ToLower())
                .FirstOrDefaultAsync();

            if(q != null) {
                dto.ErrorString = "Qualification " + qualificationName + " already exists";
                return dto;
            }

            var obj = new Qualification{QualificationName = qualificationName};

            _context.Entry(obj).State = EntityState.Added;

            if(await _context.SaveChangesAsync() > 0) {
                dto.qualification = obj;
            } else {
                dto.ErrorString = "Failed to add the qualification";
            }
            return dto;

        }

        public async Task<string> DeleteQualificationById(int id)
        {
            var q = await _context.Qualifications.FindAsync(id);
            if(q == null) return "That qualification does not exist";

            _context.Qualifications.Remove(q);
            _context.Entry(q).State = EntityState.Deleted;

            return await _context.SaveChangesAsync() > 0
                ? "" : "Failed to delete the qualification";
        }

        public async Task<string> EditQualification(Qualification qualification)
        {
            var q = await _context.Qualifications
                .Where(x => x.QualificationName.ToLower() == qualification.QualificationName.ToLower())
                .AsNoTracking()
                .FirstOrDefaultAsync();
            
            if(q == null) return "No such qualification name exists in the database";

            _context.Entry(q).CurrentValues.SetValues(qualification);

            return await _context.SaveChangesAsync() > 0
                ? "" : "Failed to update the qualification";

        }

        public async Task<Qualification> GetQualificationById(int id)
        {
            return await _context.Qualifications.FindAsync(id);
        }


        public async Task<ICollection<Qualification>> GetQualificationList()
        {
            return await _context.Qualifications.OrderBy(x => x.QualificationName).ToListAsync();
        }

    }
}