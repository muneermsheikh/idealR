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
    public class UserHistoryController : BaseApiController
    {
        private readonly IUserHistoryRepository _histRepo;
        public UserHistoryController(IUserHistoryRepository histRepo)
        {
            _histRepo = histRepo;
        }

        [HttpGet("historywithitems/{historyid}")]
        public async Task<ActionResult<UserHistoryDto>> GetUserHistoryDataByHistoryId(int historyid)
        {
            var data = await _histRepo.GetHistoryWithItemsFromHistoryId(historyid);

            if (data == null) return NotFound("No record found with selected id");
            return Ok(data);

        }

        
        [HttpGet("pagedlist")]
        public async Task<ActionResult<PagedList<UserHistoryBriefDto>>> GetPaginatedUserHistory([FromQuery] UserHistoryParams histParams )
        {
            var pagedList = await _histRepo.GetUserHistoryPaginated(histParams);
            if (pagedList==null) return NotFound(new ApiException(404, "Bad Request", "No records found"));
            
            Response.AddPaginationHeader(new PaginationHeader(pagedList.CurrentPage, 
                pagedList.PageSize, pagedList.TotalCount, pagedList.TotalPages));
            
            return Ok(pagedList);
        }
        
        [HttpGet("userhistorydto")]
        public async Task<ActionResult<UserHistoryDto>> GetUserHistoryDto([FromQuery] UserHistoryParams histParams )
        {
            var pagedList = await _histRepo.GetUserHistoryPaginated(histParams);
            if (pagedList==null) return NotFound(new ApiException(404, "Bad Request", "No records found"));
            
            Response.AddPaginationHeader(new PaginationHeader(pagedList.CurrentPage, 
                pagedList.PageSize, pagedList.TotalCount, pagedList.TotalPages));
            
            return Ok(pagedList);
        }
        
        
        [HttpPost("newuserhistory")]
        public async Task<ActionResult<UserHistory>> AddNewUserHistory(UserHistory userContact)
        {
            return await _histRepo.AddNewUserHistory(userContact);
        }

        
        [HttpDelete("{userhistoryId}")]
        public async Task<bool> DeleteUserContactById(int userhistoryId)
        {
            return await _histRepo.DeleteUserHistory(userhistoryId);
        }


        [HttpDelete("historyItemId/{historyitemid}")]
        public async Task<ActionResult<bool>> DeleteUserHistoryItem(int historyitemid)
        {
            return await _histRepo.DeleteUserHistoryItem(historyitemid);
        }

        [HttpPut]
        public async Task<ActionResult<UserHistoryReturnDto>> UpdateUserHistory(UserHistory userhistory)
        {
            var returnDto = new UserHistoryReturnDto();

            returnDto = await _histRepo.EditContactHistory(userhistory, User.GetUsername());

            if (!string.IsNullOrEmpty(returnDto.ErrorString)) 
                return BadRequest(new ApiException(402, "Bad Request", "failed to update the contact history"));

            return Ok(returnDto); 

        }

        [HttpGet("userItems")]
        public async Task<ActionResult<UserHistoryItem>> UpdateHistoryItem(UserHistoryItem userItem)
        {
            var historyItem = await _histRepo.UpdateHistoryItem(userItem, User.GetUsername());
            if (historyItem != null) return Ok(historyItem);

            return BadRequest(new ApiException(402, "Bad Request", "Failed to Update the transactions items"));
        }

        [HttpPost("userhistoryitem")]
        public async Task<ActionResult<UserHistoryItem>> InsertUserHistoryItem(UserHistoryItem userhistoryitem) {
            
            var item = await _histRepo.AddNewHistoryItem(userhistoryitem, User.GetUsername());
            return item;
        }
        
       
    }
}