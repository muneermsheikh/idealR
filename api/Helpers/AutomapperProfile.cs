using api.Data;
using api.DTOs;
using api.Entities;
using api.Extensions;
using AutoMapper;

namespace api.Helpers
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<MemberUpdateDto, AppUser>();
            CreateMap<AppUser, MemberDto>()
                .ForMember(dest => dest.photoUrl, 
                    opt => opt.MapFrom(src => src.photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(dest => dest.Age, 
                    opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
                
            CreateMap<Photo, PhotoDto>();
            
            CreateMap<MemberUpdateDto, MemberDto>();    //dimmy
            
            CreateMap<RegisterDto, AppUser>();
        }

    }
}