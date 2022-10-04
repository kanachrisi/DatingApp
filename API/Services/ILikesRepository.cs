using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Services
{
    public interface ILikesRepository
    {
        public Task<MemberLike> GetMemberLike(int sourceMemberId, int likedMemberId);

        public Task<Member> GetMemberWithLikes(int memberId);

        public Task<PaginatedList<LikeDto>> GetUserLikes(LikesParams likesParams);
    }
}
