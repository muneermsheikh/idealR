
using api.DTOs;
using api.Entities.Admin.Order;
using api.Entities.Identity;
using api.Entities.Messages;
using api.Errors;
using api.Extensions;
using api.Helpers;
using api.Interfaces;
using api.Interfaces.Messages;
using api.Interfaces.Orders;
using api.Params;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Authorize]
    public class MessagesController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        private readonly IComposeMessagesHRRepository _msgHRRepo;
        private readonly UserManager<AppUser> _userManager;
        public MessagesController(IUserRepository userRepository, IMessageRepository messageRepository, UserManager<AppUser> userManager,
            IComposeMessagesHRRepository msgHRRepo, IMapper mapper)
        {
            _userManager = userManager;
            _msgHRRepo = msgHRRepo;
            _mapper = mapper;
            _messageRepository = messageRepository;
            _userRepository = userRepository;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(Message msgModel)
        {
            var username = User.GetUsername();

            if(msgModel.RecipientUsername.ToLower() == username) return BadRequest("You cannot send message to yourself");

            var senderObj = await _userManager.FindByNameAsync(User.GetUsername());
            var recipientObj = await _userManager.FindByNameAsync(msgModel.RecipientUsername);

            if(recipientObj == null || senderObj == null || recipientObj.UserName==null ) return NotFound(new ApiException(400, "Not Found", "recipient not on found in identity object"));
            if(senderObj == null) return NotFound(new ApiException(400, "Not Found", "Sender Username not found in Identity object"));

            var message = new Message{
                //Sender = senderObj,
                //Recipient = recipientObj,
                SenderUsername = senderObj.UserName,
                RecipientUsername = recipientObj.UserName,
                Content = msgModel.Content,
                //SenderId = senderObj.Id,
                SenderEmail = senderObj.Email,
                //RecipientId = recipientObj.Id,
                RecipientEmail = recipientObj.Email,
                MessageType = msgModel.MessageType,
            };

            _messageRepository.AddMessage(message);

             if(await _messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDto>(message));

            return BadRequest("Failed to save the message");
        }
    
        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDto>>> GetMessagesForUser([FromQuery]MessageParams msgParams)
        {
            var messages = await _messageRepository.GetMessagesForUser(msgParams, User.GetUsername());

            Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage, messages.PageSize, 
                messages.TotalCount, messages.TotalPages));
            
            return Ok(messages);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> DeleteMessage(int id)
        {

            var strErr = await _messageRepository.DeleteMessage(id, User.GetUsername());

            if(strErr == "Deleted" || strErr == "Marked as Deleted") return Ok(strErr);
            return  BadRequest(new ApiException(400, "Bad Request", strErr));
            
        }
            
        public async Task<ActionResult<string>> ComposeMsgsToForwardOrdersToAgents(ICollection<OrderForwardCategory> categoryForwards)
        {
            var officialids = categoryForwards.Select(x => x.OrderForwardCategoryOfficials.Select(x => x.CustomerOfficialId)).ToList();        
            
            var msg= await _msgHRRepo.ComposeMsgsToForwardOrdersToAgents(categoryForwards, (ICollection<int>)officialids, User.GetUsername());

            if(msg==null) return BadRequest(new ApiException(400,"Bad Request", "failed to compose message for the Order Forwards"));

            return Ok("Order Forarding Messages composed. These messages are available for edit in the Messages section");
        }
    
        [HttpPost("sendMessage")]
        public async Task<ActionResult<bool>> SendMessage(Message msg)
        {
            var sent = await _messageRepository.SendMessage(msg);
            if(!sent) return BadRequest(new ApiException(400, "Failed to send the mesage", "bad request"));

            return Ok(true);
        }
    
    }       
}