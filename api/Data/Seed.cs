using System.Text.Json;
using api.Entities.Admin;
using api.Entities.Admin.Client;
using api.Entities.Admin.Order;
using api.Entities.Deployments;
using api.Entities.Finance;
using api.Entities.HR;
using api.Entities.Identity;
using api.Entities.Master;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace api.Data
{
    public class Seed
    {
        public static async Task SeedUsers (UserManager<AppUser> userManager, 
            RoleManager<AppRole> roleManager, DataContext context) 
        {
            //identity
            if (!await userManager.Users.AnyAsync()) 
            {
                var userData = await File.ReadAllTextAsync("Data/SeedData/UserSeedData.json");

                var options = new JsonSerializerOptions{PropertyNameCaseInsensitive = true};

                var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

                var roles = new List<AppRole> {
                    new() {Name = "Candidate"},
                    new() {Name = "Employee"},
                    new() {Name = "Client"},
                    new() {Name = "Admin"},
                    new() {Name = "HR Manager"},
                    new() {Name = "HR Supervisor"},
                    new() {Name = "HR Executive"},
                    new() {Name = "Asst HR Executive"},
                    new() {Name = "Accounts Manager"},
                    new() {Name = "Finance Manager"},
                    new() {Name = "Cashier"},
                    new() {Name = "Accountant"},
                    new() {Name = "Document Controller-Admin"},
                    new() {Name = "Document Controller-Processing"},
                    new() {Name = "Processing Manager"},
                    new() {Name = "Processing Supervisor"},
                    new() {Name = "Processing Executive"},
                    new() {Name = "Admin Manager"},
                    new() {Name = "Receptionist"},
                    new() {Name = "Marketing Manager"},
                    new() {Name = "Design Assessment Questions"},
                    new() {Name = "Register Selections and Rejections"},
                    new() {Name = "Approve release of documents"},
                    new() {Name = "Order"},
                    new() {Name = "Contract Review"},
                    new() {Name = "Customer Official"},
                    new() {Name = "OrderForward"},
                    new() {Name = "OrderCreate"},
                    new() {Name = "Process Executive"},
                    new() {Name = "Process Supervisor"},
                    new() {Name = "Selection"},
                    new() {Name = "CVRefer"}
                };

                foreach(var role in roles) {
                    await roleManager.CreateAsync(role);
                }
                
                int counter  =4;
                foreach(var user in users) {

                    user.UserName = user.UserName.ToLower();
                    await userManager.CreateAsync(user, "Pa$$w0rd");
                    await userManager.AddToRoleAsync(user, roles[counter++].Name);
                }

                var admin = new AppUser{
                    UserName = "Admin"
                };

                await userManager.CreateAsync(admin, "Pa$$w0rd");
                await userManager.AddToRolesAsync(admin, new[] {"Admin"});
            }   
            
            //recruitment entities
            

            var UserListCandidates = new List<AppUser>();
            var UserListEmployees = new List<AppUser>();
            var UserListOfficials = new List<AppUser>();

            if(!await context.MessageComposeSources.AnyAsync()) {
                var data = await File.ReadAllTextAsync("Data/SeedData/FlightSeedData.json");
                _ = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dbData = JsonSerializer.Deserialize<List<FlightDetail>>(data);

                foreach(var item in dbData) 
                {
                    context.FlightDetails.Add(item);
                }
             }


            if(!await context.MessageComposeSources.AnyAsync()) {
                var data = await File.ReadAllTextAsync("Data/SeedData/MessageComposeSourceSeedData.json");
                _ = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dbData = JsonSerializer.Deserialize<List<Entities.Messages.MessageComposeSource>>(data);

                foreach(var item in dbData) 
                {
                    context.MessageComposeSources.Add(item);
                }
             }

            if(!await context.ChecklistHRDatas.AnyAsync()) {
                var data = await File.ReadAllTextAsync("Data/SeedData/ChecklistHRDataSeedData.json");
                _ = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dbData = JsonSerializer.Deserialize<List<ChecklistHRData>>(data);

                foreach(var item in dbData) 
                {
                    context.ChecklistHRDatas.Add(item);
                }
            }

            if(!await context.ChecklistHRs.AnyAsync()) {
                var data = await File.ReadAllTextAsync("Data/SeedData/ChecklistSeedData.json");
                _ = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dbData = JsonSerializer.Deserialize<List<ChecklistHR>>(data);

                foreach(var item in dbData) 
                {
                    context.ChecklistHRs.Add(item);
                }
            }
        

            if (!await context.Qualifications.AnyAsync()) {
                var data = await File.ReadAllTextAsync("Data/SeedData/QualificationSeedData.json");
                _ = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dbData = JsonSerializer.Deserialize<List<Qualification>>(data);
                foreach(var item in dbData) 
                {
                    context.Qualifications.Add(item);
                }
            }

            if (!await context.COAs.AnyAsync()) {
                var data = await File.ReadAllTextAsync("Data/SeedData/COASeedData.json");
                _ = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dbData = JsonSerializer.Deserialize<List<COA>>(data);
                foreach(var item in dbData) 
                {
                    context.COAs.Add(item);
                }
            }

            if(!await context.FinanceVouchers.AnyAsync()) {
                var data = await File.ReadAllTextAsync("Data/SeedData/FinanceVoucherSeedData.json");
                _ = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dbData = JsonSerializer.Deserialize<List<FinanceVoucher>>(data);
                foreach(var item in dbData) 
                {
                    context.FinanceVouchers.Add(item);
                }
            }

            if(!await context.Customers.AnyAsync()) {
                var data = await File.ReadAllTextAsync("Data/SeedData/CustomerSeedData.json");
                _ = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dbData = JsonSerializer.Deserialize<List<Customer>>(data);
                foreach(var item in dbData) 
                {
                    var user = new AppUser{
                        UserName = item.CustomerOfficials.FirstOrDefault().UserName,
                        KnownAs = item.CustomerOfficials.FirstOrDefault().KnownAs,
                        Gender = item.CustomerOfficials.FirstOrDefault().Gender,
                        LastActive = DateTime.UtcNow,
                        Created = DateTime.UtcNow,
                        City = item.City
                    };
                    UserListOfficials.Add(user);
                    context.Customers.Add(item);
                }
            }

            if(!await context.Employees.AnyAsync()) {
                var data = await File.ReadAllTextAsync("Data/SeedData/EmployeeSeedData.json");
                _ = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dbData = JsonSerializer.Deserialize<List<Employee>>(data);
                foreach(var item in dbData) 
                {
                    var user = new AppUser{
                        UserName = item.UserName,
                        KnownAs = item.KnownAs,
                        Gender = item.Gender,
                        LastActive = DateTime.UtcNow,
                        Created = DateTime.UtcNow,
                        DateOfBirth = item.DateOfBirth,
                        City = item.City,
                    };
                    UserListEmployees.Add(user);
                    context.Employees.Add(item);
                }
            }

            if(!await context.Professions.AnyAsync()) {
                var data = await File.ReadAllTextAsync("Data/SeedData/ProfessionSeedData.json");
                _ = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dbData = JsonSerializer.Deserialize<List<Profession>>(data);
                foreach(var item in dbData) 
                {
                    context.Professions.Add(item);
                }
            }
            
            if(!await context.ContractReviewItemStddQs.AnyAsync()) {
                var data = await File.ReadAllTextAsync("Data/SeedData/ContractReviewItemStddQSeedData.json");
                _ = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dbData = JsonSerializer.Deserialize<List<ContractReviewItemStddQ>>(data);
                foreach(var item in dbData) 
                {
                    context.ContractReviewItemStddQs.Add(item);
                }
            }
            

            if(!await context.ContractReviews.AnyAsync()) {
                var data = await File.ReadAllTextAsync("Data/SeedData/ContractReviewSeedData.json");
                _ = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dbData = JsonSerializer.Deserialize<List<ContractReview>>(data);
                foreach(var item in dbData) 
                {
                    context.ContractReviews.Add(item);
                }
            }
            
            if(!await context.Industries.AnyAsync()) {
                var data = await File.ReadAllTextAsync("Data/SeedData/IndustrySeedData.json");
                _ = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dbData = JsonSerializer.Deserialize<List<Industry>>(data);
                foreach(var item in dbData) 
                {
                    context.Industries.Add(item);
                }
            }

            if(!await context.AssessmentQStdds.AnyAsync()) {
                var data = await File.ReadAllTextAsync("Data/SeedData/AssessmentQStddSeedData.json");
                _ = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dbData = JsonSerializer.Deserialize<List<AssessmentQStdd>>(data);
                foreach(var item in dbData) 
                {
                    context.AssessmentQStdds.Add(item);
                }
            }

            
             if(!await context.Orders.AnyAsync()) {
                var data = await File.ReadAllTextAsync("Data/SeedData/OrderSeedData.json");
                _ = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dbData = JsonSerializer.Deserialize<List<Order>>(data);
                foreach(var item in dbData) 
                {
                    context.Orders.Add(item);
                }
            }
            
              
             if(!await context.Remunerations.AnyAsync()) {
                var data = await File.ReadAllTextAsync("Data/SeedData/RemunerationSeedData.json");
                _ = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dbData = JsonSerializer.Deserialize<List<Remuneration>>(data);
                foreach(var item in dbData) 
                {
                    context.Remunerations.Add(item);
                }
                await context.SaveChangesAsync();
            }
              
             if(!await context.JobDescriptions.AnyAsync()) {
                var data = await File.ReadAllTextAsync("Data/SeedData/JobDescriptionSeedData.json");
                _ = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dbData = JsonSerializer.Deserialize<List<JobDescription>>(data);
                foreach(var item in dbData) 
                {
                    context.JobDescriptions.Add(item);
                }
                await context.SaveChangesAsync();
            }

            if(!await context.Candidates.AnyAsync()) {

                var data = await File.ReadAllTextAsync("Data/SeedData/CandidateSeedData.json");
                _ = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dbData = JsonSerializer.Deserialize<List<Candidate>>(data);
                foreach(var item in dbData) 
                {
                    var user = new AppUser{
                        UserName = item.UserName,
                        KnownAs = item.KnownAs,
                        Gender = item.Gender,
                        LastActive = DateTime.UtcNow,
                        Created = DateTime.UtcNow,
                        DateOfBirth = (DateTime)item.DOB,
                        City = item.City,
                    };
                    UserListCandidates.Add(user);
                    context.Candidates.Add(item);
                }
            }

            if(!await context.DeployStatuses.AnyAsync()) {
                var data = await File.ReadAllTextAsync("Data/SeedData/DeployStatusSeedData.json");
                _ = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dbData = JsonSerializer.Deserialize<List<DeployStatus>>(data);
                foreach(var item in dbData) 
                {
                    context.DeployStatuses.Add(item);
                }
                await context.SaveChangesAsync();
            }

            
            if(!await context.CVRefs.AnyAsync()) {
                var data = await File.ReadAllTextAsync("Data/SeedData/CVRefSeedData.json");
                _ = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dbData = JsonSerializer.Deserialize<List<CVRef>>(data);
                foreach(var item in dbData) 
                {
                    context.CVRefs.Add(item);
                }
                await context.SaveChangesAsync();
            }
        
            
            if(!await context.SelectionDecisions.AnyAsync()) {
                var data = await File.ReadAllTextAsync("Data/SeedData/SelectionDecisionSeedData.json");
                _ = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dbData = JsonSerializer.Deserialize<List<SelectionDecision>>(data);
                foreach(var item in dbData) 
                {
                    context.SelectionDecisions.Add(item);
                }
                await context.SaveChangesAsync();
            }
            
            if(!await context.FeedbackQs.AnyAsync()) {
                var data = await File.ReadAllTextAsync("Data/SeedData/FeedbackStddQSeedData.json");
                _ = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dbData = JsonSerializer.Deserialize<List<FeedbackQ>>(data);
                foreach(var item in dbData) 
                {
                    context.FeedbackQs.Add(item);
                }
                await context.SaveChangesAsync();
            }


            if(!await context.CustomerFeedbacks.AnyAsync()) {
                var data = await File.ReadAllTextAsync("Data/SeedData/FeedbackSeedData.json");
                _ = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dbData = JsonSerializer.Deserialize<List<CustomerFeedback>>(data);
                foreach(var item in dbData) 
                {
                    context.CustomerFeedbacks.Add(item);
                }
                await context.SaveChangesAsync();
            }

            
            if(!await context.Deps.AnyAsync()) {
                var data = await File.ReadAllTextAsync("Data/SeedData/DeploySeedData.json");
                _ = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dbData = JsonSerializer.Deserialize<List<Dep>>(data);
                foreach(var item in dbData) 
                {
                    context.Deps.Add(item);
                }
                await context.SaveChangesAsync();
            }
            
            //if(context.ChangeTracker.HasChanges())  await context.SaveChangesAsync();

            foreach(var user in UserListCandidates) {
                var result=await userManager.CreateAsync(user, "Pa$$w0rd");
                if(result.Succeeded){
                    var roleresult=await userManager.AddToRoleAsync(user, "Candidate");
                    if(roleresult.Succeeded) {
                        var dbObj = await context.Candidates.Where(x => x.UserName.ToLower()==user.UserName.ToLower())
                            .FirstOrDefaultAsync();
                        if(dbObj != null) {
                            dbObj.AppUserId=user.Id;
                            context.Entry(dbObj).State = EntityState.Modified;
                        }
                    }
                }
            }

            foreach(var user in UserListEmployees) {
                var result=await userManager.CreateAsync(user, "Pa$$w0rd");
                if(result.Succeeded) {
                    var roleresult=await userManager.AddToRoleAsync(user, "Employee");
                    if(roleresult.Succeeded) {
                        var dbObj = await context.Employees.Where(x => x.UserName==user.UserName).FirstOrDefaultAsync();
                        if(dbObj != null) {
                            dbObj.AppUserId=user.Id;
                            context.Entry(dbObj).State = EntityState.Modified;
                        }
                    }
                }
            }

            foreach(var user in UserListOfficials) {
                var result=await userManager.CreateAsync(user, "Pa$$w0rd");
                if(result.Succeeded) {
                    var roleresult=await userManager.AddToRoleAsync(user, "Client");
                    if(roleresult.Succeeded) {
                        var dbObj = await context.CustomerOfficials.Where(x => x.UserName==user.UserName).FirstOrDefaultAsync();
                        if(dbObj != null) {
                            dbObj.AppUserId=user.Id;
                            context.Entry(dbObj).State = EntityState.Modified;
                        }
                    }
                }
                
            }

            if(context.ChangeTracker.HasChanges()) await context.SaveChangesAsync();
            
        }
    }
}