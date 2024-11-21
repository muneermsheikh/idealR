using System.Data.Common;
using api.DTOs.Finance;
using api.Entities.Finance;
using api.Helpers;
using api.Interfaces.Finance;
using api.Params.Finance;
using AutoMapper;
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
            var ct=0;
            try {
                _context.COAs.Add(coa);
                ct = await _context.SaveChangesAsync();
            } catch (DbUpdateException ex) {
                throw new Exception("Database error - " + ex.Message);
            } catch (Exception ex) {
                throw new Exception(ex.Message);
            }
            
            return ct > 0 ? coa : null;

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

        public async Task<PagedList<COA>> GetCOAPagedList(ParamsCOA coaParams)
        {
            var query = _context.COAs.AsQueryable();
            if(coaParams.Id != 0) {
                query = query.Where(x => x.Id == coaParams.Id);
            } else {
                if(!string.IsNullOrEmpty(coaParams.AccountName)) query = query.Where(x => x.AccountName.ToLower() == coaParams.AccountName.ToLower());
                if(!string.IsNullOrEmpty(coaParams.AccountType)) query = query.Where(x => x.AccountName.ToLower() == coaParams.AccountType.ToLower());
                if(!string.IsNullOrEmpty(coaParams.Divn)) query = query.Where(x => x.Divn.ToLower() == coaParams.Divn.ToLower());
                if(!string.IsNullOrEmpty(coaParams.DivisionToExclude)) query = query.Where(x => x.Divn != coaParams.DivisionToExclude);
                if(!string.IsNullOrEmpty(coaParams.Search)) query = query.Where(x => x.AccountName.ToLower().Contains(coaParams.Search.ToLower()));
            }
             
            var paged = await PagedList<COA>.CreateAsync(
                query.AsNoTracking()
                //.ProjectTo<COA>(_mapper.ConfigurationProvider)
                , coaParams.PageNumber, coaParams.PageSize);
    
            return paged;
        }

        public async Task<ICollection<COA>> GetCOAList(ParamsCOA coaParams)
        {
            var query = _context.COAs.AsQueryable();
            if(coaParams.Id != 0) {
                query = query.Where(x => x.Id == coaParams.Id);
            } else {
                if(!string.IsNullOrEmpty(coaParams.AccountName)) query = query.Where(x => x.AccountName.ToLower() == coaParams.AccountName.ToLower());
                if(!string.IsNullOrEmpty(coaParams.AccountType)) query = query.Where(x => x.AccountName.ToLower() == coaParams.AccountType.ToLower());
                if(!string.IsNullOrEmpty(coaParams.Divn)) query = query.Where(x => x.Divn.ToLower() == coaParams.Divn.ToLower());
                if(!string.IsNullOrEmpty(coaParams.DivisionToExclude)) query = query.Where(x => x.Divn.ToLower() != coaParams.DivisionToExclude.ToLower());
            }
            
            return await query.OrderBy(x => x.AccountName).ToListAsync();
        }
        public async Task<ICollection<string>> GetMatchingCOANames(string testName)
        {
            var matchingnames = await _context.COAs.Where(x => x.AccountName.ToLower().Contains(testName.ToLower()))
                .Select(x => x.AccountType).ToListAsync();
            if(matchingnames.Count > 0) return matchingnames;
            return null;
        }

        public async Task<PagedList<PendingDebitApprovalDto>> GetPendingDebitApprovals(DrApprovalParams pParams)
        {
            var cashandbank = await _context.COAs.Where(x => x.AccountClass=="banks" || x.AccountClass=="personalaccount").Select(x => x.Id).ToListAsync();

			var qry = (from e in _context.VoucherEntries
				where (e.DrEntryApproved != true || e.DrEntryApproved == null) && e.Dr > 0  && cashandbank.Contains(e.CoaId)
				join v in _context.FinanceVouchers on e.FinanceVoucherId equals v.Id
				select new PendingDebitApprovalDto{
                     DrAccountId=e.CoaId, DrAccountName=e.AccountName, DrAmount=e.Dr, VoucherEntryId=e.Id, 
                     DrEntryApproved = Convert.ToBoolean(e.DrEntryApproved), Id=e.FinanceVoucherId,
                     VoucherDated = DateOnly.FromDateTime(v.VoucherDated), VoucherNo=v.VoucherNo
                }).AsQueryable();
			
            if(!string.IsNullOrEmpty(pParams.AccountName)) 
                qry = qry.Where(x => x.DrAccountName.ToLower() == pParams.AccountName.ToLower());
           
            var test = await qry.ToListAsync();

			var paged = await PagedList<PendingDebitApprovalDto>.CreateAsync(
                qry.AsNoTracking()
                //.ProjectTo<PendingDebitApprovalDto>(_mapper.ConfigurationProvider)
                , pParams.PageNumber, pParams.PageSize);
            return paged;
        }

        public async Task<StatementOfAccountDto> GetStatementOfAccount(int accountid, DateTime fromDate, DateTime uptoDate)
        {
           //DateTime uptoDate = UptoDate.Hour < 1 ? UptoDate.AddHours(23) : UptoDate;
			
			var trans =  await (from i in _context.VoucherEntries 
                    where i.CoaId == accountid && 
                        i.TransDate >= fromDate && 
                        i.TransDate <= uptoDate
				join v in _context.FinanceVouchers on i.FinanceVoucherId equals v.Id
				join a in _context.COAs on i.CoaId equals a.Id
				orderby i.TransDate descending
				select new StatementOfAccountItemDto {
                    Id = i.Id,
					VoucherNo = v.VoucherNo,
					TransDate = DateOnly.FromDateTime(i.TransDate),
					CoaId = a.Id,
					AccountName = a.AccountName,
					Dr = i.Dr,
					Cr = i.Cr,
					Narration = i.Narration
				}).ToListAsync();
						
						
			var transtest = await (from v in _context.VoucherEntries where v.CoaId==accountid 
				select new {v.Id, v.TransDate, v.CoaId, v.AccountName, v.Dr, v.Cr})
                    .OrderByDescending(x => x.TransDate).ToListAsync();
			var opBal = await (from v in _context.VoucherEntries where v.CoaId==accountid && 
                v.TransDate < fromDate
				group v by v.CoaId into g 
				select new {Id = g.Key, Bal = g.Sum(e => -e.Cr) + g.Sum(E => E.Dr)}).FirstOrDefaultAsync();
			var oclBalTest = await (from v in _context.VoucherEntries where v.CoaId==accountid 
                    && v.TransDate >= uptoDate
				select new {v.Id, v.TransDate, v.CoaId, v.AccountName, v.Dr, v.Cr}).ToListAsync();

			var BalForThePeriod = await (from v in _context.VoucherEntries 
					where v.CoaId==accountid 
						&& v.TransDate >= fromDate 
						&& v.TransDate <= uptoDate
				group v by v.CoaId into g 
				select new {Id = g.Key, Bal = -g.Sum(e => e.Cr) + g.Sum(E => E.Dr)}).FirstOrDefaultAsync();
            
            DateTime dt1 = DateTime.Parse(fromDate.ToString());
            DateTime dt2 = DateTime.Parse(uptoDate.ToString());
         
            var dto = new StatementOfAccountDto{
				AccountId=accountid,
				AccountName= trans.Count == 0 ? await GetAccountNameFromCOA(accountid) : trans[0].AccountName, 
				FromDate = DateOnly.FromDateTime(dt1),
				UptoDate = DateOnly.FromDateTime(dt2),
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

        public async Task<COA> GetCOA(ParamsCOA coaParams)
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
 
         public async Task<FinanceVoucher> AddNewVoucher(FinanceVoucher voucher, string Username)
        {
            if(voucher.VoucherNo != 0) return null;

            var accountnm = await _context.COAs.FindAsync(voucher.CoaId);
            if(accountnm==null) return null;
            voucher.AccountName = accountnm.AccountName;

            var fVoucher = new FinanceVoucher {
                VoucherNo = await GetNextVoucherNo(), AccountName = accountnm.AccountName,
                Amount = voucher.Amount, CoaId = voucher.CoaId, Divn=voucher.Divn, Narration = voucher.Narration,
                PartyName = voucher.PartyName, VoucherDated = voucher.VoucherDated};

            var entries = new List<VoucherEntry>();
            foreach(var newItem in voucher.VoucherEntries)
            {
                var itemToInsert = new VoucherEntry
                    {
                        AccountName = await GetAccountNameFromId(newItem.CoaId),
                        CoaId = newItem.CoaId,
                        Cr = newItem.Cr, Dr = newItem.Dr,
                        Narration = newItem.Narration,
                        Remarks = newItem.Remarks, TransDate=newItem.TransDate
                    };

                    entries.Add(itemToInsert);
            }

            fVoucher.VoucherEntries = entries;

            _context.FinanceVouchers.Add(fVoucher);

            try {
                await _context.SaveChangesAsync();
                return fVoucher;
            } catch {
                return null;
            }

        }

        public async Task<FinanceVoucher> GetVoucher(int id)
        {
            var voucher = await _context.FinanceVouchers.Include(x => x.VoucherEntries)
                .Where(x => x.Id == id).FirstOrDefaultAsync();
            
            return voucher;
        }

        public async Task<PagedList<FinanceVoucher>> GetVouchers(VoucherParams vParams)
        {
            var query = _context.FinanceVouchers.OrderBy(x => x.VoucherNo).AsQueryable();

            if(vParams.VoucherNo !=0) {
                query = query.Where(x => x.VoucherNo == vParams.VoucherNo);
            } else {
                if(vParams.VoucherDated.Year > 2000) query = query.Where(x => 
                    DateOnly.FromDateTime(x.VoucherDated) == vParams.VoucherDated);
                if(vParams.DateFrom.Year > 2000 && vParams.DateUpto.Year > 2000) 
                    query = query.Where(x => 
                        DateOnly.FromDateTime(x.VoucherDated) >= vParams.DateFrom &&
                        DateOnly.FromDateTime(x.VoucherDated) <= vParams.DateUpto);
                if(vParams.CoaId !=0) query = query.Where(x => x.CoaId == vParams.CoaId);
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

        public async Task<FinanceVoucher> EditVoucher(FinanceVoucher newObject)
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
                        AccountName = await GetAccountNameFromId(newItem.CoaId),
                        CoaId = newItem.CoaId,
                        Cr = newItem.Cr, Dr = newItem.Dr,
                        Narration = newItem.Narration,
                        Remarks = newItem.Remarks, TransDate=newItem.TransDate
                    };

                    existing.VoucherEntries.Add(itemToInsert);
                    _context.Entry(itemToInsert).State = EntityState.Added;
                }
            }

            _context.Entry(existing).State = EntityState.Modified;

            if (await _context.SaveChangesAsync() > 0) {return existing;} else { return null;}
        }

        public async Task<bool> DeleteVoucher(int id)
        {
            var task = await _context.FinanceVouchers.FindAsync(id);
            if(task == null) return false;

            _context.FinanceVouchers.Remove(task);
            _context.Entry(task).State = EntityState.Deleted;

            return await _context.SaveChangesAsync() > 0;
        }
   
        public async Task<string> UpdateVoucherEntries(ICollection<VoucherEntry> entries)
        {
            
            foreach(var entry in entries) {
                var existing = await _context.VoucherEntries.Where(x => x.Id==entry.Id).AsNoTracking().FirstOrDefaultAsync();
                if(existing == null) continue;
                _context.Entry(existing).CurrentValues.SetValues(entry);
                _context.Entry(existing).State = EntityState.Modified;
            }

            try {
                await _context.SaveChangesAsync();
            } catch (DbException ex) {
                return ex.Message;
            } catch (Exception ex) {
                return ex.Message;
            }

            return "";
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
						CoaId = item.CoaId,
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

        public async Task<bool> ApproveDrApprovals(ICollection<int> CoaIds, string username)
        {
            var entries = await _context.VoucherEntries.Where(x => CoaIds.Contains(x.Id)).ToListAsync();

            foreach(var entry in entries) {
                entry.DrEntryApprovedOn = DateTime.Now;
                entry.DrEntryApprovedByUsername = username;
                entry.DrEntryApproved=true;

                _context.Entry(entry).State = EntityState.Modified;
            }

            return await _context.SaveChangesAsync() > 0;
        }

    }
}