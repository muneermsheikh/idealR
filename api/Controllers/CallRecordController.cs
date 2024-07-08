using api.DTOs.Admin;
using api.DTOs.HR;
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

        [HttpGet("getOrAddHistoryWithItems")]
        public async Task<ActionResult<CallRecord>> GetUserHistorByParams([FromQuery]CallRecordItemToCreateDto histParams)
        {
            var data = await _histRepo.GetOrAddCallRecordWithItemsByParams(histParams, User.GetUsername());

            if (data == null) return NotFound(new ApiException(400, "Not Found","No record found with given parameters"));
            
            return Ok(data);
        }

        [HttpGet("callRecordFromPhoneNo/{phoneno}")]
        public async Task<ActionResult<CallRecord>> GetCallRecordFromPhoneNo(string phoneno)
        {
            var data = await _histRepo.GetCallRecordWithItemsFromPhoneNo(phoneno, User.GetUsername());
            return Ok(data);
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
        
        
        [HttpPost("newCallRecord")]
        public async Task<ActionResult<CallRecord>> AddNewCallRecord(CallRecord userContact)
        {
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
        public async Task<ActionResult<CallRecord>> UpdateCallRecord(CallRecord CallRecord)
        {
    
            var returnDto = await _histRepo.EditCallRecord(CallRecord, User.GetUsername());

            return Ok(returnDto.CallRecord); 

        }

        [HttpPut("UpdateNewItem")]
        public async Task<ActionResult<CallRecord>> UpdateCallRecordWithSingleItem(CallRecord callRecord)
        {
            var returnDto = await _histRepo.EditCallRecordWithSingleItem(callRecord, User.GetUsername());

            return Ok(returnDto.CallRecord);
        }

        [HttpPost("CallRecorditem")]
        public async Task<ActionResult<CallRecordItem>> InsertCallRecordItem(CallRecordItem CallRecorditem) {
            
            var item = await _histRepo.AddNewHistoryItem(CallRecorditem, User.GetUsername());
            return item;
        }
        
        [HttpGet("callRecordWithItems/{callRecordId}/{personType}/{personId}/{categoryRef}")]
        public async Task<ActionResult<CallRecord>> GetCallRecordWithItems(int callRecordId, string personType, string personId,
            string categoryRef )
        {
            var obj = await _histRepo.GetCallRecordWithItems(callRecordId, personType, personId, categoryRef, User.GetUsername());

            if(obj == null) return BadRequest(new ApiException(400, "Bad Request", "Failed to get the Call Record"));
            
            return Ok(obj);
        }


    }
}