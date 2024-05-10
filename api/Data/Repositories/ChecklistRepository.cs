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
        public ChecklistRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<ChecklistObj> AddNewChecklistHR(int candidateId, int orderItemId, string Username)
        {
            var checkobj = new ChecklistObj();

            //check if the candidate has aleady been checklisted for the order item
            var checkedOn = await _context.ChecklistHRs.Where(x => x.CandidateId == candidateId && x.OrderItemId == orderItemId)
                .Select(x => x.CheckedOn.Date).FirstOrDefaultAsync();
            if (checkedOn.Year > 2000) {
                checkobj.ErrorString = "Checklist on the candidate for the same requirement has been done on " + checkedOn;
                return checkobj;
            }
            var checklistobj = await AddChecklistHR(candidateId, orderItemId,  Username);
            if(!string.IsNullOrEmpty(checklistobj.ErrorString)) {
                checkobj.ErrorString=checklistobj.ErrorString;
            } else {
                checkobj.ChecklistHR = checklistobj.ChecklistHR;
            }
            return checkobj;
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


        private async Task<ChecklistObj> AddChecklistHR(int candidateid, int orderitemid, string Username)
        {
            //verify candidteid and orderitemid exist
            var checklistObj = new ChecklistObj();
            if(await _context.Candidates.FindAsync(candidateid) == null) {
                checklistObj.ErrorString = "Candidate does not exist";
                return checklistObj;
            }

            if(await _context.OrderItems.FindAsync(orderitemid) == null)  {
                checklistObj.ErrorString = "Order Item does not exist";
                return checklistObj;
            }
            
            var itemList = new List<ChecklistHRItem>();
            //populate the checklistHRItem
            var data = await _context.ChecklistHRDatas.OrderBy(x => x.SrNo).ToListAsync();
            var charges  = await _context.GetServiceChargesFromOrderItemId(orderitemid);

            foreach (var item in data)
            {
                if(item.Parameter.ToLower() == "willing to pay service charges") item.Parameter += charges == 0 ? "" : " " + charges;
                itemList.Add(new ChecklistHRItem{SrNo = item.SrNo, Parameter = item.Parameter, MandatoryTrue=item.IsMandatory});
            }
            var hrTask = new ChecklistHR{
                CandidateId=candidateid, OrderItemId= orderitemid, 
                UserName = Username, CheckedOn = System.DateTime.UtcNow, 
                Charges = await _context.GetServiceChargesFromOrderItemId(orderitemid),
                ChecklistHRItems = itemList,
                Candidate = null};

            _context.ChecklistHRs.Add(hrTask);
            var errorString="";
            try {
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                checklistObj.ErrorString = "";
                errorString = ex.Message switch
                {
                    "FOREIGN KEY constraint failed" => checklistObj.ErrorString ="Index constraint failed - pl check if the candidate has already been checklisted for the same order item",
                
                    _ => checklistObj.ErrorString= ex.Message,
                };
                return checklistObj;
            }

            if(hrTask.Candidate != null) hrTask.Candidate=null;
            if(hrTask.OrderItem != null) hrTask.OrderItem=null;

            if(!string.IsNullOrEmpty(errorString)) {
                checklistObj.ErrorString=errorString;
            } else {
                checklistObj.ChecklistHR=_mapper.Map<ChecklistHR>(hrTask);
            }
            return checklistObj;
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
                existing.ExceptionApprovedOn = DateTime.UtcNow;
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
            int orderItemId)
        {
            var lst = await(from checklist in _context.ChecklistHRs 
                    where checklist.CandidateId == candidateId && checklist.OrderItemId == orderItemId
                join orderitem in _context.OrderItems on checklist.OrderItemId equals orderitem.Id
                join candidate in _context.Candidates on checklist.CandidateId equals candidate.Id
                join order in _context.Orders on orderitem.OrderId equals order.Id
                join rvwitem in _context.ContractReviewItems on orderitem.Id equals rvwitem.OrderItemId
                select new ChecklistHRDto {
                    Id = checklist.Id, ApplicationNo = candidate.ApplicationNo, OrderItemId = orderItemId,
                    CandidateName = candidate.FullName, CandidateId = candidate.Id,
                    OrderRef =  order.OrderNo + "-" + orderitem.SrNo + "-" + 
                        orderitem.Profession.ProfessionName,
                    UserName = checklist.UserName, 
                    CheckedOn = checklist.CheckedOn, 
                    UserComments = checklist.UserComments,
                    ChecklistHRItems = checklist.ChecklistHRItems,
                    Charges = rvwitem.Charges,
                    ChargesAgreed = checklist.ChargesAgreed,
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

        public Task<ICollection<ChecklistHRData>> GetChecklistHRDataListAsync()
        {
            throw new System.NotImplementedException();
        }

    }
}