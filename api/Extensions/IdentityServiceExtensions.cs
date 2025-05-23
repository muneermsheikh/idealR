using System.Text;
using api.Data;
using api.Entities;
using api.Entities.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace api.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services,
            IConfiguration config)
        {
            services.AddIdentityCore<AppUser>(opt => {
                opt.Password.RequireNonAlphanumeric=false;
            })
                .AddRoles<AppRole>()
                .AddRoleManager<RoleManager<AppRole>>()
                .AddEntityFrameworkStores<DataContext>();


            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var tokenKey = config["TokenKey"] ?? throw new Exception("TokenKey not found");
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                    
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context => 
                        {
                            var accessToken = context.Request.Query["access_token"];

                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                            {
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        }
                    };
                });
            
            
            services.AddAuthorization(opt => {
                opt.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
                opt.AddPolicy("HRMPolicy", policy => policy.RequireRole("HR Manager", "HR Supervisor", "HR Executive", "Admin", "Admin Manager"));
                opt.AddPolicy("CVRefPolicy", policy => policy.RequireRole("CVRefer"));
                opt.AddPolicy("SelectionPolicy", policy => policy.RequireRole("Selection"));
                opt.AddPolicy("CustomerPolicy", policy => policy.RequireRole("HR Manager", "HR Supervisor", "Admin", "Admin Manager", "Document Controller-Admin"));
                opt.AddPolicy("HRSupPolicy", policy => policy.RequireRole("HR Supervisor", "HR Executive", "Asst HR Executive"));
                opt.AddPolicy("HRExecPolicy", policy => policy.RequireRole("HR Executive", "Asst HR Executive"));
                opt.AddPolicy("AsstHRExecPolicy", policy => policy.RequireRole("Asst HR Executive"));
                opt.AddPolicy("AdminManagerPolicy", policy => policy.RequireRole("Admin Manager"));
                opt.AddPolicy("MarketingManagerPolicy", policy => policy.RequireRole("Marketing Manager"));
                opt.AddPolicy("ContractReviewPolicy", policy => policy.RequireRole("Contract Review"));
                opt.AddPolicy("AccountsPolicy", policy => policy.RequireRole("Accountant", "Accounts Manager", "Finance Manager"));
                opt.AddPolicy("CashierPolicy", policy => policy.RequireRole("Cashier"));
                opt.AddPolicy("ProcessPolicy", policy => policy.RequireRole("Document Controller-Processing", "Processing Manager", "Process Supervisor", "Admin"));
                opt.AddPolicy("OrderForwardPolicy", policy => policy.RequireRole("OrderForward", "HR Supervisor", "HR Manager", "Admin"));
                //opt.AddPolicy("CustomerReviewPolicy", policy => policy.RequireRole("Customer Review"));
                opt.AddPolicy("FeedbackPolicy", policy => policy.RequireRole("Candidate", "Customer Official", "Document Controller-Processing", "Admin", "Document Controller-Admin"));
                opt.AddPolicy("VisaEditPolicy", policy => policy.RequireRole("VisaEdit"));
                opt.AddPolicy("VisaViewPolicy", policy => policy.RequireRole("VisaView"));
            }); 

            return services;
        }
    }
}