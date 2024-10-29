using api.DTOs.Admin;
using api.Entities.Admin;
using api.Entities.Master;
using api.Errors;
using api.Extensions;
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

        [HttpPost("insertIds")]
        public async Task<ActionResult<ICollection<AppId>>> InsertNewIds(ICollection<int> NewIds) {
            
            var obj =  await _helpRepo.GenerateInterviewInvitationMessages(NewIds, User.GetUsername());     //obj contans Application Nos.
            if(obj == null) return BadRequest(new ApiException(400, "Bad Request", "Failed to insert the ids"));

            return Ok(obj.ApplicationIds); 
        }

    }
}