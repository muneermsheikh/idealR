using api.Data;
using api.DTOs;
using api.Entities;
using api.Entities.Admin;
using api.Extensions;
using api.Helpers;
using api.Interfaces;
using api.Params;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Authorize]
    public class LikesController: BaseApiController
    {
        private readonly ILikesRepository _likesRepository;
        private readonly IUserRepository _userRepository;
        public LikesController(ILikesRepository likesRepository, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _likesRepository = likesRepository;
        }


        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId = User.GetUserId();
            var SourceUser = await _likesRepository.GetUserWithLikes(sourceUserId);

            var LikedUser = await  _userRepository.GetUserByUserNameAsync(username);
            if(LikedUser == null) return NotFound();

            if(SourceUser.UserName == username) return BadRequest("You cannot like yourself");

            var userLike = await _likesRepository.GetUserLike(sourceUserId, LikedUser.Id);
            
            if(userLike != null) return BadRequest("You have already liked the selected User");

            userLike = new UserLike{SourceUserId=sourceUserId, TargetUserId=LikedUser.Id};

            SourceUser.LikedUsers.Add(userLike);

            if(await _userRepository.SaveAllAsync()) return Ok();

            return BadRequest("failed to update the Likes");

        }

        [HttpGet]
        public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikes ([FromQuery]LikesParams likesParams)
        {
            likesParams.UserId = User.GetUserId();

            var users = await _likesRepository.GetUserLikes(likesParams);

            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, 
                users.PageSize, users.TotalCount, users.TotalPages));
                
            return Ok(users);

        }
    }
}