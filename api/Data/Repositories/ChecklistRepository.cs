using System.Data.Common;
using System.Runtime.CompilerServices;
using api.DTOs.HR;
using api.Entities.HR;
using api.Entities.Master;
using api.Extensions;
using api.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Repositories
{
    public class ChecklistRepository : IChecklistRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly DateTime _today;
        public ChecklistRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<ChecklistDtoObj> GetOrGenerateChecklist(int candidateId, int orderItemId, string Username)
        {
            var checkobj = new ChecklistDtoObj();
            var checklist = new ChecklistHR();

            checklist = await CandidateAlreadyChecklisted(candidateId, orderItemId);

            if(checklist == null) {

                var obj = await ComposeChecklistHR(candidateId, orderItemId, Username);
                if(!string.IsNullOrEmpty(obj.ErrorString)) {
                    checkobj.ErrorString = obj.ErrorString;
                    return checkobj;
                } else {
                    checklist = obj.ChecklistHR;
                }
            }

           var dto = _mapper.Map<ChecklistHRDto>(checklist);

            if(dto.ApplicationNo == 0) dto.ApplicationNo = 
                await _context.GetApplicationNoFromCandidateId(dto.CandidateId);
            if(string.IsNullOrEmpty(dto.CandidateName)) dto.CandidateName = 
                await _context.GetCandidateNameFromCandidateId(dto.CandidateId);
            if(string.IsNullOrEmpty(dto.CategoryRef)) dto.CategoryRef = 
                await _context.GetOrderItemDescriptionFromOrderItemId(dto.OrderItemId);
            if(string.IsNullOrEmpty(dto.HrExecUsername)) 
                dto.HrExecUsername = await _context.GetHRExecUsernameFromOrderItemId(dto.OrderItemId);

            checkobj.checklistDto = dto;

            return checkobj;
        }

        public async Task<ChecklistHR> GetChecklist(int candidateid, int orderitemid)
        {
            var obj = await _context.ChecklistHRs.Where(x => x.CandidateId == candidateid &&
                x.OrderItemId == orderitemid).FirstOrDefaultAsync();
            
            return obj;
        }

        private async Task<ChecklistHR> CandidateAlreadyChecklisted(int candidateId, int orderItemId)
        {
            var chklst = await _context.ChecklistHRs.Include(x => x.ChecklistHRItems)
                .Where(x => x.CandidateId == candidateId && x.OrderItemId == orderItemId)
                .FirstOrDefaultAsync();
            
            //if (chklst == null) 
                //return "Checklist on the candidate for the same requirement has been done on " + chklst.CheckedOn;

            return chklst;

        }
        
        private static string ChecklistErrors(ChecklistHR checklistHR) {

            var errorStrings = "";
            foreach(var item in checklistHR.ChecklistHRItems)
            {
                if(item.MandatoryTrue && !item.Accepts) errorStrings += ", " + item.Parameter + " not accepted";
            }
            
            if (checklistHR.Charges ==0 && !checklistHR.ExceptionApproved ) 
                errorStrings += "," +  "if Charges are FOC, the EXCEPTION APPROVED must be set to Y, to distinguish " +
                "it from mistakenly input value of zero for Service Charges";
            
            if (checklistHR.Charges != checklistHR.ChargesAgreed ) 
                errorStrings += "," +  "Charges and Charges Agreed values do not match";

            if (checklistHR.Charges >0 && checklistHR.ChargesAgreed != checklistHR.Charges && !checklistHR.ExceptionApproved ) 
                errorStrings += "," +  "Charges of " + checklistHR.Charges + " not agreed to by the candidate, and Exceptions not approved";
            if(!string.IsNullOrEmpty(errorStrings)) errorStrings = errorStrings[2..];
            return errorStrings;
        }

        private async Task<string> verifyChecklist(ChecklistHR checklisthr)
        {
            var strErr="";
            var candidate = await _context.Candidates.FindAsync(checklisthr.CandidateId);
            if(candidate != null) strErr = "Invalid Candidate Id";
            var item = await _context.OrderItems.FindAsync(checklisthr.OrderItemId);
            if(item != null) strErr +=" Invalid Order Item Id code";

            return strErr;
        }

        private async Task<ChecklistObj> ComposeChecklistHR(int candidateId, int orderItemId, string Username)
        {
            var checkobj = new ChecklistObj();

            if(await _context.Candidates.FindAsync(candidateId) == null) {
                checkobj.ErrorString = "Candidate does not exist";
                return checkobj;
            }

            if(await _context.OrderItems.FindAsync(orderItemId) == null)  {
                checkobj.ErrorString = "Order Item does not exist";
                return checkobj;
            }
            
            var itemList = new List<ChecklistHRItem>();
            //populate the checklistHRItem
            var data = await _context.ChecklistHRDatas.OrderBy(x => x.SrNo).ToListAsync();
            var charges  = await _context.GetServiceChargesFromOrderItemId(orderItemId);
            

            foreach(var item in data) {
                var newItem = new ChecklistHRItem{
                    Parameter=item.Parameter,SrNo=item.SrNo, MandatoryTrue=item.IsMandatory
                };
                itemList.Add(newItem);
            }

            var hrexecusername = await _context.ContractReviewItems.Where(x => x.OrderItemId == orderItemId)
                .Select(x => x.HRExecUsername).FirstOrDefaultAsync();

            var hrTask = new ChecklistHR{
                CandidateId=candidateId, OrderItemId= orderItemId, 
                UserName = Username, CheckedOn = _today, 
                Charges = charges,
                ChecklistHRItems = itemList, HrExecUsername = hrexecusername };
            
            checkobj.ChecklistHR = hrTask;

            return checkobj;

        }

        public async Task<ChecklistObj> SaveNewChecklist (ChecklistHR checklisthr, string Username)
        {
            var checkobj = new ChecklistObj();

            var errStr = await verifyChecklist(checklisthr);
            if(!string.IsNullOrEmpty(errStr)) {
                checkobj.ErrorString = errStr;
                return checkobj;
            }

            _context.Entry(checklisthr).State = EntityState.Added;

            try {
                await _context.SaveChangesAsync();
            } catch (DbException ex) {
                checkobj.ErrorString = ex.Message;
                return checkobj;
            } catch (Exception ex) {
                checkobj.ErrorString = ex.Message;
                return checkobj;
            }

            checkobj.ChecklistHR  = checklisthr;

            return checkobj;
            
        }

        public async Task<string> EditChecklistHR(ChecklistHR model, string Username)
        {
             var errorList = ChecklistErrors(model);
            if(!string.IsNullOrEmpty(errorList)) return errorList;

            var existing =  await GetChecklistHRIfEditable(model, Username);     //returns ChecklistHR
            _context.Entry(existing).CurrentValues.SetValues(model);   //saves only the parent, not children

            //the children 
            //Delete children that exist in existing record, but not in the new model order
            foreach (var existingItem in existing.ChecklistHRItems.ToList())
            {
                if (!model.ChecklistHRItems.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                {
                    _context.ChecklistHRItems.Remove(existingItem);
                    _context.Entry(existingItem).State = EntityState.Deleted;
                }
            }

            //children that are not deleted, are either updated or new ones to be added
            foreach (var modelItem in model.ChecklistHRItems)
            {
                var existingItem = existing.ChecklistHRItems.Where(c => c.Id == modelItem.Id && c.Id != default(int)).SingleOrDefault();
                if (existingItem != null)       // Update child
                {
                    _context.Entry(existingItem).CurrentValues.SetValues(modelItem);
                    _context.Entry(existingItem).State = EntityState.Modified;
                }
                else            //insert children as new record
                {
                    var newItem = new ChecklistHRItem{Id=model.Id, SrNo= modelItem.SrNo, Parameter=modelItem.Parameter, 
                        Response=modelItem.Response, Exceptions= modelItem.Exceptions};
                    existing.ChecklistHRItems.Add(newItem);
                    _context.Entry(newItem).State = EntityState.Added;
                }
            }
            
            var test1 = !model.ChecklistHRItems.Any(c => !c.Accepts);
            var test2 = model.ChecklistHRItems.Any(c => c.MandatoryTrue && model.ExceptionApproved);

            existing.ChecklistedOk = !model.ChecklistHRItems.Any(c => !c.Accepts)
                || model.ChecklistHRItems.Any(c => c.MandatoryTrue && model.ExceptionApproved);
            
            if(existing.ExceptionApproved && string.IsNullOrEmpty(existing.ExceptionApprovedBy)) {
                existing.ExceptionApprovedBy = Username;
                existing.ExceptionApprovedOn = _today;
            }
            _context.Entry(existing).State = EntityState.Modified;
            
            try{
               await _context.SaveChangesAsync() ;
            } catch (Exception ex) {
                switch(ex.Message) {
                    case "FOREIGN KEY constraint failed":
                        errorList = "Index constraint failed - pl check if the candidate has already been checklisted for the same order item";
                        break;
                    default:
                        errorList = ex.Message;
                        break;
                }

               return errorList;
            }
            
            return "";
        }

        public async Task<bool> DeleteChecklistHR(int id)
        {
            var obj = await _context.ChecklistHRs
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (obj==null) return false;
            
            _context.ChecklistHRs.Remove(obj);
            _context.Entry(obj).State = EntityState.Deleted;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<ICollection<ChecklistHR>> GetChecklistHROfCandidate(int candidateid)
        {
            var checklist = await _context.ChecklistHRs.Include(x => x.ChecklistHRItems)
                .Where(x => x.CandidateId == candidateid).ToListAsync();
           
            return checklist;
        }

        public async Task<ChecklistHRDto> GetChecklistHRFromCandidateIdAndOrderDetailId(int candidateId, 
            int orderItemId, string username)
        {
            var dto = await GetOrGenerateChecklist(candidateId, orderItemId, username);
            
            var lst = await(from checklist in _context.ChecklistHRs 
                    where checklist.CandidateId == candidateId && checklist.OrderItemId == orderItemId
                join orderitem in _context.OrderItems on checklist.OrderItemId equals orderitem.Id
                join candidate in _context.Candidates on checklist.CandidateId equals candidate.Id
                join order in _context.Orders on orderitem.OrderId equals order.Id
                join rvwitem in _context.ContractReviewItems on orderitem.Id equals rvwitem.OrderItemId
                select new ChecklistHRDto {
                    Id = checklist.Id, ApplicationNo = candidate.ApplicationNo, OrderItemId = orderItemId,
                    CandidateName = candidate.FullName, CandidateId = candidate.Id,
                    //OrderRef =  order.OrderNo + "-" + orderitem.SrNo + "-" + orderitem.Profession.ProfessionName,
                    UserName = checklist.UserName, 
                    CheckedOn = checklist.CheckedOn, 
                    HrExecUsername = rvwitem.HRExecUsername,
                    //UserComments = checklist.UserComments,
                    ChecklistHRItems = checklist.ChecklistHRItems,
                    Charges = rvwitem.Charges,
                    //ChargesAgreed = checklist.ChargesAgreed,
                    ExceptionApproved = checklist.ExceptionApproved,
                    ExceptionApprovedBy = checklist.ExceptionApprovedBy,
                    ExceptionApprovedOn = checklist.ExceptionApprovedOn,
                    ChecklistedOk = checklist.ChecklistedOk

                })
                .FirstOrDefaultAsync();

            return lst;
        }
        
        private async Task<ChecklistHR> GetChecklistHRIfEditable(ChecklistHR model, string Username)
        {
            //if cv already forwarded to Sup, then changes not allowed
            var existing = await _context.ChecklistHRs.Where(
                    p => p.CandidateId==model.CandidateId && p.OrderItemId==model.OrderItemId)
                .Include(p => p.ChecklistHRItems)
                .AsNoTracking()
                .SingleOrDefaultAsync() ?? throw new Exception("Checklist record you want edited does not exist");

            return existing;
        }

      //master data
        public async Task<ChecklistHRData> AddChecklistHRData(string checklistParameter)
        {
            var srno = await _context.ChecklistHRDatas.MaxAsync(x => x.SrNo) + 1;
            var checklist = new ChecklistHRData(srno, checklistParameter);
            _context.ChecklistHRDatas.Add(checklist);

            if (await _context.SaveChangesAsync() > 0) return checklist;
            return null;
        }

        public async Task<bool> DeleteChecklistHRDataAsync(ChecklistHRData checklistHRData)
        {
            _context.ChecklistHRDatas.Remove(checklistHRData);
        
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> EditChecklistHRDataAsync(ChecklistHRData checklistHRData)
        {
            _context.Entry(checklistHRData).State = EntityState.Modified;
            
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<ICollection<ChecklistHRData>> GetChecklistHRDataListAsync()
        {
            return await _context.ChecklistHRDatas.OrderBy(x => x.SrNo).ToListAsync();
        }

        
    }
}