using API.DTOs;
using API.Entities;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            //..We want to send to the client the MemberDto, so the maping is 
            //..From Member To MemberDto
            CreateMap<Member, MemberDto>()
                .ForMember(dest => dest.PhotoUrl, 
                opt => opt.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url));

            //..We want to send to the client the PhotoDto, so the maping is 
            //..From Photo To PhotoDto
            CreateMap<Photo, PhotoDto>();

            //..We want to receive from the client the MemberUpdateDto, so the maping is 
            //..From MemberUpdateDto To Member
            CreateMap<MemberUpdateDto, Member>();
        }
    }
}
