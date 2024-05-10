using api.DTOs.Admin;
using api.Entities.Admin.Order;
using api.Entities.HR;
using api.Entities.Messages;


namespace api.Interfaces.Admin
{
    public interface IComposeMessagesAdminRepository
    {
        Task<Message> AckEnquiryToCustomer(Order order);
        Task<Message> ForwardEnquiryToHRDept(Order order);
        Task<ICollection<Message>> ComposeCVFwdMessagesToClient(ICollection<CVFwdMsgDto> fwdMsgsDto, string Username);
        Task<Message> ComposeSelDecisionRemindersToClient(int CustomerId, string Username);
        Task<ICollection<Message>> ComposeSelectionStatusMessagesForCandidate(ICollection<SelectionMessageDto> selDto, 
           string Username);
        Task<ICollection<Message>> AdviseRejectionStatusToCandidateByEmail(ICollection<SelectionMessageDto> rejectionsDto, string loggedInUserName);
       
    }

}