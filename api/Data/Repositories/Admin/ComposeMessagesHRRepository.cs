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

        public async Task<string> ComposeEmailMsgForDLForwardToHRHead(ICollection<OrderItemBriefDto> OrderItems, 
            string senderUsername, string recipientUsername)
        {
            var dto = OrderItems.Select(x => new {x.CustomerName, x.AboutEmployer, x.OrderNo, x.OrderDate}).FirstOrDefault();
            var senderObject = await _userManager.FindByNameAsync(senderUsername);
            var recipientObject = await _userManager.FindByNameAsync(recipientUsername);

            string AboutEmployer = dto!.AboutEmployer;
            string CustomerName = dto.CustomerName;

            string Subject = "Order No.:" + dto.OrderNo + " dated " + dto.OrderDate + " from " + CustomerName + " availabe for your work";

            var TableBody = ComposeCategoryTableForEmail(OrderItems);
            
            string IntroductoryBody = "Following Requirement is tasked to you. <br><br><b>Country of requirement</b>:";
            
            var msgBody = _dateToday + "<br><br>" + recipientObject.KnownAs + ", " + 
                recipientObject.Position + "<br>eMail: " + recipientObject.Email + 
                "<br>Mobile:" + recipientObject.PhoneNumber  + "<br><br>Dear Sir: <br><br>" +
                "<br>Employer Known As: " + CustomerName + "<br>About Employer: " + AboutEmployer + 
                "<br><br>" + IntroductoryBody + "<br><br>Requirement details<br>:";

            msgBody +=  TableBody +  "<br><br>Regards<br><br>" + senderObject.KnownAs + "/" + 
                senderObject.Position + "<br>Phone:" + senderObject.PhoneNumber ?? "undefined";
            msgBody += "<br><b>" + _RAName + "</b>";
            
            var msg =new Message{MessageType = "AdviseToHRDeptHead", 
                SenderAppUserId = senderObject.Id, RecipientAppUserId = recipientObject.Id, 
                RecipientEmail = recipientObject.Email, SenderUsername = senderObject.UserName, 
                RecipientUsername = recipientObject.UserName, Subject = Subject, Content = msgBody};
            
            _context.Messages.Add(msg);
            //_context.Entry(msg).State = EntityState.Added;

            return await _context.SaveChangesAsync() > 0 ? "" : "Failed to save the email message to database";
            
        }

        public Task<Message> ComposeHTMLToAckToCandidateByEmail(Candidate candidate)
            {
                throw new NotImplementedException();
            }

        public Task<Message> ComposeHTMLToPublish_CVReadiedToForwardToClient(ICollection<CommonDataDto> commonDataDtos, string Username, int recipientAppUserId)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<Message>> ComposeMsgsToForwardOrdersToAgents(OrderForwardToAgent dlforward, 
            string Username)
        {
            var msgData = new List<OrderItemBriefDtoWithOfficialIdsDto>();
            
            var projManagerId = dlforward.ProjectManagerId;
            if(projManagerId == 0) return null;
            var projManagerAppUserId =  await _context.GetAppUserIdOfEmployee(projManagerId);
            if(projManagerAppUserId == 0) projManagerAppUserId = Convert.ToInt32(_config["HRSupAppuserId"] ?? "0");
            
            var fwdOfficials = new List<OrderForwardCategoryOfficial>();
            
            var msgs = new List<Message>();

            foreach(var itm in dlforward.OrderForwardCategories)
            {
                foreach(var dt in itm.OrderForwardCategoryOfficials)
                {
                    fwdOfficials.Add(dt);
                }
            }

            fwdOfficials = fwdOfficials.Distinct().ToList();

            var distinctOrderItemIds = dlforward.OrderForwardCategories.Select(x => x.OrderItemId).Distinct().ToList();
            //queries that has to use expressions at client level - like distinctOrderItemIds.contains(xx) - 
            //are not translated by EF Core.  The query ahs to be redesigned

            var item = new OrderItemBriefDto();
            var ItemBriefDtos = new List<OrderItemBriefDto>();

            var orderAndItems = await _context.Orders
                .Include(x => x.OrderItems).ThenInclude(x => x.JobDescription)
                .Include(x => x.OrderItems).ThenInclude(x => x.Remuneration)
                .Where(x => x.Id == dlforward.OrderId).FirstOrDefaultAsync();
    
            foreach(var itm in orderAndItems.OrderItems) {
                item = new OrderItemBriefDto {
                    OrderItemId = itm.Id, AboutEmployer = orderAndItems.Customer?.Introduction,
                     CustomerId = orderAndItems.CustomerId, CustomerName = orderAndItems.Customer?.CustomerName,
                      Ecnr = itm.Ecnr, CompleteBefore = itm.CompleteBefore, JobDescription = itm.JobDescription,
                      Remuneration = itm.Remuneration, OrderDate = orderAndItems.OrderDate,
                       OrderId=orderAndItems.Id, OrderNo = orderAndItems.OrderNo, ProfessionId=itm.ProfessionId,
                       ProfessionName = itm.Profession?.ProfessionName, Quantity=itm.Quantity, SrNo=itm.SrNo,
                    Status = itm.Status
                };
                ItemBriefDtos.Add(item);
            }

            foreach(var itm in ItemBriefDtos) {
                if(string.IsNullOrEmpty(itm.CustomerName)) itm.CustomerName = await _context.CustomerNameFromId(itm.CustomerId);
                if(string.IsNullOrEmpty(itm.ProfessionName)) itm.ProfessionName = await _context.GetProfessionNameFromId(itm.ProfessionId);
            }

            var OfficialIdsForEmails = fwdOfficials.Where(x => !string.IsNullOrEmpty(x.EmailIdForwardedTo))
                .Select(x => x.CustomerOfficialId).ToList();
            
            var OfficialIdsForSMS  = fwdOfficials.Where(x => !string.IsNullOrEmpty(x.PhoneNoForwardedTo) )
                .Select(x => x.CustomerOfficialId).ToList();
            var OfficialIdsForWhatsApp = fwdOfficials.Where(x => !string.IsNullOrEmpty(x.WhatsAppNoForwardedTo))
                .Select(x => x.CustomerOfficialId).ToList();
            
            string subject = "Requirements under Order No.: " + dlforward.OrderNo + " dated " + 
                    dlforward.OrderDate + dlforward.CustomerCity;

            msgs = (List<Message>)await ComposeEmailMsgForOrderForwards(ItemBriefDtos, subject, fwdOfficials, Username);

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
                    RecipientAppUserId = recipientObj.Id, RecipientEmail = recipientObj.Email, SenderAppUserId = senderObj.Id,
                        SenderUsername = senderObj.UserName, Subject = "Designing of Order Item Assessment Questions"};
            var msgs = new List<Message> {msg};
            msgWithErr.Messages = msgs;

            return msgWithErr;
        }

        private async Task<ICollection<Message>> ComposeEmailMsgForOrderForwards(ICollection<OrderItemBriefDto> OrderItems, 
            string subject, ICollection<OrderForwardCategoryOfficial> fwdOfficials, string SenderUsername)
        {
            
            var TableBody = ComposeCategoryTableForEmail(OrderItems);

            var msgs = new List<Message>();
            string IntroductoryBody = "We have following requirements.  If you have friends interested in the opportunity, please ask them to submit their profiles " + 
                "to us along with copies of certificates and testimonials. <br><br><b>Country of requirement</b>:";

            _ = fwdOfficials.OrderBy(x => x.CustomerOfficialId).Distinct();

            var senderObj = await _userManager.FindByNameAsync(SenderUsername);
            if(senderObj == null) return null;

            foreach(var fwd in fwdOfficials)
            {
                var off = await _custRepo.GetCustomerOfficialDto(fwd.CustomerOfficialId);
                if(off == null) continue;
                var recipientAppUserId = await _context.OfficialAppUserIdFromOfficialId(off.OfficialId);
                if(recipientAppUserId == 0) continue;
                var recipientObj = await _userManager.FindByIdAsync(recipientAppUserId.ToString());

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
                    RecipientAppUserId = recipientObj.Id, RecipientEmail = recipientObj.Email, SenderAppUserId = senderObj.Id,
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
            string TableBody = "<Table><TH>Sr No</TH><TH width='75'>Cat Ref</TH><TH width='300'>Category Name</TH><TH width='40'>Quantity</TH><TH width='350'>Job Description</TH><TH width='350'>Remuneration</TH><TH width='150'>Remarks</TH>";
            string jd = "";
            string remun = "";
            foreach (var item in orderItems)
            {
                if (item.JobDescription != null)
                {
                    if (!string.IsNullOrEmpty(item.JobDescription.JobDescInBrief)) jd = item.JobDescription.JobDescInBrief;
                    if (item.JobDescription.MaxAge > 0) jd += "Max Age:" + item.JobDescription.MaxAge + " yrs";
                    if (item.JobDescription.ExpDesiredMin > 0) jd += "Exp: " + item.JobDescription.ExpDesiredMin;
                    if (item.JobDescription.ExpDesiredMax > 0) jd += " - " + item.JobDescription.ExpDesiredMax;
                    if (item.JobDescription.ExpDesiredMin > 0 || item.JobDescription.ExpDesiredMax > 0) jd += " yrs.";
                    if (!string.IsNullOrEmpty(item.JobDescription.QualificationDesired)) jd += " Qualification: " + item.JobDescription.QualificationDesired;
                }

                if (item.Remuneration != null)
                {
                    remun = "Salary: ";
                    remun += string.IsNullOrEmpty(item.Remuneration.SalaryCurrency) ? "" : item.Remuneration.SalaryCurrency;
                    remun += item.Remuneration.SalaryMin > 0 ? item.Remuneration.SalaryMin : "";
                    remun += item.Remuneration.SalaryMax > 0 ? "-" + item.Remuneration.SalaryMax : "";
                    remun += "; Housing: ";
                    remun += item.Remuneration.HousingProvidedFree ? "Free" :
                        item.Remuneration.HousingAllowance > 0 ? item.Remuneration.HousingAllowance : "Not provided";
                    remun += "; Food: ";
                    remun += item.Remuneration.FoodAllowance > 0 ? item.Remuneration :
                        item.Remuneration.FoodProvidedFree ? "Provided Free" : "Not Provided";
                    remun += "; Transport: ";
                    remun += item.Remuneration.TransportProvidedFree ? "Provided Free" :
                        item.Remuneration.TransportAllowance > 0 ? item.Remuneration.TransportAllowance : "Not Provided";
                    remun += "; Medical Facilities: As per labour laws;";
                    if (item.Remuneration.OtherAllowance > 0) remun += "; Other Allowance: " + item.Remuneration.OtherAllowance;

                }
                TableBody += "<TR><TD>" + ++srno + "</TD><TD>" + item.OrderNo + "-" + item.SrNo + "-" + 
                    item.ProfessionName + "</TD><TD>" + item.ProfessionName + "</TD><TD>" + item.Quantity + "</TD>" +
                    "<TD></TD><TD></TD></TR>";
            }

            TableBody += "</TABLE><BR>";

            return TableBody;
        }

        private async Task<ICollection<Message>> ComposeHTMLForwardsToAgents(OrderItemsAndAgentsToFwdDto itemsAndAgents, 
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

        public Task<MessageWithError> ComposeMsgToHRExecForTaskAssignment(ICollection<OrderAssignmentDto> hrassignments, string Username)
        {
            throw new NotImplementedException();
        }

        public async Task<MessageWithError> ComposeMessagesToHRExecToSourceCVs(ICollection<OrderItemIdAndHRExecEmpNoDto> ItemAndHRExecIds, string Username)
        {
            var msgWithErr = new MessageWithError();
            var msgs = new List<Message>();

            var dtos = await (from o in _context.Orders 
                join item in _context.OrderItems on o.Id equals item.OrderId
                    where ItemAndHRExecIds.Select(x => x.OrderItemId).ToList().Contains(item.Id)
                select new OrderItemBriefDto{
                    OrderNo = o.OrderNo, OrderDate = o.OrderDate, CustomerName = o.Customer.CustomerName,
                    AboutEmployer = o.Customer.Introduction, CustomerId = o.CustomerId, SrNo = item.SrNo,
                     CompleteBefore = item.CompleteBefore, ProfessionId = item.ProfessionId,
                     ProfessionName = item.Profession.ProfessionName, Ecnr = item.Ecnr, OrderId = o.Id,
                     OrderItemId = item.Id, Quantity = item.Quantity
                }).ToListAsync();
            
            var empIds = ItemAndHRExecIds.Select(x => x.HrExecEmpId).ToList();

            var emps = await _context.Employees.Where(x => empIds.Contains(x.Id))
                .Select(x => new {EmployeeId=x.Id, AppUserId=x.AppUserId}).ToListAsync();

            var assignmenDtos = _mapper.Map<ICollection<OrderItemAssignmentDto>>(dtos);

            foreach(var dto in  assignmenDtos) {
                var empId = ItemAndHRExecIds.Where(x => x.OrderItemId == dto.OrderItemId).Select(x => x.HrExecEmpId).FirstOrDefault();
                var appUserId = await _context.GetAppUserIdOfEmployee(empId);
                var appUser = await _userManager.FindByIdAsync(appUserId.ToString());

                dto.AssignedToAppUserId = appUser.Id;
                dto.AssignedToEmpId=empId;
                dto.AssignedToUsername = appUser.UserName;
            }
            
            var itemAssignmentDtos = assignmenDtos.OrderBy(x => x.AssignedToEmpId).ThenBy(x => x.OrderItemId).ToList();

            var distinctEmpIds = ItemAndHRExecIds.Select(x => x.HrExecEmpId).Distinct().ToList();
            var msgBody="";
            foreach(var empId in distinctEmpIds) {
                var appuserid = emps.Where(x => x.EmployeeId == empId).Select(x => x.AppUserId).FirstOrDefault();
                var recipientObj = await _userManager.FindByIdAsync(appuserid.ToString());
                if(recipientObj==null) continue;

                msgBody = _dateToday + "<br><br>" + recipientObj.Gender == "Male" ? "Mr." : "Ms.";
                msgBody += recipientObj.KnownAs + "<br>" + recipientObj.Position + ", email:" + recipientObj.Email;
                msgBody +="<br><br>Following Order Categories are assigned to you. Job Descriptions and Remunertions can be " +
                    "viewed on the links provided. For any clarification, please approach your Supervisor. " +
                    "<br><br><b>Details of the Order Categories</b><br><br>:" +
                    "<b>Order No.</b>: " + 
                    "<br><b>Order Categories</b><br>:";
                
                var orderitems = assignmenDtos.Where(x => x.AssignedToEmpId == empId).ToList();
                var orderitemsBriefDto = _mapper.Map<ICollection<OrderItemBriefDto>>(orderitems);
                var orderitemTable = ComposeCategoryTableForEmail(orderitemsBriefDto);

                var senderObject = await _userManager.FindByIdAsync(_config["DocControllerAppUserId"]);
                msgBody += orderitemTable + ".  Best regards<br><br>"  + senderObject.KnownAs + ", " + senderObject.Position;

                var msg = new Message{MessageType = "DesignOrderItemAssessmentQ", Content = msgBody, 
                    MessageComposedOn = _dateToday,  RecipientUsername = recipientObj.UserName,
                    RecipientAppUserId = recipientObj.Id, RecipientEmail = recipientObj.Email, 
                    SenderAppUserId = senderObject.Id, SenderUsername = senderObject.UserName, 
                    Subject = "Task to source complying Candidates"};
                msgs.Add(msg);
            }

            msgWithErr.Messages = msgs;

            return msgWithErr;
        }
    }
}