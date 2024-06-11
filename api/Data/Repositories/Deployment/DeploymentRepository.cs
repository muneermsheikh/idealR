using System.Data.Common;
using System.Linq;
using api.DTOs.Process;
using api.Entities.Deployments;
using api.Extensions;
using api.Helpers;
using api.Interfaces.Deployments;
using api.Params.Deployments;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace api.Data.Repositories.Deployment
{
    public class DeploymentRepository : IDeploymentRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public DeploymentRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        private async Task<bool> UpdateDepStatus(int depId, bool save) {
                
                var dep = await _context.Deps.Include(x => x.DepItems).Where(x => x.Id == depId).FirstOrDefaultAsync();
                var lastSeq = dep.DepItems.OrderByDescending(x => x.Sequence).Select(x => x.Sequence).FirstOrDefault();

                dep.CurrentStatus = await _context.GetNextDepStatusName(lastSeq);
                _context.Entry(dep).State = EntityState.Modified;

                if(save) await _context.SaveChangesAsync();

                return true;
        }
        public async Task<string> AddDeploymentTransactions(ICollection<DepItemToAddDto> dto, string Username)
        {
            string strErr="";
            int itemsWithErr=0, itemsSucceeded=0;

            var items = new List<DepItem>();
            foreach(var item in dto) {
                var lastDepItem = await _context.DepItems.Where(x => x.DepId==item.DepId)
                    .OrderByDescending(x => x.Sequence).FirstOrDefaultAsync();
                    
                var SequenceShdBe = await _context.DeployStatuses
                    .Where(x => x.Sequence == lastDepItem.Sequence && !x.isOptional).Select(x => x.NextSequence).FirstOrDefaultAsync();
                if(SequenceShdBe != item.Sequence) {
                    strErr += ", item with sequence " + item.Sequence + " - Sequence Expected is: " + SequenceShdBe;
                    itemsWithErr +=1;
                } else if (DateOnly.FromDateTime(lastDepItem.TransactionDate) >= DateOnly.FromDateTime(item.TransactionDate)) {
                    strErr += ", New Transaction item date " + item.TransactionDate 
                        + " cannot be dated earlier than last Transaction Date " + lastDepItem.TransactionDate;
                } else {
                    var depStatus = await _context.GetNextDepStatus(item.Sequence);
                    var depitem = new DepItem{
                        DepId = item.DepId, Sequence = item.Sequence, NextSequence = depStatus.NextSequence,
                        TransactionDate = item.TransactionDate, 
                        NextSequenceDate = item.TransactionDate.AddDays(depStatus.WorkingDaysReqdForNextStage)
                    };
                    items.Add(depitem);
                }
            }

            if(!string.IsNullOrEmpty(strErr)) return strErr;
            
            foreach(var d in items) {
                _context.Entry(d).State = EntityState.Added;

                //update Dep.CurrentStatus;
                await UpdateDepStatus(d.DepId, false);       //dep.CurrentStatus Entry.Status is modified, but not saved
            }

            try{
                itemsSucceeded = await _context.SaveChangesAsync();
            } catch(DbException ex) {
                if(ex.Message.Contains("IX_DepItem")) {
                    strErr = "Unique Index violation - CVRefId + Sequence";
                } else {
                    strErr = ex.Message;
                }
                
            } catch(Exception ex) {
                strErr = ex.Message;
            }

            return !string.IsNullOrEmpty(strErr) 
                ? itemsWithErr + " number of items failed, " + itemsSucceeded + " items succeeded"
                : "";
        }

        public async Task<string> DeleteDep(int depId)
        {
            string strErr="";
            
            var obj = await _context.Deps.FindAsync(depId);
            if (obj == null) return "The deployment object does not exist";
            
            _context.Deps.Remove(obj); 
            _context.Entry(obj).State = EntityState.Deleted;
            
             try 
            {
                await _context.SaveChangesAsync();
            } catch (DbException ex) {
                strErr = "Database Error: " + ex.Message;
            } catch (Exception ex) {
                strErr = ex.Message;
            }

            return strErr;
        }

        public async Task<string> DeleteDepItem(int depItemId)
        {
            string strErr="";
            int ct=0;

            var obj = await _context.DepItems.FindAsync(depItemId);
            if (obj == null) return "The Deployment Item does not exist";

            var items = await _context.DepItems.Where(x => x.Sequence == obj.Sequence || x.Sequence > obj.Sequence).ToListAsync();

            foreach(var item in items) {
                _context.DepItems.Remove(item);
                _context.Entry(item).State = EntityState.Deleted;
                await UpdateDepStatus(item.DepId, false);
            }
   
             try 
            {
                ct =await _context.SaveChangesAsync();
            } catch (DbException ex) {
                strErr = "Database Error: " + ex.Message;
            } catch (Exception ex) {
                strErr = ex.Message;
            }

            return strErr;
        }
        
        public async Task<string> EditDepItem(DepItem depItem)
        {
            string strErr="";
            
            var existingItem = await _context.DepItems.FindAsync(depItem.Id);
            if(existingItem == null) return "The deployment Item to exit is not on record.";

            _context.Entry(existingItem).CurrentValues.SetValues(depItem);

            await UpdateDepStatus(existingItem.DepId, false);

             try 
            {
                await _context.SaveChangesAsync();
            } catch (DbException ex) {
                strErr = "Database Error: " + ex.Message;
            } catch (Exception ex) {
                strErr = ex.Message;
            }

            return strErr;

        }

        public async Task<string> EditDeployment(Dep model)
        {
            string strErr="";
            
            var existing = await _context.Deps.Include(x => x.DepItems)
                .Where(x => x.Id == model.Id)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (existing == null) return "Deployment Object does not exist";

            _context.Entry(existing).CurrentValues.SetValues(model);

            foreach (var existingItem in existing.DepItems.ToList())
            {
                if(!model.DepItems.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                {
                    _context.DepItems.Remove(existingItem);
                    _context.Entry(existingItem).State = EntityState.Deleted; 
                }
            }

             foreach(var newItem in model.DepItems)
            {
                var existingItem = existing.DepItems
                    .Where(c => c.Id == newItem.Id && c.Id != default(int)).SingleOrDefault();
                
                if(existingItem != null)    //update existing item
                {
                    _context.Entry(existingItem).CurrentValues.SetValues(newItem);
                    _context.Entry(existingItem).State = EntityState.Modified;
                } else {    //insert new record
                    var itemToInsert = new DepItem
                    {
                        DepId = existing.Id,
                        TransactionDate = newItem.TransactionDate,
                        NextSequence = newItem.NextSequence,
                        Sequence = newItem.Sequence,
                        NextSequenceDate = newItem.NextSequenceDate
                    };

                    existing.DepItems.Add(itemToInsert);
                    _context.Entry(itemToInsert).State = EntityState.Added;
                }

                _context.Entry(existing).State = EntityState.Modified;
            }
            
            await UpdateDepStatus(model.Id, false);

            try 
            {
                await _context.SaveChangesAsync();
            } catch (DbException ex) {
                strErr = "Database Error: " + ex.Message;
            } catch (Exception ex) {
                strErr = ex.Message;
            }

            return strErr;
        }

        public async Task<Dep> GetDeploymentByCVRefId(int cvrefid)
        {
            return await _context.Deps.Include(x => x.DepItems).Where(x => x.CVRefId == cvrefid).FirstOrDefaultAsync();
        }


        public async Task<PagedList<DeploymentPendingDto>> GetDeployments(DeployParams depParams)
        {
            
            var query = (from dep in _context.Deps where dep.CurrentStatus != "Concluded"
                join cvref in _context.CVRefs on dep.CVRefId equals cvref.Id
                join cv in _context.Candidates on cvref.CandidateId equals cv.Id
                join item in _context.OrderItems on cvref.OrderItemId equals item.Id
                join order in _context.Orders on item.OrderId equals order.Id
                
                select new DeploymentPendingTempDto {
                    DepId = dep.Id,
                    ApplicationNo = cv.ApplicationNo,
                    CandidateName = cv.FullName,
                    CategoryName = item.Profession.ProfessionName,
                    CurrentSeqName = dep.CurrentStatus,
                    CustomerName = order.Customer.CustomerName,
                    CVRefId = cvref.Id,
                    SelectedOn = cvref.SelectionStatusDate,
                    CustomerId = order.CustomerId,
                    OrderNo = order.OrderNo,
                    OrderDate = order.OrderDate,
                    OrderItemId = item.Id,
                    NextStageDate = dep.DepItems.OrderByDescending(x => x.TransactionDate)
                        .Select(x => x.NextSequenceDate).FirstOrDefault()
                }).AsQueryable();

            if(depParams.CVRefIds?.Count > 0) query = query.Where(x => depParams.CVRefIds.Contains(x.CVRefId));

            if(depParams.SelectedOn.Year > 2000) query = query.Where(x => DateOnly.FromDateTime(x.SelectedOn) == DateOnly.FromDateTime(depParams.SelectedOn));
            
            if(depParams.OrderItemIds != null && depParams.OrderItemIds.Count > 0) 
                query = query.Where(x => depParams.OrderItemIds.Contains(x.OrderItemId));
            
            if(depParams.CustomerId > 0) query = query.Where(x => x.CustomerId == depParams.CustomerId);
            var qry = await query.ToListAsync();
            var paged = await PagedList<DeploymentPendingDto>.CreateAsync(query.AsNoTracking()
            .ProjectTo<DeploymentPendingDto>(_mapper.ConfigurationProvider),
            depParams.PageNumber, depParams.PageSize);
        
            return paged;
        }

        public async Task<ICollection<DeployStatus>> GetDeploymentStatusData()
        {
            return await _context.DeployStatuses.OrderBy(x => x.Sequence).ToListAsync();
        }

        /* public async Task<PagedList<DeploymentRecordsDto>> GetDeploymentHistory()
        {
           group t by new { t.MonthName, t.FeeParticularName } into grp
            // Assuming you have a collection of records that you want to transform into a crosstab format
            var records =  (from dep in _context.Deps 
                join depitem in _context.DepItems on dep.Id equals depitem.DepId
                join depStatus in _context.DeployStatuses on depitem.Sequence equals depStatus.Sequence
                join cvref in _context.CVRefs on dep.CVRefId equals cvref.Id
                join cv in _context.Candidates on cvref.CandidateId equals cv.Id
                join item in _context.OrderItems on cvref.OrderItemId equals item.Id
                join order in _context.Orders on item.OrderId equals order.Id
                group depStatus by depStatus.StatusName into grp
                select new DeploymentRecordsDto {
                    DepId = grp.dep.Id,
                    CVRefId = grp.cvref.Id,
                    ApplicationNo = cv.ApplicationNo,
                    CandidateName = cv.FullName,
                    CategoryName = item.Profession.ProfessionName,
                    Status = depStatus.StatusName,
                    TransactionDate  = depitem.TransactionDate,
                    CustomerName = order.Customer.CustomerName,
                    OrderNo = order.OrderNo,
                }).ToList();


            // Group the records by the first key (e.g., Month)
            var groupedRecords = records
                .GroupBy(record => record.CVRefId)
                .Select(group => new
                {
                    FirstKey = group.Key,
                    // Create a dictionary for the crosstab columns
                    CrosstabColumns = group
                        .GroupBy(record => record.CVRefId)
                        .ToDictionary(g => g.Key,  g => g.First(x => x.TransactionDate.Value))
                })
                .ToList();

            // Now you have a list of anonymous objects, each with a FirstKey and a dictionary of crosstab columns

        }
        */

    }
}