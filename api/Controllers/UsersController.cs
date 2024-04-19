using System.Security.Claims;
using api.Data;
using api.DTOs;
using api.Entities;
using api.Interfaces;
using api.Params;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
        {
            var users = await _userRepository.GetUsersAsync();
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
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userRepository.GetUserByUserNameAsync(username);

            if (user==null) return NotFound();

            _mapper.Map(memberupdateDto, user);
            /* user.Introduction = memberupdateDto.Introduction;
            user.LookingFor = memberupdateDto.LookingFor;
            user.Interests = memberupdateDto.Interests;
            user.City = memberupdateDto.City;
            user.Country = memberupdateDto.Country;
            */

            if(await _userRepository.SaveAllAsync()) return NoContent();

            //if (await _userServices.UpdateMember(user)) return NoContent();

            return BadRequest("failed to update the database");
        }
    }
}