using api.DTOs.Admin;
using api.Entities.HR;
using api.Errors;
using api.Extensions;
using api.Helpers;
using api.Interfaces.Admin;
using api.Params.Admin;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    public class SelectionController : BaseApiController
    {
        private readonly ISelDecisionRepository _selRepo;
        private readonly ICVRefRepository _cvrefRepo;
        public SelectionController(ISelDecisionRepository selRepo, ICVRefRepository cvrefRepo)
        {
            _cvrefRepo = cvrefRepo;
            _selRepo = selRepo;
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<SelDecisionDto>>> GetSelectionDecisions(SelDecisionParams selParams)
        {
                       
            var decs = await _selRepo.GetSelectionDecisions(selParams);
            if (decs != null) return Ok(decs);
            return NotFound(new ApiException(404, "no records found"));
        }

        [HttpPost]
        public async Task<ActionResult<bool>> RegisterSelections(ICollection<CreateSelDecisionDto> dtos)
        {
            var decs = await _selRepo.RegisterSelections(dtos, User.GetUsername());

            if(!string.IsNullOrEmpty(decs.ErrorString)) return BadRequest(new ApiException(400, decs.ErrorString));

            return Ok("Selections registered");
        }

     
        [HttpPut]
        public async Task<ActionResult<bool>> EditSelectionDecision(SelectionDecision selDecision)
        {
            return await _selRepo.EditSelection(selDecision);
        }

    
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteSelectionDecision(int id)
        {
            return await _selRepo.DeleteSelection(id);
        }

        [HttpGet("pendingselections")]
        public async Task<ActionResult<PagedList<CVRefDto>>> SelectionDecisionPending(CVRefParams refParams)
        {
            var data = await _cvrefRepo.GetCVReferrals(refParams);
            if (data==null && data.Count == 0) return NotFound(new ApiException(404, "No referral decisions found pending as of now"));
            
            Response.AddPaginationHeader(new PaginationHeader(data.CurrentPage, 
                data.PageSize, data.TotalCount, data.TotalPages));
            
            return Ok(data);
        }
        
     
    }
}