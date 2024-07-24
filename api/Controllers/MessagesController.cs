
using api.DTOs;
using api.Entities.Admin.Order;
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
        public MessagesController(IUserRepository userRepository, IMessageRepository messageRepository, 
            IComposeMessagesHRRepository msgHRRepo, IMapper mapper)
        {
            _msgHRRepo = msgHRRepo;
            _mapper = mapper;
            _messageRepository = messageRepository;
            _userRepository = userRepository;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var username = User.GetUsername();

            if(createMessageDto.RecipientUsername.ToLower() == username) return BadRequest("You cannot send message to yourself");

            var sender = await _userRepository.GetUserByUserNameAsync(username);
            var recipient = await _userRepository.GetUserByUserNameAsync(createMessageDto.RecipientUsername);

            if(recipient == null) return NotFound("recipient not on record");

            var message = new Message{
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
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
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.GetUsername();

            var message = await _messageRepository.GetMessage(id);

            if (message.SenderUsername != username && message.RecipientUsername != username) 
                return Unauthorized();

            if (message.SenderUsername == username) message.SenderDeleted = true;

            if (message.RecipientUsername == username) message.RecipientDeleted = true;

            if (message.SenderDeleted && message.RecipientDeleted)
            {
                _messageRepository.DeleteMessage(message);
            }

            if (await _messageRepository.SaveAllAsync()) return Ok();

            return BadRequest("Problem deleting the message");
        }
            
        public async Task<ActionResult<string>> ComposeMsgsToForwardOrdersToAgents(ICollection<OrderForwardCategory> categoryForwards)
        {
            var officials = (ICollection<OrderForwardCategoryOfficial>)categoryForwards.Select(x => x.OrderForwardCategoryOfficials).ToList();        
            var msg= await _msgHRRepo.ComposeMsgsToForwardOrdersToAgents(categoryForwards, officials, User.GetUsername());

            if(msg==null) return BadRequest(new ApiException(400,"Bad Request", "failed to compose message for the Order Forwards"));

            return Ok("Order Forarding Messages composed. These messages are available for edit in the Messages section");
        }
    }       
}