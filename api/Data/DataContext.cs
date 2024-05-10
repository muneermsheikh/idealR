using api.Entities;
using api.Entities.Admin;
using api.Entities.Admin.Client;
using api.Entities.Admin.Order;
using api.Entities.Finance;
using api.Entities.HR;
using api.Entities.Identity;
using api.Entities.Master;
using api.Entities.Messages;
using api.Entities.Process;
using api.Entities.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class DataContext : IdentityDbContext<AppUser, AppRole, int, 
        IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>, 
        IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        //Admin
        public DbSet<Employee> Employees {get; set;}
        public DbSet<Feedback> Feedbacks{get; set;}
        public DbSet<FeedbackItem> FeedbackItems {get; set;}
        public DbSet<Message> Messages {get; set;}
        public DbSet<UserLike> UserLikes {get; set;}

        //client
        public DbSet<AgencySpecialty> AgencySpecialties{get; set;}
        public DbSet<Customer> Customers {get; set;}
        public DbSet<CustomerIndustry> CustomerIndustries {get; set;}
        public DbSet<CustomerOfficial> CustomerOfficials {get; set;}
        //order
        public DbSet<ContractReviewItemStddQ> ContractReviewItemStddQs {get; set;}
        public DbSet<ContractReview> ContractReviews {get; set;}
        public DbSet<ContractReviewItem> ContractReviewItems {get; set;}
        public DbSet<ContractReviewItemQ> ContractReviewItemQs {get; set;}
        public DbSet<JobDescription> JobDescriptions {get; set;}
        public DbSet<DLForwardedToAgent> DLForwardedToAgents {get; set;}
        public DbSet<Order> Orders {get; set;}
        public DbSet<OrderItem> OrderItems {get; set;}
        public DbSet<OrderItemAssessment> orderItemAssessments {get; set;}
        public DbSet<OrderItemAssessmentQ> OrderItemAssessmentQs {get; set;}
        public DbSet<Remuneration> Remunerations {get; set;}
       
        //fINANCE
        public DbSet<COA> COAs {get; set;}
        public DbSet<FinanceVoucher> FinanceVouchers {get; set;}
        public DbSet<VoucherEntry> VoucherEntries {get; set;}
        
        //HR
        public DbSet<AssessmentQStdd> AssessmentQStdds {get; set;}
        public DbSet<Candidate> Candidates{get; set;}
        public DbSet<CandidateAssessment> CandidateAssessments{get; set;}
        public DbSet<CandidateAssessmentItem> CandidatesItemAssessments{get;}
        public DbSet<ChecklistHR> ChecklistHRs {get; set;}
        public DbSet<ChecklistHRItem> ChecklistHRItems{get;}
        public DbSet<CVRef> CVRefs {get; set;}
        public DbSet<Employment> Employments {get; set;}
        public DbSet<HRSkill> HRSkills {get; set;}
        public DbSet<OtherSkill> OtherSkills {get; set;}
        public DbSet<SelectionDecision> SelectionDecisions {get; set;}
        public DbSet<UserExp> UserExps {get; set;}
        public DbSet<UserPhone> UserPhones {get; set;}
        public DbSet<UserProfession> UserProfessions {get;set;}
        public DbSet<UserQualification> UserQualifications {get; set;}

        //Master
        public DbSet<CategoryAssessmentQBank> CategoryAssessmentQBanks {get;set;}
        public DbSet<ChecklistHRData> ChecklistHRDatas {get; set;}
        public DbSet<Industry> Industries{get; set;}
        public DbSet<UserLike> Likes {get; set;}
        public DbSet<Profession> Professions {get; set;}
        public DbSet<SkillData> SkillDatas {get; set;}
        public DbSet<FeedbackStddQ> feedbackStddQs{get; set;}

        //Process
        public DbSet<Deployment> Deployments {get; set;}
        public DbSet<DeployStatus> DeployStatuses {get; set;}

        //Tasks
        public DbSet<AppTask> Tasks {get; set;}
        public DbSet<TaskItem> TaskItems {get; set;}

        public DbSet<Entities.Messages.MessageComposeSource> MessageComposeSources {get; set;}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            /*builder.Entity<Customer>()
                .HasMany(ur => ur.CustomerIndustries)
                .WithOne(u => u.Customer)
                .HasForeignKey(ur => ur.Id)
                .IsRequired();
            */
            builder.Entity<OrderItemAssessment>().HasIndex(x => x.OrderItemId).IsUnique();
            builder.Entity<OrderItemAssessmentQ>().HasIndex(x => new {x.OrderItemAssessmentId, x.Question}).IsUnique();

            builder.Entity<Customer>().HasIndex(x => new {x.CustomerName, x.City}).IsUnique();
            builder.Entity<CustomerOfficial>().HasIndex(x => new{x.CustomerId, x.OfficialName}).IsUnique();
            
            /*builder.Entity<Industry>()
                .HasMany(c => c.customerIndustries)
                .WithOne(u => u.Industry)
                //.HasForeignKey(ur => ur. .Id)
                .IsRequired();
            */

            //builder.Entity<profession>().HasIndex(x => x.ProfessionName).IsUnique();

            builder.Entity<Profession>().HasIndex(x => x.ProfessionName).IsUnique();

            builder.Entity<CVRef>().HasMany(o => o.Deployments);
            builder.Entity<CVRef>().HasIndex(i => new {i.OrderItemId, i.CandidateId}).IsUnique();
            
            //builder.Entity<AgencySpecialty>().HasIndex(i => new {i.CustomerId, i.IndustryId, i.ProfessionId}).IsUnique();
            
            builder.Entity<OrderItem>().HasOne(x => x.JobDescription).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<OrderItem>().HasOne(x => x.Remuneration).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<OrderItem>().HasOne(x => x.JobDescription).WithOne()
                .HasForeignKey<JobDescription>(x => x.OrderItemId);
            builder.Entity<OrderItem>().HasOne(x => x.Remuneration).WithOne()
                .HasForeignKey<Remuneration>(x => x.OrderItemId);
            
            builder.Entity<OrderItem>().HasOne(x => x.ContractReviewItem)
                .WithOne().HasForeignKey<ContractReviewItem>(x => x.OrderItemId).OnDelete(DeleteBehavior.Cascade);
            
              builder.Entity<ChecklistHR>()
                .HasMany(x => x.ChecklistHRItems).WithOne().OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AssessmentQStdd>().HasIndex(x => x.QuestionNo).IsUnique();
            builder.Entity<AssessmentQStdd>().HasIndex(x => x.Question).IsUnique();
            
            builder.Entity<COA>().HasIndex(i => i.AccountName).IsUnique();
            
            builder.Entity<Order>().HasOne(x => x.ContractReview).WithOne(e => e.Order)
                .HasForeignKey<ContractReview>(x => x.OrderId);
            
            builder.Entity<DLForwardedToAgent>().HasIndex(x => new {x.CustomerOfficialId, x.DateForwarded, x.OrderItemId}).IsUnique();

            builder.Entity<ContractReview>().HasIndex(x => x.OrderId).IsUnique();
            builder.Entity<ContractReview>().HasMany(x => x.ContractReviewItems).WithOne().OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ContractReviewItem>().HasIndex(x => x.OrderItemId).IsUnique();

            builder.Entity<ChecklistHR>().HasIndex(x => new {x.OrderItemId, x.CandidateId}).IsUnique();
            builder.Entity<ChecklistHR>().HasMany(x => x.ChecklistHRItems).WithOne().OnDelete(DeleteBehavior.Cascade); 

            builder.Entity<CandidateAssessment>().HasMany(x => x.CandidateAssessmentItems);
            builder.Entity<CandidateAssessment>().HasIndex(x => new { x.CandidateId, x.OrderItemId}).IsUnique();

            builder.Entity<AppTask>().HasMany(x => x.TaskItems).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<AppTask>().HasIndex(x => x.TaskType);

        //Identity
           builder.Entity<AppUser>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();

            builder.Entity<AppRole>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();
                
            builder.Entity<UserLike>()
                .HasKey(k => new {k.SourceUserId, k.TargetUserId});
            
            builder.Entity<UserLike>()
                .HasOne(s => s.SourceUser)
                .WithMany(l => l.LikedUsers)
                .HasForeignKey(s => s.SourceUserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<UserLike>()
                .HasOne(s => s.TargetUser)
                .WithMany(l => l.LikedByUsers)
                .HasForeignKey(s => s.TargetUserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            
            /*builder.Entity<Message>()
                .HasOne(s => s.Recipient)
                .WithMany(m => m.MessagesReceived)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.Entity<Message>()
                .HasOne(s => s.Sender)
                .WithMany(m => m.MessagesSent)
                .OnDelete(DeleteBehavior.Restrict);
            */
            
            
        }
    }
}