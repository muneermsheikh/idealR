using api.DTOs;
using api.DTOs.HR;
using api.Entities;
using api.Entities.Identity;
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
        Task<ICollection<CVsMatchingProfAvailableDto>> GetMatchingCandidatesAvailable(int professionid);
    }
}