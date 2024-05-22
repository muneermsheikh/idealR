using api.DTOs.HR;
using api.Entities.HR;
using api.Entities.Master;
using api.Errors;
using api.Extensions;
using api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Authorize(Policy = "HRMPolicy")]
    public class ChecklistController : BaseApiController
    {
        private readonly IChecklistRepository _repo;
        public ChecklistController(IChecklistRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("generate/{candidateid}/{orderitemid}")]
        public async Task<ActionResult<ChecklistHR>> GenerateNewChecklist(int candidateid, int orderitemid)
        {
            var checkobj = await _repo.GetOrGenerateChecklist(candidateid, orderitemid, User.GetUsername());

            if(!string.IsNullOrEmpty(checkobj.ErrorString)) return BadRequest(new ApiException(500, "Bad Request", checkobj.ErrorString));

            return Ok(checkobj.ChecklistHR);
        }

        [HttpPost("newchecklist")]
        public async Task<ActionResult<ChecklistHR>> SaveNewChecklist (ChecklistHR checklisthr)
        {
            var checklist = await _repo.SaveNewChecklist(checklisthr, User.GetUsername());

            if(!string.IsNullOrEmpty(checklist.ErrorString)) return BadRequest(checklist.ErrorString);

            return Ok(checklist.ChecklistHR);
        }

        [HttpDelete("checklistData")]
        public async Task<ActionResult<bool>> DeleteChecklistData (ChecklistHRData checklistHRData)
        {
            var deleted = await _repo.DeleteChecklistHRDataAsync(checklistHRData);

            if (!deleted) return BadRequest("Failed to delete the checklist HR data");

            return Ok(true);
        }

        [HttpPut("checklistData")]
        public async Task<ActionResult<bool>> EditChecklistData (ChecklistHRData checklistHRData)
        {
            var deleted = await _repo.EditChecklistHRDataAsync(checklistHRData);

            if (!deleted) return BadRequest("Failed to update the checklist HR data");

            return Ok(true);
        }

        [HttpGet("checklistData")]
        public async Task<ActionResult<ICollection<ChecklistHRData>>> GetChecklistData ()
        {
            var data = await _repo.GetChecklistHRDataListAsync();

            if (data == null) return BadRequest("Failed to get the checklist HR data");

            return Ok(data);
        }

        [HttpPut("checklisthr")]
        public async Task<ActionResult<string>> EditChecklistHR (ChecklistHR checklistHR)
        {
            var errorList = await _repo.EditChecklistHR(checklistHR, User.GetUsername());

            if (!string.IsNullOrEmpty(errorList)) return BadRequest("Following Error encountered: " + errorList);

            return Ok(true);
        }

        [HttpGet("checklistHR/{candidateid}/{orderitemid}")]
        public async Task<ActionResult<ChecklistHRDto>> GetChecklistHR (int candidateid, int orderitemid)
        {
            var data = await _repo.GetChecklistHRFromCandidateIdAndOrderDetailId(candidateid, orderitemid);

            if (data == null) return BadRequest("Failed to get the checklist HR");

            return Ok(data);
        }

        [HttpDelete("checklist/{id}")]
        public async Task<ActionResult<bool>> DeleteChecklistHR (int id)
        {
            var data = await _repo.DeleteChecklistHR(id);

            if (!data) return BadRequest("Failed to delete the Checklist data");

            return Ok(data);
        }

        [HttpGet("candidatechecklists/{candidateid}")]
        public async Task<ActionResult<bool>> GetCandidateChecklists (int candidateid)
        {
            var data = await _repo.GetChecklistHROfCandidate(candidateid);

            if (data == null) return BadRequest("The candidate has not been checklisted");

            return Ok(data);
        }

    }
}
