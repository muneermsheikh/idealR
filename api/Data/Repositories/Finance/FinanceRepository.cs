using api.DTOs.Finance;
using api.Entities.Finance;
using api.Helpers;
using api.Interfaces.Finance;
using api.Params;
using api.Params.Finance;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Repositories.Finance
{
    public class FinanceRepository : IFinanceRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public FinanceRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<int> GetNextVoucherNo()
        {
            int no = await _context.FinanceVouchers.Select(x => x.VoucherNo).MaxAsync();

            return no + 1;
        }
        public async Task<COA> SaveNewCOA(COA coa)
        {
            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateException ex) {
                throw new Exception("Database error - " + ex.Message);
            } catch (Exception ex) {
                throw new Exception(ex.Message);
            }
            
            return coa;

        }

        public async Task<FinanceVoucher> AddNewVoucher(FinanceVoucher voucher, string Username)
        {
            _context.Entry(voucher).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return voucher;
        }

        public async Task<COA> CreateCoaForCandidateWithNoSave(int applicationno, bool create)
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
				//coa = await SaveNewCOA(coa);
			} else if(coa == null && create) {
                return null;
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

        public async Task<PagedList<PendingDebitApprovalDto>> GetPendingDebitApprovals(DrApprovalParams pParams)
        {
            var cashandbank = await _context.COAs.Where(x => x.AccountClass=="CashAndBank").Select(x => x.Id).ToListAsync();

			var qry = (from e in _context.VoucherItems
				where e.DrEntryApproved != true & e.Dr > 0  & cashandbank.Contains(e.COAId)
				join v in _context.Vouchers on e.VoucherId equals v.Id
				select new PendingDebitApprovalDto{
                     DrAccountId=e.COAId, DrAccountName=e.AccountName, DrAmount=e.Dr, VoucherItemId=e.Id, 
                     DrEntryApproved = e.DrEntryApproved, VoucherDated =v.VoucherDated, VoucherNo=v.VoucherNo
                }).AsQueryable();
			
            if(!string.IsNullOrEmpty(pParams.AccountName)) 
                qry = qry.Where(x => x.DrAccountName.ToLower() == pParams.AccountName.ToLower());
            
			var paged = await PagedList<PendingDebitApprovalDto>.CreateAsync(
                qry.AsNoTracking()
                .ProjectTo<PendingDebitApprovalDto>(_mapper.ConfigurationProvider)
                , pParams.PageNumber, pParams.PageSize);
            return paged;
        }

        public Task<StatementOfAccountDto> GetStatementOfAccount(int accountid, DateOnly fromDate, DateOnly uptoDate)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateCashAndBankDebitApprovals(ICollection<UpdatePaymentConfirmationDto> updateDto)
        {
            throw new NotImplementedException();
        }

        public async Task<COA> GetSalesRecruitmentCOA()
        {
            var coa = await _context.COAs
                .Where(x => x.AccountName.ToLower() == "sales recruitment" && x.AccountType=="I")
                .FirstOrDefaultAsync();
            
            return coa;
        }

        public async Task<COA> GetCOA(COAParams coaParams)
        {
            var query = _context.COAs.AsQueryable();

            if(!string.IsNullOrEmpty(coaParams.AccountType)) query = query.Where(x => x.AccountType== coaParams.AccountType);
            if(!string.IsNullOrEmpty(coaParams.AccountName)) query = query.Where(x => x.AccountName.ToLower().Contains(coaParams.AccountName.ToLower()));
            
            return await query.FirstOrDefaultAsync();
        }

    }
}