using api.DTOs.Admin;
using api.DTOs.Admin.Orders;
using api.Entities.Admin.Order;
using api.Entities.HR;
using api.Entities.Messages;
using api.Interfaces.Messages;

namespace api.Interfaces.Orders
{
    public interface IComposeMessagesHRRepository
    {
         
        Task<Message> ComposeHTMLToAckToCandidateByEmail(Candidate candidate);
        Task<SMSMessage> ComposeMsgToAckToCandidateBySMS(Candidate candidate);
        Task<Message> ComposeHTMLToPublish_CVReadiedToForwardToClient(ICollection<CommonDataDto> commonDataDtos, string Username, int recipientAppUserId);
        //Task<ICollection<Message>> ComposeMsgsToForwardOrdersToAgents(OrderForwardToAgentDto dlforward, string Username);
        Task<MessageWithError> ComposeEmailMsgForDLForwardToHRHead(ICollection<OrderItemBriefDto> OrderItems, string senderUsername, string recipientUsername);
        Task<ICollection<Message>> ComposeMsgsToForwardOrdersToAgents(ICollection<OrderForwardCategory> categories, ICollection<int> officialids, string Username);

        Task<MessageWithError> ComposeEmailToDesignOrderItemAssessmentQs(int orderId, string Username);    
        Task<MessageWithError> ComposeMessagesToHRExecToSourceCVs(ICollection<OrderItemBriefDto> hrassignments, string Username);

    }
}