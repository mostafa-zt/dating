using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLogic.DTOs;
using BusinessLogic.Helpers;
using Entity;

namespace BusinessLogic.Interfaces
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLike(int sourceUserId,int likedUserId);
        Task<AppUser> GetUserWithLikes(int userId);
        Task<PagedList<LikedDto>> GetUserLikes(LikesParams likesParams);
    }
}