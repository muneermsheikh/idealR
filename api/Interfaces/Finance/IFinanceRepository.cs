using api.DTOs.Finance;
using api.Entities.Finance;
using api.Helpers;
using api.Params;
using api.Params.Finance;

namespace api.Interfaces.Finance
{
    public interface IFinanceRepository
    {
        Task<PagedList<COA>> GetCOAPagedList(COAParams coaParams);
        Task<ICollection<COA>> GetCOAList(COAParams coaParams);
        Task<COA> CreateCoaForCandidateWithNoSave(int applicationno, bool create);
        Task<COA> GetSalesRecruitmentCOA();
        Task<PagedList<PendingDebitApprovalDto>> GetPendingDebitApprovals(DrApprovalParams paginationParams);
        Task<bool>UpdateCashAndBankDebitApprovals(ICollection<UpdatePaymentConfirmationDto> updateDto);
        Task<COA> GetCOA(COAParams coaParams);
        Task<string> GetAccountNameFromId(int Id);
        Task<COA> SaveNewCOA(COA COA);
        Task<COA> EditCOA(COA COA);
        Task<bool> DeleteCOA(int id);

        //
        Task<PagedList<Voucher>> GetVouchers(VoucherParams voucherParams);
        Task<Voucher> GetVoucher(int id);
        Task<Voucher> AddNewVoucher(Voucher voucher, string Username);
        Task<bool> EditVoucher(Voucher voucher);
        Task<bool> DeleteVoucher(int id);
        Task<int> GetNextVoucherNo();
        Task<StatementOfAccountDto> GetStatementOfAccount(int accountid, DateOnly fromDate, DateOnly uptoDate);
        Task<long> GetClosingBalIncludingSuspense(int accountid);
        Task<ICollection<string>> GetMatchingCOANames(string testName);
        Task<string> AddVoucherAttachments(ICollection<VoucherAttachment> attachments);
        Task<VoucherWithNewAttachmentDto> UpdateFinanceVoucherWithFileUploads(Voucher model);
       
    }
}