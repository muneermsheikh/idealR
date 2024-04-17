using api.DTOs;
using api.Entities;
using api.Interfaces;
using AutoMapper;
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

        public async Task<CandidateDto> GetUserByIdAsync(int id)
        {
            var user= await _context.Users.FindAsync(id);
            return _mapper.Map<CandidateDto>(user);
        }

        public async Task<CandidateDto> GetUserByUserNameAsync(string username)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == username.ToLower());
            return _mapper.Map<CandidateDto>(user);
        }

        public async Task<IEnumerable<CandidateDto>> GetUsersAsync()
        {
            var users = await _context.Users.Include(x => x.photos).OrderBy(x => x.UserName).ToListAsync();
            return _mapper.Map<IEnumerable<CandidateDto>>(users);
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