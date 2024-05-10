using api.DTOs.Finance;
using api.Entities.Finance;
using api.Helpers;
using api.Interfaces.Finance;
using api.Params.Finance;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Repositories.Finance
{
    public class FinanceRepository : IFinanceRepository
    {
        private readonly DataContext _context;
        public FinanceRepository(DataContext context)
        {
            _context = context;
        }


        public Task<COA> AddNewCOA(COA COA)
        {
            throw new NotImplementedException();
        }

        public async Task<FinanceVoucher> AddNewVoucher(FinanceVoucher voucher, string Username)
        {
            _context.Entry(voucher).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return voucher;
        }

        public async Task<COA> CreateCoaForCandidate(int applicationno, bool create)
        {
            var candidate = await _context.Candidates.Where(x => x.ApplicationNo==applicationno).FirstOrDefaultAsync();
			if(candidate==null) return null;

			var ano=Convert.ToString(applicationno);
			
			var coa = await (from c in _context.COAs 
				where c.AccountClass=="Candidate" 
					&& c.AccountName.Contains(ano)
					&& c.AccountType.ToLower()=="b"
					select new COA {
                        Divn="R",
						Id=c.Id,
						AccountClass=c.AccountClass,
						AccountName=c.AccountName,
						AccountType=c.AccountType,
					}
				).SingleOrDefaultAsync();

			if(coa == null & !create) return null;
			
			if(coa==null && create) {
				coa = new COA{
					Divn = "R",
					AccountType="B",
					AccountName = candidate.KnownAs + "- App No." + candidate.ApplicationNo,
					AccountClass="Candidate",
					OpBalance=0
				};
				coa = await AddNewCOA(coa);
			}
			
			return coa;
        }

        public Task<bool> DeleteCOA(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteFinanceVoucher(int id)
        {
            throw new NotImplementedException();
        }

        public Task<COA> EditCOA(COA COA)
        {
            throw new NotImplementedException();
        }

        public Task<long> GetClosingBalIncludingSuspense(int accountid)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<COA>> GetCOAList()
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<COA>> GetCOAs(COAParams coaParams)
        {
            throw new NotImplementedException();
        }

        public Task<FinanceVoucher> GetFinanceVoucher(int id)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<FinanceVoucher>> GetFinanceVouchers(VoucherParams voucherParams)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<string>> GetMatchingCOANames(string testName)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<PendingDebitApprovalDto>> GetPendingDebitApprovals()
        {
            throw new NotImplementedException();
        }

        public Task<StatementOfAccountDto> GetStatementOfAccount(int accountid, DateOnly fromDate, DateOnly uptoDate)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateCashAndBankDebitApprovals(ICollection<UpdatePaymentConfirmationDto> updateDto)
        {
            throw new NotImplementedException();
        }

    }
}