using System.Text.Json;
using api.Entities.Admin;
using api.Entities.Admin.Client;
using api.Entities.Admin.Order;
using api.Entities.Deployments;
using api.Entities.Finance;
using api.Entities.HR;
using api.Entities.Identity;
using api.Entities.Master;
using api.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace api.Data
{
    public class Seed
    {
        public static async Task SeedUsers (UserManager<AppUser> userManager, 
            RoleManager<AppRole> roleManager, DataContext context) 
        {
            Random random = new();
            
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

            if(!await context.FinanceVouchers.AnyAsync()) {
                var data = await File.ReadAllTextAsync("Data/SeedData/FinanceVoucherSeedData.json");
                _ = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dbData = JsonSerializer.Deserialize<List<FinanceVoucher>>(data);
                foreach(var item in dbData) 
                {
                    var coaid = await context.COAs.Where(x => x.AccountName==item.AccountName).Select(x => x.Id).FirstOrDefaultAsync();
                    if(coaid != 0) item.CoaId=coaid;
                    foreach(var trans in item.VoucherEntries) {
                        var coatrans = await context.COAs.Where(x => x.AccountName==trans.AccountName).Select(x => x.Id).FirstOrDefaultAsync();
                        if(coatrans != 0) trans.CoaId=coatrans;
                    }
                    context.FinanceVouchers.Add(item);
                }
            }

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
                    context.Customers.Add(item);
                }
            }
            var maxCustomerId = Convert.ToInt32(await context.Customers.MaxAsync(x => (int?)x.Id));
 
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
            var maxIndId = Convert.ToInt32(await context.Industries.MaxAsync(x => (int?)x.Id));

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

                    foreach(var sk in item.HRSkills) {
                        sk.ProfessionId=random.Next(1, maxProfId);
                        sk.IndustryId=random.Next(1,maxIndId);
                        sk.SkillLevelName="Proficient";
                        sk.IsMain=true;
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
            
            await context.SaveChangesAsync();

       //dependent entities   

            if(!await context.Orders.AnyAsync()) {
                var nextOrderNo = Convert.ToInt32(await context.Orders.MaxAsync(x => (int?) x.OrderNo));
                var reviewQs = await context.ContractReviewItemQs.OrderBy(x => x.SrNo).ToListAsync();
                foreach(var rvw in reviewQs) {rvw.Response=true;}
                var hrExecNames = await context.Employees.Where(x => x.Department=="HR").Select(x => x.UserName).ToListAsync();
                var ctExecNames = hrExecNames.Count;
                var data = await File.ReadAllTextAsync("Data/SeedData/OrderAndOrderItemSeedData.json");
                _ = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dbData = JsonSerializer.Deserialize<List<Order>>(data);
                foreach(var order in dbData) 
                {
                    var reviewItems = new List<ContractReviewItem>();
                    order.CustomerId=random.Next(1, maxCustomerId);
                    order.CompleteBy=order.OrderDate.AddDays(2);
                    order.OrderNo=nextOrderNo;
                    order.SalesmanId=random.Next(1, maxEmpId);
                    order.ProjectManagerId=random.Next(1, maxEmpId);
                    order.ContractReview = new ContractReview{
                        CustomerName=order.Customer.CustomerName, CustomerId=order.CustomerId,
                        OrderDate=order.OrderDate, OrderNo=nextOrderNo, ReviewedByName="Admin",
                        ReviewStatus=random.Next(1,2)==1 ? "Accepted": "Rejected"};
                    order.ContractReview.CustomerId=order.CustomerId;

                    foreach(var item in order.OrderItems) {
                        item.ProfessionId=random.Next(1, maxProfId);
                        item.MinCVs=item.Quantity*3;
                        item.MaxCVs=item.Quantity*3;
                        reviewItems.Add(new ContractReviewItem{
                            Ecnr=true, OrderItemId=item.Id, 
                            ProfessionName=await context.GetProfessionNameFromId(item.ProfessionId),
                            Quantity=item.Quantity, ReviewItemStatus=order.ContractReview.ReviewStatus, 
                            SourceFrom=item.SourceFrom,
                            HrExecUsername = hrExecNames.Take(1).Skip(random.Next(1, ctExecNames)).FirstOrDefault(),
                            RequireAssess=random.Next(1,2)==1?"T":"F",
                            Charges=random.Next(15,45)*1000,
                            ContractReviewItemQs = reviewQs
                        });
                    }
                    order.ContractReview.ContractReviewItems=reviewItems;
                    context.Orders.Add(order);
                }

                await context.SaveChangesAsync();
            }
            var items = await context.OrderItems.Select(x => x.Id).ToListAsync();
            var maxOrderItemId = Convert.ToInt32(await context.OrderItems.MaxAsync(x => (int?)x.Id));

            if(!await context.Remunerations.AnyAsync()) {
                var data = await File.ReadAllTextAsync("Data/SeedData/RemunerationSeedData.json");
                _ = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dbData = JsonSerializer.Deserialize<List<Remuneration>>(data);
                
                int m = 0;
                foreach(var item in items) {
                    var remuneration = dbData.Take(1).Skip(m).FirstOrDefault();
                    remuneration.OrderItemId=item;
                    context.Remunerations.Add(remuneration);
                    m++;
                }
            }
              
            if(!await context.JobDescriptions.AnyAsync()) {
                var data = await File.ReadAllTextAsync("Data/SeedData/JobDescriptionSeedData.json");
                _ = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dbData = JsonSerializer.Deserialize<List<JobDescription>>(data);
                int m = 0;
                foreach(var item in items) {
                    var jd = dbData.Take(1).Skip(m).FirstOrDefault();
                    jd.OrderItemId=item;
                    context.JobDescriptions.Add(jd);
                    m++;
                }
                await context.SaveChangesAsync();
            }

            if(!await context.Candidates.AnyAsync()) {
                var nextAppNo = Convert.ToInt32(await context.Candidates.MaxAsync(x => (int?) x.ApplicationNo) ?? 1000);
                
                var data = await File.ReadAllTextAsync("Data/SeedData/CandidateJsonSeed.json");
                _ = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dbData = JsonSerializer.Deserialize<List<Candidate>>(data);
                foreach(var item in dbData) 
                {
                    item.AadharNo = Convert.ToString(random.Next(12341234, 87658765)) + Convert.ToString(random.Next(1234,8765));
                    item.PpNo = (char)random.Next(1,32) + Convert.ToString(random.Next(123412, 87658765));
                    var Profs = new List<UserProfession> {new() 
                        {ProfessionId=random.Next(1, maxProfId), IsMain=true}, new() {ProfessionId=random.Next(1,maxProfId)}};
                    var PhoneNos = new List<UserPhone> {new() 
                        {MobileNo=Convert.ToString(random.Next(67111111, 88882222)) + Convert.ToString(random.Next(11,99)),
                            IsMain=true, IsValid=true}, new() 
                        {MobileNo=Convert.ToString(random.Next(67111111, 888881111))+ Convert.ToString(random.Next(11,99)),
                            IsMain=true, IsValid=true}};
                    item.Created = DateTime.Now.AddYears(-5);
                    item.LastActive = DateTime.Now.AddMonths(-5);
                    item.DOB=DateTime.Now.AddYears(random.Next(-18,-40)).AddMonths(-2);
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
            
            if(!await context.ChecklistHRs.AnyAsync()) {
                var data = await File.ReadAllTextAsync("Data/SeedData/ChecklistSeedData.json");
                _ = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dbData = JsonSerializer.Deserialize<List<ChecklistHR>>(data);
                foreach(var item in dbData) 
                {
                    item.CandidateId=random.Next(1,maxCandId);
                    item.OrderItemId=random.Next(1, maxOrderItemId);

                    context.ChecklistHRs.Add(item);
                }
            }
            await context.SaveChangesAsync();

            if(!await context.CandidateAssessments.AnyAsync()) {

                var data = await File.ReadAllTextAsync("Data/SeedData/CandidateAssessmentSeedData.json");
                _ = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dbData = JsonSerializer.Deserialize<List<CandidateAssessment>>(data);
                
                foreach(var item in dbData) 
                {
                    item.CandidateId = random.Next(1, maxCandId);
                    item.OrderItemId = random.Next(1, maxOrderItemId);

                    var dtls = await (from oitem in context.OrderItems where oitem.Id==item.OrderItemId
                        join order in context.Orders on oitem.OrderId equals order.Id 
                        join cat in context.Professions on oitem.ProfessionId equals cat.Id
                        join chklst in context.ChecklistHRs on new {item.OrderItemId, item.CandidateId}
                            equals new {chklst.OrderItemId, chklst.CandidateId}
                        join rvwitem in context.ContractReviewItems on oitem.Id equals rvwitem.OrderItemId
                        select new {CustomerName=order.Customer.CustomerName, 
                            CategoryRefAndName=order.OrderNo + "-" + oitem.SrNo + "-" + cat.ProfessionName,
                            AssessedByEmployeeName=rvwitem.HrExecUsername, 
                            RequireInternalReview=rvwitem.RequireAssess,
                            ChecklistHRId=chklst.Id})
                        .FirstOrDefaultAsync();

                    if(dtls != null) {
                        item.CustomerName = dtls.CustomerName;
                        item.CategoryRefAndName=dtls.CategoryRefAndName;
                        item.AssessedByEmployeeName=dtls.AssessedByEmployeeName;
                        item.RequireInternalReview= dtls.RequireInternalReview=="N";
                        item.ChecklistHRId=dtls.ChecklistHRId;
                    } 
                    
                    context.CandidateAssessments.Add(item);
                }
                await context.SaveChangesAsync();
            }
        
            if(!await context.CVRefs.AnyAsync()) {

                var data = await File.ReadAllTextAsync("Data/SeedData/CVRefSeedData.json");
                _ = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dbData = JsonSerializer.Deserialize<List<CVRef>>(data);
                var cands = await context.Candidates.Select(x => x.Id).ToListAsync();

                foreach(var item in dbData) 
                {
                    item.CandidateId = random.Next(1, maxCandId);
                    item.OrderItemId = random.Next(1, maxOrderItemId);
                    var assessmt =await (from assessent in context.CandidateAssessments where
                        assessent.CandidateId==item.CandidateId && assessent.OrderItemId==item.OrderItemId
                        join rvwitem in context.ContractReviewItems on assessent.OrderItemId equals rvwitem.OrderItemId
                        select new {AssessmentId=assessent.Id, HRExecUsername=rvwitem.HrExecUsername}
                        ).FirstOrDefaultAsync();
                    
                    if(assessmt != null) {
                        item.CandidateAssessmentId=assessmt.AssessmentId;
                        item.HRExecUsername=assessmt.HRExecUsername;
                    }
                    context.CVRefs.Add(item);
                }
                await context.SaveChangesAsync();
            }
            
            if(!await context.SelectionDecisions.AnyAsync()) {
                //var data = await File.ReadAllTextAsync("Data/SeedData/SelectionDecisionSeedData.json");
                //_ = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                //var dbData = JsonSerializer.Deserialize<List<SelectionDecision>>(data);

                var selections = await (from cvref in context.CVRefs where cvref.Id % 4 == 0
                    join cand in context.Candidates on cvref.CandidateId equals cand.Id
                    join orderitem in context.OrderItems on cvref.OrderItemId equals orderitem.OrderId
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
                        SelectedOn=cvref.ReferredOn.AddDays(5), Charges=chklst.ChargesAgreed,
                        Employment = {CvRefId=cvref.Id, SelectedOn=cvref.ReferredOn.AddDays(5),
                        Charges=chklst.Charges, ChargesFixed=chklst.ChargesAgreed, 
                        SalaryCurrency=remun.SalaryCurrency, Salary=remun.SalaryMin, 
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
                        context.Entry(cvref).State=EntityState.Modified;
                    }
                    var dep = new Dep{
                        CityOfWorking=item.CityOfWorking, CurrentStatus="Selected", CurrentStatusDate=item.SelectionDate,
                        CustomerId=item.CustomerId, CustomerName = item.CustomerName, CvRefId=item.CvRefId, 
                        OrderItemId=item.OrderItemId, SelectedOn=item.SelectedOn, DepItems = new List<DepItem>{
                            new() {TransactionDate=item.SelectedOn, Sequence=100, NextSequence=300, 
                            NextSequenceDate=item.SelectedOn.AddDays(9)}
                        }};
                    context.Entry(dep).State=EntityState.Added;
                }
                await context.SaveChangesAsync();
            }
            
            if(!await context.CustomerFeedbacks.AnyAsync()) {
                var data = await File.ReadAllTextAsync("Data/SeedData/FeedbackSeedData.json");
                _ = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dbData = JsonSerializer.Deserialize<List<CustomerFeedback>>(data);

                var feedbackitems = new List<FeedbackItem>{
                    new() {FeedbackGroup= "documentation", QuestionNo = 1, 
                        Question = "How satisfied you are with our documentations",
                        Prompt1="Very Satisfied", Prompt2="Satisfied", Prompt3="Can Do Better`", Prompt4="Not Satisfied",
                        Response = ""},
                    new() {FeedbackGroup= "documentation", QuestionNo = 2, 
                        Question = "How satisfied are you with our response to your queries",
                        Prompt1="Very Satisfied", Prompt2="Satisfied", Prompt3="Can Do Better", Prompt4="Not Satisfied",
                        Response = ""},
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
                        Prompt1="Highly Skilled", Prompt2="Skilled", Prompt3="Acceptable", Prompt4="Not Skilled",
                        Response = ""},
                    new() {FeedbackGroup= "Deployment Process Time", QuestionNo = 6, 
                        Question = "How fast did we deploy your selected candidates",
                        Prompt1="Very Quick", Prompt2="Quick", Prompt3="Should Do Better", Prompt4="Slow",
                        Response = ""},
                    
                        new() {FeedbackGroup= "Marketing Services", QuestionNo = 7, 
                        Question = "How satisfied are you with the services of our marketing personnel",
                        Prompt1="Very Satisfied", Prompt2="Satisfied", Prompt3="Should Do Better", Prompt4="Not Satisfied",
                        Response = ""},
                    new() {FeedbackGroup= "Overall Services", QuestionNo = 8, 
                        Question = "How satisfied are you with our overall services",
                        Prompt1="Very Satisfied", Prompt2="Satisfied", Prompt3="Should Do Better", Prompt4="Not Satisfied",
                        Response = ""},
                    new() {FeedbackGroup= "Recommendation to others", QuestionNo = 8, 
                        Question = "Would you recommend us to your other associates for availing recruitment services from us?",
                        Prompt1="Yes", Prompt2="No",Response = ""}
                };

                var feedbackdata = await(from cust in context.Customers
                    join off in context.CustomerOfficials on cust.Id equals off.CustomerId where off.Divn=="H.R."
                    select new CustomerFeedback{
                        CustomerId = cust.Id, CustomerName=cust.CustomerName, City = cust.City,
                        Country=cust.Country, OfficialName = off.OfficialName, OfficialAppUserId=off.AppUserId,
                        Designation=off.Designation, Email=off.Email, PhoneNo=off.PhoneNo, DateIssued = 
                            new DateTime( random.Next(2018,2023), random.Next(1,11), random.Next(1,28)),
                        FeedbackItems = feedbackitems}).ToListAsync();
                    
                foreach(var item in feedbackdata) 
                {
                    context.CustomerFeedbacks.Add(item);
                }
                await context.SaveChangesAsync();
            }
            
            if(context.ChangeTracker.HasChanges()) await context.SaveChangesAsync();
            
        }
    }
}