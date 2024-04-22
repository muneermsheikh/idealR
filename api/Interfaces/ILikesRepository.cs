using api.DTOs;
using api.Entities;
using api.Helpers;
using api.Params;

namespace api.Interfaces
{
    public interface ILikesRepository
    {
         Task<UserLike> GetUserLike (int sourceUserId, int targetUserId);
         Task<AppUser> GetUserWithLikes (int userId);
         Task<PagedList<LikeDto>> GetUserLikes (LikesParams likesParams);
    }
}