using api.DTOs;
using api.DTOs.Admin;
using api.DTOs.Admin.Orders;
using api.DTOs.Finance;
using api.DTOs.HR;
using api.DTOs.Order;
using api.DTOs.Orders;
using api.DTOs.Process;
using api.Entities;
using api.Entities.Admin;
using api.Entities.Admin.Client;
using api.Entities.Admin.Order;
using api.Entities.Deployments;
using api.Entities.Finance;
using api.Entities.HR;
using api.Entities.Identity;
using api.Entities.Master;
using api.Entities.Messages;
using api.Entities.Tasks;
using api.Extensions;
using AutoMapper;
using AutoMapper.Execution;


namespace api.Helpers
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            /*CreateMap<MemberUpdateDto, AppUser>();
            CreateMap<AppUser, MemberDto>()
                .ForMember(dest => dest.photoUrl, 
                    opt => opt.MapFrom(src => src.photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(dest => dest.Age, 
                    opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
                
            CreateMap<Photo, PhotoDto>();
            
            CreateMap<MemberUpdateDto, MemberDto>(); */    //dimmy
            
            CreateMap<AppUser, MemberDto>()
                .ForMember(dest => dest.Age, 
                    opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            
            //CreateMap<MemberDto, MemberDto>();
            
            
            CreateMap<RegisterDto, AppUser>();
            /*CreateMap<Message, MessageDto>()
                .ForMember(d => d.SenderPhotoUrl, o => o.MapFrom(s => s.Sender.photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(d => d.RecipientPhotoUrl, o => o.MapFrom(s => s.Recipient.photos.FirstOrDefault(x => x.IsMain).Url)); */
            CreateMap<Customer, CustomerDto>();
            CreateMap<CreateCustomerDto, Customer>();
            CreateMap<Candidate, CandidateBriefDto>();

            CreateMap<CreateCandidateDto, Candidate>();
            CreateMap<ProspectiveCandidate, ProspectiveBriefDto>();

            CreateMap<CustomerFeedback, FeedbackDto>();
            //CreateMap<FeedbackInputItem, FeedbackItem>();
            //CreateMap<FeedbackItem, FeedbackInputItem>();

            CreateMap<Order, OrderBriefDto>();
            CreateMap<OrderBriefDto, OrderBriefDto>();
            CreateMap<Order, OrderDisplayDto>();
            CreateMap<OrderItem, OrderItemDisplayDto>();
            CreateMap<OrderItem, OrderItemBriefDto>();
            CreateMap<OrderItemToCreateDto, OrderItem>();
            CreateMap<Order, OrderDisplayWithItemsDto>();
            
            //CreateMap<OrderForwardToAgent, OrderForwardToAgentDto>();
            CreateMap<OrderForwardCategory, OrderForwardCategory>();
            CreateMap<OrderForwardCategory, OrderForwardToAgentDto>();
                //.ForMember(s=>s.OrderForwardCategoriesDto, o => o.MapFrom(x => x.OrderForwardCategories));
            CreateMap<OrderForwardCategoryOfficial, OrderForwardToOfficialDto>();
            
            CreateMap<OrderForwardCategory, OrderForwardCategoryDto>();
            CreateMap<OrderForwardCategoryOfficial, OrderForwardToOfficialDto>();
            
            CreateMap<ContractReviewItemQ, ContractReviewItemStddQ>();
            CreateMap<ContractReview, ContractReviewDto>();
            CreateMap<ContractReviewItem, ContractReviewItemDto>(); //add OrderDate, CustomerName, OrderNo

            CreateMap<ChecklistHR, ChecklistHRDto>();
            CreateMap<ChecklistHRDto, ChecklistHR>();

            CreateMap<AssessmentQStdd, CandidateAssessmentItem>();

            CreateMap<CVRefDto, CVRefDto>();

            CreateMap<Intervw, InterviewBriefDto>();
            CreateMap<OrderAssessmentItemDto, OrderAssessmentItem>();
            CreateMap<OrderAssessmentItem, OrderAssessmentItemDto>();
            CreateMap<OrderAssessmentItemQ, CandidateAssessmentItem>();

            CreateMap<CandidateAssessment, CandidateAssessedDto>();
            CreateMap<OrderAssessmentItemQ, CandidateAssessmentItem>();

            CreateMap<AssessmentQStdd, OrderAssessmentItemQ>();

            CreateMap<Employee, EmployeeBriefDto>();
            CreateMap<EmployeeToAddDto, Employee>();

            CreateMap<SelectionDecision, SelDecisionDto>();
                
            CreateMap<SelDecisionDto, SelectionDecision>();
            
            CreateMap<Employment, EmploymentDto>();
            CreateMap<EmploymentDto, Employment>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.SelectionDecisionId));
                
            CreateMap<Employment, EmploymentsNotConcludedDto>();

            CreateMap<PendingDebitApprovalDto, PendingDebitApprovalDto>();

            CreateMap<DeploymentPendingTempDto, DeploymentPendingDto>();

            CreateMap<Dep, DeploymentPendingDto>();
            CreateMap<DeploymentPendingDto, DeploymentPendingDto>();
            CreateMap<CandidateFlightGrp, CandidateFlightGrpDto>();

            CreateMap<CreateCustomerDto, Customer>();
            CreateMap<Customer, CustomerAndOfficialsDto>();

            CreateMap<Employment, Employment>();    //AutoMapper configuration needs the mapping even between similar objects
            CreateMap<OpenOrderItemCategoriesDto, OpenOrderItemCategoriesDto>();

            CreateMap<Profession, Profession>();
            CreateMap<Qualification, Qualification>();
            CreateMap<Industry, Industry>();
            
            CreateMap<OrderItemBriefDto, OrderItemAssignmentDto>();

            CreateMap<AppTask, TaskInBriefDto>();

            CreateMap<CallRecord, CallRecordDto>();
            CreateMap<CallRecord,CallRecordBriefDto>();

            //finance
            //CreateMap<COA, COA>();
            //CreateMap<Voucher, Voucher>();
            CreateMap<PendingDebitApprovalDto, VoucherEntry>();

            //DateTime difference between dates retrieved from DB and generated from code
            CreateMap<DateTime, DateTime>().ConvertUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));
            CreateMap<DateTime?, DateTime?>().ConvertUsing(d => d.HasValue ? DateTime.SpecifyKind(d.Value, DateTimeKind.Utc) : null);

        }

    }
}