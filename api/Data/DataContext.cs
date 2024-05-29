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
        public DbSet<CustomerReview> CustomerReviews {get; set;}
        public DbSet<CustomerReviewItem> CustomerReviewItems {get; set;}
        
        //order
        public DbSet<ContractReviewItemStddQ> ContractReviewItemStddQs {get; set;}
        public DbSet<ContractReview> ContractReviews {get; set;}
        public DbSet<ContractReviewItem> ContractReviewItems {get; set;}
        public DbSet<ContractReviewItemQ> ContractReviewItemQs {get; set;}
        public DbSet<JobDescription> JobDescriptions {get; set;}
        public DbSet<OrderForwardToHR> OrderForwardToHRs {get; set;}
        public DbSet<OrderForwardToAgent> OrderForwardToAgents {get; set;}
        public DbSet<OrderForwardCategory> OrderForwardCategories {get; set;}   
        public DbSet<OrderForwardCategoryOfficial> OrderForwardCategoryOfficials {get; set;}    
        public DbSet<Order> Orders {get; set;}
        public DbSet<OrderItem> OrderItems {get; set;}
        public DbSet<OrderAssessment> OrderAssessments {get; set;}
        public DbSet<OrderItemAssessment> orderItemAssessments {get; set;}
        public DbSet<OrderItemAssessmentQ> OrderItemAssessmentQs {get; set;}

        public DbSet<OrderAssessmentItem> OrderAssessmentItems {get; set;}
        public DbSet<OrderAssessmentItemQ> OrderAssessmentItemQs {get; set;}
        public DbSet<Remuneration> Remunerations {get; set;}
       
        //fINANCE
        public DbSet<COA> COAs {get; set;}
        public DbSet<FinanceVoucher> FinanceVouchers {get; set;}
        public DbSet<VoucherEntry> VoucherEntries {get; set;}
        public DbSet<Voucher> Vouchers {get; set;}
        public DbSet<VoucherAttachment> VoucherAttachments{get; set;}
        public DbSet<VoucherItem> VoucherItems {get; set;}
        
        //HR
        public DbSet<AssessmentQBank> AssessmentQBanks {get; set;}
        public DbSet<AssessmentStddQ> AssessmentStddQs {get; set;}
        public DbSet<AssessmentQStdd> AssessmentQStdds {get; set;}
        public DbSet<Candidate> Candidates{get; set;}
        public DbSet<UserAttachment> UserAttachments{get; set;}
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
        public DbSet<Qualification> Qualifications {get; set;}  
        public DbSet<SkillData> SkillDatas {get; set;}
        public DbSet<FeedbackStddQ> feedbackStddQs{get; set;}

        //Process
        public DbSet<Deployment> Deployments {get; set;}        //unable to delete this, as it throws Index error
        public DbSet<Process> Processes {get; set;}
        public DbSet<ProcessItem> ProcessItems {get; set;}
        public DbSet<Dep> Deps { get; set; }
        public DbSet<DepItem> DepItems {get; set;}
        public DbSet<DeployStatus> DeployStatuses {get; set;}

        //Tasks
        public DbSet<AppTask> Tasks {get; set;}
        public DbSet<TaskItem> TaskItems {get; set;}
        
        //UserHistory
        public DbSet<UserHistory> UserHistories {get; set;}
        public DbSet<UserHistoryItem> UserHistoryItems {get; set;}

        public DbSet<Entities.Messages.MessageComposeSource> MessageComposeSources {get; set;}

        public DbSet<Help> Helps {get; set;}
        public DbSet<HelpItem> HelpItems {get; set;}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            /*builder.Entity<Customer>()
                .HasMany(ur => ur.CustomerIndustries)
                .WithOne(u => u.Customer)
                .HasForeignKey(ur => ur.Id)
                .IsRequired();
            */
            builder.Entity<CustomerReview>().HasMany(x => x.CustomerReviewItems).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<CustomerReview>().HasIndex(x => x.CustomerId).IsUnique();
            builder.Entity<AssessmentQBank>().HasMany(x => x.AssessmentStddQs).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<AssessmentQBank>().HasIndex(x => x.ProfessionId).IsUnique();
            
            builder.Entity<OrderAssessment>().HasIndex(x => x.OrderId).IsUnique();
            builder.Entity<OrderAssessment>().HasMany(x => x.OrderAssessmentItems).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<OrderAssessmentItem>().HasIndex(x => x.OrderItemId).IsUnique();
            builder.Entity<OrderAssessmentItemQ>().HasIndex(x => new {x.OrderAssessmentItemId, x.Question}).IsUnique();

            builder.Entity<Customer>().HasIndex(x => new {x.CustomerName, x.City}).IsUnique();
            builder.Entity<CustomerOfficial>().HasIndex(x => new{x.CustomerId, x.OfficialName}).IsUnique();
            
            /*builder.Entity<Industry>()
                .HasMany(c => c.customerIndustries)
                .WithOne(u => u.Industry)
                //.HasForeignKey(ur => ur. .Id)
                .IsRequired();
            */

            builder.Entity<Profession>().HasIndex(x => x.ProfessionName).IsUnique();
            builder.Entity<Industry>().HasIndex(x => x.IndustryName).IsUnique();
            builder.Entity<Qualification>().HasIndex(x => x.QualificationName).IsUnique();
            
            builder.Entity<CVRef>().HasOne(o => o.Process);
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
            
            builder.Entity<ContractReviewItem>().HasIndex(x => x.OrderItemId).IsUnique();
            
            builder.Entity<ChecklistHR>()
                .HasMany(x => x.ChecklistHRItems).WithOne().OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AssessmentQStdd>().HasIndex(x => x.QuestionNo).IsUnique();
            builder.Entity<AssessmentQStdd>().HasIndex(x => x.Question).IsUnique();
            
            builder.Entity<Order>().HasOne(x => x.ContractReview).WithOne(e => e.Order)
                .HasForeignKey<ContractReview>(x => x.OrderId);
            
            builder.Entity<OrderForwardToHR>().HasIndex(x => new {x.OrderId, x.DateOnlyForwarded}).IsUnique();

            builder.Entity<ContractReview>().HasIndex(x => x.OrderId).IsUnique();
            builder.Entity<ContractReview>().HasMany(x => x.ContractReviewItems).WithOne().OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ContractReviewItem>().HasIndex(x => x.OrderItemId).IsUnique();

            builder.Entity<ChecklistHR>().HasIndex(x => new {x.OrderItemId, x.CandidateId}).IsUnique();
            builder.Entity<ChecklistHR>().HasMany(x => x.ChecklistHRItems).WithOne().OnDelete(DeleteBehavior.Cascade); 

            builder.Entity<CandidateAssessment>().HasMany(x => x.CandidateAssessmentItems).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<CandidateAssessment>().HasIndex(x => new { x.CandidateId, x.OrderItemId}).IsUnique();

            builder.Entity<AppTask>().HasMany(x => x.TaskItems).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<AppTask>().HasIndex(x => x.TaskType);
            //builder.Entity<AppTask>().HasIndex(x => new {x.CVRefId, x.TaskType})
                //.HasFilter("CVRefId is NOT ")     //provided by default
                //.IsUnique();

            builder.Entity<SelectionDecision>().HasIndex(x => x.CVRefId).IsUnique();
            //builder.Entity<SelectionDecision>().HasOne(x => x.Employment).WithOne().OnDelete(DeleteBehavior.Cascade);
            //builder.Entity<SelectionDecision>().HasOne(x => x.Dep).WithOne().OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Employment>().HasIndex(x => x.CVRefId).IsUnique();
            builder.Entity<Employment>().HasIndex(x => x.SelectionDecisionId).HasFilter("SelectionDecisionId is NOT NULL");

            builder.Entity<FinanceVoucher>().HasMany(x => x.VoucherEntries).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Process>().HasMany(x => x.ProcessItems).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Dep>().HasIndex(x => x.CVRefId).IsUnique();
            builder.Entity<Dep>().HasMany(x => x.DepItems).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<DepItem>().HasIndex(x => new {x.DepId, x.Sequence}).IsUnique();
            
            builder.Entity<Voucher>().HasMany(x => x.VoucherItems).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<VoucherAttachment>().HasIndex(x => new {x.FileName, x.VoucherId}).IsUnique();
            builder.Entity<COA>().HasIndex(i => i.AccountName).IsUnique();

            builder.Entity<UserHistory>().HasMany(x => x.UserHistoryItems).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<UserHistory>().HasIndex(x => x.CandidateId).HasFilter("CandidateId Is Null").IsUnique();
            
            
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
            
            builder.Entity<OrderForwardToAgent>().HasIndex(x => x.OrderId).IsUnique();
            
            builder.Entity<OrderForwardCategory>()
                .HasIndex(x => x.OrderItemId).IsUnique();
            
            builder.Entity<OrderForwardCategoryOfficial>()
                .HasIndex(x => new {x.OrderForwardCategoryId, 
                    x.DateOnlyForwarded, x.CustomerOfficialId}).IsUnique();
            
            builder.Entity<Help>().HasMany(x => x.HelpItems).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Help>().HasIndex(x => x.Topic).IsUnique();
            builder.Entity<HelpItem>().HasIndex(x => new {x.HelpId, x.Sequence}).IsUnique();
            
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