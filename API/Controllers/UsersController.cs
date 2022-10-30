using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
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
        private readonly IPhotoService _photoService;

        public UsersController(IMemberRepository memberRepository, IMapper mapper, IPhotoService photoService)
        {
            
            _memberRepository = memberRepository;
            _mapper = mapper;
            _photoService = photoService;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery]UserParams userParams)
        {
            var user = await _memberRepository.GetMemberByUsernameAsync(User.GetUsername());

            userParams.CurrentUsername = user.UserName;

            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = user.Gender == "male" ? "female" : "male";
            }

            var users = await _memberRepository.GetMembersAsync(userParams);

            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount,
                users.TotalPages);

            var usersToReturn = _mapper.Map<IEnumerable<MemberDto>>(users);

            return Ok(usersToReturn);

            
        }

        
        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            var member = await _memberRepository.GetMemberByUsernameAsync(username);

            //..Instead of manually mapping property by property like this:
            // var memberDto = new MemberDto();
            //memberDto.UserName = member.UserName
            //memberDto.Introduction = member.Introduction
            //return memberDto
            //..we can do like this:
            return _mapper.Map<MemberDto>(member);


        }

        [HttpPut]
        public async Task<ActionResult> UpdateMember(MemberUpdateDto memberUpdateDto)
        {
            var username = User.GetUsername();
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

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var username = User.GetUsername();

            var member = await _memberRepository.GetMemberByUsernameAsync(username);

            var result = await _photoService.AddPhotoAsync(file);

            if(result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
            };

            if (member.Photos.Count == 0)
            {
                photo.IsMain = true;
            }

            member.Photos.Add(photo);

            if(await _memberRepository.SaveAllAsync())
            {

                //return _mapper.Map<PhotoDto>(photo);

                //..When creating a ressource on the server the best practice is to return 201 response. 
                //..Inside that response there should be a location header indicating where to get that
                //..ressource and in the body we should return the ressource created.
                return CreatedAtRoute("GetUser", new { username = username }, _mapper.Map<PhotoDto>(photo));
                // the first username is the name of our route parameter
            }
            return BadRequest("Problem adding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var username = User.GetUsername();

            var member = await _memberRepository.GetMemberByUsernameAsync(username);

            var photo = member.Photos.FirstOrDefault(p => p.Id == photoId);

            if (photo.IsMain) return BadRequest("This is already your main photo");

            var currentMainPhoto = member.Photos.FirstOrDefault(p => p.IsMain);

            if(currentMainPhoto != null)
            {
                currentMainPhoto.IsMain = false;
            }
            photo.IsMain = true;

            if(await _memberRepository.SaveAllAsync())
            {
                return NoContent();
            }
            else
            {
                return BadRequest("Failed to set main photo");
            }
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var username = User.GetUsername();

            var member = await _memberRepository.GetMemberByUsernameAsync(username);

            var photo = member.Photos.FirstOrDefault(p => p.Id == photoId);

            if(photo == null)
            {
                return NotFound();
            }

            if (photo.IsMain)
            {
                return BadRequest("You cannot delete your main photo");
            }

            if(photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if(result.Error != null) return BadRequest(result.Error.Message);

            }

            member.Photos.Remove(photo);
            if (await _memberRepository.SaveAllAsync()) return Ok();

            return BadRequest("Failed to delete the photo");
        }
    }
}
