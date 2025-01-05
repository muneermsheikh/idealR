using api.DTOs.Admin;
using api.DTOs.HR;
using api.Entities.Identity;
using api.Entities.Messages;
using api.Extensions;
using api.Interfaces.Admin;
using api.Interfaces.HR;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace api.Data.Repositories.Admin
{
    public class ComposeMsgsForInterviews: IComposeMsgForIntrviews
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _config;
        private readonly IProspectiveCandidatesRepository _prosRepo;
        private readonly string _RAName;
        public ComposeMsgsForInterviews(DataContext context, IMapper mapper, IProspectiveCandidatesRepository prosRepo,
             UserManager<AppUser> userManager, IConfiguration config)
        {
            _prosRepo = prosRepo;
            _config = config;
            _userManager = userManager;
            _mapper = mapper;
            _context = context;
            _RAName = _config["IdealUsername"];
        }

        //**TODO** combine following 2 procedures into one - the only diff here is InterviewItemCandidateIds name
        //and text of email body
        public async Task<MessagesWithErrDto> EditInviteForInterviews(ICollection<int> InterviewItemCandidateIds, string username)
        {
            var msgWithErr = new MessagesWithErrDto();

            await _prosRepo.ConvertProspectiveNoDeleteFromProspective(InterviewItemCandidateIds, username);

            var candData = await (from interviewCandidate in _context.IntervwItemCandidates where 
                    InterviewItemCandidateIds.Contains(interviewCandidate.Id)
                join interviewItem in _context.IntervwItems on interviewCandidate.InterviewItemId equals interviewItem.Id
                join interview in _context.Intervws on interviewItem.IntervwId equals interview.Id
                join orderitem in _context.OrderItems on interviewItem.OrderItemId equals orderitem.Id
                join order in _context.Orders on orderitem.OrderId equals order.Id
                select new InterviewCandidateInviteDataDto{
                    InterviewItemCandidateId=interviewCandidate.Id,
                    ApplicationNo = interviewCandidate.ApplicationNo, 
                    CandidateId = interviewCandidate.CandidateId,
                    CandidateTitle = "Mr. ",
                    CandidateName = interviewCandidate.CandidateName,
                    ProfessionName = interviewItem.ProfessionName,
                    CustomerName = interview.CustomerName,
                    CustomerCity = order.Customer.City + ", " + order.Customer.Country,
                    SiteRepName = interviewItem.SiteRepName,
                    SitePhoneNo = interviewItem.SitePhoneNo,
                    CustomerId = interview.CustomerId,
                    InterviewVenue = interviewItem.InterviewVenue,
                    VenueAddress=interviewItem.VenueAddress,
                    VenueAddress2 = interviewItem.VenueAddress2,
                    VenueCityAndPIN = interviewItem.VenueCityAndPIN,
                    InterviewMode = interviewItem.InterviewMode,
                    ScheduledAt = interviewCandidate.ScheduledFrom                  
                }).ToListAsync();
            
            if(candData == null || candData.Count == 0) {
                msgWithErr.ErrorString = "Your instructions did not yield any result";
                return msgWithErr;
            }
            /*var candData = await PagedList<InterviewCandidateInviteDataDto>.CreateAsync(
                query.AsNoTracking().ProjectTo<InterviewCandidateInviteDataDto>(_mapper.ConfigurationProvider)
                , 1,100);
            */
            var HRSupUsername = _config["HRSupUsername"] ?? "Admin";
            var Obj = await _userManager.FindByNameAsync(HRSupUsername) 
                ?? throw new Exception("Cannot find Sender User object");
            var SendereObj = new AppUserBriefDto{AppUserEmail=Obj.Email, AppUserId=Obj.Id, KnownAs=Obj.KnownAs,
                Name = Obj.UserName, Position = Obj.Position, Username = Obj.UserName};

            var msgs = new List<Message>();
            var ids = new List<AppId>();

            foreach(var data in candData) {
               
                var userid = await _context.GetAppUserIdOfCandidate(data.CandidateId);
                if(userid==0) continue;
                var obj = await _userManager.FindByIdAsync(userid.ToString());
                if(obj==null) continue;

                var recipientObj = new AppUserBriefDto {
                    AppUserEmail = obj.Email, AppUserId = obj.Id, KnownAs = obj.KnownAs, 
                    Name=obj.UserName, Position=obj.Position, Username=obj.UserName};
                var ToCC="";

                var subject = "Rescheduling of Your Interview for the position of " + data.ProfessionName + " in " + data.CustomerCity;
                var subjectInBody = "<b><u>Subject: </b>Your Interview for the position of " + data.ProfessionName +
                    " in " + data.CustomerCity + " for M/S " + data.CustomerName;
                
                var interviewDate = data.ScheduledAt;

                var msgBody = string.Format("{0: dd-MMM-yyyy}", interviewDate) + "<br><br>" + 
                    data.CandidateTitle + " " + data.CandidateName + "<br>email: " + recipientObj.AppUserEmail ?? "";
                if(!string.IsNullOrEmpty(ToCC)) msgBody += "<br>copy: " + ToCC ?? "";
                msgBody += "<br><br>Dear " + data.CandidateTitle + " " + 
                    data.CandidateName + ":" + "<br><br>Subject: " + subject;

                msgBody += "<br><br>Please note as per client instructions, your interview for the above position is re-scheduled as follows:";
                msgBody += "<br><br><tab><b>Date and Time Re-Scheduled</b>:" + 
                    string.Format("{0: ddd dd-MMM-yyyy HH:mm tt}", interviewDate) + 
                    "<br><tab><b>Venue</b>: " + data.VenueAddress ;
                msgBody += string.IsNullOrEmpty(data.VenueAddress2) ? "" : "<br><tab>" + data.VenueAddress2;
                msgBody += "<br><tab>" + data.VenueCityAndPIN;
                msgBody += "<br><b>Interview Mode:</b>: " + data.InterviewMode;
                msgBody += "<br><b>Interview Help Desk</b>: " + data.SiteRepName + " (Phone No:" + data.SitePhoneNo + ")";
                    
                    msgBody += "<br><br>Please do bring with you all your original testimonials along with your original Passport.  " +
                        "Please report at the interview venue at-least 15 minutes in advance, and report your arrival to the Help desk at the Venue";
                    msgBody += "<br><br>If for any reason, you are not able to attend the interview, please advise us by return email or by phone Number given.";

                    msgBody += "<br><br>Looking forward to see you at the interview. <br><br>With best regards<br><br>" +
                        SendereObj.KnownAs + ", " + SendereObj.Position;

                    var message = new Message
                    {
                        SenderUsername=SendereObj.Username,
                        SenderEmail= _config["EmailId"] ?? "",
                        RecipientUsername = recipientObj.Username ?? "",
                        RecipientEmail = recipientObj.AppUserEmail ?? "",
                        Subject = subject,
                        Content = msgBody,
                        MessageType = "InterviewInvitation",
                        MessageComposedOn = DateTime.UtcNow,
                        RecipientId=data.InterviewItemCandidateId
                    };

                    msgs.Add(message);
                    ids.Add(new AppId {ApplicationNo = data.ApplicationNo, InterviewItemCandidateId = data.InterviewItemCandidateId });
            }
            
            msgWithErr.Messages=msgs;
            msgWithErr.ApplicationIds = ids;
            
            return msgWithErr;
        }


        public async Task<MessagesWithErrDto> InviteCandidatesForInterviews(ICollection<int> InterviewItemCandidateIds, string username)
        {
            var msgWithErr = new MessagesWithErrDto();

            await _prosRepo.ConvertProspectiveNoDeleteFromProspective(InterviewItemCandidateIds, username);

            var candData = await (from interviewCandidate in _context.IntervwItemCandidates where 
                    InterviewItemCandidateIds.Contains(interviewCandidate.Id)
                join interviewItem in _context.IntervwItems on interviewCandidate.InterviewItemId equals interviewItem.Id
                join interview in _context.Intervws on interviewItem.IntervwId equals interview.Id
                join orderitem in _context.OrderItems on interviewItem.OrderItemId equals orderitem.Id
                join order in _context.Orders on orderitem.OrderId equals order.Id
                select new InterviewCandidateInviteDataDto{
                    InterviewItemCandidateId=interviewCandidate.Id,
                    ApplicationNo = interviewCandidate.ApplicationNo, 
                    CandidateId = interviewCandidate.CandidateId,
                    CandidateTitle = "Mr. ",
                    CandidateName = interviewCandidate.CandidateName,
                    ProfessionName = interviewItem.ProfessionName,
                    CustomerName = interview.CustomerName,
                    CustomerCity = order.Customer.City + ", " + order.Customer.Country,
                    SiteRepName = interviewItem.SiteRepName,
                    SitePhoneNo = interviewItem.SitePhoneNo,
                    CustomerId = interview.CustomerId,
                    InterviewVenue = interviewItem.InterviewVenue,
                    VenueAddress=interviewItem.VenueAddress,
                    VenueAddress2 = interviewItem.VenueAddress2,
                    VenueCityAndPIN = interviewItem.VenueCityAndPIN,
                    InterviewMode = interviewItem.InterviewMode,
                    ScheduledAt = interviewCandidate.ScheduledFrom                  
                }).ToListAsync();
            
            if(candData == null || candData.Count == 0) {
                msgWithErr.ErrorString = "Your instructions did not yield any result";
                return msgWithErr;
            }
            /*var candData = await PagedList<InterviewCandidateInviteDataDto>.CreateAsync(
                query.AsNoTracking().ProjectTo<InterviewCandidateInviteDataDto>(_mapper.ConfigurationProvider)
                , 1,100);
            */
            var HRSupUsername = _config["HRSupUsername"] ?? "Admin";
            var Obj = await _userManager.FindByNameAsync(HRSupUsername) 
                ?? throw new Exception("Cannot find Sender User object");
            var SendereObj = new AppUserBriefDto{AppUserEmail=Obj.Email, AppUserId=Obj.Id, KnownAs=Obj.KnownAs,
                Name = Obj.UserName, Position = Obj.Position, Username = Obj.UserName};

            var msgs = new List<Message>();
            var ids = new List<AppId>();

            foreach(var data in candData) {
               
                var userid = await _context.GetAppUserIdOfCandidate(data.CandidateId);
                if(userid==0) continue;
                var obj = await _userManager.FindByIdAsync(userid.ToString());
                if(obj==null) continue;

                var recipientObj = new AppUserBriefDto {
                    AppUserEmail = obj.Email, AppUserId = obj.Id, KnownAs = obj.KnownAs, 
                    Name=obj.UserName, Position=obj.Position, Username=obj.UserName};
                var ToCC="";

                var subject = "Your Interview is scheduled for the position of " + data.ProfessionName + " in " + data.CustomerCity;
                var subjectInBody = "<b><u>Subject: </b>Your Interview for the position of " + data.ProfessionName +
                    " in " + data.CustomerCity + " for M/S " + data.CustomerName;
                
                var interviewDate = data.ScheduledAt;

                var msgBody = string.Format("{0: dd-MMM-yyyy}", interviewDate) + "<br><br>" + 
                    data.CandidateTitle + " " + data.CandidateName + "<br>email: " + recipientObj.AppUserEmail ?? "";
                if(!string.IsNullOrEmpty(ToCC)) msgBody += "<br>copy: " + ToCC ?? "";
                msgBody += "<br><br>Dear " + data.CandidateTitle + " " + 
                    data.CandidateName + ":" + "<br><br>Subject: " + subject;

                msgBody += "<br><br>Pleased be advised your interview for the above position is scheduled as follows:";
                msgBody += "<br><br><tab><b>Date and Time Scheduled</b>:" + 
                    string.Format("{0: ddd dd-MMM-yyyy HH:mm tt}", interviewDate) + 
                    "<br><tab><b>Venue</b>: " + data.VenueAddress ;
                msgBody += string.IsNullOrEmpty(data.VenueAddress2) ? "" : "<br><tab>" + data.VenueAddress2;
                msgBody += "<br><tab>" + data.VenueCityAndPIN;
                msgBody += "<br><b>Interview Mode:</b>: " + data.InterviewMode;
                msgBody += "<br><b>Interview Help Desk</b>: " + data.SiteRepName + " (Phone No:" + data.SitePhoneNo + ")";
                    
                    msgBody += "<br><br>Please do bring with you all your original testimonials along with your original Passport.  " +
                        "Please report at the interview venue at-least 15 minutes in advance, and report your arrival to the Help desk at the Venue";
                    msgBody += "<br><br>If for any reason, you are not able to attend the interview, please advise us by return email or by phone Number given.";

                    msgBody += "<br><br>Looking forward to see you at the interview. <br><br>With best regards<br><br>" +
                        SendereObj.KnownAs + ", " + SendereObj.Position;

                    var message = new Message
                    {
                        SenderUsername=SendereObj.Username,
                        SenderEmail= _config["EmailId"] ?? "",
                        RecipientUsername = recipientObj.Username ?? "",
                        RecipientEmail = recipientObj.AppUserEmail ?? "",
                        Subject = subject,
                        Content = msgBody,
                        MessageType = "InterviewInvitation",
                        MessageComposedOn = DateTime.UtcNow,
                        RecipientId = data.InterviewItemCandidateId
                    };

                    msgs.Add(message);
                    ids.Add(new AppId {ApplicationNo = data.ApplicationNo, InterviewItemCandidateId = data.InterviewItemCandidateId });
            }
            
            msgWithErr.Messages=msgs;
            msgWithErr.ApplicationIds = ids;
            
            return msgWithErr;
        }


        public async Task<MessagesWithErrDto> ComposeEmailsForEmploymentInterest(CallRecordCandidateAdviseDto Dto, string username)
        {
            var msgWithErr = new MessagesWithErrDto();

            var catRef = Dto.CategoryRef;
            var ProfessionName = catRef[catRef.LastIndexOf("-")..];
            ProfessionName=ProfessionName[1..];
            var PersonId = Dto.PersonId;
            var item = Dto.CallRecordItem;

            var data = await (
                from prosp in _context.ProspectiveCandidates where prosp.PersonId == PersonId
                join orderitem in _context.OrderItems on prosp.OrderItemId equals orderitem.Id
                join order in _context.Orders on orderitem.OrderId equals order.Id 
                select new {
                    CandidateTitle = prosp.Gender,
                    CandidateName = prosp.CandidateName,
                    ContactResult = item.ContactResult, 
                    Email= prosp.Email,
                    DateOfContact = item.DateOfContact, 
                    CustomerCity = order.Customer.City,
                    Source = prosp.Source,
                    City = prosp.City,
                    PhoneNo = prosp.PhoneNo
            }).FirstOrDefaultAsync();
            
            if(data == null) {
                msgWithErr.ErrorString = "Your instructions did not yield any result";
                return msgWithErr;
            }
           
            var Obj = await _userManager.FindByNameAsync(username);

            var SendereObj = new AppUserBriefDto{AppUserEmail=Obj.Email, AppUserId=Obj.Id, KnownAs=Obj.KnownAs,
                Name = Obj.UserName, Position = Obj.Position, Username = Obj.UserName};

            var recipientObj = new {
                CandidateName = data.CandidateName, Email = data.Email, 
                Position=ProfessionName};

            var ToCC="";
            var Title = data.CandidateTitle.ToLower()=="f" ? "Ms. " : "Mr. ";

            var subject = "Requirement of " + ProfessionName + " in " +  data.CustomerCity;
            
            var DateOfContact = data.DateOfContact;

            var msgBody = string.Format("{0: dd-MMM-yyyy}", DateOfContact) + "<br><br>" + Title + 
                data.CandidateName + "<br>" + data.City + "<br>Phone: " + data.PhoneNo + "; email: " + recipientObj.Email ?? "";

            if(!string.IsNullOrEmpty(ToCC)) msgBody += "<br>copy: " + ToCC ?? "";
                
            msgBody += "<br><br>Dear " + Title + data.CandidateName + ":" + "<br><br><u>Subject: " + subject + "</u><br><br>";

            msgBody += "We are " + _RAName + ", a licensed recruitment firm based in Mumbai, " + 
                "in the business of providing recruitment services for the last 40+ years.  We have your details from " + data.Source + 
                ". <br><br>One of our clients in " + data.CustomerCity + " needs a " + ProfessionName +
                ", in which connection we had a brief conversation with you on " + string.Format("{0: dddd, dd-MMM-yyyy}", DateOfContact);
                
            var thanks = ".  We thank you for the courtesy extended during the conversation.<br><br>";
            var clPara = "<br><br>Look forward to hear from you.";

            switch (data.ContactResult.ToLower()) {
                case "interested": case "interested, and keen":
                    msgBody += thanks + "We note of your interest in the said employment opportunity, for which we thank you.  " +
                    "<br><br>If you have not shared with us your updated resume, please do so now to the below mentioned email Id " + 
                    "in order to forward the same to our client for his review." + clPara;
                    break;
                case "interested, but doubtful":
                    msgBody += thanks + "We note of your interest in the said employment opportunity, for which we thank you.  " +
                    "<br><br>We note from our conversation that your interest in the employment is not yet confirmed.  As we " + 
                    "have soon to compile profiles for consideration of the client, kindly let us have your considered and confirmed " +
                    "decision at the very earliest." + clPara;
                    break;
                case "interested, undecided":
                    msgBody += thanks + "We note that you have not decided on your interest in the employment. As we have to finalize " +
                    "compilation of suitble profiles for consideration of the client, we request you to please let us know of your " +
                    "decision soon - whether your interest or non-interest.  If you are interested, we have to initiate the " +
                    "formalities at the earliest." + clPara;
                    break;
                case "declined-low remuneration":
                    msgBody += thanks + "You declined to consider the said employment opportunity due to low remuneration offered by the " +
                    "client. We appreciate your response, and will be advising the client suitably to explore if the remuneration offer " +
                    "could be suitably amended to suit market expectations.  If their response is positive, we will revert to you with the " +
                    "details for your consideration.";
                    break;
                case "declined for overseas":
                    msgBody += "We note that you are not interested in overseas employments. We have marked your profile accordingly, and will not approach you again for overseas opportunities." +
                        "Thank you for your time.";
                    break;
                
                case "declined - sc not agreed": case "declined - other reasons":
                    msgBody += thanks + "We regret to note you have declined to consider the employment. We will approach you again whenever other suitable " +
                    "opportunities arise.";
                    break;

                case "wrong number":
                    msgBody += "We tried to reach you on the telephone number " + data.PhoneNo + " that is mentioned in your profile in " +
                    data.Source + ".  Unfortunately, this number is reported as a <i>Wrong Number</i> by the Telephone Services Provider.  " +
                    "Kindly revert with your correct telephone number, if you are interested in the said employment opportunity." + clPara;
                    break;
                case "not responding":
                    msgBody += "We tried to reach you on the telephone number " + data.PhoneNo + " which is mentioned in your profile in " +
                    data.Source + ".  Unfortunately, this number is <i>Not Attended</i>.  If this is not the correct number, please " +
                    "inform us of the correct number.  Please respond to this message by a brief message <i>Interested<i> or <i>Not Interested<i>. " +
                    "If you are interested, then please share with us your updated profile." + clPara;
                
                    break;
                default:
                    msgBody="";
                    break;
            }

            msgBody += "<br><br>With best regards<br>" + SendereObj.KnownAs + ", " + SendereObj.Position + "<br>" + _RAName;

            var message = new Message
            {
                SenderUsername=SendereObj.Username,
                SenderEmail= _config["EmailId"] ?? "",
                RecipientEmail = recipientObj.Email ?? "",
                Subject = subject,
                Content = msgBody,
                MessageType = "CandidateInterest",
                MessageComposedOn = DateTime.UtcNow,
            };
            
            msgWithErr.Messages=new List<Message>{message};
            
            return msgWithErr;
        }


    }
}