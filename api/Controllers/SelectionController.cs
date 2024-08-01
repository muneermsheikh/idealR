using api.DTOs.Admin;
using api.Entities.HR;
using api.Errors;
using api.Extensions;
using api.Helpers;
using api.Interfaces.Admin;
using api.Interfaces.Messages;
using api.Params.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace api.Controllers
{
    [Authorize(Policy = "SelectionPolicy")]     //RequireRole("Selection"));
    public class SelectionController : BaseApiController
    {
        private readonly ISelDecisionRepository _selRepo;
        private readonly ICVRefRepository _cvrefRepo;
        public SelectionController(ISelDecisionRepository selRepo, ICVRefRepository cvrefRepo)
        {
            _cvrefRepo = cvrefRepo;
            _selRepo = selRepo;
        }

        [HttpGet("selection/{cvrefid}")]
        public async Task<ActionResult<SelectionDecision>> GetSelectionDecisionFromCVRefId(int cvrefid)
        {
            var sel = await _selRepo.GetSelectionDecisionFromCVRefId(cvrefid);
            if(sel == null) return BadRequest(new ApiException(400, "Not Found", "failed to retrieve the selection record"));

            return Ok(sel);
        }
        [HttpGet]
        public async Task<ActionResult<PagedList<SelDecisionDto>>> GetSelectionDecisionsAsync([FromQuery]SelDecisionParams selParams)
        {
             
            var pagedList = await _selRepo.GetSelectionDecisions(selParams);
            if (pagedList == null) return NotFound(new ApiException(404, "no records found"));
             
            Response.AddPaginationHeader(new PaginationHeader(pagedList.CurrentPage, 
                pagedList.PageSize, pagedList.TotalCount, pagedList.TotalPages));
            
            return Ok(pagedList);
        }

        [HttpGet("selectionBySelDecisionId/{selDecisionId}")]
        public async Task<ActionResult<SelDecisionDto>> GetSelectionById(int selDecisionId)
        {
            var sel = await _selRepo.GetSelDecisionDtoFromId(selDecisionId);

            if(sel == null) return NotFound(new ApiException(404, "Not Found", "Failed to find the selection object"));

            return Ok(sel);
        }
        [HttpPost]
        public async Task<ActionResult<MessageWithError>> RegisterSelections(ICollection<CreateSelDecisionDto> dtos)
        {
            if(dtos.Count == 0) return BadRequest(new ApiException(404, "Bad Request", "Blank array"));
            
            var msgWithErr = await _selRepo.RegisterSelections(dtos, User.GetUsername());

            return Ok(msgWithErr);
        }
     
        [HttpPut]
        public async Task<ActionResult<bool>> EditSelectionDecision(SelectionDecision selDecision)
        {
            return await _selRepo.EditSelection(selDecision);
        }

        /*[HttpPut("employment")]
        public async Task<ActionResult<bool>> EditEmployment(Employment employment)
        {
            var edited = await _selRepo.EditEmployment(employment, User.GetUsername());

            if(!edited) return BadRequest(new ApiException(400, "Failed to edit the employment details"));

            return Ok(edited);
        }
        */

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteSelectionDecision(int id)
        {
            var errString= await _selRepo.DeleteSelection(id);

            if(!string.IsNullOrEmpty(errString)) return BadRequest(new ApiException(400, "Failed to delete the Selection", errString));

            return Ok(true);
            
        }

        /*[HttpGet("pendingselections")]
        public async Task<ActionResult<PagedList<CVRefDto>>> SelectionDecisionPending(CVRefParams refParams)
        {
            var data = await _cvrefRepo.GetPendingReferrals(refParams);
            if (data==null && data.Count == 0) return NotFound(new ApiException(404, "No referral decisions found pending as of now"));
            
            Response.AddPaginationHeader(new PaginationHeader(data.CurrentPage, 
                data.PageSize, data.TotalCount, data.TotalPages));
            
            return Ok(data);
        }
        */

        [HttpGet("employment/{employmentid}")]
        public async Task<ActionResult<Employment>> GetEmployment (int employmentid)
        {
            var emp = await _selRepo.GetEmployment(employmentid);

            if(emp == null) return BadRequest("Failed to get the employment data");

            return Ok(emp);
        }


        [HttpGet("offeracceptancespending")]
        public async Task<ActionResult<EmploymentsNotConcludedDto>> OfferAcceptancesPending(EmploymentParams empParams)
        {
            var data = await _selRepo.EmploymentsAwaitingConclusion(empParams);

            if (data == null || data.Count == 0) return BadRequest("No employment offers are pending conclusion");

            Response.AddPaginationHeader(new PaginationHeader(data.CurrentPage, 
                data.PageSize, data.TotalCount, data.TotalPages));
            
            return Ok(data);
        }

        [HttpPost("acceptancereminders")]
        public async Task<ActionResult<string>> RemindCandidatesForOfferAcceptance(List<int> CVRefIds )
        {
            var status = await _selRepo.ComposeAcceptanceReminderToCandidates(CVRefIds, User.GetUsername());

            if(string.IsNullOrEmpty(status)) return Ok("");
            
            return BadRequest(new ApiException(400, "Bad Request", status));
        }
    
        [HttpGet("housekeepingcvrefandsel")]
        public async Task<ActionResult<string>> DoHouseKeeping() {
            var err = await _selRepo.HousekeepingCVRefAndSelDeecisions();

            if(!string.IsNullOrEmpty(err)) return BadRequest(new ApiException(400, "Failed to do the housekeeping", err));

            return Ok("");
        }
        
    }
}