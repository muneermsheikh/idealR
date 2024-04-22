using api.DTOs;
using api.Entities;
using api.Helpers;
using api.Interfaces;
using api.Params;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Data
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

        public Task<IEnumerable<MemberDto>> GetMemberAsync(string username)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedList<MemberDto>> GetMembersAsync([FromQuery]UserParams userParams)
        {
                var query = _context.Users.AsQueryable();
                //query = query.Where(x => x.UserName != userParams.CurrentUsername);
                query = query.Where(x => x.Gender == userParams.Gender);
                
                if(userParams.MaxAge > 0) {
                    var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
                    query = query.Where(u => u.DateOfBirth >= minDob);
                }

                if(userParams.MinAge > 0) {
                    var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));                   
                    query = query.Where(u => u.DateOfBirth <= maxDob);
                }

                query = userParams.OrderBy switch
                {
                    "created" => query.OrderByDescending(x => x.Created),
                    _ => query.OrderByDescending(x => x.LastActive)
                };

             
                var paged = await PagedList<MemberDto>.CreateAsync(query.AsNoTracking()
                    .ProjectTo<MemberDto>(_mapper.ConfigurationProvider),
                    userParams.PageNumber, userParams.PageSize);
                return paged;
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
                return await _context.Users.Where(x => x.Id == id)
                //.ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<MemberDto> GetUserByUserNameAsync(string username)
        {
            var dto = await _context.Users
                .Where(x => x.UserName == username)
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
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