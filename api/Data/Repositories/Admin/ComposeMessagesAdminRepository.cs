using api.DTOs.Admin;
using api.Entities.Admin.Client;
using api.Entities.Admin.Order;
using api.Entities.HR;
using api.Entities.Identity;
using api.Entities.Messages;
using api.Extensions;
using api.Interfaces;
using api.Interfaces.Admin;
using api.Interfaces.Messages;
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
        private readonly DateTime _today = DateTime.UtcNow;
        private readonly ICustomerRepository _custRepo;
        public ComposeMessagesAdminRepository(DataContext context, UserManager<AppUser> userManager, 
             IComposeMessagesForTypes commonMsg, IEmployeeRepository empRepo, IMapper mapper, 
             IQueryableRepository queryRepo, IConfiguration confg, ICustomerRepository custRepo)
        {
            _custRepo = custRepo;
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
                    var candidateAppUserId = await _context.GetAppUserIdOfCandidate(sel.CandidateId);
                    var recipientObj = await _userManager.FindByIdAsync(candidateAppUserId.ToString());
                    if(recipientObj == null) continue; 

                    var HRSupobj = await _userManager.FindByIdAsync(_confg["HRSupAppUserId"]);

                    subject = "Your selection as " + sel.ProfessionName + " for " + sel.CustomerCity;
                    subjectInBody = "<b><u>Subject: </b>Your selection as " + sel.ProfessionName + " for " + 
                        sel.CustomerName + "</u>";
                    msgBody = string.Format("{0: dd-MMMM-yyyy}", _today) + "<br><br>" + 
                         sel.CandidateTitle + " " + sel.CandidateName + "email: " + sel.CandidateEmail ?? "" + "<br><br>" + 
                         "copy: " + HRSupobj?.UserName?? "" + ", email: " + HRSupobj?.Email?? "" + "<br><br>Dear " + 
                         sel.CandidateTitle + " " + sel.CandidateName + ":" + "<br><br>" + subject + "<br><br>";

                    msgBody += await GetEmailMessageBodyContents("selectionadvisetocandidate", 
                        sel.CandidateName, sel.ApplicationNo, sel.CustomerName, sel.ProfessionName, sel.Employment);
                    msgBody += "<br>Best Regards<br>HR Supervisor";
                    var candidateUsernameandemail = await _userManager.AppUserEmailAndUsernameFromAppUserId(
                        await _context.GetAppUserIdOfCandidate(sel.CandidateId));

                    var message = new Message
                    {
                        SenderUsername=senderUsername,
                        //RecipientAppUserId=recipientObj.Id,
                        //SenderAppUserId=senderObj.Id,
                        SenderEmail=senderObj.Email ?? "",
                        RecipientUsername = recipientObj.UserName ?? "",
                        RecipientEmail = recipientObj.Email ?? "",
                        CCEmail = HRSupobj?.Email ?? "",
                        Subject = subject,
                        Content = msgBody,
                        MessageType = "SelectionAdvisebyemail",
                        MessageComposedOn = _today
                    };

                    msgs.Add(message);
               }

               if (msgs.Count > 0) return msgs;
               return null;
          }


        public async Task<MessageWithError> ComposeAcceptanceReminderToCandidates(ICollection<SelectionMessageDto> selectionsDto, string senderUsername)
        {
            var msgWithErr = new MessageWithError();
            var senderObj = await _userManager.FindByNameAsync(senderUsername);
            
            var subject = "";
            var subjectInBody = "";
            var msgBody = "";
            var msgs = new List<Message>();
            var HRSupobj = await _userManager.FindByIdAsync(_confg["HRSupAppUserId"]);  
            var RAName =  _confg["RAName"]; 
            foreach(var sel in selectionsDto)
            {
                var candidateAppUserId = await _context.GetAppUserIdOfCandidate(sel.CandidateId);
                if(candidateAppUserId ==0) {
                    msgWithErr.Notification += "Failed to get AppUserId of the Candidate " + sel.CandidateName + ".  This is required to define the recipient of message";
                    continue;
                }

                var recipientUserNmAndEmail = await _userManager.AppUserEmailAndUsernameFromAppUserId(candidateAppUserId);
                if(recipientUserNmAndEmail == null) {
                    msgWithErr.Notification += "Failed to get Username and Email of Candidate " + sel.CandidateName + ".  This is required to define the recipient of message";
                    continue;
                }

                subject = "Subject: Your selection as " + sel.ProfessionName + " for " + sel.CustomerCity;
                subjectInBody = "<b><u>Subject: Your selection as " + sel.ProfessionName + " for " + 
                    sel.CustomerName + " for " + sel.CustomerCity + "</b></u>";
                msgBody = string.Format("{0: dd-MMMM-yyyy}", _today) + "<br><br>" + 
                        sel.CandidateTitle + " " + sel.CandidateName + "<br>email: " + recipientUserNmAndEmail.Email ?? "";
                if(string.IsNullOrEmpty(HRSupobj.UserName)) msgBody += "<br><br>copy: " + HRSupobj?.Email?? "";
                msgBody += "<br><br>Dear " + sel.CandidateTitle + " " + sel.CandidateName + ":";
                msgBody += "<br><br>" + subjectInBody + "<br><br>";

                msgBody += await GetEmailMessageBodyContents("acceptanceremindertocandidate", 
                    sel.CandidateName, sel.ApplicationNo, sel.CustomerName, sel.ProfessionName, sel.Employment);
                msgBody += "<br><br>Best Regards<br><br>HR Supervisor<br>" + RAName;
                var recipientAppUserId = Convert.ToString(await _context.GetAppUserIdOfCandidate(sel.CandidateId));
                var recipientObj = await _userManager.FindByIdAsync(recipientAppUserId);

                var message = new Message
                {
                    //Sender = senderObj,
                    //Recipient = recipientObj,
                    SenderUsername=senderObj.UserName,
                    RecipientUsername = recipientObj.UserName ?? "",
                    //RecipientAppUserId=recipientObj.Id,
                    //SenderAppUserId=senderObj.Id,
                    SenderEmail=senderObj.Email ?? "",
                    RecipientEmail = recipientObj.Email ?? "",
                    CCEmail = HRSupobj?.Email ?? "",
                    Subject = subject,
                    Content = msgBody,
                    MessageType = "acceptanceremindertocandidate",
                    MessageComposedOn = _today
                };

                msgs.Add(message);
            }

            msgWithErr.Messages=msgs;

            return msgWithErr;
        }

 
        private async Task<string> GetEmailMessageBodyContents(string messageType, string candidatename, int applicationno, 
            string customername, string categoryname, Employment employment)
        {
            //MessageComposeSources contains collection of text lines for each type of message.
            var msgLines = await _context.MessageComposeSources
                .Where(x => x.MessageType.ToLower() == messageType.ToLower()  && x.Mode == "mail")
                .OrderBy(x => x.SrNo).ToListAsync();
            
            var mailbody="";
            foreach (var m in msgLines)
            {
                    //if m.LineText equals "<tableofselectiondetails>", then it is a dynamic data, to be
                    //retreived from SelectionDecision, else accept the static data m.LineText
                    switch(messageType) {
                        case "acceptanceremindertocandidate" :
                            if(employment != null) {
                                mailbody += m.LineText == "<tableofselectiondetails>" 
                                ? _commonMsg.GetSelectionDetails(candidatename, applicationno, 
                                    customername, categoryname, employment)
                                : m.LineText;
                            }
                            break;
                        case "selectionadvisetocandidate":
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
                        where cvref.Id == selection.CvRefId
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

        public async Task<MessageWithError> AdviseRejectionStatusToCandidateByEmail(ICollection<SelectionMessageDto> rejectionsDto, string senderUsername)
        {
            var returnDto = new MessageWithError();

            var senderObj = await _userManager.FindByNameAsync(senderUsername);
            if(senderObj == null) {
                returnDto.ErrorString = "Unable to retrieve HR Exec User Object";
                return returnDto;
            }

            AppUser recipientObj;

            string subject, subjectInBody, msgBody;
            
            var msgs = new List<Message>();
           
            foreach(var rej in rejectionsDto)
            {
                    recipientObj = await _userManager.FindByIdAsync(rej.CandidateAppUserId.ToString());
                    
                    if(recipientObj == null) {
                        returnDto.ErrorString="Unable to retrieve Candidate User Object";
                        continue;
                    }
                    
                    subject = "<b><u>Subject: Your candidature as " + rej.ProfessionName + " for " + rej.CustomerCity + " is NOT approved</u></b>";
                    subjectInBody = "<b><u>Subject: </b>Your candidature for " + rej.ProfessionName + " for " + rej.CustomerName + "</u>";
                    msgBody = string.Format("{0: dd-MMMM-yyyy}", _today) + "<br><br>" + 
                            rej.CandidateTitle + " " + rej.CandidateName + "<br>email: " + rej.CandidateEmail + "<br><br>" + 
                        "copy: " + "" + ", email: " + "" + "<br><br>Dear " + rej.CandidateTitle + " " + rej.CandidateName + ":" +
                        "<br><br>" + subject + "<br><br>";

                    msgBody += "We regret to inform you that M/S " + rej.CustomerName + " have not approved of your candidature for the position " +
                            "of " + rej.ProfessionName + " giving following reason:<br><ul><li>" + rej.SelectionStatus + "</li></ul>";
                    msgBody += "The rejection by the Customer is indicative of only their specific needs for the position and does not reflect your suitabiity for the position in general. " +
                            "We will therefore be continuing to look for " +
                            "alternate opportunities for you and hope to revert to you as soon as possible.<br><br>" + 
                            "In case you do not want us to continue looking for opportunities for you, please do mark yourself as unavailable by cicking here " + 
                                "so as not to include your profile in our active search profioes for the position<br><br>Best regards/HR Supervisor";
                    msgBody += "<br><br>This is a system generated message";
                    
                    msgBody += "<br>Best Regards<br>HR Supervisor";

                    var emailMessage = new Message
                    {
                        //Sender = senderObj,
                        //Recipient = recipientObj,
                        SenderUsername=senderObj.UserName,
                        SenderEmail=senderObj.Email,
                        RecipientUsername = recipientObj.UserName,
                        RecipientEmail = recipientObj.Email,       //TODO - HRExecEmail included in Recipient, as CC and BCC not working
                        MessageComposedOn = _today,
                        Subject = subject,
                        Content = msgBody,
                        MessageType = "SelectionAdvisebyemail",
                    };

                msgs.Add(emailMessage);
            }

            returnDto.Messages = msgs;
            
            return returnDto;
        }
        public async Task<SMSMessage> AdviseRejectionStatusToCandidateBySMS(SelectionDecision selection)
        {
            var candidate = await (from cvref in _context.CVRefs
                    where cvref.Id == selection.CvRefId
                    join cand in _context.Candidates on cvref.CandidateId equals cand.Id
                    select cand).FirstOrDefaultAsync();

            var candidateName = candidate.KnownAs;
            var title = candidate.Gender.ToLower() == "m" ? "Mr." : "Ms.";

            string msg = "";

            var mobileNo = candidate.UserPhones.Where(x => x.IsMain && x.IsValid).Select(x => x.MobileNo).FirstOrDefault();
            if (string.IsNullOrEmpty(mobileNo)) return null;

            msg = "Yr candidature for the position of " + selection.SelectedAs + _smsNewLine;
            var msgssms = await _context.MessageComposeSources.Where(x => x.MessageType.ToLower() == "rejectionadvisetocandidate" && x.Mode == "sms")
                .OrderBy(x => x.SrNo).ToListAsync();
            foreach (var m in msgssms)
            {
                msg += m.LineText;
            }
            msg += "<br><br>HR Supervisor";

            var smsMessage = new SMSMessage
            {
                UserId = candidate.Id,
                PhoneNo = mobileNo,
                SMSText = msg
            };

            return smsMessage;
        }

        public async Task<MessageWithError> AckEnquiryToCustomerWithoutSave(Order order)
        {
            var msgWithErr = new MessageWithError();

            var acknowledged = await _context.AckanowledgeToClients.Where(x => x.OrderId == order.Id).FirstOrDefaultAsync();

            if(acknowledged != null) {
                msgWithErr.ErrorString = "This Order has already been acknowledged on " + acknowledged.DateAcknowledged;
                return msgWithErr;
            }
            
            var obj = await _context.Orders.Include(x => x.OrderItems.OrderBy(x => x.SrNo))
                .Where(x => x.Id == order.Id)
                .FirstOrDefaultAsync();
            
            if (obj==null) {
                msgWithErr.ErrorString = "failed to retrieve Order data for customer";
                return msgWithErr;
            }
                
            var OrderItems = obj.OrderItems;

            var officials = await _context.CustomerOfficials.Where(x => x.CustomerId == obj.CustomerId).ToListAsync();
            if(officials == null || officials.Count == 0) {
                msgWithErr.ErrorString = "Customer Officials not defined";
                return msgWithErr;
            }

            string[] officialDepts = { "main contact", "hr", "accounts", "logistics" };
            CustomerOfficial official=null;
            
            foreach(var dept in officialDepts) {
                official = officials.Where(x => dept.ToLower() == dept).FirstOrDefault();
                if(official!=null) break;    
            }
            if(official==null) {
                msgWithErr.ErrorString = "Failed to locate Customer Official with the required departments";
                return msgWithErr;
            }

            var projectManagerId = obj.ProjectManagerId == 0 ? 8 : order.ProjectManagerId;
            var senderObj = await _userManager.GetAppUserObjFromEmployeeId(_context, projectManagerId);
            var recipientObj = await _userManager.GetAppUserObjFromCustomerOfficial(_context, official.Id);

            if(senderObj==null) {
                msgWithErr.ErrorString = "The acknowledgement for the Demand Letter will be authored by its Project Manager, " 
                    + "but the Project Manager is not defined, or if the Project Manager is defined, its Identity User is not defied";
                return msgWithErr;
            }
            if(recipientObj==null) {
                msgWithErr.ErrorString = "The acknowledgement for the Demand Letter will be addressed to the Customer Official, " +
                    "who is either not defined, or its Identity User not defined";
                return msgWithErr;
            }
            
            var messageType = "OrderAcknowledgement";
            var custName = obj.Customer?.CustomerName ?? await _context.CustomerNameFromId(obj.CustomerId);
            var ackn = new AcknowledgeToClient{OrderId = obj.Id, CustomerId = obj.CustomerId, 
                CustomerName = custName,
                DateAcknowledged=DateTime.Now, MessageType = messageType, RecipientEmailId=recipientObj.AppUserEmail,
                RecipientUsername=recipientObj.Username, SenderEmailId=senderObj.AppUserEmail,  SenderUsername=senderObj.Username};

            _context.AckanowledgeToClients.Add(ackn);

            bool HasException = false;
            var msg = _today + "<br><br>M/S" + custName;
            if (!string.IsNullOrEmpty(obj.Customer?.Add)) msg += "<br>" + obj.Customer?.Add ?? "";
            if (!string.IsNullOrEmpty(obj.Customer?.Add2)) msg += "<br>" + obj.Customer?.Add2 ?? "";
            msg += "<br>" + obj.Customer?.City ?? "" + ", " + obj.Customer?.Country ?? "" + "<br><br>";
            msg += official == null ? "" : "Kind Attn : " + official.Title + official.OfficialName + ", " + official.Designation + "<br><br>";
            msg += "Dear " + official?.Gender == "F" ? "Madam:" : "Sir:" + "<br><br>";
            msg += "Thank you very much for your manpower enquiry dated " + DateOnly.FromDateTime(obj.OrderDate) + 
                " for following personnel: ";
            msg += "<br><br>" + await _commonMsg.ComposeOrderItems(obj.OrderNo, OrderItems, HasException) + "<br><br>";
            msg += HasException == true
                ? "Please note the exceptions mentioned under the column <i>Exceptions</i> and respond ASAP.  " +
                        "We will initiate execution of the wroks at this end on receipt of your clarificatins.<br><br>"
                : "We have initiated the works, and will revert to you soon with our delivery plan.<br><br>";
            msg += "Your point of contact for this order execution shall be the undersigned<br><br>";
            msg += "Please feel free to reach me for any clarification, or our Senior Management in case of escalation.<br><br>Best regards<br><br>" +
                senderObj.KnownAs + "<br>" + senderObj.Position + "<br>" + senderObj.Username;
        
            var subject = "Your enquiry dated " + order.OrderDate + " is registered by us under Serial No. " + order.OrderNo;
                        
            var emailMessage = new Message
            {
                //Sender = senderObj,
                //Recipient = recipientObj,
                MessageComposedOn = _today,
                SenderEmail = senderObj.AppUserEmail,
                SenderUsername = senderObj.Username,
                RecipientUsername = recipientObj.Username,
                RecipientEmail = recipientObj.AppUserEmail,
                Subject = subject,
                Content = msg,
                MessageType = messageType,
                //RecipientAppUserId =recipientObj.AppUserId,
                //SenderAppUserId = senderObj.AppUserId
            };
            var msgs = new List<Message>();
            msgs.Add(emailMessage);
            msgWithErr.Messages = msgs;
            return msgWithErr;
        }

        public async Task<Message> ForwardEnquiryToHRDept(Order order)
        {
            string msg = "";
            
            var senderObj = await _userManager.FindByIdAsync(_confg["DocControllerAdminAppUserId"]); 
            var recipientObj = await _userManager.FindByIdAsync(_confg["HRSupAppuserId"]);
               
            var cust = await _context.CustomerBriefFromId(order.CustomerId);

            msg = _today + "<br><br>" + recipientObj.KnownAs + ", " + 
                recipientObj.Position + "<br>" +
                "<br>HR Supervisor<br>Email: " + recipientObj.Email + "<br><br>";
            
            if (order.ForwardedToHRDeptOn == null )
            {
                msg += "Following requirement is forwarded to you for execution within time periods shown:<br><br>";
            }
            else
            {
                msg += "Following requirement forwarded to you on " +
                        order.ForwardedToHRDeptOn + " <b><font color='blue'>is revised</font></b>as follows:<br><br>";
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

            var emailMsg = new Message {MessageType="forwardToHR", //SenderAppUserId= senderObj.Id,
                SenderUsername = senderObj.UserName, SenderEmail = senderObj.Email, 
                //RecipientAppUserId = recipientObj.Id, 
                MessageComposedOn=_today,
                RecipientUsername = recipientObj.UserName, RecipientEmail= recipientObj.Email,
                Subject = "New Requirement No. " + order.OrderNo, Content = msg};
            
            return emailMsg;
        }
        public async Task<ICollection<Message>> ComposeCVFwdMessagesToClient(ICollection<CVFwdMsgDto> cvfwddtos, string Username)
        {
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
                    "candidates availability becomes volatile, and it is always good to keep candidates positively " +
                    "engaged.";

            concludingMsg += "While rejecting a profile, if you also advise us reasons for the rejection, it will help us " +
                    "adjust our criteria for further shortlistings, which will ultimately help in minimizing your rejections.";
            
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
                            MessageComposedOn = _today,
                            MessageType="cv forward", 
                            SenderUsername = Username, 
                            SenderEmail=senderObj.Email,
                            //RecipientAppUserId = recipientappuserid, 
                            //SenderAppUserId=senderObj.AppUserId,
                            RecipientUsername=recipientObj.Username,
                            RecipientEmail=recipientObj.Email, 
                            Subject="CVs Forwarded against your requirement",
                            CvRefId = cvref.CvRefId,
                            Content=msg
                        };
                        
                        emails.Add(email);
                    }
                    msg = _today + "<br><br>"+  cvref.OfficialTitle + " " + cvref.OfficialName + ", " + 
                        cvref.Designation + "<br>M/S " + cvref.CustomerName + ", " + cvref.City + "<br>Email:" + cvref.OfficialEmail +
                        "<br><br>Dear Sir:<br><br>We are pleased to enclose following CVs for your consideration against your requirements mentioned:<br>" +
                        "<table border='1'><tr><th width=10%>Order ref and dated</th><th width=15%>Category</th><th width=12%>Appli-<br>cation</th>" + 
                        "<th width=20%>Candidate Name</th><th width=5%>PP No</th><th width=15%>Attachments</th><th width=5%>Total Fwd</th>" + 
                        "<th width=10%>assessmt<br>Grade</th><th width=10%>Salary Expectation</th></tr>";
                    counter=0;
                }

                msg += "<tr><td>" + cvref.OrderNo +"-"+ cvref.ItemSrNo + "/<br>" + cvref.OrderDated.ToString("ddMMMyy") + "</td><td>" +
                    cvref.CategoryName + "</td><td>" + cvref.ApplicationNo + "</td><td>" + cvref.CandidateName + 
                    "</td><td>"+ cvref.PPNo + "</td><td></td><td>" + cvref.CumulativeSentSoFar + "</td><td>" + 
                    cvref.AssessmentGrade + "</td><td>" + cvref.SalaryExpected + "</td></tr>";
                
                counter+=1;
                lastOfficialId=cvref.OfficialId;
                lastCVRef=cvref;
            }
            
            msg += concludingMsg;
            email = new Message{
                MessageType="cv forward", SenderUsername = Username, SenderEmail=senderObj.Email,
                //RecipientAppUserId = recipientappuserid, 
                RecipientUsername=recipientObj.Username,
                RecipientEmail=recipientObj.Email, Subject="CVs Forwarded against your requirement",
                Content=msg, 
                //SenderAppUserId = senderObj.AppUserId, 
                MessageComposedOn = _today, 
                CvRefId = lastCVRef.CvRefId
            };

            emails.Add(email);

            return emails;
        }

        public async Task<Message> ComposeSelDecisionRemindersToClient(int CustomerId, string Username)
        {
            var refParam = new CVRefParams
            {
                CustomerId = CustomerId,
                RefStatus = "Referred"
            };

            var QueryableQuery =  _queryRepo.GetCustomerAndOfficialQueryable(CustomerId, "HR");
            var custAndOfficialDto = await QueryableQuery.FirstOrDefaultAsync();
            
            var query = await _queryRepo.GetCVReDtoQueryable(refParam);
            var cvfwddtos = await query.ToListAsync();

            var senderAppUserDetails = await _userManager.AppUserEmailAndUsernameFromAppUsername(Username);
            var recipientAppUserDetails = await _userManager.AppUserEmailAndUsernameFromAppUserId(
                custAndOfficialDto.AppUserId);

            string msg=_today + "<br><br>" + custAndOfficialDto.OfficialTitle + " " + custAndOfficialDto.OfficialName
                + "<br>" + custAndOfficialDto.Designation +"<br>" + custAndOfficialDto.CustomerName + 
                "<br>" + custAndOfficialDto.City + "<br>" + custAndOfficialDto.Country + "<br><br>Dear Sir:<br><br>";

            msg +="Following profiles are awaiting your decision on their selection.  " +
                "Pending your decision, we are doing our best to retain their interest.<br><br>" +
                "engaged.<br><br>Candidates awaiting your decision:<table>";

            foreach(var cvref in cvfwddtos) // result)
            {
                msg += "<tr><td>" + cvref.ReferredOn.ToString("ddMMMyy") + "</td><td>" + cvref.OrderNo +"-"+ cvref.SrNo + 
                    "</td><td>" + cvref.OrderDate.ToString("ddMMMyy") + "</td><td>" +
                    cvref.ProfessionName + "</td><td>" + cvref.ApplicationNo + "</td><td>" + cvref.CandidateName + 
                    "</td><td>"+ "</td><td></td><td>" + "</td></tr>";
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
                //SenderAppUserId = senderAppUserDetails.AppUserId, 
                MessageComposedOn = _today,
                SenderUsername = senderAppUserDetails.Username, SenderEmail = senderAppUserDetails.Email,
                //RecipientAppUserId = recipientAppUserDetails.AppUserId, 
                RecipientUsername = recipientAppUserDetails.Username,
                RecipientEmail = recipientAppUserDetails.Email, Subject = "Request for decision on selection of Profiles",
                Content = msg
            };

            return message;
        }

        public async Task<MessageWithError> ComposeFeedbackMailToCustomer(int feedbackId, string username)
        {
            var msgWithErr = new MessageWithError();
            AppUser senderObj, recipientObj;
            //souce for hyperiink - https://stackoverflow.com/questions/13959864/inserting-a-valid-html-link-into-a-string-for-an-email
            var Url = String.Format("{0}", "<a href=\"https://localhost:4200/feedback/edit/" +  feedbackId + "/0\">link</a>");
           
            var fdbk = await _context.CustomerFeedbacks.Include(x => x.FeedbackItems)
                .Where(x => x.Id == feedbackId).FirstOrDefaultAsync();

            if(string.IsNullOrEmpty(fdbk.OfficialUsername)) {
                var customerid=fdbk.CustomerId;
                var officials = await _context.CustomerOfficials
                    .Where(x => x.CustomerId == fdbk.CustomerId).ToListAsync();
                var official = officials.Where(x => x.Divn.ToLower()=="admin").FirstOrDefault()
                        ?? officials.Where(x => x.Divn.ToLower() == "hr" || x.Divn.ToLower()=="h.r.").FirstOrDefault()
                        ?? officials.Where(x => x.Divn.ToLower()=="account" || x.Divn.ToLower()=="accounts").FirstOrDefault();
                if(official==null) {
                    msgWithErr.ErrorString="Failed to retrieve any official from divisions 'Admin', 'HR', or 'accounts'.  Cannot generate Identity User for the official";
                    return msgWithErr;
                } else {
                    recipientObj = await _custRepo.CreateAppUserForCustomerOfficial(official, "");
                }
            } else {
                recipientObj = await _userManager.FindByNameAsync(fdbk.OfficialUsername);
            }

            if(recipientObj==null) {
                msgWithErr.ErrorString = "Failed to get Identity user of customer official";
                return msgWithErr;
            }

            var senderid=_confg["DocControllerAdminAppUserId"];
            if(senderid=="" || senderid=="0") {
                msgWithErr.ErrorString = "Failed to get DocControllerAdmin AppUserId from Environment variables";
                return msgWithErr;
            }
            
            senderObj = await _userManager.FindByIdAsync(senderid);
            if(senderObj == null) {
                msgWithErr.ErrorString = "Failed to get DocControllerAdmin Identity User";
                return msgWithErr;
            }

            
            string msg= string.Format("{0: dd-MMMM-yyyy}", fdbk.DateIssued) + "<br><br>Mr. " + fdbk.OfficialName + 
                "<br>" + fdbk.Designation +"<br>" + fdbk.CustomerName + 
                "<br>" + fdbk.City + "<br>" + fdbk.Country + "<br>Email: " + fdbk.Email +
                "<br><br>Dear Sir:<br><br>";
            
            msg +="As per requirements of ISO:9001-2015 certification, we are required to compile customer " +
                "feedbacks to analyse their appreciations as well as grievances, so as to upgrade our systems " +
                "to improve our customer satisfaction index. Please Click " + Url + 
                " to open the page containing the feedback questionnaire. " +
                "If for some reason you are not able to access the link, a hard copy is attached, which will require " + 
                "it to be printed, filled-in, scanned and send back to us.  The online version will be quite convenient to you, "+
                "but we leave it to your convenience. <br><br>We thank you for your cooperation, which will certainly " +
                "lead to our providing you improved services.";
            msg +="<br><br>We thank you for the opportunity to serve you, and assure you of our best and prompt services, always!" +
                "<br><br>Best regards<br><br>" + senderObj.KnownAs;

            var message = new Message
            {
                MessageType = "CustomerFeedback",
                //SenderAppUserId = senderObj.Id, 
                MessageComposedOn = fdbk.DateIssued,
                SenderUsername = senderObj.UserName, SenderEmail = senderObj.Email,
                //RecipientAppUserId = recipientObj.Id, 
                RecipientUsername = recipientObj.UserName,
                RecipientEmail = recipientObj.Email, Subject = "Customer Feedback as per ISO-9001:2015",
                Content = msg
            };

            msgWithErr.ErrorString="";
            msgWithErr.Messages = new List<Message>{message};
            
            return msgWithErr;
        }


   }
}