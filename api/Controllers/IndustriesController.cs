using api.Entities.Master;
using api.Errors;
using api.Extensions;
using api.Helpers;
using api.Interfaces.Masters;
using api.Params.Masters;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    
    public class IndustriesController : BaseApiController
    {
        private readonly IIndustryRepository _indRepo;
        public IndustriesController(IIndustryRepository indRepo)
        {
            _indRepo = indRepo;
        }

        [HttpGet("industryPaged")]
        public async Task<ActionResult<PagedList<Industry>>> GetIndustriesPagedList([FromQuery]IndustryParams iParams)
        {
            var obj = await _indRepo.GetIndustries(iParams);

            if(obj == null) return BadRequest(new ApiException(400, "Bad Request", "Your instructions did not produce any data"));

            Response.AddPaginationHeader(new PaginationHeader(obj.CurrentPage, 
                obj.PageSize, obj.TotalCount, obj.TotalPages));
            
            return Ok(obj);
        }

        [HttpGet("industrylist")]
        public async Task<ActionResult<ICollection<Industry>>> GetIndustriesList()
        {
            var obj = await _indRepo.GetIndustriesList();

            if(obj == null) return BadRequest(new ApiException(400, "Bad Request", "Your instructions did not produce any data"));

            return Ok(obj);
        }

        [HttpGet("industrybyid/{id}")]
        public async Task<ActionResult<Industry>> GetIndustryById(int id)
        {
            var ind = await _indRepo.GetIndustryFromId(id);

            if(ind == null) return BadRequest(new ApiException(400, "Bad Request", "failed to get the Industry"));

            return Ok(ind);
        }

        [HttpGet("industrylist")]
        public async Task<ActionResult<ICollection<Industry>>> GetIndustryList()
        {
            var obj = await _indRepo.GetIndustriesList();
            if(obj == null) return BadRequest(new ApiException(400, "Bad Request", "No industry list on record"));
            return Ok(obj);
        }

        [HttpPost("add/{industryName}")]
        public async Task<ActionResult<Industry>> AddNewIndustry(string industryName)
        {
            var obj = await _indRepo.AddIndustry(industryName);
            
            if(obj == null) return BadRequest(new ApiException(400, "Failed to add", "Failed to insert the Industry"));
            
            return Ok(obj);

        }

        [HttpDelete("delete/{qName}")]
        public async Task<ActionResult<bool>> DeleteProfession(string qName)
        {
            var errString = await _indRepo.DeleteIndustry(qName);
            if(string.IsNullOrEmpty(errString)) 
                return Ok("Industry deleted successfully");
            
            return BadRequest(new ApiException(400, "Bad Request", errString));
        }

        [HttpDelete("deletebyid/{id}")]
        public async Task<ActionResult<bool>> DeleteProfessionById(int id)
        {
            var errString = await _indRepo.DeleteIndustryById(id);
            if(string.IsNullOrEmpty(errString)) 
                return Ok("Industry deleted successfully");
            
            return BadRequest(new ApiException(400, "Bad Request", errString));
        }

        [HttpPut("edit")]
        public async Task<ActionResult<bool>> EditQuaification(Industry industry)
        {
            var errString = await _indRepo.EditIndustry(industry);

            if(string.IsNullOrEmpty(errString)) 
                return Ok("Industry updated successfully");
            
            return BadRequest(new ApiException(400, "Bad Request", errString));
        }
    }
}