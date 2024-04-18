using api.DTOs;
using api.Entities;

namespace api.Interfaces
{
    public interface IUserRepository
    {
        void Update (AppUser user);
        Task<bool> SaveAllAsync();
        Task<IEnumerable<MemberDto>> GetUsersAsync();
        Task<MemberDto> GetUserByIdAsync (int id);
        Task<MemberDto> GetUserByUserNameAsync (string username);
        Task<IEnumerable<MemberDto>> GetCandidatesAsync();
        Task<MemberDto> GetCandidateAsync(string username);
    }
}