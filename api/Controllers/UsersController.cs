using api.Data;
using api.DTOs;
using api.Entities;
using api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Authorize]
    public class UsersController: BaseApiController
    {
        private readonly IUserRepository _userRepository;
        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            
       
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            var users = await _userRepository.GetUsersAsync();
            return Ok(users);

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CandidateDto>> GetUser(int id)
        {
            return await _userRepository.GetUserByIdAsync(id);
            
        }
        
        
        [HttpGet("{username}")]
        public async Task<ActionResult<CandidateDto>> GetUserByUsername(string username)
        {
            return await _userRepository.GetUserByUserNameAsync(username);
        }
    }
}