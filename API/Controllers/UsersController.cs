using API.Data;
using API.DTOs;
using API.Entities;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.Controllers
{

    [Authorize]
    public class UsersController : BaseApiController
    {
     
        private readonly IMemberRepository _memberRepository;
        private readonly IMapper _mapper;

        public UsersController(IMemberRepository memberRepository, IMapper mapper)
        {
            
            _memberRepository = memberRepository;
            _mapper = mapper;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var users = await _memberRepository.GetMembersAsync();

            var usersToReturn = _mapper.Map<IEnumerable<MemberDto>>(users);

            return Ok(usersToReturn);

            
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            var user = await _memberRepository.GetMemberByUsernameAsync(username);

            return _mapper.Map<MemberDto>(user);

        }

        [HttpPut]
        public async Task<ActionResult> UpdateMember(MemberUpdateDto memberUpdateDto)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var member = await _memberRepository.GetMemberByUsernameAsync(username);

            //..Instead of manually mapping property by property like this:
            //member.Introduction = memberUpdateDto.Introduction;
            //member.LookingFor = memberUpdateDto.LookingFor;
            //..we can do like this:
            _mapper.Map(memberUpdateDto, member);

            _memberRepository.Update(member);

            if(await _memberRepository.SaveAllAsync())
            {
                return NoContent();
            }
            return BadRequest("Failed to update user");
        }
    }
}
