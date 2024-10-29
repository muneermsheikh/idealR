using api.DTOs.Admin;
using api.DTOs.HR;
using api.Entities.Identity;
using api.Entities.Messages;
using api.Extensions;
using api.Helpers;
using api.Interfaces.Admin;
using api.Interfaces.HR;
using api.Interfaces.Messages;
using AutoMapper;
using AutoMapper.QueryableExtensions;
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
        public ComposeMsgsForInterviews(DataContext context, IMapper mapper, IProspectiveCandidatesRepository prosRepo,
             UserManager<AppUser> userManager, IConfiguration config)
        {
            _prosRepo = prosRepo;
            _config = config;
            _userManager = userManager;
            _mapper = mapper;
            _context = context;
        }

        public async Task<MessagesWithErrDto> InviteCandidatesForInterviews(ICollection<int> InterviewItemCandidateIds, string username)
        {
            var msgWithErr = new MessagesWithErrDto();

            await _prosRepo.ConvertProspectiveNoDeleteFromProspective(InterviewItemCandidateIds, username);

            var query =  (from interviewCandidate in _context.IntervwItemCandidates where 
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
                    ScheduledAt = interviewCandidate.ScheduledFrom                  
                }).AsQueryable();
            
            var candData = await PagedList<InterviewCandidateInviteDataDto>.CreateAsync(
                query.AsNoTracking().ProjectTo<InterviewCandidateInviteDataDto>(_mapper.ConfigurationProvider)
                , 1,100);

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
                    data.CandidateTitle + " " + data.CandidateName + "email: " + recipientObj.AppUserEmail ?? "" + 
                    "<br><br>" + "copy: " + ToCC ?? ""  + "<br><br>Dear " + data.CandidateTitle + " " + 
                    data.CandidateName + ":" + "<br><br>Subject: " + subject + "<br><br>";

                    msgBody += "Pleased be advised your interview for the above position is scheduled as follows:";
                    msgBody += "<br><br><tab><b>Date and Time Scheduled</b>:" + 
                        string.Format("{0: ddd dd-MMM-yyyy hh:nn}", interviewDate) + 
                        "<br><tab><b>Venue</b>: " + data.VenueAddress ;
                    msgBody += string.IsNullOrEmpty(data.VenueAddress2) ? "" : "<br><tab>" + data.VenueAddress2;
                    msgBody += "<br><tab>" + data.VenueCityAndPIN;
                    msgBody += "<br><b>Interview Help Desk</b>: " + data.SiteRepName + " (Phone No:" + data.SitePhoneNo + ")";
                    
                    msgBody += "<br><br>Please do bring with you all your original testimonials along with your original Passport.  " +
                        "Please report at the interview venue atleast 15 minutes in advance, and report your arrival to the Help desk at the Venue";
                    msgBody += "<br><br>If for any reason, you are not able to attend the interview, please advise us by return email or by phone Number given.";

                    msgBody += "Looking forward to see you at the interview. <br><br>With best regards<br><br>" +
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
                        MessageComposedOn = DateTime.UtcNow
                    };

                    msgs.Add(message);
                    ids.Add(new AppId {ApplicationNo = data.ApplicationNo, InterviewItemCandidateId = data.InterviewItemCandidateId });
            }
            
            msgWithErr.Messages=msgs;
            msgWithErr.ApplicationIds = ids;
            
            return msgWithErr;
        }

    }
}