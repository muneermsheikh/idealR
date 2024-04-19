using api.DTOs;
using api.Entities;

namespace api.Interfaces
{
    public interface IUserRepository
    {
        void Update (AppUser user);
        Task<bool> SaveAllAsync();
        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<AppUser> GetUserByIdAsync (int id);
        Task<AppUser> GetUserByUserNameAsync (string username);
        //Task<IEnumerable<MemberDto>> GetCandidatesAsync();
        //Task<PagedList<MemberDto>> GetMembersAsync(string username);
    }
}