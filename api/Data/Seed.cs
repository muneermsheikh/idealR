using System.Text.Json;
using api.Entities.Admin;
using api.Entities.Admin.Client;
using api.Entities.Admin.Order;
using api.Entities.Deployments;
using api.Entities.Finance;
using api.Entities.HR;
using api.Entities.Identity;
using api.Entities.Master;
using api.Entities.Tasks;
using api.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class Seed
    {
            
        public static async Task SeedUsers (UserManager<AppUser> userManager, 
            RoleManager<AppRole> roleManager, DataContext context) 
        {
            Random random = new();
            string[] usernames = {"Sanjay", "Kadir", "Dutta", "Munir" };
            
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
                await userManager.AddToRoleAsync(admin, "Admin");//await userManager.AddToRolesAsync(admin, new[] {"Admin"});
            }   
            
            //recruitment entities
            if(!await context.AttendanceStatuses.AnyAsync()) {
                var data = await File.ReadAllTextAsync("Data/SeedData/AttendanceStatusSeedData.json");
                _ = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dbData = JsonSerializer.Deserialize<List<AttendanceStatus>>(data);

                foreach(var item in dbData) 
                {
                    context.AttendanceStatuses.Add(item);
                }
             }

            if(!await context.SkillDatas.AnyAsync()) {
                var data = await File.ReadAllTextAsync("Data/SeedData/SkillSeedData.json");
                _ = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dbData = JsonSerializer.Deserialize<List<SkillData>>(data);

                foreach(var item in dbData) 
                {
                    context.SkillDatas.Add(item);
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
            await context.SaveChangesAsync();

            if(!await context.FinanceVouchers.AnyAsync()) {
                var data = await File.ReadAllTextAsync("Data/SeedData/FinanceVoucherSeedData.json");
                _ = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dbData = JsonSerializer.Deserialize<List<FinanceVoucher>>(data);
                foreach(var item in dbData) 
                {
                    var coa = await context.COAs.Where(x => x.AccountName==item.AccountName).FirstOrDefaultAsync();
                    if(coa != null) {
                        item.CoaId=coa.Id;
                        item.AccountName=coa.AccountName;
                        foreach(var trans in item.VoucherEntries) {
                            var coatrans = await context.COAs.Where(x => x.AccountName==trans.AccountName).FirstOrDefaultAsync();
                            if(coatrans != null) {
                                trans.CoaId=coatrans.Id;
                            }
                        }
                    }
                    context.FinanceVouchers.Add(item);
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
            
            var maxProfId=Convert.ToInt32(await context.Professions.MaxAsync(x => (int?)x.Id));

            if(!await context.Industries.AnyAsync()) {
                var data = await File.ReadAllTextAsync("Data/SeedData/IndustrySeedData.json");
                _ = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dbData = JsonSerializer.Deserialize<List<Industry>>(data);
                foreach(var item in dbData) 
                {
                    context.Industries.Add(item);
                }
            }
            var maxIndId = Convert.ToInt32(await context.Industries.MaxAsync(x => (int?)x.Id) ?? 1) ;

            if(!await context.Customers.AnyAsync()) {
                var data = await File.ReadAllTextAsync("Data/SeedData/CustomerSeedData.json");
                _ = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dbData = JsonSerializer.Deserialize<List<Customer>>(data);
                foreach(var item in dbData) 
                {
                    foreach(var off in item.CustomerOfficials) {
                        var user = new AppUser{
                            UserName = off.UserName,
                            KnownAs = off.KnownAs,
                            Gender = off.Gender,
                            LastActive = DateTime.UtcNow,
                            Created = DateTime.UtcNow,
                            City = item.City
                        };
                        var result = await userManager.CreateAsync(user, "Pa$$w0rd");
                        if(result.Succeeded) {
                            var roleResult = await userManager.AddToRoleAsync(user, "Client");
                            if(roleResult.Succeeded) off.AppUserId=user.Id;
                        }
                    }
                    
                    /*foreach(var sp in item.AgencySpecialties) {
                        sp.ProfessionId=random.Next(1, maxProfId);
                        sp.IndustryId=random.Next(1,maxIndId);

                        sp.SpecialtyName=await context.GetProfessionAndIndustryNameFromId(sp.ProfessionId, (int)sp.IndustryId);
                    }
                    */
                    context.Customers.Add(item);
                }
            }
            var maxCustomerId = Convert.ToInt32(await context.Customers.MaxAsync(x => (int?)x.Id));
 
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
                    
                    var result = await userManager.CreateAsync(user, "Pa$$w0rd");
                    if(result.Succeeded) {
                        var roleResult = await userManager.AddToRoleAsync(user, "Employee");
                        if(roleResult.Succeeded) item.AppUserId=user.Id;
                    }

                    if(maxProfId > 0) {
                        foreach(var sk in item.HRSkills) {
                            sk.ProfessionId=random.Next(1, maxProfId);
                            sk.ProfessionName = await context.GetProfessionNameFromId(sk.ProfessionId);
                            sk.IndustryId=random.Next(1,maxIndId);
                            sk.SkillLevelName= random.Next(1,2)==1 ? "Proficient" : "Good";
                            sk.IsMain=true;
                        }
                    }

                   context.Employees.Add(item);

                }
                await context.SaveChangesAsync();

            }
            
            var maxEmpId=Convert.ToInt32(await context.Employees.MaxAsync(x => (int?)x.Id));
            
            if(!await context.ContractReviewItemStddQs.AnyAsync()) {
                var data = await File.ReadAllTextAsync("Data/SeedData/ContractReviewItemStddQSeedData.json");
                _ = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dbData = JsonSerializer.Deserialize<List<ContractReviewItemStddQ>>(data);
                foreach(var item in dbData) 
                {
                    context.ContractReviewItemStddQs.Add(item);
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
            
            if(!await context.DeployStatuses.AnyAsync()) {
                var data = await File.ReadAllTextAsync("Data/SeedData/DeployStatusSeedData.json");
                _ = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dbData = JsonSerializer.Deserialize<List<DeployStatus>>(data);
                foreach(var item in dbData) 
                {
                    context.DeployStatuses.Add(item);
                }
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
            
            if(!await context.FlightDetails.AnyAsync()) {
                var data = await File.ReadAllTextAsync("Data/SeedData/FlightSeedData.json");
                _ = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dbData = JsonSerializer.Deserialize<List<FlightDetail>>(data);
                foreach(var item in dbData) 
                {
                    context.FlightDetails.Add(item);
                }
            }
            
            if(context.ChangeTracker.HasChanges()) await context.SaveChangesAsync();

       //dependent entities   
       
            if(!await context.CustomerFeedbacks.AnyAsync()) {
                
                var feedbackitems = new List<FeedbackItem>{
                    new() {FeedbackGroup= "documentation", QuestionNo = 1, 
                        Question = "How satisfied you are with our documentations",
                        Prompt1="Very Satisfied", Prompt2="Satisfied", Prompt3="Can Do Better`", 
                        Prompt4="Not Satisfied", Response = ""},
                    new() {FeedbackGroup= "documentation", QuestionNo = 2, 
                        Question = "How satisfied are you with our response to your queries",
                        Prompt1="Very Satisfied", Prompt2="Satisfied", Prompt3="Can Do Better", 
                        Prompt4="Not Satisfied", Response = ""},
                    new() {FeedbackGroup= "Response Time to Queries", QuestionNo = 3, 
                        Question = "How fast was our response time to your queries",
                        Prompt1="Very Fast", Prompt2="Fast", Prompt3="Can Do Better", Prompt4="Slow",
                        Response = ""},
                    new() {FeedbackGroup= "Interview Arrangements", QuestionNo = 4, 
                        Question = "How good were our interview arrangements",
                        Prompt1="Very Good", Prompt2="Good", Prompt3="Can Do Better", Prompt4="Bad",
                        Response = ""},
                    new() {FeedbackGroup= "Candidate Skills On average", QuestionNo = 5, 
                        Question = "How satisfied are you with the skills of candidates presented to you",
                        Prompt1="Highly Skilled", Prompt2="Skilled", Prompt3="Acceptable", 
                        Prompt4="Not Skilled",Response = ""},
                    new() {FeedbackGroup= "Deployment Process Time", QuestionNo = 6, 
                        Question = "How fast did we deploy your selected candidates",
                        Prompt1="Very Quick", Prompt2="Quick", Prompt3="Should Do Better", Prompt4="Slow",
                        Response = ""},                    
                    new() {FeedbackGroup= "Marketing Services", QuestionNo = 7, 
                        Question = "How satisfied are you with the services of our marketing personnel",
                        Prompt1="Very Satisfied", Prompt2="Satisfied", Prompt3="Should Do Better", 
                        Prompt4="Not Satisfied", Response = ""},
                    new() {FeedbackGroup= "Overall Services", QuestionNo = 8, 
                        Question = "How satisfied are you with our overall services",
                        Prompt1="Very Satisfied", Prompt2="Satisfied", Prompt3="Should Do Better", 
                        Prompt4="Not Satisfied", Response = ""},
                    new() {FeedbackGroup= "Recommendation to others", QuestionNo = 8, 
                        Question = "Would you recommend us to your other associates for availing recruitment services from us?",
                        Prompt1="Yes", Prompt2="No",Response = ""}
                };

                int intFeedbk = 1000;
                var custfeedbacks = await(from cust in context.Customers
                    join off in context.CustomerOfficials on cust.Id equals off.CustomerId where off.Divn=="HR"
                    select new CustomerFeedback{
                        CustomerId = cust.Id, CustomerName=cust.CustomerName, City = cust.City,
                        Country=cust.Country, OfficialName = off.OfficialName, OfficialUsername=off.UserName,
                        Designation=off.Designation, Email=off.Email, PhoneNo=off.PhoneNo, DateIssued = 
                            new DateTime( random.Next(2020,2022), random.Next(1,11), random.Next(1,28))}
                    ).ToListAsync();
                    
                foreach(var item in custfeedbacks) 
                {
                    item.FeedbackNo=intFeedbk++;
                    item.FeedbackItems=feedbackitems;
                    context.CustomerFeedbacks.Add(item);
                }

                await context.SaveChangesAsync();
            }

            var IncludeTasks=false;
            if(!await context.Tasks.AnyAsync()) {
                IncludeTasks=true;
                var dbData = await context.CustomerFeedbacks.OrderBy(x => x.DateIssued).ToListAsync();

                foreach(var item in dbData) {
                    var usernameassignedto=usernames[random.Next(0,3)];
                    var loggedinusername = usernames[random.Next(0,3)];

                    var task = new AppTask{TaskType="CustomerFeedbackResponse", TaskDate=item.DateIssued,
                        AssignedByUsername=loggedinusername, AssignedToUsername=usernameassignedto,
                        TaskDescription="Follow up with " + item.CustomerName + " for their response to Feedback Quetionnaire dated " + 
                        string.Format("{0: dd-MMMM-yyyy}", item.DateIssued), CompleteBy = item.DateIssued.AddDays(7) };

                    context.Entry(task).State = EntityState.Added;
                }

                await context.SaveChangesAsync();
                var tasks=await context.Tasks.Where(x => x.TaskType == "CustomerFeedbackResponse").ToListAsync();
                
                foreach(var t in tasks) {
                    t.TaskItems=new List<TaskItem>(){new(){AppTaskId=t.Id, TransactionDate=t.TaskDate, TaskStatus="Not Started", 
                        TaskItemDescription = "follow-up for feedback questionnaire", NextFollowupByName=t.AssignedToUsername, 
                        NextFollowupOn= t.TaskDate.AddDays(2), UserName= t.AssignedByUsername }};
                    context.Entry(t).State=EntityState.Modified;
                }
            }
          
            if(!await context.Candidates.AnyAsync()) {
                maxProfId=Convert.ToInt32(await context.Professions.MaxAsync(x => (int?)x.Id));
                var nextAppNo = Convert.ToInt32(await context.Candidates.MaxAsync(x => (int?) x.ApplicationNo) ?? 1000);
                
                var data = await File.ReadAllTextAsync("Data/SeedData/CandidateJsonSeed.json");
                _ = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dbData = JsonSerializer.Deserialize<List<Candidate>>(data);
                foreach(var item in dbData) 
                {
                    item.AadharNo = Convert.ToString(random.Next(12341234, 87658765)) + Convert.ToString(random.Next(1234,8765));
                    item.PpNo = (char)random.Next(65,90) + Convert.ToString(random.Next(123412, 87658765));
                    var Profs = new List<UserProfession> {
                        new() {ProfessionId=random.Next(1, maxProfId), IsMain=true}, 
                        new() {ProfessionId=random.Next(1, maxProfId)}};
                    var PhoneNos = new List<UserPhone> {new() 
                        {MobileNo=Convert.ToString(random.Next(67111111, 88882222)) + Convert.ToString(random.Next(11,99)),
                            IsMain=true, IsValid=true}, new() 
                        {MobileNo=Convert.ToString(random.Next(67111111, 888881111))+ Convert.ToString(random.Next(11,99)),
                            IsMain=true, IsValid=true}};
                    item.Created = DateTime.Now.AddYears(-5);
                    item.LastActive = DateTime.Now.AddMonths(-5);
                    item.DOB=DateTime.Now.AddYears(-random.Next(18,40)).AddMonths(-2);
                    item.NotificationDesired = Convert.ToBoolean(random.Next(0,1));
                    item.ApplicationNo = ++nextAppNo;

                    var user = new AppUser{
                        UserName = item.Username,
                        KnownAs = item.KnownAs,
                        Gender = item.Gender,
                        LastActive = DateTime.UtcNow,
                        Created = DateTime.UtcNow,
                        DateOfBirth = (DateTime)item.DOB,
                        City = item.City,
                    };
                    
                    var result = await userManager.CreateAsync(user, "Pa$$w0rd");
                    if(result.Succeeded) {
                        var roleresult=await userManager.AddToRoleAsync(user, "Candidate");
                        if(roleresult.Succeeded) item.AppUserId=user.Id;
                    }

                    context.Candidates.Add(item);
                }

                await context.SaveChangesAsync();
            }

            var maxCandId = Convert.ToInt32(await context.Candidates.MaxAsync(x => (int?)x.Id));
            var candidateIds = await context.Candidates.Select(x => x.Id).ToListAsync();
         
            await context.SaveChangesAsync();

            if(!await context.Orders.AnyAsync()) {
                var nextOrderNo = Convert.ToInt32(await context.Orders.MaxAsync(x => (int?) x.OrderNo) ?? 1000);
                var reviewQs = await context.ContractReviewItemQs.OrderBy(x => x.SrNo).ToListAsync();
                foreach(var rvw in reviewQs) {rvw.Response=true;}
                //var hrExecNames = await context.Employees.Where(x => x.Department=="HR").Select(x => x.UserName).ToListAsync();
                //var ctExecNames = hrExecNames.Count;
                var data = await File.ReadAllTextAsync("Data/SeedData/OrderAndOrderItemSeedData.json");
                _ = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dbData = JsonSerializer.Deserialize<List<Order>>(data);
                foreach(var order in dbData) 
                {
                    var reviewItems = new List<ContractReviewItem>();
                    order.CustomerId=random.Next(1, maxCustomerId);
                    order.CompleteBy=order.OrderDate.AddDays(2);
                    order.OrderNo=nextOrderNo++;
                    order.SalesmanId=random.Next(1, maxEmpId);
                    order.ProjectManagerId=random.Next(1, maxEmpId);

                    foreach(var item in order.OrderItems) {
                        item.ProfessionId=random.Next(1, maxProfId);
                    }
                    context.Orders.Add(order);
                }

                await context.SaveChangesAsync();
            }
            var items = await context.OrderItems.Select(x => new {x.Id,x.ProfessionId}).ToListAsync();
            var maxOrderItemId = Convert.ToInt32(await context.OrderItems.MaxAsync(x => (int?)x.Id));

            if(!await context.Remunerations.AnyAsync()) {
        
                foreach(var item in items) {
                    string profName = await context.GetProfessionNameFromId(item.ProfessionId);
                    int intSalMin = profName.Contains("Engineer") 
                        ? 4000 : profName.Contains("Supervisor") 
                        ? 2500 : profName.Contains("Technician") 
                        ? 1800 : 1200;
                    int intSalMax = intSalMin + 500;
                    
                    var remuneration = new Remuneration{
                        OrderItemId=item.Id, WorkHours= 8, SalaryCurrency= "SAR", SalaryMin = intSalMin,
                        SalaryMax = intSalMax, ContractPeriodInMonths = 24, HousingProvidedFree = false, 
                        HousingAllowance = Convert.ToInt32(intSalMin > 3500 ? 0.25*intSalMin: 0), 
                        HousingNotProvided = false, FoodProvidedFree = intSalMin < 4000, 
                        FoodAllowance = 0, FoodNotProvided = intSalMin >= 4000, 
                        TransportAllowance= intSalMin >=4000 ? Convert.ToInt32(0.1*intSalMin) : 0, 
                        TransportProvidedFree = intSalMin<4000, TransportNotProvided=intSalMin >=4000,
                        OtherAllowance = 0, LeavePerYearInDays= 28, 
                        LeaveAirfareEntitlementAfterMonths = intSalMin >=4000 ? 12: 24
                    };
                    
                    context.Remunerations.Add(remuneration);
                }
            }
              
            if(!await context.JobDescriptions.AnyAsync()) {
      
                foreach(var item in items) {
                    var cat = await context.GetProfessionIdFromOrderItemId(item.Id);
                    var jd = new JobDescription{
                        OrderItemId = item.Id, 
                        JobDescInBrief = "job desc " + (char)random.Next(65, 90) + " for " + cat, 
                        QualificationDesired = "Qualification desired three " + (char)random.Next(65,90) + " for " + cat, 
                        ExpDesiredMin = random.Next(1,5), ExpDesiredMax = random.Next(2,10), 
                        MinAge = random.Next(18,25), MaxAge = random.Next(25,45) };

                    context.JobDescriptions.Add(jd);
                }
                await context.SaveChangesAsync();
            }
   
            if(!await context.ContractReviews.AnyAsync()) {

                var reviews= await context.Orders
                    .Select(x => new ContractReview{CustomerId=x.CustomerId, 
                        CustomerName=x.Customer.CustomerName, OrderId=x.Id, OrderNo=x.OrderNo,
                        OrderDate=x.OrderDate,  ReviewedOn=x.OrderDate.AddDays(1),
                        ReviewedByName="Admin",
                        HrExecAssignedToUsername=random.Next(1,2) == 1 ? "Kadir" : "Munir", ReviewStatus="Accepted"})
                    .ToListAsync();

                var orderitems= await context.OrderItems.OrderBy(x => x.OrderId).ToListAsync();
                
                var reviewItems=new List<ContractReviewItem>();
                var review=new ContractReview();
                var reviewItemQs = new List<ContractReviewItemQ>{
                    new() {SrNo=1, IsMandatoryTrue=true, ReviewParameter= "Salary Offered Feasible", Response=true,
                        ResponseText="", IsResponseBoolean=true, Remarks="" },
                    new() {SrNo=2, IsMandatoryTrue=true, ReviewParameter= "Technically Feasible", Response=true,
                        ResponseText="", IsResponseBoolean=true, Remarks="" },
                    new() {SrNo=3, IsMandatoryTrue=true, ReviewParameter= "Commercially Feasible", Response=true,
                        ResponseText="", IsResponseBoolean=true, Remarks="" },
                    new() {SrNo=4, IsMandatoryTrue=true, ReviewParameter= "Logistically Feasible", Response=true,
                        ResponseText="", IsResponseBoolean=true, Remarks="" },
                    new() {SrNo=5, IsMandatoryTrue=true, ReviewParameter= "Visa Available", Response=true,
                        ResponseText="", IsResponseBoolean=true, Remarks="" },
                    new() {SrNo=6, IsMandatoryTrue=true, ReviewParameter= "Documentation will be available", Response=true,
                        ResponseText="", IsResponseBoolean=true, Remarks="" },
                    new() {SrNo=7, IsMandatoryTrue=true, ReviewParameter= "Commercially Feasible", Response=true,
                        ResponseText="", IsResponseBoolean=true, Remarks="" },
                    new() {SrNo=8, IsMandatoryTrue=false,ReviewParameter= "Historically Data Available", Response=true,
                        ResponseText="", IsResponseBoolean=true, Remarks="" },
                    new() {SrNo=9, IsMandatoryTrue=true, ReviewParameter= "Commercially Feasible", Response=true,
                        ResponseText="", IsResponseBoolean=true, Remarks="" },
                    new() {SrNo=10, IsMandatoryTrue=false, ReviewParameter= "Amount of Fee from Client", Response=true,
                        ResponseText="", IsResponseBoolean=false, Remarks="" },
                    new() {SrNo=11, IsMandatoryTrue=false, ReviewParameter= "Current of Client Fee", Response=true,
                        ResponseText="", IsResponseBoolean=false, Remarks="" }
                };

                foreach(var rvw in reviews) {
                    var oitems = orderitems.Where(x => x.OrderId==rvw.OrderId).ToList();
                    reviewItems=new List<ContractReviewItem>();

                    var hrexecnames = await context.Employees.Where(x => x.Department=="HR").SelectMany(x => x.UserName).ToListAsync();
                    foreach(var item in oitems) {
                        foreach(var q in reviewItemQs) {q.OrderItemId=item.Id; }
                            
                        reviewItems.Add(new ContractReviewItem{OrderId=item.OrderId, OrderItemId=item.Id,
                            Charges=random.Next(1,3)==1 ? 45000 : random.Next(2,3)==3 ? 22000 : 30000, Ecnr=item.Ecnr, 
                            HrExecUsername=random.Next(1,2)==1 ? "Kadir" : "Munira",
                            ProfessionName=await context.GetProfessionNameFromId(item.ProfessionId),
                            Quantity=item.Quantity, RequireAssess=random.Next(1,2)==1 ? "T": "F", 
                            ReviewItemStatus="Accepted", SourceFrom="India", ContractReviewItemQs=reviewItemQs});
                    }
                    rvw.ContractReviewItems=reviewItems;

                    context.ContractReviews.Add(rvw);
                }

            }

            await context.SaveChangesAsync();

            if(!await context.ChecklistHRs.AnyAsync()) {

                var candidates = await context.Candidates.Where(x => x.Id % 3 == 0).Select(x => new {x.Id, x.Created}).ToListAsync();
                var orderitems = await (from item in context.OrderItems where item.Id % 3 == 0 
                    join rvwItem in context.ContractReviewItems on item.Id equals rvwItem.OrderItemId
                    select new {orderitemid=item.Id, charges=rvwItem.Charges, HRExecName=rvwItem.HrExecUsername})
                .ToListAsync();

                var checklistItems = new List<ChecklistHRItem>
                    { 
                        new() {SrNo=1, Parameter="Is Seriously interested", Response = "True", Accepts = true, 
                            Exceptions = "", MandatoryTrue = true},
                        new() {SrNo = 2, Parameter = "Has Original Passport in his possession", Response =  "true",
                            Accepts = true, Exceptions = "", MandatoryTrue = true},
                        new() {SrNo = 3, Parameter = "Willing to pay as Service Charges", Response = "true",
                            Accepts = true, Exceptions = "Willing to pay service charges", MandatoryTrue = true},
                        new() {SrNo = 4, Parameter = "Willing to travel as scheduled", Response =  "true", Accepts= true, 
                            Exceptions = "", MandatoryTrue = true},
                        new() {SrNo = 5, Parameter = "If ECNR required, candidate has ECNR PP", Response = "true", 
                            Accepts = true, Exceptions = "", MandatoryTrue = true},
                        new() {SrNo = 6, Parameter= "Accepts to do assigned work", Response = "true", 
                            Accepts = true,  Exceptions = "", MandatoryTrue = true},
                        new() {SrNo= 7, Parameter= "If Trade Test reqd, verified that it conducted on same person as the PP", 
                            Response = "true", Accepts= true, Exceptions = "", MandatoryTrue = true}
                    };
         
                foreach(var orderitem in orderitems) {
                    foreach(var cand in candidates) {
                        var chklst = new ChecklistHR{
                            OrderItemId = orderitem.orderitemid, CandidateId=cand.Id,
                            Charges=orderitem.charges, 
                            CheckedOn =cand.Created.AddDays(5), 
                            HrExecUsername =orderitem.HRExecName, 
                            ChargesAgreed = orderitem.charges, 
                            ExceptionApproved = true, ExceptionApprovedBy="",
                            ChecklistedOk = true, Username=usernames[random.Next(0,3)],
                            ChecklistHRItems=checklistItems
                        };
                        context.ChecklistHRs.Add(chklst);
                    }
                }
            }

            if(!await context.CandidateAssessments.AnyAsync()) {

                var dtls = await (from oitem in context.OrderItems where oitem.Id % 3== 0
                    from cand in context.Candidates where cand.Id % 3 == 0
                    join order in context.Orders on oitem.OrderId equals order.Id 
                    join cat in context.Professions on oitem.ProfessionId equals cat.Id
                    join rvwitem in context.ContractReviewItems on oitem.Id equals rvwitem.OrderItemId
                    select new CandidateAssessment {CustomerName=order.Customer.CustomerName, 
                        CategoryRefAndName=order.OrderNo + "-" + oitem.SrNo + "-" + cat.ProfessionName,
                        AssessedByEmployeeName=rvwitem.HrExecUsername, CandidateId=cand.Id,
                        RequireInternalReview= rvwitem.RequireAssess=="T", OrderItemId=oitem.Id,
                        AssessedOn=order.OrderDate.AddDays(3)
                        })
                    .ToListAsync();

                var chklsts = await context.ChecklistHRs.Where(x => 
                    dtls.Select(x => x.CandidateId).ToList().Contains(x.CandidateId) 
                    && dtls.Select(x => x.OrderItemId).ToList().Contains(x.OrderItemId)
                ).ToListAsync();

                foreach(var dtl in dtls) {
                    var chklst = chklsts.Where(x => x.CandidateId==dtl.CandidateId && x.OrderItemId==x.OrderItemId).FirstOrDefault();
                    if(chklst != null) dtl.ChecklistHRId=chklst.Id;
                }
                
                foreach(var item in dtls) {context.CandidateAssessments.Add(item);}
                
                await context.SaveChangesAsync();
            }
        
            if(!await context.CVRefs.AnyAsync()) {

                var cands = await context.Candidates.Select(x => x.Id).ToListAsync();

                var CVRefs =await (from assessent in context.CandidateAssessments
                    join rvwitem in context.ContractReviewItems on assessent.OrderItemId equals rvwitem.OrderItemId
                    select new CVRef {CandidateAssessmentId=assessent.Id, OrderItemId=assessent.OrderItemId,
                        CustomerId=0, CandidateId=assessent.CandidateId, ReferredOn = assessent.AssessedOn,
                        HRExecId=assessent.AssessedByEmployeeId, HRExecUsername=rvwitem.HrExecUsername,
                        RefStatus="Referred"}
                    ).ToListAsync();

                foreach(var cvref in CVRefs) {
                    context.CVRefs.Add(cvref);
                }
                
                await context.SaveChangesAsync();
            }
            
            //CVRef
            if(IncludeTasks) {
                var cvrefs = await context.CVRefs.ToListAsync();
                foreach(var cv in cvrefs) {
                    var username = usernames[random.Next(0,3)];
                    var customername = await context.GetCustomerNameFromOrderItemId(cv.OrderItemId);
                    var followupby = usernames[random.Next(0,3)];
                    var appno = await context.GetApplicationNoFromCandidateId(cv.CandidateId);

                    var task = new AppTask{CVRefId=cv.Id, TaskType="CVFwdTask", 
                        AssignedByUsername=cv.HRExecUsername, CandidateAssessmentId=cv.CandidateAssessmentId, 
                        AssignedToUsername="Sanjay", OrderItemId=cv.OrderItemId, CandidateId=cv.CandidateId, 
                        TaskDate=cv.ReferredOn, ApplicationNo=appno,
                        TaskDescription="Forward CV to client by email - to customer " + customername + " app " + 
                            appno + " forwarded on " + string.Format("{0: dd-MMM-yyyy}", cv.ReferredOn), 
                        CompleteBy = cv.ReferredOn.AddDays(7) };  

                    var task2 = new AppTask{CVRefId=cv.Id, TaskType="SelectionFollowupWithClient", AssignedByUsername=cv.HRExecUsername,
                        CandidateAssessmentId=cv.CandidateAssessmentId, AssignedToUsername="Sanjay", OrderItemId=cv.OrderItemId,
                        TaskDescription="Follow up with client for selection - customer " + customername + 
                            " app " + appno + " forwarded to " + customername + " on " + string.Format("{0: dd-MMM-yy}", cv.ReferredOn), 
                        CandidateId=cv.CandidateId, TaskDate=cv.ReferredOn, ApplicationNo=appno};
                    
                    context.Entry(task).State = EntityState.Added;
                    context.Entry(task2).State=EntityState.Added;
                }

                await context.SaveChangesAsync();

                var tasks = await context.Tasks.Where(x => x.TaskStatus == "CVFwdTask").ToListAsync();
                foreach(var t in tasks) {
                    t.TaskItems = new List<TaskItem>() {new() {TaskStatus="Not Started", AppTaskId = t.Id,
                            TaskItemDescription="Document Controller to forward CV to client - Application " + t.ApplicationNo, 
                            TransactionDate= t.TaskDate, UserName= t.AssignedByUsername, NextFollowupByName= t.AssignedToUsername,
                            NextFollowupOn= t.TaskDate.AddDays(2)  }};
                    context.Entry(t).State = EntityState.Modified;
                }

                var tasks2 = await context.Tasks.Where(x => x.TaskStatus == "SelectionFollowupWithClient").ToListAsync();
                foreach(var t in tasks2) {
                    t.TaskItems = new List<TaskItem>() {new() {TaskStatus="Not Started", AppTaskId = t.Id,
                        TaskItemDescription="Follow up with client for selection - Application " + t.ApplicationNo, 
                        TransactionDate= t.TaskDate, UserName= t.AssignedByUsername, NextFollowupByName= t.AssignedToUsername,
                        NextFollowupOn= t.TaskDate.AddDays(2)  }};
                    context.Entry(t).State = EntityState.Modified;
                }
            }
                        
            await context.SaveChangesAsync();

            var assessed = await context.CandidateAssessments.Where(x => x.CVRefId==0).ToListAsync();

            foreach(var ass in assessed) {
                var cvref = await context.CVRefs.FirstOrDefaultAsync(x => x.OrderItemId==ass.OrderItemId && x.CandidateId==ass.CandidateId);
                if(cvref != null) {
                    var t = await context.Tasks.Where(x => x.CandidateAssessmentId == cvref.CandidateAssessmentId
                        && x.TaskType=="CVFwdTask").FirstOrDefaultAsync();
                    ass.CVRefId = cvref.Id;
                    if(t != null) ass.TaskIdDocControllerAdmin=t.Id;
                    context.Entry(ass).State = EntityState.Modified;
                }
            }

            await context.SaveChangesAsync();
        
            if(!await context.SelectionDecisions.AnyAsync()) {
                var selections = await (from cvref in context.CVRefs where cvref.OrderItemId % 3 == 0
                    join cand in context.Candidates on cvref.CandidateId equals cand.Id
                    join orderitem in context.OrderItems on cvref.OrderItemId equals orderitem.Id
                    join order in context.Orders on orderitem.OrderId equals order.Id
                    join cat in context.Professions on orderitem.ProfessionId equals cat.Id
                    join remun in context.Remunerations on orderitem.Id equals remun.OrderItemId
                    join chklst in context.ChecklistHRs on new {cvref.CandidateId, cvref.OrderItemId}
                        equals new {chklst.CandidateId, chklst.OrderItemId} 
                    select new SelectionDecision {OrderItemId=cvref.OrderItemId, 
                        CandidateId=cvref.CandidateId, CandidateName=cand.FullName, 
                        ApplicationNo=cand.ApplicationNo, Gender=cand.Gender,
                        CustomerId=order.CustomerId, CvRefId=cvref.Id, SelectedAs= cat.ProfessionName,
                        CustomerName=order.Customer.CustomerName, ProfessionId=orderitem.ProfessionId,
                        SelectionStatus="Selected", CityOfWorking=order.CityOfWorking, 
                        SelectionDate=cvref.ReferredOn.AddDays(5),
                        SelectedOn=cvref.ReferredOn.AddDays(5)
                        , Charges= chklst.ChargesAgreed

                        , Employment = new Employment{CvRefId=cvref.Id, SelectedOn=cvref.ReferredOn.AddDays(5)
                        , Charges=chklst.Charges
                        , ChargesFixed=chklst.ChargesAgreed
                        , SalaryCurrency=remun.SalaryCurrency, Salary=remun.SalaryMin, 
                        ContractPeriodInMonths=remun.ContractPeriodInMonths, WeeklyHours=remun.WorkHours,
                        HousingProvidedFree=remun.HousingProvidedFree, HousingNotProvided=remun.HousingNotProvided,
                        HousingAllowance=remun.HousingAllowance, FoodProvidedFree=remun.FoodProvidedFree,
                        FoodNotProvided=remun.FoodNotProvided, FoodAllowance=remun.FoodAllowance,
                        TransportAllowance=remun.TransportAllowance, TransportNotProvided=remun.TransportNotProvided,
                        TransportProvidedFree=remun.TransportProvidedFree, OtherAllowance=remun.OtherAllowance,
                        LeaveAirfareEntitlementAfterMonths=remun.LeaveAirfareEntitlementAfterMonths,
                        LeavePerYearInDays=remun.LeavePerYearInDays}}
                    ).ToListAsync();

                foreach(var item in selections) 
                {
                    context.SelectionDecisions.Add(item);
                    
                    var cvref = await context.CVRefs.FindAsync(item.CvRefId);
                    if(cvref !=null) {
                        cvref.SelectionStatus="Selected";
                        cvref.SelectionStatusDate=item.SelectedOn;
                        cvref.RefStatus = "Referred For Medical Tests";
                        cvref.RefStatusDate=item.SelectedOn;
                        context.Entry(cvref).State=EntityState.Modified;
                    }
                }
                await context.SaveChangesAsync();

            }

            var sels = await context.SelectionDecisions.ToListAsync();
            if(IncludeTasks) {
                var selTasks = await context.Tasks.Where(x => x.TaskType=="SelectionFollowupWithClient").ToListAsync();
                foreach(var selTask in selTasks) {
                    var sel = sels.FirstOrDefault(x => x.CvRefId == selTask.CVRefId);
                    selTask.TaskStatus="Completed";
                    if(sel==null) selTask.CompletedOn = sel.SelectedOn;

                    selTask.TaskItems = new List<TaskItem>(){new() {AppTaskId=selTask.Id, 
                        UserName = selTask.AssignedByUsername, NextFollowupByName=selTask.AssignedToUsername,
                        NextFollowupOn = selTask.TaskDate.AddDays(2), TaskStatus = "Not Started", 
                        TransactionDate=selTask.TaskDate, TaskItemDescription="Client finalized decision on " + 
                            string.Format("{0: dd-MMM-yy}", sel.SelectionDate) + 
                        ", DECISION " + sel.SelectionStatus=="Selected" ? "Selected" : "Rejected"}};

                    context.Entry(selTask).State=EntityState.Modified;
                }

                var selected = sels.Where(x => x.SelectionStatus=="Selected").ToList();
                foreach(var s in selected) {

                    var taskemp = new AppTask{CVRefId= s.CvRefId, TaskType="EmploymentAcceptance", 
                        AssignedByUsername= "Sanjay", AssignedToUsername="Sanjay", 
                        OrderItemId=s.OrderItemId, CandidateId=s.CandidateId,
                        TaskDate=s.SelectionDate, ApplicationNo=s.ApplicationNo,
                        TaskDescription="Selection Acceptance by App No " + s.ApplicationNo + ", " + s.CandidateName + 
                            " selected by " + s.CustomerName + " on "  + string.Format("{0: dd-MMM-yy}", s.SelectionDate), 
                            CompleteBy = s.SelectionDate.AddDays(7)};
                    context.Entry(taskemp).State=EntityState.Added;            
                }

                await context.SaveChangesAsync();

                var tasks = await context.Tasks.Where(x => x.TaskType=="EmploymentAcceptance").ToListAsync();
                foreach(var t in tasks) {
                    t.TaskItems = new List<TaskItem>() {new() {TaskStatus="Not Started", AppTaskId = t.Id,
                        TaskItemDescription="Candidate acceptance of the selection - Application " + t.ApplicationNo, 
                        TransactionDate=t.TaskDate, UserName="Sanjay", NextFollowupByName="Sanjay",
                        NextFollowupOn=t.TaskDate.AddDays(2)  }};
                    context.Entry(t).State=EntityState.Modified;
                }

            }

            if(!await context.Deps.AnyAsync()) {
                var selections = await context.SelectionDecisions.ToListAsync();
                      
                foreach(var item in selections) {

                    var dep = new Dep{
                        CityOfWorking=item.CityOfWorking, CurrentStatus="Selected", CurrentStatusDate=item.SelectedOn,
                        CustomerId=item.CustomerId, CustomerName = item.CustomerName, CvRefId=item.CvRefId, 
                        OrderItemId=item.OrderItemId, SelectedOn=item.SelectedOn};

                    context.Entry(dep).State=EntityState.Added;
                }
                await context.SaveChangesAsync();
             }

             if(!await context.DepItems.AnyAsync()) {
                var deps= await context.Deps.ToListAsync();
                
                foreach(var item in deps) {
                    
                    var depitem = new DepItem {DepId=item.Id, TransactionDate=item.SelectedOn, Sequence=100, 
                        NextSequence=300, NextSequenceDate=item.SelectedOn.AddDays(9)};
                    
                    context.Entry(depitem).State=EntityState.Added;
                }
                await context.SaveChangesAsync();
             }

            if(context.ChangeTracker.HasChanges()) await context.SaveChangesAsync();
            
        }
    }
}