using api.DTOs;
using api.DTOs.HR;
using api.Entities.Identity;
using api.Helpers;
using api.Interfaces;
using api.Params;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public UserRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        
        public async Task<AppUser> GetCandidateAsync(string username)
        {
            return await _context.Users.Where(x => x.UserName == username)
                //.ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<ICollection<CVsMatchingProfAvailableDto>> GetMatchingCandidatesAvailable(int professionid)
        {
            var query2 = await (from prof in _context.UserProfessions where prof.ProfessionId == professionid
                    join cv in _context.Candidates on prof.CandidateId equals cv.Id where
                        "!traveledcanceledselectedblacklisted".Contains(cv.Status.ToLower())
                    join ph in _context.UserPhones on cv.Id equals ph.CandidateId where ph.IsValid
                    orderby cv.ApplicationNo
                    select new CVsMatchingProfAvailableDto {
                        ApplicationNo = cv.ApplicationNo, City = cv.City, FullName = cv.FullName, 
                        CandidateId = Convert.ToString(cv.Id), Gender=  cv.Gender, Checked = false, 
                        ProfessionName = prof.ProfessionName, Source="Candidates", MobileNo=ph.MobileNo
                    }).ToListAsync();

            var query = await (from prosp in _context.ProspectiveCandidates where prosp.ProfessionId==professionid
                select new CVsMatchingProfAvailableDto {
                    CandidateId = prosp.PersonId, City = prosp.CurrentLocation, FullName = prosp.CandidateName,
                    Gender = prosp.Gender, Checked=false, Source = "Prospectives", 
                    ProfessionName=prosp.ProfessionName, MobileNo = prosp.PhoneNo
                }).ToListAsync();
            
            foreach(var q in query) {
                query2.Add(q);
            }

            return query2;
        }
        public async Task<AppUser> GetUserByIdAsync(int id)
        {
                return await _context.Users.Where(x => x.Id == id)
                //.ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<AppUser> GetUserByUserNameAsync(string username)
        {
            var dto = await _context.Users
                .Where(x => x.UserName == username)
                //.ProjectTo<AppUser>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();

            return dto;
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;

        }
    }
}