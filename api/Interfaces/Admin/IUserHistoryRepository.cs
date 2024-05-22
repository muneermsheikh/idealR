using api.DTOs.Admin;
using api.Entities.Admin;
using api.Helpers;
using api.Params.Admin;

namespace api.Interfaces.Admin
{
    public interface IUserHistoryRepository
    {
        Task<UserHistory> AddNewUserHistory(UserHistory userHistory);
        Task<UserHistoryItem> AddNewHistoryItem(UserHistoryItem historyItem, string Username);
        Task<UserHistoryReturnDto> EditContactHistory(UserHistory model, string Username);
        Task<ICollection<UserHistoryItem>> EditHistoryItems(ICollection<UserHistoryItem> items, string Username);
        Task<bool> DeleteUserHistory (int historyId);
        Task<bool> DeleteUserHistoryItem(int historyitemid);
        Task<UserHistory> GetHistoryByPersonId(UserHistoryParams historyParams);
        
        Task<PagedList<UserHistoryBriefDto>> GetUserHistoryPaginated(UserHistoryParams pParams);
        Task<UserHistoryItem>UpdateHistoryItem(UserHistoryItem userhistoryitem, string UserDisplayName);
        Task<UserHistoryDto> GetHistoryWithItemsFromHistoryId(int historyId);
    }
}