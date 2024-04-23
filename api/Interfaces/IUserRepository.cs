using api.DTOs;
using api.Entities;
using api.Helpers;
using api.Params;

namespace api.Interfaces
{
    public interface IUserRepository
    {
        void Update (AppUser user);
        Task<bool> SaveAllAsync();
        Task<AppUser> GetUserByIdAsync (int id);
        Task<AppUser> GetUserByUserNameAsync (string username);
        Task<IEnumerable<MemberDto>> GetMemberAsync(string username);
        Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);
    }
}