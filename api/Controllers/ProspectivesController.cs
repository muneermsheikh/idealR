using api.DTOs.HR;
using api.Errors;
using api.Extensions;
using api.Helpers;
using api.Interfaces.HR;
using api.Params;
using api.Params.Admin;
using api.Params.HR;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    public class ProspectivesController : BaseApiController
    {
        public IProspectiveCandidatesRepository _ProspectiveRepo { get;set; }
        public ProspectivesController(IProspectiveCandidatesRepository prospectiveRepo)
        {
            _ProspectiveRepo = prospectiveRepo;
        }

        [HttpGet("pagedlist")]
        public async Task<ActionResult<PagedList<ProspectiveBriefDto>>> GetProspectivePagedList([FromQuery]CallRecordParams pParams)
        {
            var pagedList = await _ProspectiveRepo.GetProspectivePagedList(pParams);

            if(pagedList.Count ==0) return BadRequest(new ApiException(400,"Bad Request", "failed to retrieve matching orders"));

            Response.AddPaginationHeader(new PaginationHeader(pagedList.CurrentPage, 
                pagedList.PageSize, pagedList.TotalCount, pagedList.TotalPages));
            
            return Ok(pagedList);
            
        }

    }
}