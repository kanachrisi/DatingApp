using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;

        public LikesRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<MemberLike> GetMemberLike(int sourceMemberId, int likedMemberId)
        {
            return await _context.Likes.FindAsync(sourceMemberId, likedMemberId);
        }

        public async Task<Member> GetMemberWithLikes(int memberId)
        {
            return await _context.Users
                .Include(m => m.LikedMembers)
                .FirstOrDefaultAsync(m => m.Id == memberId);
        }

        public async Task<PaginatedList<LikeDto>> GetUserLikes(LikesParams likesParams)
        {
            var members = _context.Users.OrderBy(m => m.UserName).AsQueryable();
            var likes = _context.Likes.AsQueryable();

            if(likesParams.Predicate == "liked")
            {
                likes = likes.Where(like => like.SourceMemberId == likesParams.MemberId);
                members = likes.Select(like => like.LikedMember);
            }

            if(likesParams.Predicate == "likedBy")
            {
                likes = likes.Where(like => like.LikedMemberId == likesParams.MemberId);
                members = likes.Select(like => like.SourceMember);
            }

            var likedUsers =  members.Select(member => new LikeDto
            {
                Username = member.UserName,
                KnownAs = member.KnownAs,
                Age = member.DateOfBirth.CalculateAge(),
                PhotoUrl = member.Photos.FirstOrDefault(p => p.IsMain).Url,
                City = member.City,
                Id = member.Id
            });

            return await PaginatedList<LikeDto>.CreatePaginatedListAsync(likedUsers, likesParams.PageNumber, likesParams.PageSize);
        }
    }
}
