using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.DTOs;
using BusinessLogic.Extensions;
using BusinessLogic.Helpers;
using BusinessLogic.Interfaces;
using DAL;
using Entity;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Repositories
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;
        public LikesRepository(DataContext context)
        {
            this._context = context;
        }

        public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
        {
            return await _context.Likes.FindAsync(sourceUserId, likedUserId);
        }

        public async Task<PagedList<LikedDto>> GetUserLikes(LikesParams likesParams)
        {
            var users = _context.Users.OrderBy(o => o.UserName).AsQueryable();
            var likes = _context.Likes.AsQueryable();

            if (likesParams.Predicate == "liked")
            {
                likes = likes.Where(like => like.SourceUserId == likesParams.UserId);
                users = likes.Select(like => like.LikedUser);
            }
            if (likesParams.Predicate == "likedBy")
            {
                likes = likes.Where(like => like.LikedUserId == likesParams.UserId);
                users = likes.Select(like => like.SourceUser);
            }

            var likedUsers = users.Select(user => new LikedDto()
            {
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain).Url,
                City = user.City,
                Id = user.Id
            });

            return await PagedList<LikedDto>.CreateAsync(likedUsers, likesParams.PageNumber, likesParams.PageSize);
        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _context.Users.Include(x => x.LikedUsers).FirstOrDefaultAsync(x => x.Id == userId);
        }
    }
}