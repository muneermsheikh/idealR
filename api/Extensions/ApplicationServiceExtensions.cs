using api.Data;
using api.Data.Repositories;
using api.Data.Repositories.Admin;
using api.Data.Repositories.Customer;
using api.Data.Repositories.Deployment;
using api.Data.Repositories.Finance;
using api.Data.Repositories.HR;
using api.Data.Repositories.Master;
using api.Data.Repositories.Orders;
using api.Interfaces;
using api.Interfaces.Admin;
using api.Interfaces.Customers;
using api.Interfaces.Deployments;
using api.Interfaces.Finance;
using api.Interfaces.HR;
using api.Interfaces.Masters;
using api.Interfaces.Messages;
using api.Interfaces.Orders;
using api.Services;
using API.Helpers;
using Microsoft.EntityFrameworkCore;

namespace api.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, 
            IConfiguration config)
        {
            services.AddDbContext<DataContext>(opt =>
            {
                opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });

            services.AddCors();

            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserServices, UserServices>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            //photo sservice and cloudinary services not added here pending creation of photos
            services.AddScoped<LogUserActivity>();
            services.AddScoped<ILikesRepository, LIkesRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ICandidateRepository, CandidatesRepository>();
            services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            services.AddScoped<IOrdersRepository, OrdersRepository>();
            services.AddScoped<IJDAndRemunRepository, JDAndRemunRepository>();

            services.AddScoped<IOrderAssessmentRepository, OrderAssessmentRepository>();
            services.AddScoped<IContractReviewRepository, ContractReviewRepository>();
            services.AddScoped<IChecklistRepository, ChecklistRepository>();
            services.AddScoped<ICandidateAssessentRepository, CandidateAssessmentRepository>();
            services.AddScoped<ICVRefRepository, CVRefRepository>();

            services.AddScoped<IComposeMessagesAdminRepository, ComposeMessagesAdminRepository>();
            
            services.AddScoped<IComposeMessagesHRRepository, ComposeMessagesHRRepository>();

            services.AddScoped<IComposeMessagesForTypes, ComposeMessagesForTypes>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IEmploymentRepository, EmploymentRepository>();

            services.AddScoped<ITaskRepository, TaskRepository>(); 
            
            services.AddScoped<IQueryableRepository, QueryableRepository>();
            
            services.AddScoped<IFinanceRepository, FinanceRepository>();
            services.AddScoped<ISelDecisionRepository, SelDecisionRepository>();
            services.AddScoped<IDeploymentRepository, DeploymentRepository>();
            services.AddScoped<ICustomerReviewRepository, CustomerReviewRepository>();
            services.AddScoped<IOrderForwardRepository, OrderForwardRepository>();
            services.AddScoped<ICallRecordRepository, CallRecordRepository>();
            services.AddScoped<IAssessmentQBankRepository, AssessmentQBankRepository>();
            services.AddScoped<IFileUploadRepository, FileUploadRepository>();
            services.AddScoped<IIndustryRepository, IndustryRepository>();
            services.AddScoped<IProfessionRepository, ProfessionRepository>();
            services.AddScoped<IQualificationRepository, QualificationRepository>();
            services.AddScoped<IHelpRepository, HelpRepository>();
            services.AddScoped<IComposeMsgsForCandidates, ComposeMsgsForCandidates>();
            
            services.AddScoped<IProspectiveCandidatesRepository, ProspectiveCandidatesRepository>();
            services.AddScoped<ICustomerReviewRepository, CustomerReviewRepository>();
            services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            
            return services;
        }

    }
}