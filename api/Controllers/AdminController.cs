using System.Security.Claims;
using api.DTOs;
using api.Entities;
using api.Entities.Identity;
using api.Errors;
using api.Extensions;
using api.Helpers;
using api.Params;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    public class AdminController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IMapper _mapper;
        public AdminController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IMapper mapper)
        {
            _mapper = mapper;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [Authorize(Policy = "AdminPolicy")]     //Roles: Admin
        [HttpGet("identityroles")]
        public async Task<ICollection<string>> GetRoles() {
            var roles = await _roleManager.Roles.OrderBy(x => x.Name).Select(x => x.Name).ToListAsync();
            return roles;
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles()
        {
            var users = await _userManager.Users
                .Include(x => x.UserRoles)    //not Candidzte
                .Where(x => x.UserRoles.Any(y => y.Role.Name != "Candidate"))
                .OrderBy(u => u.UserName)
                .Select(u => new
                {
                    u.Id,
                    UserName = u.UserName,
                    Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
                })
                .ToListAsync();

            return Ok(users);
            
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberDto memberDto)
        {
            var user = await _userManager.FindByNameAsync(memberDto.UserName);

            if (user==null) return NotFound("User not found");

            //this procedure does not touch the roles property, therefore update all other properties individually
            user.PhoneNumber=memberDto.PhoneNumber;
            user.City = memberDto.City;
            user.Gender = memberDto.Gender;
            user.KnownAs = memberDto.KnownAs;
            user.Position = memberDto.Position;
            user.DateOfBirth = memberDto.DateOfBirth;
            user.Email = memberDto.Email;

            var result = await _userManager.UpdateAsync(user);
           
            if(result.Succeeded) return Ok(result);

            return BadRequest(new ApiException(400, "failure", "Failed to update the User"));
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
        {
            if (string.IsNullOrEmpty(roles)) return BadRequest("You must select at least one role");

            var selectedRoles = roles.Split(",").ToArray();

            var user = await _userManager.FindByNameAsync(username);

            if (user == null) return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);

            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded) return BadRequest("Failed to add to roles");

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded) return BadRequest("Failed to remove from roles");

            return Ok(await _userManager.GetRolesAsync(user));
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("pagedlist")]
        public async Task<ActionResult<PagedList<MemberDto>>> GetUsers([FromQuery]UserParams uParams)
        {
            var query =  _userManager.Users
                .Include(x => x.UserRoles)    //not Candidzte
                .Where(x => x.UserRoles.Any(y => y.Role.Name != "Candidate"))
                .OrderBy(u => u.UserName)
                .Select(u => new MemberDto
                {
                    Id = u.Id, UserName = u.UserName, KnownAs = u.KnownAs, 
                    PhoneNumber = u.PhoneNumber, Gender = u.Gender,
                    Email = u.Email, City = u.City, Country = u.Country, Position = u.Position,
                    DateOfBirth = u.DateOfBirth, Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
                })
            .AsQueryable();
            
            if(!string.IsNullOrEmpty(uParams.Username)) query = query.Where(x => x.UserName.ToLower()==uParams.Username.ToLower());
            if(!string.IsNullOrEmpty(uParams.KnownAs)) query = query.Where(x => x.KnownAs.ToLower()==uParams.KnownAs.ToLower());
            if(!string.IsNullOrEmpty(uParams.Email)) query = query.Where(x => x.Email.ToLower()==uParams.Email.ToLower());
            if(!string.IsNullOrEmpty(uParams.PhoneNumber)) query = query.Where(x => x.PhoneNumber==uParams.PhoneNumber);
            if(!string.IsNullOrEmpty(uParams.Role)) query = query.Where(x => x.Roles.Contains(uParams.Role));
            
            var paged = await PagedList<MemberDto>.CreateAsync(query.AsNoTracking()
                //.ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                , uParams.PageNumber, uParams.PageSize);

            if(paged == null || paged.Count == 0) return null;

            Response.AddPaginationHeader(new PaginationHeader(paged.CurrentPage, 
                paged.PageSize, paged.TotalCount, paged.TotalPages));
            
            return Ok(paged);
            
        }

        /*[Authorize(Policy = "HRMPolicy")]
        [HttpGet("hrmanager")]
        public ActionResult GetPhotosForModeration()
        {
            return Ok("HRManager Role can see this");
        }
        */
    }
}