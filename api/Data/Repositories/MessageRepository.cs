using System.Data.Common;
using System.Globalization;
using api.DTOs;
using api.DTOs.Admin;
using api.Entities.Admin.Client;
using api.Entities.Identity;
using api.Entities.Messages;
using api.Helpers;
using api.Interfaces.Admin;
using api.Interfaces.Messages;
using api.Params;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace api.Data.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        //private readonly ITaskRepository _taskRepo;
        private readonly IComposeMessagesAdminRepository _msgAdminRepo;
        public MessageRepository(UserManager<AppUser> userManager,
            DataContext context, IMapper mapper, IConfiguration config
            //, ITaskRepository taskRepo
            , IComposeMessagesAdminRepository msgAdminRepo)
        {
            _msgAdminRepo = msgAdminRepo;
            //_taskRepo = taskRepo;
            _config = config;
            _mapper = mapper;
            _userManager = userManager;
            _context = context;
        }
       
        public async Task<MessageWithError> GenerateMessageForCVForward(ICollection<CandidatesAssessedButNotRefDto> candidatesNotRefDto,
            string messageType, string Username)
        {
            var DocControllerAdminAppUserId=  Convert.ToInt32(_config["DocControllerAdminAppUserId"] ?? "0");
            var DocControllerAdminAppUsername=  _config["DocControllerAdminAppUsername"] ?? "";
            var DocControllerAdminAppUserEmail= _config["DocControllerAdminAppUserId"] ?? "";

            var dateTimeNow = DateTime.UtcNow;
            var ErrorString="";
            var returnDto = new MessageWithError();

            //find customerOfficialIds - which are recipients for the mail messages
            var candidateAndOrderItemIds = candidatesNotRefDto.Select(x => new {x.CandidateId, x.OrderItemId}).ToList();
 
            var distinctOrderItemIds = candidatesNotRefDto.Select(x => x.OrderItemId).Distinct().ToList();

            var customerids = candidatesNotRefDto.Select(x => x.CustomerId).Distinct().ToList();

            var SelectedCustomerOfficials = await _context.CustomerOfficials
                    .Where(x => customerids.Contains(x.CustomerId)).ToListAsync();
            
            if(SelectedCustomerOfficials==null || SelectedCustomerOfficials.Count == 0) {
                ErrorString +=", Customer officials for the order Items not defined.  Data not available to define Recipient";
                returnDto.ErrorString=ErrorString;
                return returnDto;
            }
                
            var dataToComposeMsg = new List<CVFwdMsgDto>();
       
            foreach(var item in candidatesNotRefDto) 
            {
                //var orderitem = distinctOrderItems.Where(x => x.OrderItemId==item.OrderItemId).FirstOrDefault();
                var Officials=SelectedCustomerOfficials.Where(x => x.CustomerId==item.CustomerId).ToList();

                var customerOfficial = new CustomerOfficial();
                var officialToAssign = new CustomerOfficial();
                
                if(Officials == null || Officials.Count == 0) {
                    returnDto.ErrorString += "Customer Officials not defined";
                } else if(Officials.Count ==1 ) {
                    officialToAssign = Officials.FirstOrDefault();
                } else{
                    customerOfficial = Officials.Where(x => x.Divn?.ToLower()=="hr").FirstOrDefault();
                    if (customerOfficial != null) {
                        officialToAssign=customerOfficial;
                    } else {
                        customerOfficial = Officials.Where(x => x.Divn?.ToLower()=="admin").FirstOrDefault(); 
                        if(customerOfficial != null) {
                            officialToAssign=customerOfficial;
                        } else {        //if official divn is not hr and not admin, then accept whatever is available
                            officialToAssign=Officials.FirstOrDefault();
                        }
                    }
                }

                if(!string.IsNullOrEmpty(returnDto.ErrorString)) return returnDto;
                
                var count = await _context.CVRefs.Where(x => x.OrderItemId == item.OrderItemId).CountAsync();
                                
                dataToComposeMsg.Add(new CVFwdMsgDto{
                    CvRefId = item.CvRefId,
                    City=item.CustomerCity ?? "",
                    OrderDated=item.OrderDate,
                    ItemSrNo=item.SrNo,
                    PPNo=item.PPNo ?? "",
                    CustomerId=item.CustomerId,
                    CustomerName=item.CustomerName,
                    OfficialId=officialToAssign.Id,
                    OfficialTitle=officialToAssign.Title ?? "Mr.",
                    OfficialName=officialToAssign.OfficialName,
                    OfficialUserId=officialToAssign.Id,
                    OfficialAppUserId=officialToAssign.AppUserId,
                    Designation=officialToAssign.Designation ?? "",
                    OfficialEmail=officialToAssign.Email ?? "",
                    OrderItemId=item.OrderItemId,
                    OrderNo=item.OrderNo,
                    CategoryName=item.CustomerName,
                    ApplicationNo=item.ApplicationNo,
                    CandidateName=item.CandidateName,
                    CumulativeSentSoFar=count,
                    SalaryExpected = item.SalaryExpectation,
                    AssessmentGrade = item.Grade
                });
           }
           
            var msgs = await _msgAdminRepo.ComposeCVFwdMessagesToClient(dataToComposeMsg, Username);
            
            returnDto.Messages = msgs;
            returnDto.ErrorString="";

            return returnDto;
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public async Task<string> DeleteMessage(int id, string username)
        {
            var strErr="";
            var msg = await _context.Messages.FindAsync(id);
            username = username.ToLower();

            if(msg==null) return "Message Not Found";

            if (msg.SenderUsername.ToLower() != username && msg.RecipientUsername != username) return "Unauthorized";

            if (msg.SenderUsername.ToLower() == username) msg.SenderDeleted = true;

            if (msg.RecipientUsername == username) msg.RecipientDeleted = true;

            if (msg.SenderDeleted && msg.RecipientDeleted) _context.Messages.Remove(msg);

            try {
                await _context.SaveChangesAsync();
            } catch (DbException ex) {
                strErr = ex.Message;
            } catch (Exception ex) {
                strErr = ex.Message;
            }

            if(strErr=="") strErr = msg.SenderDeleted && msg.RecipientDeleted ? "Deleted" : "Marked as Deleted";

            return strErr;
           
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FindAsync(id);
        }


        public async Task<PagedList<Message>> GetMessagesForUser(MessageParams mParams, string loggedinUsername)
        {
            var qry = _context.Messages.OrderByDescending(x => x.MessageSentOn).AsQueryable();

            if(loggedinUsername.ToLower()=="admin") {
                 qry = mParams.Container switch
                {
                    "Inbox" => qry = qry.Where(x => x.RecipientDeleted == false),
                    "Outbox" => qry = qry.Where(x => x.SenderDeleted == false && x.IsMessageSent) ,
                    _ => qry = qry.Where(x => x.RecipientDeleted == false && !x.IsMessageSent)
                };

            } else {
                qry = mParams.Container switch
                {
                    "Inbox" => qry = qry.Where(x => x.RecipientUsername.ToLower() == loggedinUsername.ToLower() && 
                        x.RecipientDeleted == false),
                    "Outbox" => qry = qry.Where(x => x.SenderUsername.ToLower() == loggedinUsername.ToLower() && 
                        x.SenderDeleted == false && x.IsMessageSent),
                    _ => qry = qry.Where(x => x.RecipientUsername.ToLower() == loggedinUsername.ToLower() && 
                        x.RecipientDeleted == false && !x.IsMessageSent)
                };
            }
            
            if(!string.IsNullOrEmpty(mParams.Search)) qry = qry
                .Where(x => x.SenderUsername.ToLower().Contains(mParams.Search) || x.RecipientUsername.ToLower().Contains(mParams.Search) );
            
            if(!string.IsNullOrEmpty(mParams.MessageType)) qry = qry
                .Where(x => x.MessageType.ToLower()==mParams.MessageType.ToLower());
            
            qry = mParams.Container == "Outbox" 
                ? qry.OrderByDescending(x => x.MessageComposedOn) 
                : qry.OrderByDescending(x => x.MessageSentOn);
            if(!string.IsNullOrEmpty(mParams.MessageType)) 
                qry.Where(x => x.MessageType.ToLower()==mParams.MessageType.ToLower());

            var paged = await PagedList<Message>.CreateAsync(qry.AsNoTracking(), mParams.PageNumber, mParams.PageSize);
            
            foreach(var msg in paged)
            {
                if(string.IsNullOrEmpty(msg.RecipientEmail) && !string.IsNullOrEmpty(msg.RecipientUsername)) {
                    var recipient = await _userManager.FindByNameAsync(msg.RecipientUsername);
                    if(recipient != null) {
                        msg.RecipientEmail = recipient.Email;
                        _context.Entry(msg).State = EntityState.Modified;
                    }
                }
            }

            if(_context.ChangeTracker.HasChanges()) await _context.SaveChangesAsync();
            return paged;
        }

        public async Task<ICollection<MessageDto>> GetMessageThread(string currentUserName, string recipientUserName)
        {
            var query = _context.Messages
            .Where(
                m => m.RecipientUsername == currentUserName && m.RecipientDeleted == false &&
                m.SenderUsername == recipientUserName ||
                m.RecipientUsername == recipientUserName && m.SenderDeleted == false &&
                m.SenderUsername == currentUserName
            )
            .OrderBy(m => m.MessageSentOn)
            .AsQueryable();

            return await query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider).ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> SendMessage(Message msg)
        {

           // create email message
            var email = new MimeMessage();

            try{
                email.From.Add(MailboxAddress.Parse("muneer.m.sheikh@gmail.com"));    // "munir.sheikh@live.com"));
                email.To.Add(MailboxAddress.Parse("munir.sheikh@live.com"));    // "munir.sheikh@live.com"));
                if(msg.RecipientEmail != null) email.Bcc.Add(MailboxAddress.Parse(msg.RecipientEmail));
                email.Subject = msg.Subject;
                email.Body = new TextPart(TextFormat.Html) { Text = msg.Content };

                // send email
                using var smtp = new SmtpClient();
                smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                smtp.Authenticate("muneer.m.sheikh@gmail.com", "farHeenMail*058");
                smtp.Send(email);
                smtp.Disconnect(true);

                msg.MessageSentOn = DateTime.UtcNow;
                msg.IsMessageSent=true;
                msg.SenderEmail = "muneer.m.sheikh@gmail.com";
                msg.IsMessageSent = true;
                _context.Entry(msg).State = EntityState.Modified;

                //update CVRef.
                var taskDocController = await _context.Tasks.Where(x => 
                    x.TaskType == "CVFwdTask" && x.CVRefId == msg.CvRefId && x.TaskStatus != "Completed").FirstOrDefaultAsync();
                if(taskDocController != null) {
                    taskDocController.TaskStatus="Completed";
                    taskDocController.CompletedOn=DateTime.UtcNow;
                    taskDocController.TaskItems.Add(new() {TaskItemDescription="CV Forward Email sent to client on " +
                    string.Format("{0: dd-MMM-yyyy}", DateTime.UtcNow), AppTaskId=taskDocController.Id, TaskStatus="Completed", 
                    TransactionDate = DateTime.UtcNow});
                    _context.Entry(taskDocController).State = EntityState.Modified;
                }
                await _context.SaveChangesAsync();

                return true;
            } catch (Exception ex) {
                throw new Exception("error in sending emails - " + ex.Message);
            }



        }
    }
}
