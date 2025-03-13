using api.DTOs.Admin;
using api.DTOs.HR;
using api.DTOs.Process;
using api.Entities.Identity;
using api.Errors;
using api.Extensions;
using api.Interfaces;
using API.Helpers;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    public class UsersController: BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IUserServices _userServices;
        public UsersController(IUserRepository userRepository, IMapper mapper, IUserServices userServices)
        {
            _userServices = userServices;
            _mapper = mapper;
            _userRepository = userRepository;
        }

    
        [HttpGet("byid/{id}")]
        public async Task<ActionResult<AppUser>> GetUser(int id)
        {
            return await _userRepository.GetUserByIdAsync(id);
            
        }
        
        
        [HttpGet("byusername/{username}")]
        public async Task<ActionResult<AppUser>> GetUserByUsername(string username)
        {
            return await _userRepository.GetUserByUserNameAsync(username);
        }

       
        [HttpGet("candidatesmatchingprof/{professionid}")]
        public async Task<ActionResult<CVsMatchingProfAvailableDto>> GetMatchingCandidates(int professionid)
        {
            var obj = await _userRepository.GetMatchingCandidatesAvailable(professionid);

            return Ok(obj);
        }

        [HttpDelete("delete/{memberid}")]
        public async Task<ActionResult<bool>> DeleteMember(int memberid)
        {
            return await _userRepository.DeleteMember(memberid);

        }

        [HttpPost("newappuser/{usertype}/{usertypevalue}")]
        public async Task<ActionResult<AppUserReturnDto>> CreateAppUser(string usertype, int usertypevalue)
        {
            var dtoErr = await _userRepository.CreateAppUser(usertype, usertypevalue, User.GetUsername());
            if(!string.IsNullOrEmpty(dtoErr.Error)) return BadRequest(new ApiException(400, dtoErr.Error, "Failed to create the App User"));

            return dtoErr;
        }

        [HttpGet("getNextProcess/{passportno}")]
        public async Task<ActionResult<NextDepDataDto>> GetNextProcessData(string passportno)
        {
            var dto = await _userRepository.GetNextRecruitmentProcess(passportno);
            return dto;
        }

        [HttpGet("candidateDto/{candidateId}")]
        public async Task<ActionResult<CandidateDto>> GetCandidateHistory(int candidateId)
        {
            var dto = await _userRepository.GetUserHistory(candidateId);

            return dto;
        }
        
    }
}