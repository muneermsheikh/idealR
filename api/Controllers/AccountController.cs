using api.DTOs;
using api.Entities.Identity;
using api.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        [HttpPost("LogIn")]
        public async Task<ActionResult<UserDto>> LogIn (LoginDto loginDto)
        {
            var user = await _userManager.Users
                .Include(p => p.photos)
                .SingleOrDefaultAsync(x => x.UserName == loginDto.UserName.ToLower());
            
            if(user==null) return Unauthorized("invalid credentials");

            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if(!result) return Unauthorized("invalid credentials");
            
            var usr = new UserDto {
                UserName = user.UserName,
                Token = await _tokenService.CreateToken(user),
                //photoUrl = user.photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };

            return usr;
        }

        /*[HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if(await userExists(registerDto.Username)) return BadRequest("username is already taken");

            var user = _mapper.Map<AppUser>(registerDto);
   
            user.UserName = registerDto.Username.ToLower();
            
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if(!result.Succeeded) return BadRequest(result.Errors);

            var roleResult = await _userManager.AddToRoleAsync(user, registerDto.Role);
            if(!roleResult.Succeeded) return BadRequest(roleResult.Errors);

             return new UserDto {
                UserName = user.UserName,
                Token = await _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }
        */

        /*private async Task<bool> userExists (string username)
        {
            return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
        */
    }
}