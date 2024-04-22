using api.DTOs;
using api.Entities;
using api.Extensions;
using api.Helpers;
using api.Interfaces;
using api.Params;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class LIkesRepository : ILikesRepository
    {
        private readonly DataContext _context;
        
        public LIkesRepository(DataContext context)
        {
            _context = context;
            
        }
        public async Task<UserLike> GetUserLike(int sourceUserId, int targetUserId)
        {
            return await _context.Likes.FindAsync(sourceUserId, targetUserId);
        }

        public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
        {
            var users = _context.Users.OrderBy(x => x.UserName).AsQueryable();
            var likes = _context.Likes.AsQueryable();

            if(likesParams.Predicate == "liked") {
                likes = likes.Where(x => x.SourceUserId == likesParams.UserId);
                users = likes.Select(x => x.TargetUser);
            }

            if(likesParams.Predicate == "likedBy") {
                likes = likes.Where(x => x.TargetUserId == likesParams.UserId);
                users = likes.Select(x => x.SourceUser);
            }

            var likedUsers = users.Select(user => new LikeDto
            {
                UserName = user.UserName,
                Id = user.Id,
                KnownAs = user.KnownAs,
                PhotoUrl = user.photos.FirstOrDefault(x =>x.IsMain).Url,
                Age = user.DateOfBirth.CalculateAge(),
                City = user.City
            });

            return await PagedList<LikeDto>.CreateAsync(likedUsers, likesParams.PageNumber, likesParams.PageSize);

        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _context.Users.Where(x => x.Id == userId)
                .Include(x => x.LikedUsers)
                .FirstOrDefaultAsync();
        }

    }
}