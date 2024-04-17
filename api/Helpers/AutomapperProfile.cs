using api.DTOs;
using api.Entities;
using AutoMapper;

namespace api.Helpers
{
    public class AutomapperProfile : Profile
    {
        protected AutomapperProfile()
        {
            CreateMap<AppUser, CandidateDto>();
            CreateMap<Photo, PhotoDto>();
            
        }

    }
}