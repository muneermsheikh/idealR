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
        private readonly IConfiguration _config;
        public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, IMapper mapper, IConfiguration config)
        {
            _config = config;
            _userManager = userManager;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        [HttpPost("LogIn")]
        public async Task<ActionResult<UserDto>> LogIn (LoginDto loginDto)
        {
            var user = await _userManager.Users
                //.SingleOrDefaultAsync(x => x.UserName == loginDto.UserName.ToLower());
                .SingleOrDefaultAsync(x => x.NormalizedUserName == loginDto.UserName.ToUpper());
            
            if(user==null) return Unauthorized("invalid credentials");

            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if(!result) return Unauthorized("invalid credentials");
            
            var usr = new UserDto {
                UserName = user.UserName,
                Employer = _config["RAName"],
                Token = await _tokenService.CreateToken(user),
                //photoUrl = user.photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender,
                Address = _config["RAName"],
                City = _config["City"],
                Country = _config["Country"]
            };

            return usr;
        }

    }
}