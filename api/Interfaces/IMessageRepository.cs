using System.Text.RegularExpressions;
using api.DTOs;
using api.Entities;
using api.Helpers;
using api.Params;

namespace api.Interfaces
{
    public interface IMessageRepository
    {
        void AddMessage(Message message);
        void DeleteMessage(Message message);
        Task<Message> GetMessage(int id);
        Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);
        Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipientUserName);
        // group);
//        void RemoveConnection(Connection connection);
        //Task<Connection> GetConnection(string connectionId);
        Task<Group> GetMessageGroup(string groupName);
        //Task<Group> GetGroupForConnection(string connectionId);
        Task<bool> SaveAllAsync();
    }
}