using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class LikesController : BaseApiController
    {
        private readonly IMemberRepository _memberRepository;
        private readonly ILikesRepository _likesRepository;

        public LikesController(IMemberRepository memberRepository, ILikesRepository likesRepository)
        {
            _memberRepository = memberRepository;
            _likesRepository = likesRepository;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceMemberId = User.GetUserId();
            var likedMember = await _memberRepository.GetMemberByUsernameAsync(username);
            var sourceMember = await _likesRepository.GetMemberWithLikes(sourceMemberId);

            if (likedMember == null) return NotFound(); // we did not find the user they want to like

            if(sourceMember.UserName == username)
            {
                return BadRequest("You cannot like yourself");
            }

            var memberLike = await _likesRepository.GetMemberLike(sourceMemberId, likedMember.Id);
            if (memberLike != null) return BadRequest("You already liked this member");

            memberLike = new MemberLike
            {
                SourceMemberId = sourceMemberId,
                LikedMemberId = likedMember.Id
            };

            sourceMember.LikedMembers.Add(memberLike);

            if (await _memberRepository.SaveAllAsync()) return Ok();

            return BadRequest("Failed to like member");
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedList<LikeDto>>> GetMemberLikes([FromQuery]LikesParams likesParams)
        {
            likesParams.MemberId = User.GetUserId();
            var memberLikes =  await _likesRepository.GetUserLikes(likesParams);

            Response.AddPaginationHeader(memberLikes.CurrentPage, memberLikes.PageSize, memberLikes.TotalCount, memberLikes.TotalPages);

            return Ok(memberLikes);
        }
    }
}
