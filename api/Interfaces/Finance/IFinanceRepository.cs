using api.DTOs.Finance;
using api.Entities.Finance;
using api.Helpers;
using api.Params;
using api.Params.Finance;

namespace api.Interfaces.Finance
{
    public interface IFinanceRepository
    {
        Task<PagedList<COA>> GetCOAPagedList(ParamsCOA coaParams);
        Task<ICollection<COA>> GetCOAList(ParamsCOA coaParams);
        Task<COA> GetOrCreateCoaForCandidateWithNoSave(int applicationno, bool create);
        Task<COA> GetSalesRecruitmentCOA();
        Task<PagedList<PendingDebitApprovalDto>> GetPendingDebitApprovals(DrApprovalParams paginationParams);
        Task<string>UpdateVoucherEntries(ICollection<VoucherEntry> entries);
        Task<COA> GetCOA(ParamsCOA coaParams);
        Task<string> GetAccountNameFromId(int Id);
        Task<COA> SaveNewCOA(COA COA);
        Task<COA> EditCOA(COA COA);
        Task<bool> DeleteCOA(int id);

        //
        Task<PagedList<FinanceVoucher>> GetVouchers(VoucherParams voucherParams);
        Task<FinanceVoucher> GetVoucher(int id);
        Task<FinanceVoucher> AddNewVoucher(FinanceVoucher voucher, string Username);
        Task<FinanceVoucher> EditVoucher(FinanceVoucher voucher);
        Task<bool> DeleteVoucher(int id);
        Task<int> GetNextVoucherNo();
        Task<StatementOfAccountDto> GetStatementOfAccount(int accountid, DateTime fromDate, DateTime uptoDate);
        Task<long> GetClosingBalIncludingSuspense(int accountid);
        Task<ICollection<string>> GetMatchingCOANames(string testName);
        Task<string> AddVoucherAttachments(ICollection<VoucherAttachment> attachments);
        Task<FinanceVoucher> UpdateFinanceVoucher(FinanceVoucher model);
        Task<bool> ApproveDrApprovals(ICollection<int> CoaIds, string username);
        Task<long> CandidateBalance(int applicationNo);
    }
}