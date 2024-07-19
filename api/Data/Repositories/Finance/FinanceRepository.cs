using System.Data.Common;
using api.DTOs.Finance;
using api.Entities.Finance;
using api.Helpers;
using api.Interfaces.Finance;
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
        public async Task<COA> GetOrCreateCoaForCandidateWithNoSave(int applicationno, bool create)
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

        public async Task<bool> DeleteCOA(int id)
        {
            var coa = await _context.COAs.FindAsync(id);
            if(coa == null) return false;

            _context.COAs.Remove(coa);
            _context.Entry(coa).State = EntityState.Deleted;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<COA> EditCOA(COA coa)
        {
            var existing = await _context.COAs.FindAsync(coa.Id);

            if(existing == null) return null;

            _context.Entry(existing).CurrentValues.SetValues(coa);

            return await _context.SaveChangesAsync() > 0 ? coa : null;
        }

        public Task<long> GetClosingBalIncludingSuspense(int accountid)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedList<COA>> GetCOAPagedList(COAParams coaParams)
        {
            var query = _context.COAs.AsQueryable();
            if(coaParams.Id != 0) {
                query = query.Where(x => x.Id == coaParams.Id);
            } else {
                if(!string.IsNullOrEmpty(coaParams.AccountName)) query = query.Where(x => x.AccountName.ToLower() == coaParams.AccountName.ToLower());
                if(!string.IsNullOrEmpty(coaParams.AccountType)) query = query.Where(x => x.AccountName.ToLower() == coaParams.AccountType.ToLower());
                if(!string.IsNullOrEmpty(coaParams.Divn)) query = query.Where(x => x.Divn.ToLower() == coaParams.Divn.ToLower());
            }
             
            var paged = await PagedList<COA>.CreateAsync(
                query.AsNoTracking()
                //.ProjectTo<COA>(_mapper.ConfigurationProvider)
                , coaParams.PageNumber, coaParams.PageSize);
    

            return paged;
        }

        public async Task<ICollection<COA>> GetCOAList(COAParams coaParams)
        {
            var query = _context.COAs.AsQueryable();
            if(coaParams.Id != 0) {
                query = query.Where(x => x.Id == coaParams.Id);
            } else {
                if(!string.IsNullOrEmpty(coaParams.AccountName)) query = query.Where(x => x.AccountName.ToLower() == coaParams.AccountName.ToLower());
                if(!string.IsNullOrEmpty(coaParams.AccountType)) query = query.Where(x => x.AccountName.ToLower() == coaParams.AccountType.ToLower());
                if(!string.IsNullOrEmpty(coaParams.Divn)) query = query.Where(x => x.Divn.ToLower() == coaParams.Divn.ToLower());
            }
            
            return await query.OrderBy(x => x.AccountName).ToListAsync();
        }
        public Task<ICollection<string>> GetMatchingCOANames(string testName)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedList<PendingDebitApprovalDto>> GetPendingDebitApprovals(DrApprovalParams pParams)
        {
            var cashandbank = await _context.COAs.Where(x => x.AccountClass=="CashAndBank").Select(x => x.Id).ToListAsync();

			var qry = (from e in _context.VoucherEntries
				where e.DrEntryApproved != true && e.Dr > 0  && cashandbank.Contains(e.COAId)
				join v in _context.FinanceVouchers on e.FinanceVoucherId equals v.Id
				select new PendingDebitApprovalDto{
                     DrAccountId=e.COAId, DrAccountName=e.AccountName, DrAmount=e.Dr, VoucherEntryId=e.Id, 
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

        public async Task<StatementOfAccountDto> GetStatementOfAccount(int accountid, DateOnly fromDate, DateOnly uptoDate)
        {
           //DateTime uptoDate = UptoDate.Hour < 1 ? UptoDate.AddHours(23) : UptoDate;
			
			var trans =  await (from i in _context.VoucherEntries 
                    where i.COAId == accountid && i.TransDate >= fromDate && i.TransDate <= uptoDate
				join v in _context.FinanceVouchers on i.FinanceVoucherId equals v.Id
				join a in _context.COAs on i.COAId equals a.Id
				orderby i.TransDate descending
				select new StatementOfAccountItemDto {
                    Id = i.Id,
					VoucherNo = v.VoucherNo,
					TransDate = i.TransDate,
					COAId = a.Id,
					AccountName = a.AccountName,
					Dr = i.Dr,
					Cr = i.Cr,
					Narration = i.Narration
				}).ToListAsync();
						
						
			var transtest = await (from v in _context.VoucherEntries where v.COAId==accountid 
				select new {v.Id, v.TransDate, v.COAId, v.AccountName, v.Dr, v.Cr})
                    .OrderByDescending(x => x.TransDate).ToListAsync();
			var opBal = await (from v in _context.VoucherEntries where v.COAId==accountid && v.TransDate < fromDate
				group v by v.COAId into g 
				select new {Id = g.Key, Bal = g.Sum(e => -e.Cr) + g.Sum(E => E.Dr)}).FirstOrDefaultAsync();
			var oclBalTest = await (from v in _context.VoucherEntries where v.COAId==accountid && v.TransDate >= uptoDate
				select new {v.Id, v.TransDate, v.COAId, v.AccountName, v.Dr, v.Cr}).ToListAsync();

			var BalForThePeriod = await (from v in _context.VoucherEntries 
					where v.COAId==accountid 
						&& v.TransDate >= fromDate 
						&& v.TransDate <= uptoDate
				group v by v.COAId into g 
				select new {Id = g.Key, Bal = -g.Sum(e => e.Cr) + g.Sum(E => E.Dr)}).FirstOrDefaultAsync();

			var dto = new StatementOfAccountDto{
				AccountId=accountid,
				AccountName= trans.Count()==0 ? await GetAccountNameFromCOA(accountid) : trans[0].AccountName, 
				FromDate = fromDate,
				UptoDate = uptoDate,
				StatementOfAccountItems = trans,
				OpBalance = opBal==null? 0 : opBal.Bal,
				ClBalance = BalForThePeriod==null ? 0 : BalForThePeriod.Bal
			};

			return dto;	
        }

        private async Task<string> GetAccountNameFromCOA(int coaid) {
			var s = await _context.COAs.Where(x => x.Id==coaid).Select(x=> x.AccountName).FirstOrDefaultAsync();
			if(s==null) return "";
			return s;
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

        public async Task<string> GetAccountNameFromId(int Id)
        {
            var coa = await _context.COAs.FindAsync(Id);
            if(coa==null) return "";
            return coa.AccountName;
        }

        //vouchers
        public async Task<int> GetNextVoucherNo()
        {
            var vno = await _context.FinanceVouchers
                .OrderByDescending(x => x.VoucherNo)
                .Select(x => x.VoucherNo)
                .Take(1).FirstOrDefaultAsync();


            return vno == 0 ? 1000 : vno + 1;
        }
 
         public async Task<Voucher> AddNewVoucher(Voucher voucher, string Username)
        {
            _context.Entry(voucher).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return voucher;
        }

        public async Task<FinanceVoucher> GetVoucher(int id)
        {
            var voucher = await _context.FinanceVouchers.Include(x => x.VoucherEntries)
                .Where(x => x.Id == id).FirstOrDefaultAsync();
            
            return voucher;
        }

        public async Task<PagedList<FinanceVoucher>> GetVouchers(VoucherParams vParams)
        {
            var query = _context.FinanceVouchers.AsQueryable();

            if(vParams.VoucherNo !=0) {
                query = query.Where(x => x.VoucherNo == vParams.VoucherNo);
            } else {
                if(vParams.VoucherDated.Year > 2000) query = query.Where(x => x.VoucherDated == vParams.VoucherDated);
                if(vParams.DateFrom.Year > 2000 && vParams.DateUpto.Year > 2000) 
                    query = query.Where(x => x.VoucherDated >= vParams.DateFrom &&
                        x.VoucherDated <= vParams.DateUpto);
                if(vParams.CoaId !=0) query = query.Where(x => x.COAId == vParams.CoaId);
                if(vParams.Amount != 0) query = query.Where(x => x.Amount == vParams.Amount);
                if(!string.IsNullOrEmpty(vParams.Divn)) query = query.Where(x => x.Divn == vParams.Divn);
                if(!string.IsNullOrEmpty(vParams.Search)) query = query.Where(x => x.AccountName.ToLower().Contains(vParams.Search.ToLower()));
            }
         
            var paged = await PagedList<FinanceVoucher>.CreateAsync(
                query.AsNoTracking()
                //.ProjectTo<Voucher>(_mapper.ConfigurationProvider)
                , vParams.PageNumber, vParams.PageSize);
    

            return paged;
            
        }

        public async Task<bool> EditVoucher(FinanceVoucher newObject)
        {
            var existing = await _context.FinanceVouchers.Include(x => x.VoucherEntries)
                .Where(x => x.Id == newObject.Id).AsNoTracking().FirstOrDefaultAsync();
            
            _context.Entry(existing).CurrentValues.SetValues(newObject);

             //delete records in existingObject that are not present in new object
            foreach (var existingItem in existing.VoucherEntries.ToList())
            {
                if(!newObject.VoucherEntries.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                {
                    _context.VoucherEntries.Remove(existingItem);
                    _context.Entry(existingItem).State = EntityState.Deleted; 
                }
            }

            //items in current object - either updated or new items
            foreach(var newItem in newObject.VoucherEntries)
            {
                var existingItem = existing.VoucherEntries
                    .Where(c => c.Id == newItem.Id && c.Id != default(int)).SingleOrDefault();
                if(existingItem != null)    //update navigation record
                {
                    _context.Entry(existingItem).CurrentValues.SetValues(newItem);
                    _context.Entry(existingItem).State = EntityState.Modified;
                } else {    //insert new navigation record
                        
                    var itemToInsert = new VoucherEntry
                    {
                        AccountName = await GetAccountNameFromId(newItem.COAId),
                        COAId = newItem.COAId,
                        Cr = newItem.Cr, Dr = newItem.Dr,
                        Narration = newItem.Narration,
                        Remarks = newItem.Remarks, TransDate=newItem.TransDate
                    };

                    existing.VoucherEntries.Add(itemToInsert);
                    _context.Entry(itemToInsert).State = EntityState.Added;
                }
            }

            _context.Entry(existing).State = EntityState.Modified;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteVoucher(int id)
        {
            var task = await _context.FinanceVouchers.FindAsync(id);
            if(task == null) return false;

            _context.FinanceVouchers.Remove(task);
            _context.Entry(task).State = EntityState.Deleted;

            return await _context.SaveChangesAsync() > 0;
        }
   
        public async Task<bool> UpdateTransactionConfirmations(ICollection<PendingDebitApprovalDto> updateDto)
        {
            var transactions = (ICollection<VoucherItem>)_mapper.Map<VoucherItem>(updateDto);

            foreach(var entry in transactions) {
                _context.Entry(entry).State = EntityState.Modified;
            }

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<string> AddVoucherAttachments(ICollection<VoucherAttachment> attachments)
        {
            var ct = 0;
            foreach(var item in attachments) {
                if(item.FinanceVoucherId ==0 || string.IsNullOrEmpty(item.FileName) || item.AttachmentSizeInBytes ==0) continue;
                ct++;
                _context.VoucherAttachments.Add(item);
            }

            if(ct==0) return ct + " number of file attachments invalid";

            try { 
                await _context.SaveChangesAsync();
            } catch(DbException ex) {
                return ex.Message;
            } catch (Exception ex) {
                return ex.Message;
            }

            return "";
        }

        public async Task<FinanceVoucher> UpdateFinanceVoucher(FinanceVoucher model)
		{
			var fileDirectory = Directory.GetCurrentDirectory();
			List<string>  attachmentsToDelete = new List<string>();          //lsit of files to delete physically from the api space
               	List<VoucherAttachment> attachmentsToAdd = new List<VoucherAttachment>();
			
			var existingVoucher = await _context.FinanceVouchers.Where(x => x.Id == model.Id)
				.Include(x => x.VoucherEntries)
				//.Include(x => x.VoucherAttachments)
				.FirstOrDefaultAsync();

            	if(existingVoucher==null) return null;

            	_context.Entry(existingVoucher).CurrentValues.SetValues(model);

			//delete from DB those child items which are not present in the model
			foreach(var existingItem in existingVoucher.VoucherEntries)
			{
				if(!model.VoucherEntries.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
				{
					_context.VoucherEntries.Remove(existingItem);
					_context.Entry(existingItem).State=EntityState.Deleted;
				}
			}
            	
			//items that are not deleted, are either to be updated or new added;
			foreach(var item in model.VoucherEntries)
			{
				var existingItem = existingVoucher.VoucherEntries.Where(c => c.Id == item.Id && c.Id != default(int)).SingleOrDefault();
				if (existingItem != null) {
					_context.Entry(existingItem).CurrentValues.SetValues(item);
					_context.Entry(existingItem).State = EntityState.Modified;
				} else {
					var newItem = new VoucherEntry {
						FinanceVoucherId=existingVoucher.Id,
						TransDate = item.TransDate,
						COAId = item.COAId,
						AccountName = item.AccountName,
						Dr = item.Dr,
						Cr = item.Cr,
						Narration = item.Narration
					};
					existingVoucher.VoucherEntries.Add(newItem);
					_context.Entry(newItem).State = EntityState.Added;
				}
			}

			_context.Entry(existingVoucher).State=EntityState.Modified;

			int recordsAffected = 0;

			try {
				recordsAffected = await _context.SaveChangesAsync();
            } catch (DbException ex) {
                Console.Write(ex.Message);
                return null;
			} catch (Exception ex) {
				Console.Write(ex.Message);
				return null;
			}

            return existingVoucher;
			
		}

    }
}