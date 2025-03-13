using api.DTOs;
using api.DTOs.Admin;
using api.DTOs.HR;
using api.DTOs.Process;
using api.Entities;
using api.Entities.Identity;
using api.Helpers;
using api.Params;
using AutoMapper.Execution;

namespace api.Interfaces
{
    public interface IUserRepository
    {
        void Update (AppUser user);
        Task<AppUserReturnDto> CreateAppUser(string userType, int userTypeValue, string loggedInUsername);
        Task<bool> SaveAllAsync();
        Task<AppUser> GetUserByIdAsync (int id);
        Task<AppUser> GetUserByUserNameAsync (string username);
        Task<bool> DeleteMember(int id);
        Task<ICollection<CVsMatchingProfAvailableDto>> GetMatchingCandidatesAvailable(int professionid);
        Task<NextDepDataDto> GetNextRecruitmentProcess(string PPNo);
        Task<CandidateDto> GetUserHistory(int CandidateId);
    }
}