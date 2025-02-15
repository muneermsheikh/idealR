using api.Data.Repositories.Orders;
using api.DTOs.Admin;
using api.DTOs.Admin.Orders;
using api.Entities.Admin.Order;
using api.Entities.HR;
using api.Entities.Identity;
using api.Entities.Messages;
using api.Extensions;
using api.Interfaces;
using api.Interfaces.Messages;
using api.Interfaces.Orders;
using AutoMapper;
using DocumentFormat.OpenXml.Drawing;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Repositories.Admin
{
    public class ComposeMessagesHRRepository : IComposeMessagesHRRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;
        private readonly UserManager<AppUser> _userManager;
        private readonly ICustomerRepository _custRepo;
        private readonly DateTime _dateToday = DateTime.Now;
        private readonly IMapper _mapper;
        private readonly string _RAName;
        public ComposeMessagesHRRepository(DataContext context, IConfiguration config,       
            UserManager<AppUser> userManager, ICustomerRepository custRepo, IMapper mapper)
        {
            _mapper = mapper;
            _custRepo = custRepo;
            _userManager = userManager;
            _config = config;
            _context = context;
            _RAName = _config["RAName"];
        }

        public async Task<MessageWithError> ComposeEmailMsgForDLForwardToHRHead(ICollection<OrderItemBriefDto> OrderItems, 
            string senderUsername, AppUser recipientObject)
        {
            var dtoErr = new MessageWithError();

            var dto = OrderItems.Select(x => new {x.CustomerName, x.AboutEmployer, x.OrderNo, x.OrderDate, 
                x.CityOfEmployment}).FirstOrDefault();
            var senderObject = await _userManager.FindByNameAsync(senderUsername);
            if( recipientObject == null) {
                dtoErr.ErrorString = "Recipient Object is not defined";
                return dtoErr;
            }

            string AboutEmployer = dto!.AboutEmployer;
            string CustomerName = dto.CustomerName;
            
            var TableBody = ComposeCategoryTableForEmail(OrderItems);
                        
            var msgBody = string.Format("{0: dd-MMM-yyyy}", _dateToday) + "<br><br>" + 
                recipientObject.Gender=="male" ? "Mr. " : "Ms. " + recipientObject.KnownAs + ", " + 
                recipientObject.Position + "<br>eMail: " + recipientObject.Email + 
                "<br>Phone No:" + recipientObject.PhoneNumber  + "<br><br>Dear Sir: <br><br>";
           
            msgBody += "<u><b>Order No.:" + dto.OrderNo + " dated " + string.Format("{0: dd-MMM-yyyy}", dto.OrderDate) + 
                "<br>Customer:" + CustomerName + ", City of Employment" + dto.CityOfEmployment + "</b></u>";
            msgBody += (!string.IsNullOrEmpty(AboutEmployer)) ? "<br>About Employer: " + AboutEmployer : "";

            msgBody += "<br><br>Following requirement is forwarded to you to execute as per details given.<br><br>Requirement details:<br>";
   
            msgBody +=  TableBody +  "Regards<br><br>" + senderObject.KnownAs + "/" + 
                senderObject.Position + "<br>Phone:" + senderObject.PhoneNumber ?? "undefined";
            msgBody += "<br><b>" + _RAName + "</b>";
            
            var msg =new Message{MessageType = "AdviseToHRDeptHead", 
                //SenderAppUserId = senderObject.Id, RecipientAppUserId = recipientObject.Id, 
                RecipientEmail = recipientObject.Email, SenderUsername = senderObject.UserName, 
                RecipientUsername = recipientObject.UserName, MessageComposedOn = _dateToday,
                Subject = "Order Forward to HR Head - Order No. " + dto.OrderNo, Content = msgBody};
            
            _context.Messages.Add(msg);
            dtoErr.Messages.Add(msg);
            //_context.Entry(msg).State = EntityState.Added;

            return dtoErr;
            //return await _context.SaveChangesAsync() > 0 ? "" : "Failed to save the email message to database";
            
        }

        public Task<Message> ComposeHTMLToAckToCandidateByEmail(Candidate candidate)
            {
                throw new NotImplementedException();
            }

        public Task<Message> ComposeHTMLToPublish_CVReadiedToForwardToClient(ICollection<CommonDataDto> commonDataDtos, string Username, int recipientAppUserId)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<Message>> ComposeMsgsToForwardOrdersToAgents(ICollection<OrderForwardCategory> categories, ICollection<int> officialIds, string Username)
        {
            
            var msgData = new List<OrderItemBriefDtoWithOfficialIdsDto>();
            var orderid = categories.Select(x => x.OrderId).FirstOrDefault();

            var order = await _context.Orders
                .Include(x => x.OrderItems).ThenInclude(x => x.JobDescription)
                .Include(x => x.OrderItems).ThenInclude(x => x.Remuneration)
                .Include(x => x.Customer)
                .Where(x => x.Id == orderid)
                .FirstOrDefaultAsync();

            var projManagerId = order.ProjectManagerId == 0 ? Convert.ToInt32(_config["DocControllerAdminEmpId"]) : order.ProjectManagerId;
            if(order.ProjectManagerId==0) {
                projManagerId = Convert.ToInt32(_config["DocControllerAdminEmpId"]);
            } else {
                projManagerId=order.ProjectManagerId;
            }
            if(projManagerId == 0) return null;
            var projManagerAppUserId =  await _context.GetAppUserIdOfEmployee(projManagerId);
            if(projManagerAppUserId == 0) projManagerAppUserId = Convert.ToInt32(_config["HRSupAppuserId"] ?? "0");
           
            var msgs = new List<Message>();

            var distinctOrderItemIds = categories.Select(x => x.OrderItemId).Distinct().ToList();
            //queries that has to use expressions at client level - like distinctOrderItemIds.contains(xx) - 
            //are not translated by EF Core.  The query ahs to be redesigned

            var item = new OrderItemBriefDto();
            var ItemBriefDtos = new List<OrderItemBriefDto>();

            foreach(var itm in order.OrderItems) {
                item = new OrderItemBriefDto {
                    OrderItemId = itm.Id, AboutEmployer = order.Customer?.Introduction,
                    CustomerId = order.CustomerId, CustomerName = order.Customer?.CustomerName,
                    Ecnr = itm.Ecnr, CompleteBefore = itm.CompleteBefore, JobDescription = itm.JobDescription,
                    Remuneration = itm.Remuneration, OrderDate = order.OrderDate,
                    OrderId=order.Id, OrderNo = order.OrderNo, ProfessionId=itm.ProfessionId,
                    ProfessionName = itm.Profession?.ProfessionName, Quantity=itm.Quantity, SrNo=itm.SrNo,
                    Status = itm.Status
                };
                ItemBriefDtos.Add(item);
            }

            foreach(var itm in ItemBriefDtos) {
                if(string.IsNullOrEmpty(itm.CustomerName)) itm.CustomerName = await _context.CustomerNameFromId(itm.CustomerId);
                if(string.IsNullOrEmpty(itm.ProfessionName)) itm.ProfessionName = await _context.GetProfessionNameFromId(itm.ProfessionId);
            }
            
            
            string subject = "Requirements under Order No.: " + order.OrderNo + " dated " + 
                    order.OrderDate + order.Customer.City;

            msgs = (List<Message>)await ComposeEmailMsgForOrderForwards(ItemBriefDtos, subject, officialIds, Username);

            return msgs;
           
        }

        public Task<SMSMessage> ComposeMsgToAckToCandidateBySMS(Candidate candidate)
        {
            throw new NotImplementedException();
        }

        public async Task<MessageWithError> ComposeEmailToDesignOrderItemAssessmentQs(int orderId, string username)
        {
            var msgWithErr = new MessageWithError();

            var order = await _context.Orders.Include(x => x.OrderItems).Where(x => x.Id == orderId).FirstOrDefaultAsync();
            if(order==null) {
                msgWithErr.ErrorString = "Invalid Order Id";
                return msgWithErr;
            }
            
            if(order.ProjectManagerId == 0) {
                msgWithErr.ErrorString = "Order Project Manager not defined. Without this data, task to design Assessment Q cannot be defined";
                return msgWithErr;
            }

            var recipientObj = await _userManager.FindByIdAsync(_config["HRSupAppUserId"]);
            if(recipientObj==null) {
                msgWithErr.ErrorString = "Cannot identify HR Supervisor data";
                return msgWithErr;
            }

            var senderObj = await _userManager.FindByNameAsync(username);

            var ItemBriefDtos = (ICollection<OrderItemBriefDto>)_mapper.Map<OrderItemBriefDto>(order.OrderItems);
            
            string IntroductoryBody = "Plase design Assessment Questions for following Order Categories.  Job Descriptions can be " +
                "downloaded from the system.  Any clarifications required by you concerning the Job Description may be addressed to " +
                "the undersigned. <br><br><b>Details of the Order Categories</b><br><br>:" +
                "<b>Order No.</b>: " + order.OrderNo + " <b>dated</b> " + order.OrderDate + "<br>" +
                "<b>Customer: </b>" + order.Customer.CustomerName + ", " + order.CityOfWorking +
                "<br><b>Customer website: </b>" + order.Customer.Website ?? "Not Available" +
                "<br><b>Order Categories</b><br>:" + ComposeCategoryTableForEmail(ItemBriefDtos);
             
            var msgBody = _dateToday + "<br><br>" + recipientObj.KnownAs + ", " + 
                    recipientObj.Position??"" + "<br>" + recipientObj.City??"" + "<br>eMail: " + recipientObj.Email +
                    "<br>Mobile:" + recipientObj.PhoneNumber + "<br><br>Dear " + recipientObj.UserName + ": <br><br>" + 
                    IntroductoryBody ;

            msgBody +=  "<br><br>Regards<br><br>" + senderObj.KnownAs + "/" + senderObj.Position + 
                    "<br>Phone:" + senderObj.PhoneNumber ?? "undefined" + "<br><b>" + _RAName + "</b>";

            var msg = new Message{MessageType = "DesignOrderItemAssessmentQ", Content = msgBody, 
                    MessageComposedOn = _dateToday,  RecipientUsername = recipientObj.UserName,
                   // RecipientAppUserId = recipientObj.Id, 
                    RecipientEmail = recipientObj.Email, //SenderAppUserId = senderObj.Id,
                    SenderUsername = senderObj.UserName, Subject = "Designing of Order Item Assessment Questions"};
            var msgs = new List<Message> {msg};
            msgWithErr.Messages = msgs;

            return msgWithErr;
        }

        private async Task<ICollection<Message>> ComposeEmailMsgForOrderForwards(ICollection<OrderItemBriefDto> OrderItems, 
            string subject, ICollection<int> officialids, string SenderUsername)
        {

            var TableBody = ComposeCategoryTableForEmail(OrderItems);

            var msgs = new List<Message>();
            string IntroductoryBody = "We have following requirements.  If you have friends interested in the opportunity, please ask them to submit their profiles " + 
                "to us along with copies of certificates and testimonials. <br><br><b>Country of requirement</b>:";

            //_ = fwdOfficials.OrderBy(x => x.CustomerOfficialId).Distinct();

            var senderObj = await _userManager.FindByNameAsync(SenderUsername);
            if(senderObj == null) return null;
                        
            var fwdOfficials =await(from off in _context.CustomerOfficials where officialids.Contains(off.Id)
                join cust in _context.Customers on off.CustomerId equals cust.Id
                select  new OrderForwardCategoryOfficial {
                    AgentName = cust.CustomerName, 
                    CustomerOfficialId = off.Id,
                    OfficialName = off.OfficialName,
                    DateForwarded = DateTime.Now,
                    EmailIdForwardedTo = off.Email,
                    PhoneNoForwardedTo = off.PhoneNo, Username = SenderUsername
                }).ToListAsync();

            foreach(var fwd in fwdOfficials)
            {
                var off = await _custRepo.GetCustomerOfficialDto(fwd.CustomerOfficialId);
                if(off == null) continue;
                var recipientObj=new AppUser();
                var recipientAppUserId = 0;
                recipientObj = await _userManager.FindByNameAsync(off.OfficialEmailId);
                if(recipientObj != null) {
                    recipientAppUserId = recipientObj.Id;
                } else {
                    recipientAppUserId = await _context.OfficialAppUserIdFromOfficialId(off.OfficialId);
                    if(recipientAppUserId == 0) {
                        recipientObj = await _userManager.FindByEmailAsync(off.OfficialEmailId);
                        if(recipientObj == null) {
                            var user = new AppUser{UserName=off.OfficialEmailId, Gender = off.Gender, KnownAs=off.OfficialName[..10], 
                                PhoneNumber=off.MobileNo, Position = off.Designation, Email=off.OfficialEmailId};
                            var result = await _userManager.CreateAsync(user);
                            if(result.Succeeded) {
                                var roleAdded = await _userManager.AddToRoleAsync(user, "Client");
                                if(roleAdded.Succeeded) {
                                    recipientAppUserId = user.Id;
                                    recipientObj = user;
                                }
                                var official = await _context.CustomerOfficials.FindAsync(fwd.CustomerOfficialId);
                                official.AppUserId=user.Id;
                                _context.Entry(official).State=EntityState.Modified;
                            }
                        }
                    }  else {
                        recipientObj = await _userManager.FindByIdAsync(recipientAppUserId.ToString());
                    }
                }

                var msgBody = _dateToday + "<br><br>" + off.OfficialName + ", " + 
                    off.CustomerName + "<br>" + off.City + "<br>eMail: " + off.OfficialEmailId +
                    "<br>Mobile:" + off.MobileNo + "<br><br>Dear Sir: <br><br>" + 
                    IntroductoryBody + off.Country ?? "" + "<br>City of employment: " + off.City +
                    "<br>Employer Known As: " + off.CompanyKnownAs + "<br>About Employer: " + 
                    off.AboutCompany ?? "undefined" + "<br><br>Requirement details<br>:";

                msgBody +=  TableBody +  "<br><br>Regards<br><br>" + senderObj.KnownAs + "/" + senderObj.Position + 
                    "<br>Phone:" + senderObj.PhoneNumber ?? "undefined";
                msgBody += "<br><b>" + _RAName + "</b>";
                
                msgs.Add(new Message{MessageType = "forwardToAssociate", Content = msgBody, 
                    MessageComposedOn = _dateToday,  RecipientUsername = recipientObj.UserName,
                    //RecipientAppUserId = recipientObj.Id, 
                    RecipientEmail = recipientObj.Email ?? "", 
                    //SenderAppUserId = senderObj.Id, 
                    SenderUsername = senderObj.UserName, Subject = subject});
            }
            
            return msgs;
        }

        private static string ComposeCategoryTableForDLFwd(ICollection<OrderItemToFwdDto> orderitems) {
            
            var para = "<Table border='1'><tr><th>Order Ref</th><th>Customer</th>" +
                "<th>Category</th><th>Quantity</th><th>Basic Salary</th>" +
                "<th>Job Description</th>" +
                "<th>Remuneration and Facilities</th></tr>";
            
            foreach(var item in orderitems ) {
                para += "<tr><td>" + item.CategoryRef + "</td><td>" + 
                        item.CategoryName + "</td><td>" + item.Quantity + "</td><td>" 
                        /*+
                        item.SalaryCurrency + " " + 
                        item.BasicSalary  + "<td><td>" +
                        item.JobDescriptionURL + "</td><td>" + 
                        item.RemunerationURL + "</td></tr>";
                        */
                        ;
            }
            para += "</table>";

            return para;
        }
        
        private static string ComposeCategoryTableForEmail(ICollection<OrderItemBriefDto> orderItems)
        {
            int srno = 0;
            string TableBody = "<Table border='1'><TH width='25'>Sr<br>No</TH><TH width='300'>Cat Ref</TH>" +
                "<TH width='40'>Qnty</TH><TH width='250'>Job Description</TH>" +
                "<TH width='250'>Remuneration</TH><TH width='150'>Remarks</TH>";
            string jd = "";
            string remun = "";
            foreach (var item in orderItems)
            {
                if (item.JobDescription != null)
                {
                    if (!string.IsNullOrEmpty(item.JobDescription.JobDescInBrief)) jd = item.JobDescription.JobDescInBrief;
                    if (item.JobDescription.MaxAge > 0) jd += "<br>Max Age:" + item.JobDescription.MaxAge + " yrs";
                    if (item.JobDescription.ExpDesiredMin > 0) jd += "<br>Exp: " + item.JobDescription.ExpDesiredMin;
                    if (item.JobDescription.ExpDesiredMax > 0) jd += " - " + item.JobDescription.ExpDesiredMax;
                    if (item.JobDescription.ExpDesiredMin > 0 || item.JobDescription.ExpDesiredMax > 0) jd += " yrs.";
                    if (!string.IsNullOrEmpty(item.JobDescription.QualificationDesired)) jd += "<br>Qualification: " + item.JobDescription.QualificationDesired;
                }

                if (item.Remuneration != null)
                {
                    remun = "Salary: ";
                    remun += string.IsNullOrEmpty(item.Remuneration.SalaryCurrency) ? "" : item.Remuneration.SalaryCurrency;
                    remun += item.Remuneration.SalaryMin > 0 ? item.Remuneration.SalaryMin : "";
                    remun += item.Remuneration.SalaryMax > 0 ? "-" + item.Remuneration.SalaryMax : "";
                    remun += "<br>Housing: ";
                    remun += item.Remuneration.HousingProvidedFree ? "Free" :
                        item.Remuneration.HousingAllowance > 0 ? item.Remuneration.HousingAllowance : "Not provided";
                    remun += "<br>Food: ";
                    remun += item.Remuneration.FoodAllowance > 0 ? item.Remuneration :
                        item.Remuneration.FoodProvidedFree ? "Provided Free" : "Not Provided";
                    remun += "<br>Transport: ";
                    remun += item.Remuneration.TransportProvidedFree ? "Provided Free" :
                        item.Remuneration.TransportAllowance > 0 ? item.Remuneration.TransportAllowance : "Not Provided";
                    remun += "; Medical Facilities: As per labour laws;";
                    if (item.Remuneration.OtherAllowance > 0) remun += "<br>Other Allowance: " + item.Remuneration.OtherAllowance;

                }
                TableBody += "<TR><TD style='vertical-align: top;'>" + ++srno + 
                    "</TD><TD style='vertical-align: top;'>" + item.OrderNo + "-" + item.SrNo + "-" + 
                    item.ProfessionName + "</TD><TD style='vertical-align: top;'>" + 
                    item.Quantity + "</TD><TD style='vertical-align: top;'>" +
                    jd + "</TD><TD style='vertical-align: top;'>" + remun + 
                    "</TD><TD style='vertical-align: top;'></TD></TR>";
            }

            TableBody += "</TABLE><BR>";

            return TableBody;
        }

        private async Task<ICollection<Message>> ComposeHTMLForwardsddToAgents(OrderItemsAndAgentsToFwdDto itemsAndAgents, 
            string recipientUsername, string Username)
        {
            
            var catTable = ComposeCategoryTableForDLFwd(itemsAndAgents.Items);
            var senderObj = await _userManager.FindByNameAsync(Username);
            var recipientObj = await _userManager.FindByNameAsync(recipientUsername);
            
            var msgs = new List<Message>();
            foreach(var agent in itemsAndAgents.Agents) {
                var hdr = DateTime.Now + "<br><br>" + agent.Title + " " + agent.CustomerOfficialName;
                if(!string.IsNullOrEmpty(agent.OfficialDesignation)) hdr +=", " + agent.OfficialDesignation;
                hdr +="<br>" + agent.CustomerName + "<br>" + agent.CustomerCity;
                hdr += "email: " + agent.OfficialEmailId + "<br><br>";
                hdr += "Sub.: Manpower requirement for " + agent.CustomerCity;
                hdr += "<br><br>We have following manpower requirement.  If you have interested candidates, please refer them to us<br><br>";
                hdr += catTable + "<br><br>Regards<br><br>" + senderObj.KnownAs + "<br>" + senderObj.Position;

                var message = new Message{MessageType = "DLFwdToAgent", 
                    RecipientEmail = agent.OfficialEmailId, SenderEmail = senderObj.Email,
                    Subject = "Requirement", Content = hdr};
                msgs.Add(message);
            }

            return msgs;
        }

        public async Task<MessageWithError> ComposeMessagesToHRExecToSourceCVs(ICollection<OrderItemBriefDto> assignmentDtos, string Username)
        {
            var msgWithErr = new MessageWithError();
            var msgs = new List<Message>();

            var msgBody="";

            var senderObject = await _userManager.FindByIdAsync(_config["DocControllerAdminAppUserId"]);
            if(senderObject == null) {
                msgWithErr.ErrorString = "Failed to get user object of sender - DocController Admin";
                return msgWithErr;
            }

            var HrExecUsernameDistinct = assignmentDtos.Select(x => x.HrExecUsername).Distinct().ToList();

            foreach(var hrexecuser in HrExecUsernameDistinct) {
                var orderitemsAssignedToHRExec = assignmentDtos.Where(x => x.HrExecUsername.ToLower() == hrexecuser.ToLower())
                    .OrderBy(x => x.OrderItemId).ToList();
                var orderItemTable = ComposeCategoryTableForEmail(orderitemsAssignedToHRExec);
                var recipientObj = await _userManager.FindByNameAsync(hrexecuser);
                if(recipientObj==null) continue;

                msgBody = _dateToday + "<br><br>" + recipientObj.Gender.ToLower() == "m" ? "Mr." : "Ms.";
                msgBody += recipientObj.KnownAs + "<br>" + recipientObj.Position + ", email:" + recipientObj.Email;
                msgBody +="<br><br>Following Order Categories are assigned to you. Job Descriptions and Remunertions can be " +
                    "viewed on the links provided. For any clarification, please approach your Supervisor. " +
                    "<br><br><b>Details of the Order Categories:</b><br><br>" +
                    "<b>Order No.</b>: " + orderitemsAssignedToHRExec.Select(x => x.OrderNo).FirstOrDefault() + " dated " + 
                        orderitemsAssignedToHRExec.Select(x => x.OrderDate).FirstOrDefault();
                msgBody += "<br><b>Order Categories</b>: <br>";
                                
                msgBody += orderItemTable + "Best regards<br><br>"  + senderObject.KnownAs + ", " + senderObject.Position;

                var msg = new Message{
                    MessageType = "AssignTaskToHRExec", Content = msgBody, 
                    MessageComposedOn = _dateToday,  RecipientUsername = recipientObj.UserName,
                    RecipientEmail = recipientObj.Email, SenderUsername = senderObject.UserName, 
                    Subject = "Task to source complying Candidates"};
                msgs.Add(msg);
            }

            
            msgWithErr.Messages = msgs;

            return msgWithErr;
            
        }

    }
   
}