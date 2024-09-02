using System.Data.Common;
using api.DTOs;
using api.DTOs.Admin;
using api.Entities.Admin.Client;
using api.Entities.Messages;
using api.Helpers;
using api.Interfaces.Admin;
using api.Interfaces.Messages;
using api.Params;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly ITaskRepository _taskRepo;
        private readonly IComposeMessagesAdminRepository _msgAdminRepo;
        public MessageRepository(DataContext context, IMapper mapper, IConfiguration config, 
            ITaskRepository taskRepo, IComposeMessagesAdminRepository msgAdminRepo)
        {
            _msgAdminRepo = msgAdminRepo;
            _taskRepo = taskRepo;
            _config = config;
            _mapper = mapper;
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
            
            foreach(var candDtl in candidateAndOrderItemIds) {

            }
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
                var count = await _context.CVRefs.Where(x => x.OrderItemId == item.OrderItemId).CountAsync();
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
                    CumulativeSentSoFar=count
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
                    "Outbox" => qry = qry.Where(x => x.SenderDeleted == false),
                    _ => qry = qry.Where(x => x.RecipientDeleted == false)
                };

            } else {
                qry = mParams.Container switch
                {
                    "Inbox" => qry = qry.Where(x => x.RecipientUsername.ToLower() == loggedinUsername.ToLower() && 
                        x.RecipientDeleted == false),
                    "Outbox" => qry = qry.Where(x => x.SenderUsername.ToLower() == loggedinUsername.ToLower() && 
                        x.SenderDeleted == false),
                    _ => qry = qry.Where(x => x.RecipientUsername.ToLower() == loggedinUsername.ToLower() && 
                        x.RecipientDeleted == false)
                };
            }
            
            if(!string.IsNullOrEmpty(mParams.Search)) qry = qry
                .Where(x => x.SenderUsername.ToLower().Contains(mParams.Search) || x.RecipientUsername.ToLower().Contains(mParams.Search) );
            if(!string.IsNullOrEmpty(mParams.SearchInContents)) qry = qry
                .Where(x => x.Content.Contains(mParams.SearchInContents));
            qry = mParams.Container == "Outbox" 
                ? qry.OrderByDescending(x => x.MessageComposedOn) 
                : qry.OrderByDescending(x => x.MessageSentOn);

            var paged = await PagedList<Message>.CreateAsync(qry.AsNoTracking(), mParams.PageNumber, mParams.PageSize);
            
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
    }
}