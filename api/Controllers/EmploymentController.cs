using api.DTOs.Admin;
using api.Entities.HR;
using api.Errors;
using api.Extensions;
using api.Helpers;
using api.Interfaces.Admin;
using api.Params.Admin;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    public class EmploymentController : BaseApiController
    {
        private readonly IEmploymentRepository _employmentRepo;
        public EmploymentController(IEmploymentRepository employmentRepo)
        {
            _employmentRepo = employmentRepo;
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<Employment>>> GetEmployments([FromQuery] EmploymentParams employmentParams)
        {
            var pagedList  = await _employmentRepo.GetEmployments(employmentParams);
            
            if(pagedList == null) return BadRequest(new ApiException(400, "Not Found Error", "failed to return matching records"));

            Response.AddPaginationHeader(new PaginationHeader(pagedList.CurrentPage, 
                pagedList.PageSize, pagedList.TotalCount, pagedList.TotalPages));
            
            return Ok(pagedList);

        }

        [HttpGet("generate/{selDecisionId}")]
        public async Task<ActionResult<Employment>> GenerateEmployment(int SelDecisionId)
        {
            var sel = await _employmentRepo.GenerateEmploymentObject(SelDecisionId);

            if (sel == null) return BadRequest(new ApiException(400, "Error", "Failed to generate employment object"));

            return Ok(sel);
        }

        [HttpPut("employment")]
        public async Task<ActionResult<bool>> UpdateEmployment(Employment dto)
        {
            var strErr = await _employmentRepo.EditEmployment(dto, User.GetUsername());

            if(!string.IsNullOrEmpty(strErr)) 
                return BadRequest(new ApiException(400, "Failed to update the employment", strErr));
            
            return Ok(true);
        }

        [HttpDelete("{employmentid}")]
        public async Task<ActionResult<bool>> DeleteEmployment(int employmentid)
        {
            var strErr = await _employmentRepo.DeleteEmployment(employmentid);

            if(!string.IsNullOrEmpty(strErr)) return BadRequest(new ApiException(400,"Error in deleting the employment object", strErr));

            return Ok("Employment objected deleted successfully");
        }

    
    
        [HttpPut("offeraccepted")]
        public async Task<ActionResult<bool>> RegisterOfferAcceptance(ICollection<OfferConclusionDto> dto)
        {
            dto = dto.Where(x => !"acceptedrejected".Contains(x.acceptedString.ToLower())).ToList();

            if(dto.Count == 0) return BadRequest(new ApiException(400, "invalid accepted String", "accepted value are 'Accepted' or 'Rejected"));
            
            var errorString= await _employmentRepo.RegisterOfferAcceptance(dto, User.GetUsername());

            if(!string.IsNullOrEmpty(errorString)) return BadRequest(errorString);

            return Ok("offer acceptance registered");
        }

    }
}