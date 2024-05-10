using System.Text.RegularExpressions;
using api.DTOs;
using api.DTOs.Admin;
using api.Entities;
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
        void DeleteMessage(Message message);
        Task<Message> GetMessage(int id);
        Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);
        Task<ICollection<MessageDto>> GetMessageThread(string currentUserName, string recipientUserName);
        Task<Group> GetMessageGroup(string groupName);
        Task<bool> SaveAllAsync();
    }
}