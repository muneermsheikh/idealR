using api.DTOs.Finance;
using api.Entities.Finance;
using api.Helpers;
using api.Params;
using api.Params.Finance;

namespace api.Interfaces.Finance
{
    public interface IFinanceRepository
    {
        Task<PagedList<COA>> GetCOAs(COAParams coaParams);
        Task<COA> CreateCoaForCandidateWithNoSave(int applicationno, bool create);
        Task<COA> GetSalesRecruitmentCOA();
        Task<PagedList<PendingDebitApprovalDto>> GetPendingDebitApprovals(DrApprovalParams paginationParams);
        Task<bool>UpdateCashAndBankDebitApprovals(ICollection<UpdatePaymentConfirmationDto> updateDto);
        Task<COA> GetCOA(COAParams coaParams);
        Task<COA> SaveNewCOA(COA COA);
        Task<COA> EditCOA(COA COA);
        Task<bool> DeleteCOA(int id);
        Task<PagedList<FinanceVoucher>> GetFinanceVouchers(VoucherParams voucherParams);
        Task<FinanceVoucher> GetFinanceVoucher(int id);
        Task<FinanceVoucher> AddNewVoucher(FinanceVoucher voucher, string Username);
        Task<bool> DeleteFinanceVoucher(int id);
        Task<int> GetNextVoucherNo();
        Task<StatementOfAccountDto> GetStatementOfAccount(int accountid, DateOnly fromDate, DateOnly uptoDate);
        Task<long> GetClosingBalIncludingSuspense(int accountid);
        Task<ICollection<string>> GetMatchingCOANames(string testName);
       
    }
}