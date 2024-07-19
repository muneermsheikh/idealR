using System.Security.Claims;
using api.DTOs;
using api.Entities.Identity;
using api.Extensions;
using api.Helpers;
using api.Interfaces;
using api.Params;
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

        [HttpGet("userswithroles")]
        public async Task<ActionResult<ICollection<MemberDto>>> GetUsersWithRoles()
        {
            var users = await _userRepository.GetMembersWithRoles();
            return Ok(users);
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<MemberDto>>> GetUsers([FromQuery]UserParams userParams)
        {
            var currentusernamestring = User.GetUsername();
            var currentuser = await _userRepository.GetUserByUserNameAsync(User.GetUsername());
            userParams.CurrentUsername = currentuser.UserName;

            /* if(string.IsNullOrEmpty(userParams.Gender)) {
                userParams.Gender = currentuser.Gender=="male" ? "female" : "male";
            }
            */
            if(string.IsNullOrEmpty(userParams.Gender)) userParams.Gender="male";
            
            var users = await _userRepository.GetMembersAsync(userParams);

            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, 
                users.PageSize, users.TotalCount, users.TotalPages));
                
            return Ok(users);
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

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberupdateDto)
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var user = await _userRepository.GetUserByUserNameAsync(username);

            if (user==null) return NotFound("User not found");

            _mapper.Map(memberupdateDto, user);
           
            if(await _userRepository.SaveAllAsync()) return NoContent();

            //if (await _userServices.UpdateMember(user)) return NoContent();

            return BadRequest("failed to update the database");
        }
    }
}