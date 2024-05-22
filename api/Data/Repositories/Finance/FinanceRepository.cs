using System.Data.Common;
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
                .ProjectTo<COA>(_mapper.ConfigurationProvider)
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
            int no = await _context.Vouchers.Select(x => x.VoucherNo).MaxAsync();

            return no == 0 ? 1000 : no + 1;
        }
 
 
        public async Task<Voucher> AddNewVoucher(Voucher voucher, string Username)
        {
            _context.Entry(voucher).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return voucher;
        }

        public async Task<Voucher> GetVoucher(int id)
        {
            var voucher = await _context.Vouchers.Include(x => x.VoucherItems)
                .Where(x => x.Id == id).FirstOrDefaultAsync();
            
            return voucher;
        }

        public async Task<PagedList<Voucher>> GetVouchers(VoucherParams vParams)
        {
            var query = _context.Vouchers.AsQueryable();

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
            }
            
            var paged = await PagedList<Voucher>.CreateAsync(
                query.AsNoTracking()
                .ProjectTo<Voucher>(_mapper.ConfigurationProvider)
                , vParams.PageNumber, vParams.PageSize);
    

            return paged;
            
        }

        public async Task<bool> EditVoucher(Voucher newObject)
        {
            var existing = await _context.Vouchers.Include(x => x.VoucherItems)
                .Where(x => x.Id == newObject.Id).AsNoTracking().FirstOrDefaultAsync();
            
            _context.Entry(existing).CurrentValues.SetValues(newObject);

             //delete records in existingObject that are not present in new object
            foreach (var existingItem in existing.VoucherItems.ToList())
            {
                if(!newObject.VoucherItems.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                {
                    _context.VoucherItems.Remove(existingItem);
                    _context.Entry(existingItem).State = EntityState.Deleted; 
                }
            }

            //items in current object - either updated or new items
            foreach(var newItem in newObject.VoucherItems)
            {
                var existingItem = existing.VoucherItems
                    .Where(c => c.Id == newItem.Id && c.Id != default(int)).SingleOrDefault();
                if(existingItem != null)    //update navigation record
                {
                    _context.Entry(existingItem).CurrentValues.SetValues(newItem);
                    _context.Entry(existingItem).State = EntityState.Modified;
                } else {    //insert new navigation record
                        
                    var itemToInsert = new VoucherItem
                    {
                        AccountName = await GetAccountNameFromId(newItem.COAId),
                        COAId = newItem.COAId,
                        Cr = newItem.Cr, Dr = newItem.Dr,
                        Narration = newItem.Narration,
                        Remarks = newItem.Remarks, TransDate=newItem.TransDate
                    };

                    existing.VoucherItems.Add(itemToInsert);
                    _context.Entry(itemToInsert).State = EntityState.Added;
                }
            }

            _context.Entry(existing).State = EntityState.Modified;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteVoucher(int id)
        {
            var task = await _context.Vouchers.FindAsync(id);
            if(task == null) return false;

            _context.Vouchers.Remove(task);
            _context.Entry(task).State = EntityState.Deleted;

            return await _context.SaveChangesAsync() > 0;
        }
   
        public Task<bool> UpdateCashAndBankDebitApprovals(ICollection<UpdatePaymentConfirmationDto> updateDto)
        {
            throw new NotImplementedException();
        }

        public async Task<string> AddVoucherAttachments(ICollection<VoucherAttachment> attachments)
        {
            var ct = 0;
            foreach(var item in attachments) {
                if(item.VoucherId ==0 || string.IsNullOrEmpty(item.FileName) || item.AttachmentSizeInBytes ==0) continue;
                ct++;
                _context.voucherAttachments.Add(item);
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

        public async Task<VoucherWithNewAttachmentDto> UpdateFinanceVoucherWithFileUploads(Voucher model)
		{
			var fileDirectory = Directory.GetCurrentDirectory();
			List<string>  attachmentsToDelete = new List<string>();          //lsit of files to delete physically from the api space
               	List<VoucherAttachment> attachmentsToAdd = new List<VoucherAttachment>();
			
			var existingVoucher = await _context.Vouchers.Where(x => x.Id == model.Id)
				.Include(x => x.VoucherItems)
				.Include(x => x.VoucherAttachments)
				.FirstOrDefaultAsync();

            	if(existingVoucher==null) return null;

            	_context.Entry(existingVoucher).CurrentValues.SetValues(model);

			//delete from DB those child items which are not present in the model
			foreach(var existingItem in existingVoucher.VoucherItems)
			{
				if(!model.VoucherItems.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
				{
					_context.VoucherItems.Remove(existingItem);
					_context.Entry(existingItem).State=EntityState.Deleted;
				}
			}
            	
			//items that are not deleted, are either to be updated or new added;
			foreach(var item in model.VoucherItems)
			{
				var existingItem = existingVoucher.VoucherItems.Where(c => c.Id == item.Id && c.Id != default(int)).SingleOrDefault();
				if (existingItem != null) {
					_context.Entry(existingItem).CurrentValues.SetValues(item);
					_context.Entry(existingItem).State = EntityState.Modified;
				} else {
					var newItem = new VoucherItem {
						VoucherId=existingVoucher.Id,
						TransDate = item.TransDate,
						COAId = item.COAId,
						AccountName = item.AccountName,
						Dr = item.Dr,
						Cr = item.Cr,
						Narration = item.Narration
					};
					existingVoucher.VoucherItems.Add(newItem);
					_context.Entry(newItem).State = EntityState.Added;
				}
			}

			foreach(var existingItem in existingVoucher.VoucherAttachments)
			{
				if(!model.VoucherAttachments.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
				{
					_context.voucherAttachments.Remove(existingItem);
					_context.Entry(existingItem).State=EntityState.Deleted;
					//prepare to delete files physically from storage folder
					var filepath = existingItem.Url ?? fileDirectory + "/assets/images";
                              attachmentsToDelete.Add(filepath + "/" + existingItem.FileName);        //save file nams to delete later
				}
			}
            	
			//items that are not deleted, are either to be updated or new added;
			foreach(var item in model.VoucherAttachments)
			{
				var existingItem = existingVoucher.VoucherAttachments.Where(c => c.Id == item.Id && c.Id != default(int)).SingleOrDefault();
				if (existingItem != null) {
					_context.Entry(existingItem).CurrentValues.SetValues(item);
					_context.Entry(existingItem).State = EntityState.Modified;
				} 
				/*//new attachments are inserted in voucherAttachment table after they are uploaded to the designated folder - in the Controller
				else {
					var newItem = new VoucherAttachment (model.Id, 
					item.AttachmentSizeInBytes, item.FileName, item.Url, item.DateUploaded, 0);
					
					existingVoucher.VoucherAttachments.Add(newItem);
					_context.Entry(newItem).State = EntityState.Added;
					attachmentsToAdd.Add(item);
				}
				*/
			}


			_context.Entry(existingVoucher).State=EntityState.Modified;

			int recordsAffected = 0;

			try {
				recordsAffected = await _context.SaveChangesAsync();
			} catch (Exception ex)
			{
				Console.Write(ex.Message);
				return null;
			}
			
			if(recordsAffected > 0 && attachmentsToDelete.Count > 0) {
				do {
					try {
						File.Delete(attachmentsToDelete[attachmentsToDelete.Count]);
					} catch (Exception ex) {
						Console.Write(ex.Message);
					}
				} while (attachmentsToDelete.Count > 0);
			}
        
            return new VoucherWithNewAttachmentDto{
                Voucher = existingVoucher,
                NewAttachments = attachmentsToAdd
            };

		}

    }
}