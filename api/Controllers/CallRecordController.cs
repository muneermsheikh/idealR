using api.DTOs.Admin;
using api.Entities.Admin;
using api.Errors;
using api.Extensions;
using api.Helpers;
using api.Interfaces.Admin;
using api.Params.Admin;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    public class CallRecordController : BaseApiController
    {
        private readonly ICallRecordRepository _histRepo;
        public CallRecordController(ICallRecordRepository histRepo)
        {
            _histRepo = histRepo;
        }
   
        [HttpGet("dto")]
        public async Task<ActionResult<CallRecordDto>> GetCallRecordDto(CallRecordParams histParams)
        {
            var dto = await _histRepo.GetCallRecordDtoByParams(histParams);

            if(dto==null) return NotFound(new ApiException(400, "Not Found", "Failed to retrieve User History from given parameters"));

            return Ok(dto);
        }
        
        [HttpGet("pagedlist")]
        public async Task<ActionResult<PagedList<CallRecordBriefDto>>> GetPaginatedCallRecord([FromQuery] CallRecordParams histParams )
        {
            var pagedList = await _histRepo.GetCallRecordPaginated(histParams, User.GetUsername());
            if (pagedList==null) return NotFound(new ApiException(404, "Bad Request", "No records found"));
            
            Response.AddPaginationHeader(new PaginationHeader(pagedList.CurrentPage, 
                pagedList.PageSize, pagedList.TotalCount, pagedList.TotalPages));
            
            return Ok(pagedList);
        }

        [HttpGet("report")]
        public async Task<ActionResult<ICollection<CallRecordBriefDto>>> GetCallRecordsReport([FromQuery] CallRecordParams histParams )
        {
            var list = await _histRepo.GetCallRecordsForReport(histParams, User.GetUsername());
            if (list==null) return NotFound(new ApiException(404, "Bad Request", "No records found"));
            
            return Ok(list);
        }
        
        
        [HttpPost("newCallRecord")]
        public async Task<ActionResult<CallRecord>> AddNewCallRecord(CallRecord userContact)
        {
            if(string.IsNullOrEmpty(userContact.Username)) userContact.Username = User.GetUsername();
            return await _histRepo.AddNewCallRecord(userContact);
        }

        
        [HttpDelete("deletehist/{CallRecordId}")]
        public async Task<bool> DeleteCallRecordById(int CallRecordId)
        {
            return await _histRepo.DeleteCallRecord(CallRecordId);
        }


        [HttpDelete("historyItemId/{historyitemid}")]
        public async Task<ActionResult<bool>> DeleteCallRecordItem(int historyitemid)
        {
            return await _histRepo.DeleteCallRecordItem(historyitemid);
        }

        [HttpPut]
        public async Task<ActionResult<CallRecordStatusReturnDto>> UpdateCallRecord(CallRecord CallRecord)
        {
    
            var returnDto = await _histRepo.EditOrAddNewCallRecord(CallRecord, User.GetUsername());

            if(!string.IsNullOrEmpty(returnDto.strError)) 
                return BadRequest(new ApiException(400, "Bad Request", returnDto.strError));
            
            return Ok(returnDto); 

        }

        [HttpPut("UpdateNewItem")]
        public async Task<ActionResult<CallRecord>> UpdateCallRecordWithSingleItem(CallRecord callRecord)
        {
            callRecord.Username ??= User.GetUsername();
            var returnDto = await _histRepo.EditCallRecordWithSingleItem(callRecord, User.GetUsername());

            return Ok(returnDto.CallRecord);
        }

        [HttpPut("InsertCallRecordItem")]
        public async Task<ActionResult<CallRecordItemAddedReturnValueDto>> UpdateOrInsertCallRecordItem(CallRecordItemToAddDto callItem)
        {
            var item = await _histRepo.InsertCallRecordItem(callItem, User.GetUsername());
            if(item == null || !string.IsNullOrEmpty(item.ErrorString)) return BadRequest(new ApiException(400,"Failed to insert the call record item", item.ErrorString));

            return Ok(item);
        }

        [HttpGet("callRecordWithItems/{personType}/{personId}")]
        public async Task<ActionResult<CallRecord>> GetCallRecordWithItems(string personType, string personId)
        {
            var obj = await _histRepo.GetOrGenerateCallRecord(personType, personId, User.GetUsername());

            if(obj == null) return BadRequest(new ApiException(400, "Bad Request", "Failed to get the Call Record"));
            
            return Ok(obj);
        }

        [HttpGet("CallRecordSummary/{PersonType}/{PersonId}")]
        public async Task<ICollection<CallRecordBriefDto>> GetCallRecordSummaryOfCandidate(string PersonType, string PersonId)
        {
            return await _histRepo.GetCallRecordSummaryOfCandidate(PersonId, PersonType);
        }

    }
}