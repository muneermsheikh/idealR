using api.DTOs.Finance;
using api.Errors;
using api.Extensions;
using api.Helpers;
using api.Interfaces.Finance;
using api.Params.Finance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Authorize(Policy = "AccountsPolicy")]
    public class FinanceController : BaseApiController
    {
        private readonly IFinanceRepository _finRepo;
        public FinanceController(IFinanceRepository finRepo)
        {
            _finRepo = finRepo;
        }

        [HttpGet("DrApprovalsPending")]
        public async Task<ActionResult<PagedList<PendingDebitApprovalDto>>> GetPendingDebitApproval(DrApprovalParams pParams)
        {
            var data = await _finRepo.GetPendingDebitApprovals(pParams);

            if(data == null || data.Count ==0) return NotFound(new ApiException(400, "No data found", ""));

              Response.AddPaginationHeader(new PaginationHeader(data.CurrentPage, data.PageSize, 
                data.TotalCount, data.TotalPages));
            
            return Ok(data);
        }



    }
}