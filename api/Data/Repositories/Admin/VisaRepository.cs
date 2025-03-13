using api.Data.Migrations;
using api.DTOs.Admin;
using api.Entities.Admin;
using api.Helpers;
using api.Interfaces.Admin;
using api.Params.Admin;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Bcpg.OpenPgp;

namespace api.Data.Repositories.Admin
{
    public class VisaRepository : IVisaRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public VisaRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<bool> DeleteVisa(int VisaId)
        {
            var recordExists = await _context.VisaHeaders.Include(x => x.VisaItems).Where(x => x.Id == VisaId).FirstOrDefaultAsync();
            if(recordExists == null) return false;
            
            _context.Entry(recordExists).State = EntityState.Deleted;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteVisaItem(int VisaItemId)
        {
            var recordExists = await _context.VisaItems.FindAsync(VisaItemId);
            if(recordExists == null) return false;
            
            _context.Entry(recordExists).State = EntityState.Deleted;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteVisaTransaction(int visaTransactionId)
        {
            var trans = await _context.VisaTransactions.FindAsync(visaTransactionId);
            if(trans == null) return false;

            _context.Entry(trans).State = EntityState.Deleted;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<VisaHeader> EditVisa(VisaHeader newObject)
        {
            var existingObject = await _context.VisaHeaders.Where(x => x.Id == newObject.Id)
                .Include(x => x.VisaItems).AsNoTracking().SingleOrDefaultAsync();
            
            if(existingObject == null) return null;

            _context.Entry(existingObject).CurrentValues.SetValues(newObject);

            foreach(var existingItem in existingObject.VisaItems)
            {
                if(!newObject.VisaItems.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                {
                    _context.VisaItems.Remove(existingItem);
                    _context.Entry(existingItem).State = EntityState.Deleted;
                }
            }

            foreach(var newItem in newObject.VisaItems)
            {
                var existingItem = existingObject.VisaItems
                    .Where(c => c.Id == newItem.Id && c.Id != default(int)).SingleOrDefault();
                if(existingItem != null)
                {
                    _context.Entry(existingItem).CurrentValues.SetValues(newItem);
                    _context.Entry(existingItem).State = EntityState.Modified;
                } else {
                    var itemToInsert = new VisaItem {
                        SrNo = newItem.SrNo,
                        VisaHeaderId = existingObject.Id, 
                        VisaCategoryArabic = newItem.VisaCategoryArabic,
                        VisaCategoryEnglish = newItem.VisaCategoryEnglish,
                        VisaConsulate = newItem.VisaConsulate,
                        VisaQuantity = newItem.VisaQuantity,
                        VisaCanceled=false
                    };
                    existingObject.VisaItems.Add(itemToInsert);
                    _context.Entry(itemToInsert).State = EntityState.Added;
                }
            }

            _context.Entry(existingObject).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                throw new Exception(ex.Message, ex);
            }

            return existingObject;
        }

        public async Task<VisaTransaction> EditVisaTransaction(VisaTransaction vTransaction)
        {
            var existingObject = await _context.VisaTransactions.Where(x => x.Id == vTransaction.Id)
                .AsNoTracking().SingleOrDefaultAsync();
            
            if(existingObject == null) return null;

            _context.Entry(existingObject).CurrentValues.SetValues(vTransaction);

            _context.Entry(existingObject).State = EntityState.Modified;
            
            return await _context.SaveChangesAsync() > 0 ? existingObject : null;
        }

        public async Task<VisaHeader> GetVisaHeader(int VisaHeaderId)
        {
            return await _context.VisaHeaders.Include(x => x.VisaItems)
                .Where(x => x.Id == VisaHeaderId).FirstOrDefaultAsync();
        }


        public async Task<PagedList<VisaBriefDto>> GetVisaPagedList(VisaParams vParams)
        {
            var query = (from hdr in _context.VisaHeaders
                join cust in _context.Customers on hdr.CustomerId equals cust.Id 
                join vItem in _context.VisaItems on hdr.Id equals vItem.VisaHeaderId
                //join vTrans in _context.VisaTransactions on vItem.Id equals vTrans.VisaItemId     //**TODO** RESOLVE this with GroupBy
                select new VisaBriefDto {
                    Id = hdr.Id, CustomerId = hdr.CustomerId, CustomerKnownAs = cust.KnownAs,
                    VisaNo = hdr.VisaNo, VisaDateG = hdr.VisaDateG, VisaCategoryEnglish = vItem.VisaCategoryEnglish,
                    VisaSponsorName = hdr.VisaSponsorName, VisaQuantity = vItem.VisaQuantity,
                    VisaBalance = 0, VisaConsulate = vItem.VisaConsulate, VisaCanceled = vItem.VisaCanceled,
                    VisaItemId = vItem.Id
                }
            ).AsQueryable();

            if(vParams.Id != 0) {
                query = query.Where(x => x.Id == vParams.Id);
            } else {
                if(!string.IsNullOrEmpty(vParams.VisaNo)) query = query.Where(x => x.VisaNo.ToLower() == vParams.VisaNo.ToLower());
                if(!string.IsNullOrEmpty(vParams.CustomerName)) query = query.Where(x => x.CustomerKnownAs.ToLower() == vParams.CustomerName.ToLower());
                if(vParams.CustomerId != 0) query = query.Where(x => x.CustomerId == vParams.CustomerId);
                if(!string.IsNullOrEmpty(vParams.VisaConsulate)) 
                    query = query.Where(x => x.VisaConsulate.ToLower() == vParams.VisaConsulate.ToLower() );
                if(vParams.DepItemId != 0) query = query.Where(x => x.DepItemId == vParams.DepItemId);
                //if(vParams.VisaApproved) query = query.Where(x => x.VisaTransactions.Select(x => x.VisaApproved)).FirstOrDefault() == vParams.VisaApproved;
                if(vParams.VisaCanceled) query = query.Where(x => x.VisaCanceled == vParams.VisaCanceled);
                if(!string.IsNullOrEmpty(vParams.VisaCategory)) query = query.Where(x => x.VisaCategoryEnglish.ToLower() == vParams.VisaCategory.ToLower());

            }

            var paged = await PagedList<VisaBriefDto>.CreateAsync(query.AsNoTracking(),
                //.ProjectTo<VisaBriefDto>(_mapper.ConfigurationProvider)
                vParams.PageNumber, vParams.PageSize);

            var Ids = paged.Select(x => x.VisaItemId).ToList();

            var balances = await  _context.VisaAssignments
                .Where(x => Ids.Contains(x.VisaItemId))
                .GroupBy(x => x.VisaItemId)
                .Select(g => new {
                    VisaItemId = g.Key, TotalAssigned=g.Sum(x => x.VisaQntyAssigned)
                }).ToListAsync();

            var consumed = await  _context.VisaTransactions
                .Where(x => Ids.Contains(x.VisaItemId))
                .GroupBy(x => new {VisaItemId=x.VisaItemId, VisaNo=x.VisaNo})
                .Select(g => new {
                    VisaItemId = g.Key.VisaItemId, VisaNo = g.Key.VisaNo, TotalConsumed=g.Count()
                }).ToListAsync();

            
            foreach(var pg in paged) {
                var bal = balances == null || balances.Count == 0 ? 0 : 
                    balances.Where(x => x.VisaItemId == pg.VisaItemId).Select(x => x.TotalAssigned).FirstOrDefault();
                    var vBal = pg.VisaQuantity - bal;
                    vBal = vBal < 0 ? 0 : vBal;
                    pg.VisaBalance = vBal;

                var cons = 0;
                var visano="";
                if(consumed==null || consumed.Count == 0 ) {
                    cons = 0;
                }else {
                    cons = consumed.Where(x => x.VisaItemId==pg.VisaItemId).Select(x => x.TotalConsumed).FirstOrDefault();
                    visano = consumed.Where(x => x.VisaItemId==pg.VisaItemId).Select(x => x.VisaNo).FirstOrDefault();
                }

                    pg.VisaConsumed = cons;
                    pg.VisaNo = visano;
            }
            return paged;
        }

        public async Task<VisaHeader> InsertVisa(VisaHeader visaHeader)
        {
            var customer = await _context.Customers.FindAsync(visaHeader.CustomerId);
            if(customer == null) return null;
            var len = customer.CustomerName.Length;
       
            visaHeader.CustomerName = len > 48 ? customer.CustomerName[..48] : customer.CustomerName;

            if(visaHeader.VisaItems.Count== 0) return null;

            _context.VisaHeaders.Add(visaHeader);

            return await _context.SaveChangesAsync() > 0 ? visaHeader : null;
        }

        public async Task<VisaItem> InsertVisaItem(VisaItem visaitem)
        {
            var visa = await _context.VisaHeaders.FindAsync(visaitem.VisaHeaderId);

            if(visa == null) return null;
            if(string.IsNullOrEmpty(visaitem.VisaCategoryEnglish)) return null;
            if(visaitem.VisaQuantity == 0) return null;

            _context.VisaItems.Add(visaitem);

            return await _context.SaveChangesAsync() > 0 ? visaitem : null;            
        }

        public async Task<VisaTransaction> InsertVisaTransaction(VisaTransaction vTransaction)
        {
            var depitem = await _context.DepItems.FindAsync(vTransaction.DepItemId);
            if(depitem == null) return null;

            var cand = await (from item in _context.DepItems where item.Id == vTransaction.DepItemId
                join dep in _context.Deps on item.DepId equals dep.Id
                join cvref in _context.CVRefs on dep.CvRefId equals cvref.Id
                join cv in _context.Candidates on cvref.CandidateId equals cv.Id 
                select new {CandidateName = cv.FullName, ApplicationNo=cv.ApplicationNo})
                .FirstOrDefaultAsync();
            
            vTransaction.ApplicationNo = cand.ApplicationNo;
            vTransaction.CandidateName = cand.CandidateName;

            _context.VisaTransactions.Add(vTransaction);

            return await _context.SaveChangesAsync() > 0 ? vTransaction : null;
        }

        public async Task<ICollection<OrderItemForVisaAssignmentDto>> GetOpenOrderItemsForCustomer(int customerId)
        {
            var dto = await (from order in _context.Orders where order.CustomerId == customerId
                join cust in _context.Customers on order.CustomerId equals cust.Id
                join item in _context.OrderItems on order.Id equals item.OrderId
                join cat in _context.Professions on item.ProfessionId equals cat.Id 
                //join assignment in  _context.VisaAssignments on item.Id equals assignment.OrderItemId into vitems
                //from vitem in vitems.DefaultIfEmpty()
                    //group vitem by new {vitem.OrderItemId, cust.KnownAs, cat.ProfessionName, item.Quantity} into grp 
                select new OrderItemForVisaAssignmentDto {
                    OrderItemId = item.Id,
                    //Assigned = grp.Sum(x => x.VisaQntyAssigned),
                    CustomerInBrief = cust.KnownAs,
                    Category = cat.ProfessionName,
                    Quantity = item.Quantity
                }).ToListAsync();
            var orderitemIds = dto.Select(x => x.OrderItemId).ToList();
            var assignments = await _context.VisaAssignments.Where(x => orderitemIds.Contains(x.OrderItemId)).ToListAsync();

            foreach(var item in dto) {
                var assigned = assignments.Where(x => x.OrderItemId == item.OrderItemId)
                    .Select(x => x.VisaQntyAssigned).FirstOrDefault() ;
                item.Unassigned = item.Quantity - assigned;
            }
            return dto;

        }

        public async Task<ICollection<Entities.Admin.VisaAssignment>> InsertVisaAssignments(ICollection<Entities.Admin.VisaAssignment> visaAssignments)
        {
            foreach(var assignment in visaAssignments) {
                _context.VisaAssignments.Add(assignment);
            }

            return await _context.SaveChangesAsync() == visaAssignments.Count ? visaAssignments : null;
        }

        public async Task<PagedList<VisaTransaction>> GetVisaTransactionPagedList(VisaParams vParams)
        {
            var query = (from vTrans in _context.VisaTransactions
                select new VisaTransaction {
                    Id = vTrans.Id, ApplicationNo = vTrans.ApplicationNo, 
                    CandidateName = vTrans.CandidateName, VisaItemId = vTrans.VisaItemId, 
                    VisaNo = vTrans.VisaNo, VisaCategory = vTrans.VisaCategory, 
                    DepItemId = vTrans.DepItemId, VisaAppSubmitted = vTrans.VisaAppSubmitted,
                    VisaApproved = vTrans.VisaApproved, VisaDateG = vTrans.VisaDateG
                }).AsQueryable();
                
            if(vParams.Id != 0) {
                query = query.Where(x => x.Id == vParams.Id);
            } else if(vParams.VisaItemId != 0) {
                query = query.Where(x => x.VisaItemId == vParams.VisaItemId);
            } else if(!string.IsNullOrEmpty(vParams.VisaNo)) {
                query = query.Where(x => x.VisaNo == vParams.VisaNo);
            } else {
                if(vParams.CustomerId != 0) query = query.Where(x => x.CustomerId == vParams.CustomerId);
                if(vParams.VisaExpiryG.Year > 2000) 
                    query = query.Where(x => x.VisaDateG.Date == new DateTime(vParams.VisaExpiryG.Year, vParams.VisaExpiryG.Month, vParams.VisaExpiryG.Day));
                if(!string.IsNullOrEmpty(vParams.VisaCategory)) query = query.Where(x => x.VisaCategory.ToLower().Contains(vParams.VisaCategory.ToLower()));
            }
            
            var paged = await PagedList<VisaTransaction>.CreateAsync(query.AsNoTracking(),
                //.ProjectTo<VisaBriefDto>(_mapper.ConfigurationProvider)
                vParams.PageNumber, vParams.PageSize);
            
            return paged;
        }

    }
}