using api.Data;
using api.Data.Repositories;
using api.Data.Repositories.Admin;
using api.Data.Repositories.Finance;
using api.Data.Repositories.HR;
using api.Data.Repositories.Master;
using api.Interfaces;
using api.Interfaces.Admin;
using api.Interfaces.Finance;
using api.Interfaces.HR;
using api.Interfaces.Messages;
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

            services.AddScoped<IAssessmentRepository, AssessmentRepository>();
            services.AddScoped<IContractReviewRepository, ContractReviewRepository>();
            services.AddScoped<IChecklistRepository, ChecklistRepository>();
            services.AddScoped<ICandidateAssessentRepository, CandidateAssessmentRepository>();
            services.AddScoped<ICVRefRepository, CVRefRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();

            services.AddScoped<IComposeMessagesAdminRepository, ComposeMessagesAdminRepository>();
            
            services.AddScoped<IComposeMessagesAdminRepository, ComposeMessagesAdminRepository>();

            services.AddScoped<IComposeMessagesForTypes, ComposeMessagesForTypes>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();

            services.AddScoped<ITaskRepository, TaskRepository>(); 
            
            services.AddScoped<IQueryableRepository, QueryableRepository>();
            //services.AddScoped<ISelDecisionRepository, SelDecisionRepository>();

            services.AddScoped<IFinanceRepository, FinanceRepository>();

            return services;
        }

    }
}