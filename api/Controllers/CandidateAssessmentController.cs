using System.ComponentModel;
using api.DTOs.HR;
using api.Entities.HR;
using api.Errors;
using api.Extensions;
using api.Helpers;
using api.Interfaces.HR;
using api.Params.HR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Authorize(Policy ="HRMPolicy")]
    public class CandidateAssessmentController : BaseApiController
    {
        private readonly ICandidateAssessentRepository _repo;
        public CandidateAssessmentController(ICandidateAssessentRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("generate/{candidateid}/{orderitemid}")]
        public async Task<ActionResult<CandidateAssessment>> GenerateCandidateAssessment(int candidateid, int orderitemid)
        {
            var assessment = await _repo.GenerateCandidateAssessment(candidateid, orderitemid, User.GetUsername());

            if(assessment == null) return BadRequest("Failed to generate candidate assessment");

            return Ok(assessment);
        }

        [HttpPost("assessment")]
        public async Task<ActionResult<CandidateAssessment>> InsertCandidateAssessment(CandidateAssessment candidateAssessment)
        {
            var assessment = await _repo.SaveCandidateAssessment(candidateAssessment, User.GetUsername());
            if(assessment == null) return BadRequest();
            return Ok(assessment);
        }

        [HttpPut("assessment")]
        public async Task<ActionResult<bool>> UpdateCandidateAssessment(CandidateAssessment candidateAssessment)
        {
            var errorString = await _repo.EditCandidateAssessment(candidateAssessment, User.GetUsername());
            if(!string.IsNullOrEmpty(errorString)) return BadRequest(errorString);

            return Ok(true);
        }

        [HttpDelete("assessment/{assessmentid}")]
        public async Task<ActionResult<bool>> DeleteCandidateAssessment(int assessmentid)
        {
            var deleted = await _repo.DeleteCandidateAssessment(assessmentid);
            if(!deleted) return BadRequest();

            return Ok(true);
        }

        [HttpGet("candidateassessments/{candidateid}")]
        public async Task<ActionResult<ICollection<CandidateAssessedShortDto>>> GetCandidateAssessmentsDto(int candidateid)
        {
            var dtos = await _repo.GetCandidateAssessmentsByCandidateId(candidateid);

            return Ok(dtos);
        }

        [HttpGet("assessmentspaged")]
        public async Task<ActionResult<PagedList<CandidateAssessedDto>>> GetCandidateAssessments([FromQuery]CandidateAssessmentParams assessmentParams)
        {
            var assessments = await _repo.GetCandidateAssessments(assessmentParams);
            
            Response.AddPaginationHeader(new PaginationHeader(assessments.CurrentPage, assessments.PageSize, 
                assessments.TotalCount, assessments.TotalPages));
            
            return Ok(assessments);

        }

        [HttpGet("assessment")]
        public async Task<ActionResult<CandidateAssessment>> GetCandidateAssessment([FromQuery]CandidateAssessmentParams assessmentParams)
        {
            var assessments = await _repo.GetCandidateAssessment(assessmentParams);
            if(assessments == null) return BadRequest("Your parameters did not produce any result");

            return Ok(assessments);

        }

        [HttpGet("assessmentbyid/{candidateAssessmentId}")]
        public async Task<ActionResult<CandidateAssessmentAndChecklistDto>> GetCandidateAssessmentById(int candidateAssessmentId)
        {
            var assessmentAndChecklist = await _repo.GetCandidateAssessmentById(candidateAssessmentId, User.GetUsername());

            if(assessmentAndChecklist.Assessed == null && assessmentAndChecklist.ChecklistHRDto == null) 
                return BadRequest("Your parameters did not produce any result");

            return Ok(assessmentAndChecklist);

        }

        [HttpGet("candidateAssessmentDto/{candidateid}/{orderitemid}")]
        public async Task<ActionResult<CandidateAssessmentDto>> GetCandidateAssessmentWithItems(int candidateid, int orderitemid)
        {
            if(candidateid == 0 || orderitemid == 0) return BadRequest(new ApiException(400, "Bad Request", "Candidate Id and/Or OrderItemId not provided"));
            
            var assessments = await _repo.GetCandidateAssessmentDtoWithItems(candidateid, orderitemid);
            if(assessments == null) return BadRequest(new ApiException(404, "Bad Request", "Your parameters did not produce any result"));

            return Ok(assessments);
        }

        [HttpPost("additemstocandidateassessment/{candidateAssessmentId}")]
        public async Task<ActionResult<bool>> AddItemsToCandidateAssessment(int candidateAssessmentId)
        {
            return await _repo.AddCandidateAssessmentItems(candidateAssessmentId);
        }

        [HttpPut("updatestatus/{candidateassessmentid}")]
        public async Task<ActionResult<string>> UpdateCandidateAssessmentStatus(int candidateassessmentid)
        {
            var errorString = await _repo.UpdateCandidateAssessmentStatus(candidateassessmentid, User.GetUsername());
            if(!string.IsNullOrEmpty(errorString)) return BadRequest(errorString);
            return Ok("updated to " + errorString);
        }
    
        [HttpGet("assessmentandchecklist/{candidateId}/{orderItemId}")]
        public async Task<ActionResult<CandidateAssessmentAndChecklistDto>> GetCandidateAssessmentWithChecklist(int candidateId, int orderItemId)
        {
            var assessmentWithErr = await _repo.GetChecklistAndAssessment(candidateId, orderItemId, User.GetUsername());
            
            if(assessmentWithErr.ChecklistHRDto != null) assessmentWithErr.ErrorString="";
            if(!string.IsNullOrEmpty(assessmentWithErr.ErrorString)) {
                return BadRequest(new ApiException(400, "Bad Request", assessmentWithErr.ErrorString));
            }

            var dto = new CandidateAssessmentAndChecklistDto {
                Assessed = assessmentWithErr.Assessed,
                ChecklistHRDto = assessmentWithErr.ChecklistHRDto
            };


            return Ok(dto);
        }
    }
}