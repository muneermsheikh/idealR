using api.Entities.Master;
using api.Errors;
using api.Extensions;
using api.Helpers;
using api.Interfaces.Masters;
using api.Params.Masters;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    public class ProfessionController : BaseApiController
    {
        private readonly IProfessionRepository _profRepo;
        public ProfessionController(IProfessionRepository profRepo)
        {
            _profRepo = profRepo;
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<Profession>>> GetProfessionPagedList([FromQuery]ProfessionParams professionParams)
        {
            var obj = await _profRepo.GetProfessions(professionParams);

            if(obj == null) return BadRequest(new ApiException(400, "Bad Request", "Your instructions did not produce any data"));

            Response.AddPaginationHeader(new PaginationHeader(obj.CurrentPage, 
                obj.PageSize, obj.TotalCount, obj.TotalPages));
            
            return Ok(obj);
        }

        [HttpGet("professionlist")]
        public async Task<ActionResult<ICollection<Profession>>> GetProfessionList()
        {
            var obj = await _profRepo.GetProfessionList();

            if(obj == null) return BadRequest(new ApiException(400, "Bad Request", "Your instructions did not produce any data"));

            return Ok(obj);
        }

        [HttpPost("add")]
        public async Task<ActionResult<Profession>> AddANewProfession(Profession profession)
        {
            var obj = await _profRepo.AddProfession(profession);
            if(obj==null) return BadRequest(new ApiException(400, "Failed to add the proession", "Failed to add the Profession"));

            return Ok(obj);

        }

        [HttpDelete("delete/{professionName}")]
        public async Task<ActionResult<bool>> DeleteProfession(string professionName)
        {
            var errString = await _profRepo.DeleteProfession(professionName);
            if(string.IsNullOrEmpty(errString)) 
                return Ok("Profession deleted successfully");
            
            return BadRequest(new ApiException(400, "Bad Request", errString));
        }

        [HttpDelete("deletebyid/{professionid}")]
        public async Task<ActionResult<bool>> DeleteProfessionById(int professionid)
        {
            var errString = await _profRepo.DeleteProfessionById(professionid);
            if(string.IsNullOrEmpty(errString)) 
                return Ok("Profession deleted successfully");
            
            return BadRequest(new ApiException(400, "Bad Request", errString));
        }

        [HttpPut("edit")]
        public async Task<ActionResult<bool>> EditProfession(Profession profession)
        {
            var errString = await _profRepo.EditProfession(profession);

            if(string.IsNullOrEmpty(errString)) 
                return Ok(true);
            
            return BadRequest(new ApiException(400, "Bad Request", errString));
        }

        [HttpGet("profession/{professionid}")]
        public async Task<ActionResult<Profession>> GetProfessionbyId (int professionid)
        {
            return await _profRepo.GetProfessionById(professionid);
        }

    }
}