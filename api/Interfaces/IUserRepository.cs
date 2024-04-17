using api.DTOs;
using api.Entities;

namespace api.Interfaces
{
    public interface IUserRepository
    {
        void Update (AppUser user);
        Task<bool> SaveAllAsync();
        Task<IEnumerable<CandidateDto>> GetUsersAsync();
        Task<CandidateDto> GetUserByIdAsync (int id);
        Task<CandidateDto> GetUserByUserNameAsync (string username);
    }
}