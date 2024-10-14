using api.DTOs;
using api.DTOs.Admin;
using api.Entities.Messages;
using api.Helpers;
using api.Params;

namespace api.Interfaces.Messages
{
    public interface IMessageRepository
    {
        Task<MessageWithError> GenerateMessageForCVForward(ICollection<CandidatesAssessedButNotRefDto> candidatesNotRefDto,
            string messageType, string Username);
        void AddMessage(Message message);
        Task<string> DeleteMessage(int id, string username);
        Task<Message> GetMessage(int id);
        Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams, string username);
        Task<ICollection<MessageDto>> GetMessageThread(string currentUserName, string recipientUserName);
        //Task<Group> GetMessageGroup(string groupName);
        Task<bool> SaveAllAsync();
        Task<bool> SendMessage(Message msg);
    }
}