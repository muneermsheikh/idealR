using api.DTOs.Admin;
using api.DTOs.HR;
using api.Entities.Admin;
using api.Helpers;
using api.Params.Admin;

namespace api.Interfaces.Admin
{
    public interface ICallRecordRepository
    {
        Task<CallRecord> AddNewCallRecord(CallRecord CallRecord);
        Task<CallRecordStatusReturnDto> EditOrAddNewCallRecord(CallRecord model, string Username);
        Task<CallRecordReturnDto> EditCallRecordWithSingleItem(CallRecord model, string Username);
        Task<ICollection<CallRecordItem>> EditHistoryItems(ICollection<CallRecordItem> items, string Username);
        Task<bool> DeleteCallRecord (int historyId);
        Task<bool> DeleteCallRecordItem(int historyitemid);
        Task<PagedList<CallRecordBriefDto>> GetCallRecordPaginated(CallRecordParams pParams, string username);
        Task<CallRecordItem> UpdateHistoryItem(CallRecordItem CallRecorditem, string UserDisplayName);
        Task<CallRecordDto> GetCallRecordDtoByParams(CallRecordParams histParams);
        Task<CallRecord> GetOrGenerateCallRecord(string personType, string personId,string Username);
    }
}