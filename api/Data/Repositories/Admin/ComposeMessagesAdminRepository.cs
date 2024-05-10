using api.DTOs.Admin;
using api.Entities.Admin.Client;
using api.Entities.Admin.Order;
using api.Entities.HR;
using api.Entities.Identity;
using api.Entities.Messages;
using api.Extensions;
using api.Interfaces.Admin;
using api.Params.Admin;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Repositories.Admin
{
    public class ComposeMessagesAdminRepository : IComposeMessagesAdminRepository
    {
        private const string _smsNewLine = "<smsbr>";
        private readonly DataContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IComposeMessagesForTypes _commonMsg;
        private readonly IEmployeeRepository _empRepo;
        private readonly IMapper _mapper;
        private readonly IConfiguration _confg;
        private readonly IQueryableRepository _queryRepo;
        public ComposeMessagesAdminRepository(DataContext context, UserManager<AppUser> userManager, 
             IComposeMessagesForTypes commonMsg, IEmployeeRepository empRepo, IMapper mapper, 
             IQueryableRepository queryRepo, IConfiguration confg)
        {
            _queryRepo = queryRepo;
            _confg = confg;
            _mapper = mapper;
            _empRepo = empRepo;
            _commonMsg = commonMsg;
            _userManager = userManager;
            _context = context;
        }

        public async Task<ICollection<Message>> ComposeSelectionStatusMessagesForCandidate(ICollection<SelectionMessageDto> selectionsDto, 
            string senderUsername)
          {  
                var senderObj = await _userManager.FindByNameAsync(senderUsername);
                if(senderObj == null) return null;
                var subject = "";
                var subjectInBody = "";
                var msgBody = "";
                var msgs = new List<Message>();
 
               foreach(var sel in selectionsDto)
               {
                    AppUser recipientObj, HRSupobj;

                    if(sel.CandidateAppUserId == 0) {
                        recipientObj = await _userManager.FindByEmailAsync(sel.CandidateEmail);
                    } else {
                        recipientObj = await _userManager.FindByIdAsync(sel.CandidateAppUserId.ToString());
                    }

                    if(recipientObj == null) continue;

                    if(sel.HRSupAppUserId == 0) {
                        HRSupobj = await _userManager.FindByEmailAsync(sel.HRExecEmail);
                    } else {
                        HRSupobj = await _userManager.FindByIdAsync(sel.HRSupAppUserId.ToString());
                    }

                    subject = "Your selection as " + sel.ProfessionName + " for " + sel.CustomerCity;
                    subjectInBody = "<b><u>Subject: </b>Your selection as " + sel.ProfessionName + " for " + 
                        sel.CustomerName + "</u>";
                    msgBody = string.Format("{0: dd-MMMM-yyyy}", DateTime.Today) + "<br><br>" + 
                         sel.CandidateTitle + " " + sel.CandidateName + "email: " + sel.CandidateEmail + "<br><br>" + 
                         "copy: " + HRSupobj.UserName + ", email: " + HRSupobj.Email + "<br><br>Dear " + 
                         sel.CandidateTitle + " " + sel.CandidateName + ":" + "<br><br>" + subject + "<br><br>";

                    msgBody += await GetEmailMessageBodyContents("selectionadvisetocandidate", 
                        sel.CandidateName, sel.ApplicationNo, sel.CustomerName, sel.ProfessionName, sel.Employment);
                    msgBody += "<br>Best Regards<br>HR Supervisor";
                    var candidateUsernameandemail = await _userManager.AppUserEmailAndUsernameFromAppUserId(
                        await _context.GetAppUserIdOfCandidate(sel.CandidateId));

                    var message = new Message
                    {
                        SenderUsername=senderUsername,
                        SenderEmail=senderObj.Email,
                        RecipientUsername = recipientObj.UserName,
                        RecipientEmail = recipientObj.Email,
                        CCEmail = HRSupobj.Email,
                        Subject = subject,
                        Content = msgBody,
                        MessageType = "SelectionAdvisebyemail",
                    };

                    msgs.Add(message);
               }

               if (msgs.Count > 0) return msgs;
               return null;
          }

        private async Task<string> GetEmailMessageBodyContents(string messageType, string candidatename, int applicationno, 
            string customername, string categoryname, Employment employment)
        {
            //MessageComposeSources contains collection of static text lines for each type of message.
            var msgLines = await _context.MessageComposeSources
                .Where(x => x.MessageType.ToLower() == messageType.ToLower()  && x.Mode == "mail")
                .OrderBy(x => x.SrNo).ToListAsync();
            
            var mailbody="";
            foreach (var m in msgLines)
            {
                    switch(messageType) {
                        case "selectionadvisetocandidate":
                            //if m.LineText equals "<tableofselectiondetails>", then it is a dynamic data, to be
                            //retreived from SelectionDecision, else accept the static data m.LineText
                            if(employment != null) {
                                mailbody += m.LineText == "<tableofselectiondetails>" 
                                ? _commonMsg.GetSelectionDetails(candidatename, applicationno, 
                                    customername, categoryname, employment)
                                : m.LineText;
                            }
                            break;
                        
                        default:
                            break;    
                    }
            }

            return mailbody;
        }         
        public async Task<SMSMessage> AdviseSelectionStatusToCandidateBySMS(SelectionDecision selection)
          {
               var candidate = await (from cvref in _context.CVRefs
                        where cvref.Id == selection.CVRefId
                        join cand in _context.Candidates on cvref.CandidateId equals cand.Id
                        select cand).FirstOrDefaultAsync();

               var candidateName = candidate.KnownAs;
               var title = candidate.Gender.ToLower() == "m" ? "Mr." : "Ms.";

               string msg = "";

               var mobileNo = candidate.UserPhones.Where(x => x.IsMain && x.IsValid).Select(x => x.MobileNo).FirstOrDefault();
               if (string.IsNullOrEmpty(mobileNo)) return null;

               msg = "Dear " + title + " " + candidateName + _smsNewLine + _smsNewLine;
               var msgssms = await _context.MessageComposeSources.Where(x => x.MessageType.ToLower() == "selectionadvisetocandidate" && x.Mode == "sms")
                    .OrderBy(x => x.SrNo).ToListAsync();
               foreach (var m in msgssms)
               {
                    msg += m.LineText == "<tableofselectiondetailssms>" ? 
                        _commonMsg.GetSelectionDetailsBySMS(selection) : m.LineText;
               }
               msg += msg + "<br><br>HR Supervisor";

               var smsMessage = new SMSMessage
               {
                    UserId = candidate.Id,
                    PhoneNo = mobileNo,
                    SMSText = msg
               };

               return smsMessage;
          }

        public async Task<ICollection<Message>> AdviseRejectionStatusToCandidateByEmail(ICollection<SelectionMessageDto> rejectionsDto, string senderUsername)
        {
            
            var senderObj = await _userManager.FindByNameAsync(senderUsername);
            if(senderObj == null) return null;
            
            var subject = "";
            var subjectInBody = "";
            var msgBody = "";
            var msgs = new List<Message>();
            AppUser HRSupObj, recipientObj;

            foreach(var rej in rejectionsDto)
            {
                    if(rej.CandidateAppUserId == 0) {
                        recipientObj = await _userManager.FindByEmailAsync(rej.CandidateEmail);
                    } else {
                        recipientObj = await _userManager.FindByIdAsync(rej.CandidateAppUserId.ToString());
                    }

                    if(recipientObj == null) continue;

                    if(rej.HRSupAppUserId == 0) {
                        HRSupObj = await _userManager.FindByEmailAsync(rej.HRExecEmail);
                    } else {
                        HRSupObj = await _userManager.FindByIdAsync(rej.HRSupAppUserId.ToString());
                    }


                    if(rej.CandidateAppUserId == 0) {
                        recipientObj = await _userManager.FindByEmailAsync(rej.CandidateEmail);
                    } else {
                        recipientObj = await _userManager.FindByIdAsync(rej.CandidateAppUserId.ToString());
                    }

                    if(recipientObj == null) continue;

                    subject = "Your cadidature as " + rej.ProfessionName + " for " + rej.CustomerCity + " is NOT approved";
                    subjectInBody = "<b><u>Subject: </b>Your candidature for " + rej.ProfessionName + " for " + rej.CustomerName + "</u>";
                    msgBody = string.Format("{0: dd-MMMM-yyyy}", DateTime.Today) + "<br><br>" + 
                            rej.CandidateTitle + " " + rej.CandidateName + "email: " + rej.CandidateEmail + "<br><br>" + 
                        "copy: " + HRSupObj.UserName + ", email: " + HRSupObj.Email + "<br><br>Dear " + rej.CandidateTitle + " " + rej.CandidateName + ":" +
                        "<br><br>" + subject + "<br><br>";

                    msgBody += "We regret to inform you that M/S " + rej.CustomerName + " have not approved of your candidature for the position " +
                            "of " + rej.ProfessionName + " giving following reason:<br><ul><li>" + rej.RejectionReason + "</li></ul><br>";
                    msgBody += "The rejection by the Customer is indicative of only their specific needs and does not reflect your suitabiity for the position in general. " +
                            "We will therefore be continuing to look for " +
                            "alternate opportunities for you and hope to revert to you as soon as possible.<br><br>" + 
                            "In case you do not want us to continue looking for opportunities for you, please do mark yourself as unavailable by cicking here " + 
                                "so as not to include your profile in our future searches<br><br>Best regards/HR Supervisor";
                    msgBody += "<br><br>This is a system generated message";
                    
                    msgBody += "<br>Best Regards<br>HR Supervisor";

                    var emailMessage = new Message
                    {
                        SenderUsername=senderUsername,
                        SenderEmail=senderObj.Email,
                        RecipientUsername = recipientObj.UserName,
                        RecipientEmail = recipientObj.Email + "; " + HRSupObj.Email ,       //TODO - HRExecEmail included in Recipient, as CC and BCC not working
                        Subject = subject,
                        Content = msgBody,
                        MessageType = "SelectionAdvisebyemail",
                    };

                msgs.Add(emailMessage);
            }

            if (msgs.Count > 0) return msgs;
            return null;
        }
        public async Task<SMSMessage> AdviseRejectionStatusToCandidateBySMS(SelectionDecision selection)
        {
            var candidate = await (from cvref in _context.CVRefs
                    where cvref.Id == selection.CVRefId
                    join cand in _context.Candidates on cvref.CandidateId equals cand.Id
                    select cand).FirstOrDefaultAsync();

            var candidateName = candidate.KnownAs;
            var title = candidate.Gender.ToLower() == "m" ? "Mr." : "Ms.";

            string msg = "";

            var mobileNo = candidate.UserPhones.Where(x => x.IsMain && x.IsValid).Select(x => x.MobileNo).FirstOrDefault();
            if (string.IsNullOrEmpty(mobileNo)) return null;

            msg = "Yr candidature for the position of " + selection.ProfessionName + _smsNewLine;
            var msgssms = await _context.MessageComposeSources.Where(x => x.MessageType.ToLower() == "rejectionadvisetocandidate" && x.Mode == "sms")
                .OrderBy(x => x.SrNo).ToListAsync();
            foreach (var m in msgssms)
            {
                msg += m.LineText;
            }
            msg = msg + "<br><br>HR Supervisor";

            var smsMessage = new SMSMessage
            {
                UserId = candidate.Id,
                PhoneNo = mobileNo,
                SMSText = msg
            };

            return smsMessage;
        }

        public async Task<Message> AckEnquiryToCustomer(Order order)
        {
            var customer = await _context.Customers.Where(x => x.Id == order.CustomerId)
                .Include(x => x.CustomerOfficials).FirstOrDefaultAsync();

            if (customer==null || customer.CustomerOfficials==null || customer.CustomerOfficials.Count()==0) 
                throw new Exception("failed to retrieve customer data for customer no. " + order.CustomerId);
            
            var OrderItems = order.OrderItems.OrderBy(x => x.SrNo).ToList();

            var projectManagerId = order.ProjectManagerId == 0 ? 8 : order.ProjectManagerId;

            EmployeeBriefDto projManager = _mapper.Map<EmployeeBriefDto>(await _empRepo.GetEmployeeFromEmpId(projectManagerId)) ?? throw new Exception("Project Manager for the DL undefined");

            string[] officialDepts = { "main contact", "hr", "accounts", "logistics" };
            CustomerOfficial official = null;
            foreach (var off in officialDepts)
            {
                official = customer.CustomerOfficials.Where(x => x.Divn?.ToLower() == off).FirstOrDefault();
                if (official != null) break;
            }

            official??=customer.CustomerOfficials.FirstOrDefault();

            bool HasException = false;
            var msg = DateTime.Now.Date + "<br><br>M/S" + customer.CustomerName;
            if (!string.IsNullOrEmpty(customer.Add)) msg += "<br>" + customer.Add;
            if (!string.IsNullOrEmpty(customer.Add2)) msg += "<br>" + customer.Add2;
            msg += "<br>" + customer.City + ", " + customer.Country + "<br><br>";
            msg += official == null ? "" : "Kind Attn : " + official.Title + official.OfficialName + ", " + official.Designation + "<br><br>";
            msg += "Dear " + official?.Gender == "F" ? "Madam:" : "Sir:" + "<br><br>";
            msg += "Thank you very much for your manpower enquiry dated " + order.OrderDate + " for following personnel: ";
            msg += "<br><br>" + _commonMsg.ComposeOrderItems(order.OrderNo, OrderItems, HasException) + "<br><br>";
            msg += HasException == true
                ? "Please note the exceptions mentioned under the column <i>Exceptions</i> and respond ASAP.  " +
                        "We will initiate execution of the wroks at this end on receipt of your clarificatins.<br><br>"
                : "We have initiated the works, and will revert to you soon with our delivery plan.<br><br>";
            msg += "Your point of contact for this order execution shall be the undersigned<br><br>";
            msg += "Please feel free to reach me for any clarification.<br><br>Best regards<br><br>" +
                projManager.FirstName + " " + projManager.FamilyName 
                    + "<br>" + projManager.Position + "<br>" + _confg.GetSection("IdealUserName").Value;
            msg += string.IsNullOrEmpty(projManager.OfficialPhoneNo) == true ? "" : "<br>Phone: " + projManager.OfficialPhoneNo;
            msg += string.IsNullOrEmpty(projManager.OfficialMobileNo) == true ? "" : "<br>Mobile: " + projManager.OfficialMobileNo;
            msg += string.IsNullOrEmpty(projManager.OfficialEmail) == true ? "" : "<br>Email: " + projManager.OfficialEmail;

            var senderEmailAddress = _confg["EmailSenderEmailId"] ?? "";
            var senderUserName = _confg["EmailSenderDisplayName"] ?? "";
            var recipientUserName = customer.CustomerName ?? "";
            var recipientEmailAddress = official?.Email ?? "";
            var ccEmailAddress = _confg["EmailCCandAck"] ?? "";
            var bccEmailAddress = _confg["EmailBCCandAck"] ?? "";
            var subject = "Your enquiry dated " + order.OrderDate + " is registered by us under Serial No. " + order.OrderNo;
            var messageTypeId = "OrderAcknowledgement";
            
            var emailMessage = new Message
            {
                SenderEmail = senderEmailAddress,
                SenderUsername = senderUserName,
                RecipientUsername = recipientUserName,
                RecipientEmail = recipientEmailAddress,
                Subject = subject,
                Content = msg,
                MessageType = messageTypeId,
                RecipientAppUserId = official.AppUserId,
                SenderAppUserId = projManager.AppUserId
            };

            
            return emailMessage;
        }

        public async Task<Message> ForwardEnquiryToHRDept(Order order)
        {
            string msg = "";
            var HRSup = _confg.GetSection("EmpHRSupervisorId").Value;
            int HRSupId = HRSup == null ? 0 : Convert.ToInt32(HRSup);
            var recipientObj =await _userManager.AppUserEmailAndUsernameFromAppUserId(
                await _context.GetAppUserIdOfEmployee(HRSupId));

            var senderObj = await _userManager.AppUserEmailAndUsernameFromAppUserId(
                await _context.GetAppUserIdOfEmployee(order.ProjectManagerId == 0 ? 1 : order.ProjectManagerId));

            var cust = await _context.CustomerBriefFromId(order.CustomerId);

            msg = DateTime.Now.Date + "<br><br>" + recipientObj.KnownAs + ", " + 
                recipientObj.Position + "<br>" +
                "<br>HR Supervisor<br>Email: " + recipientObj.Email + "<br><br>";
            
            if (order.ForwardedToHRDeptOn == null || ((DateTime)(order.ForwardedToHRDeptOn)).Date.Year < 200)
            {
                msg += "Following requirement is forwarded to you for execution within time periods shown:<br><br>";
            }
            else
            {
                msg += "Following requirement forwarded to you on " +
                        ((DateTime)order.ForwardedToHRDeptOn).Date + " <b><font color='blue'>is revised</font></b>as follows:<br><br>";
            }
            msg += "Order No.:" + order.OrderNo + " dated " + order.OrderDate +
                "<br>Customer:" + cust.CustomerName + ", " + cust.City + ", Place of work: " + order.CityOfWorking;
            msg += "<br><br>Overall Project Completion target: " + order.CompleteBy + "<br><br>";
            msg += "Requirement details are as follows.  For Job Description, click the relevant link<br>";

            var itemids = await _context.OrderItems.Where(x => x.OrderId == order.Id).Select(x => x.Id).ToListAsync();
            var tbl = _commonMsg.TableOfOrderItemsContractReviewedAndApproved(itemids);
            msg += tbl;
            msg += "<br><br>Task for this requirement is also assigned to you.<br><br>" + senderObj.KnownAs +
                "<br>Project Manager-Order" + order.OrderNo;

            var emailMsg = new Message {MessageType="forwardToHR", SenderAppUserId= senderObj.AppUserId,
                SenderUsername = senderObj.Username, SenderEmail = senderObj.Email, 
                RecipientAppUserId = recipientObj.AppUserId,
                RecipientUsername = recipientObj.Username, RecipientEmail= recipientObj.Email,
                Subject = "New Requirement No. " + order.OrderNo, Content = msg};
            
            return emailMsg;
        }
        public async Task<ICollection<Message>> ComposeCVFwdMessagesToClient(ICollection<CVFwdMsgDto> cvfwddtos, string Username)
        {
            DateTime dateTimeNow = DateTime.Now;
            var emails = new List<Message>();

            int lastOfficialId=0;
            int recipientappuserid=cvfwddtos.Select(x => x.OfficialAppUserId).FirstOrDefault();
            var recipientObj =await _userManager.AppUserEmailAndUsernameFromAppUserId(recipientappuserid);
            var senderObj = await _userManager.AppUserEmailAndUsernameFromAppUserId(
                cvfwddtos.Select(x => x.OfficialAppUserId).FirstOrDefault()
            );

            string msg="";
            int counter=0;

            string concludingMsg ="</table><br>Please review the CVs and provide us your feedback at the very earliest.<br><br>" +
                    "While we try to retain candidates as long as possible, due to the dynamic market conditions, " +
                    "candidates availability becomes volatile, and it is always preferable to keep candidates positively " +
                    "engaged.  While you may take a little longer to make up your minds for selections, the candidates " +
                    "that you are not interested in can be advised to us, so that they may be released for other opportunities.";

            concludingMsg += "While rejecting a profile, if you also advise us reasons for the rejection, it will help us " +
                    "adjust our criteria for shortlistings, which will ultimately help in minimizing rejections at your end.";
            
            concludingMsg +="<br><br>Regards<br><br>This is system generated message";

            var lastCVRef = new CVFwdMsgDto();

            var email = new Message();
     
            foreach(var cvref in cvfwddtos) // result)
            {
                if(cvref.OfficialAppUserId==0) continue;   //failed to get a custmer official
                if(lastOfficialId != cvref.OfficialId) {
                    if (lastOfficialId != 0) {
                        msg += concludingMsg;
                        email = new Message{
                            MessageType="cv forward", SenderUsername = Username, SenderEmail=senderObj.Email,
                            RecipientAppUserId = recipientappuserid, RecipientUsername=recipientObj.Username,
                            RecipientEmail=recipientObj.Email, Subject="CVs Forwarded against your requirement",
                            Content=msg
                        };
                        
                        emails.Add(email);
                    }
                    msg = dateTimeNow.Date.ToString("dd-MMM-yy") + "<br><br>"+  cvref.OfficialTitle + " " + cvref.OfficialName + ", " + 
                        cvref.Designation + "<br>M/S " + cvref.CustomerName + ", " + cvref.City + "<br>Email:" + cvref.OfficialEmail +
                        "<br><br>Dear Sir:<br><br>We are pleased to enclose following CVs for your consideration against your requirements mentioned:<br>" +
                        "<table border='1'><tr><th width=10%>Order ref and dated</th><th width=20%>Category</th><th width=5%>Application<br>No</th>" + 
                        "<th width=20%>Candidate Name</th><th width=5%>Passport No</th><th width=15%>Attachments</th><th width=5%>Forwarded<br>so far</th>" + 
                        "<th width=10%>Our assessment<br>Grade</th><th width=10%>Salary Expectation</th></tr>";
                    counter=0;
                }

                msg += "<tr><td>" + cvref.OrderNo +"-"+ cvref.ItemSrNo + "/<br>" + cvref.OrderDated.ToString("ddMMMyy") + "</td><td>" +
                    cvref.CategoryName + "</td><td>" + cvref.ApplicationNo + "</td><td>" + cvref.CandidateName + 
                    "</td><td>"+ cvref.PPNo + "</td><td></td><td>" + cvref.CumulativeSentSoFar + "</td><td>" + 
                    cvref.AssessmentGrade + "</td></tr>";
                
                counter+=1;
                lastOfficialId=cvref.OfficialId;
                lastCVRef=cvref;
            }
            
            msg += concludingMsg;
            email = new Message{
                MessageType="cv forward", SenderUsername = Username, SenderEmail=senderObj.Email,
                RecipientAppUserId = recipientappuserid, RecipientUsername=recipientObj.Username,
                RecipientEmail=recipientObj.Email, Subject="CVs Forwarded against your requirement",
                Content=msg
            };

            emails.Add(email);

            return emails;
        }

        public async Task<Message> ComposeSelDecisionRemindersToClient(int CustomerId, string Username)
        {
            var refParam = new CVRefParams();
            refParam.CustomerId = CustomerId;
            refParam.RefStatus = "Referred";

            var QueryableQuery =  _queryRepo.GetCustomerAndOfficialQueryable(CustomerId, "HR");
            var custAndOfficialDto = await QueryableQuery.FirstOrDefaultAsync();
            
            var query = await _queryRepo.GetCVReDtoQueryable(refParam);
            var cvfwddtos = await query.ToListAsync();

            DateTime dateTimeNow = DateTime.Now;
            
            var senderAppUserDetails = await _userManager.AppUserEmailAndUsernameFromAppUsername(Username);
            var recipientAppUserDetails = await _userManager.AppUserEmailAndUsernameFromAppUserId(
                custAndOfficialDto.AppUserId);

            string msg=dateTimeNow + "<br><br>" + custAndOfficialDto.OfficialTitle + " " + custAndOfficialDto.OfficialName
                + "<br>" + custAndOfficialDto.OfficialDesignation +"<br>" + custAndOfficialDto.CustomerName + 
                "<br>" + custAndOfficialDto.City + "<br>" + custAndOfficialDto.Country + "<br><br>Dear Sir:<br><br>";

            msg +="Following profiles are awaiting your decision on their selection.  " +
                "Pending your decision, we are doing our best to retain their interest.<br><br>" +
                "engaged.<br><br>Candidates awaiting your decision:<table>";

            foreach(var cvref in cvfwddtos) // result)
            {
                msg += "<tr><td>" + cvref.ReferredOn.ToString("ddMMMyy") + "</td><td>" + cvref.OrderNo +"-"+ cvref.SrNo + 
                    "</td><td>" + cvref.OrderDate.ToString("ddMMMyy") + "</td><td>" +
                    cvref.ProfessionName + "</td><td>" + cvref.ApplicationNo + "</td><td>" + cvref.CandidateName + 
                    "</td><td>"+ cvref.PPNo + "</td><td></td><td>" + "</td></tr>";
            }
            
            msg += "If a Profile is not selected by you, it would be immensely helpful to us if you let us know in brief " +
                "reasons for the rejection (for example, lacks relevant exp, lacks relevant qualification, lacks sufficient exp, " +
                "over age, high salary expectation, Not proficient in languages, etc. it will help us " +
                "adjust our criteria for further shortlistings, which will ultimately help in minimizing rejections at your end.";
            
            msg +="<br><br>We thank you for the opportunity to serve you, and assure you of our best and prompt services, always!" +
                "<br><br>Best regards<br><br>" + Username;


            var message = new Message
            {
                MessageType = "SelectionReminderToClient",
                SenderAppUserId = senderAppUserDetails.AppUserId,
                SenderUsername = senderAppUserDetails.Username, SenderEmail = senderAppUserDetails.Email,
                RecipientAppUserId = recipientAppUserDetails.AppUserId, RecipientUsername = recipientAppUserDetails.Username,
                RecipientEmail = recipientAppUserDetails.Email, Subject = "Request for decision on selection of Profiles",
                Content = msg
            };

            return message;
        }

    }
}