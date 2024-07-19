using api.DTOs.HR;
using api.Extensions;
using api.Helpers;
using api.Interfaces.HR;
using api.Params.HR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Authorize(Policy ="HRMPolicy")]    //RequireRole("HR Manager", "HR Supervisor", "HR Executive", "Admin", "Admin Manager"));
    public class ProspectivesController : BaseApiController
    {
        public IProspectiveCandidatesRepository _ProspectiveRepo { get;set; }
        public ProspectivesController(IProspectiveCandidatesRepository prospectiveRepo)
        {
            _ProspectiveRepo = prospectiveRepo;
        }

        [HttpGet("pagedlist")]
        public async Task<ActionResult<PagedList<ProspectiveBriefDto>>> GetProspectivePagedList([FromQuery]ProspectiveCandidateParams pParams)
        {
            var pagedList = await _ProspectiveRepo.GetProspectivePagedList(pParams);

            if(pagedList.Count ==0) return Ok(null);    //  return BadRequest(new ApiException(400,"Bad Request", "failed to retrieve matching orders"));

            Response.AddPaginationHeader(new PaginationHeader(pagedList.CurrentPage, 
                pagedList.PageSize, pagedList.TotalCount, pagedList.TotalPages));
            
            return Ok(pagedList);
            
        }

        [HttpDelete("delete/{prospectiveid}")]
        public async Task<ActionResult<bool>> DeleteProspectiveCandidate(int prospectiveid)
        {
            return await _ProspectiveRepo.DeleteProspectiveCandidate(prospectiveid);

        }


        [HttpPut("convertProspective/{prospectiveid}")]
        public async Task<ActionResult<int>> ConvertProspectiveToCandidate(int prospectiveid)
        {
            var appno = await _ProspectiveRepo.ConvertProspectiveToCandidate(prospectiveid);

            return Ok(appno);
        }
    }
}