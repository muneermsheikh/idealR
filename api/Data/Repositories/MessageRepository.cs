
using System.Text.RegularExpressions;
using api.Controllers;
using api.DTOs;
using api.DTOs.Admin;
using api.Entities;
using api.Entities.Admin;
using api.Entities.Admin.Client;
using api.Entities.Messages;
using api.Entities.Tasks;
using api.Helpers;
using api.Interfaces;
using api.Interfaces.Admin;
using api.Interfaces.Messages;
using api.Params;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DocumentFormat.OpenXml.Office.CustomUI;
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
            
            var cvrefwithCandidates = await (from ttask in _context.Tasks where
                    candidateAndOrderItemIds.Select(x => x.CandidateId).ToList().Contains((int)ttask.CandidateId) &&
                    candidateAndOrderItemIds.Select(x => x.OrderItemId).ToList().Contains((int)ttask.OrderItemId) &&
                    ttask.TaskType=="CVFwdTask"
                join cv in _context.Candidates on ttask.CandidateId equals cv.Id
                join cvref in _context.CVRefs on new 
                { o1=ttask.OrderItemId, o2=ttask.CandidateId } equals new
                    {o1=cvref.OrderItemId, o2=cvref.CandidateId} 
                select new CVRefWithCandidateDetailDto {
                    CVRefId=cvref.Id, CandidateName=cv.FullName, ApplicationNo=cv.ApplicationNo, 
                    OrderItemId=(int)ttask.OrderItemId, PassportNo= cv.PpNo, CandidateId = cv.Id}
            ).ToListAsync();
                
            var distinctOrderItemIds = cvrefwithCandidates.Select(x => x.OrderItemId).Distinct().ToList();

            var distinctOrderItems = await (from items in _context.OrderItems where distinctOrderItemIds.Contains(items.Id) 
                    join o in _context.Orders on items.OrderId equals o.Id 
                select new {OrderItemId=items.Id, CustomerId=o.CustomerId, 
                    CustomerName = o.Customer.CustomerName, City = o.Customer.City,
                    OrderNo = o.OrderNo, OrderDated = o.OrderDate,
                    CategoryRef = o.OrderNo + "-" + items.SrNo + "-" + items.Profession.ProfessionName,
                    ItemSrNo = items.SrNo
            }).ToListAsync();

            var customerids = distinctOrderItems.Select(x => x.CustomerId).Distinct().ToList();

            var SelectedCustomerOfficials = await _context.CustomerOfficials
                    .Where(x => customerids.Contains(x.CustomerId)).ToListAsync();
            
            if(SelectedCustomerOfficials==null || SelectedCustomerOfficials.Count == 0) {
                ErrorString +=", Customer officials for the order Items not defined.  Data not available to define Recipient";
                returnDto.ErrorString=ErrorString;
                return returnDto;
            }
                
            var dataToComposeMsg = new List<CVFwdMsgDto>();
       
            foreach(var cvRef in cvrefwithCandidates) 
            {
                var orderitem = distinctOrderItems.Where(x => x.OrderItemId==cvRef.OrderItemId).FirstOrDefault();
                var count = await _context.CVRefs.Where(x => x.OrderItemId == (int)orderitem.OrderItemId).CountAsync();
                var Officials=SelectedCustomerOfficials.Where(x => x.CustomerId==orderitem.CustomerId).ToList();

                var customerOfficial = new CustomerOfficial();
                var officialToAssign = new CustomerOfficial();

                /* var CVsReferredCount = await _context.CVRefs.Where(a => a.OrderItemId == orderitem.OrderItemId)
                    .ToList().GroupBy(a => a.OrderItemId)
                        .Select(g => new { orderitemid = g.Key, refcount = g.Count() }).ToListAsync();
                */
                
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
                    CvRefId = cvRef.CVRefId,
                    CustomerId=orderitem.CustomerId,
                    CustomerName=orderitem.CustomerName,
                    City=orderitem.City ?? "",
                    OfficialId=officialToAssign.Id,
                    OfficialTitle=officialToAssign.Title ?? "Mr.",
                    OfficialName=officialToAssign.OfficialName,
                    OfficialUserId=officialToAssign.Id,
                    OfficialAppUserId=officialToAssign.AppUserId,
                    Designation=officialToAssign.Designation ?? "",
                    OfficialEmail=officialToAssign.Email ?? "",
                    OrderItemId=orderitem.OrderItemId,
                    OrderNo=orderitem.OrderNo,
                    OrderDated=orderitem.OrderDated,
                    ItemSrNo=orderitem.ItemSrNo,
                    CategoryName=orderitem.CustomerName,
                    ApplicationNo=cvRef.ApplicationNo,
                    CandidateName=cvRef.CandidateName,
                    PPNo=cvRef.PassportNo ?? "",
                    CumulativeSentSoFar=count
                });
           }
           
            var msgs = (List<Message>)await _msgAdminRepo.ComposeCVFwdMessagesToClient(dataToComposeMsg, Username);
            
            foreach(var msg in msgs) {
                _context.Entry(msg).State = EntityState.Added;
            }

            if(await _context.SaveChangesAsync() ==0) return null;

            returnDto.Messages = msgs;
            returnDto.ErrorString="";

            return returnDto;
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FindAsync(id);
        }


        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var qry = _context.Messages.OrderByDescending(x => x.MessageSentOn).AsQueryable();

            qry = messageParams.Container switch
            {
                "Inbox" => qry = qry.Where(x => x.RecipientUsername == messageParams.Username && 
                    x.RecipientDeleted == false),
                "Outbox" => qry = qry.Where(x => x.SenderUsername == messageParams.Username && 
                    x.SenderDeleted == false),
                _ => qry = qry.Where(x => x.RecipientUsername == messageParams.Username && x.RecipientDeleted == false)
            };

            var messages = qry.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

            return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);

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