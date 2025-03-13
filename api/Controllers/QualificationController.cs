using api.Entities.Master;
using api.Errors;
using api.Extensions;
using api.Helpers;
using api.Interfaces.Masters;
using api.Params.Masters;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    public class QualificationController : BaseApiController
    {
        private readonly IQualificationRepository _qRepo;
        public QualificationController(IQualificationRepository qRepo)
        {
            _qRepo = qRepo;
        }

        [HttpGet("qById/{id}")]
        public async Task<ActionResult<Qualification>> GetQualificationById(int id)
        {
            var obj = await _qRepo.GetQualificationById(id);

            if(obj == null) return BadRequest(new ApiException(400, "Bad Request", "No such Qualification exists"));

            return Ok(obj);
        }

        [HttpGet("qualificationlist")]
        public async Task<ActionResult<ICollection<Qualification>>> GetQualificationList()
        {
            var obj = await _qRepo.GetQualificationList();

            if(obj == null) return BadRequest(new ApiException(400, "Bad Request", "Your instructions did not produce any data"));

            return Ok(obj);
        }

        [HttpPost("add/{qName}")]
        public async Task<ActionResult<Qualification>> AddANewQualification(string qName)
        {
            var dto = await _qRepo.AddQualification(qName);
            if(!string.IsNullOrEmpty(dto.ErrorString)) return BadRequest(new ApiException(400, "Failed to add the Qualification", dto.ErrorString));

            return Ok(dto.qualification);

        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<bool>> DeleteQualificationById(int id)
        {
            var errString = await _qRepo.DeleteQualificationById(id);
            if(string.IsNullOrEmpty(errString)) 
                return Ok("Qualification deleted successfully");
            
            return BadRequest(new ApiException(400, "Bad Request", errString));
        }

        [HttpPut("edit")]
        public async Task<ActionResult<bool>> EditQuaification(Qualification qualification)
        {
                if(string.IsNullOrEmpty(qualification.QualificationName)) return BadRequest(new ApiException(404, "Bad Request", "Qualification Name not provided"));

                var errString = await _qRepo.EditQualification(qualification);

                if(string.IsNullOrEmpty(errString)) 
                    return Ok("Qualification updated successfully");
                
                return BadRequest(new ApiException(400, "Bad Request", errString));
        }
    }
}