using api.Entities.Master;
using api.Errors;
using api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    public class HelpController : BaseApiController
    {
        private readonly IHelpRepository _helpRepo;
        public HelpController(IHelpRepository helpRepo)
        {
            _helpRepo = helpRepo;
        }

        [HttpGet("help/{topic}")]
        public async Task<ActionResult<Help>> GetHelp(string topic)
        {
            var obj = await _helpRepo.GetHelpOnATopic(topic);

            if(obj == null) return BadRequest(new ApiException(404, "Bad Request", "The help topic was not found"));

            return Ok(obj);
        }

    }
}