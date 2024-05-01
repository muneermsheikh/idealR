using api.Data;
using api.DTOs;
using api.DTOs.Admin;
using api.DTOs.Admin.Orders;
using api.DTOs.HR;
using api.Entities;
using api.Entities.Admin;
using api.Entities.Admin.Client;
using api.Entities.Admin.Order;
using api.Entities.HR;
using api.Entities.Identity;
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
            CreateMap<Message, MessageDto>()
                .ForMember(d => d.SenderPhotoUrl, o => o.MapFrom(s => s.Sender.photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(d => d.RecipientPhotoUrl, o => o.MapFrom(s => s.Recipient.photos.FirstOrDefault(x => x.IsMain).Url));
            CreateMap<Customer, CustomerDto>();
            CreateMap<CreateCustomerDto, Customer>();
            CreateMap<Candidate, CandidateBriefDto>();
            CreateMap<CreateCandidateDto, Candidate>();
            CreateMap<FeedbackStddQ, FeedbackItem>();
            CreateMap<Order, OrderBriefDto>();
            CreateMap<Order, OrderDisplayDto>();
            CreateMap<OrderItem, OrderItemDisplayDto>();
            CreateMap<OrderItem, OrderItemBriefDto>();
            CreateMap<OrderItemToCreateDto, OrderItem>();
            CreateMap<Order, OrderDisplayWithItemsDto>();
        }

    }
}