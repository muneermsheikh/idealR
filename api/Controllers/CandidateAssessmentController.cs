using System.ComponentModel;
using api.Entities.HR;
using api.Extensions;
using api.Helpers;
using api.Interfaces.HR;
using api.Params.HR;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
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

        [HttpPost]
        public async Task<ActionResult<CandidateAssessment>> InsertCandidateAssessment(CandidateAssessment candidateAssessment)
        {
            var assessment = await _repo.SaveCandidateAssessment(candidateAssessment, User.GetUsername());
            if(assessment == null) return BadRequest();
            return Ok(assessment);
        }

        [HttpPut]
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

        [HttpGet("candidateAssessmentspaged")]
        public async Task<ActionResult<PagedList<CandidateAssessment>>> GetCandidateAssessments([FromQuery]CandidateAssessmentParams assessmentParams)
        {
            var assessments = await _repo.GetCandidateAssessments(assessmentParams);
            
            Response.AddPaginationHeader(new PaginationHeader(assessments.CurrentPage, assessments.PageSize, 
                assessments.TotalCount, assessments.TotalPages));
            
            return Ok(assessments);

        }

        [HttpGet("candidateAssessment")]
        public async Task<ActionResult<CandidateAssessment>> GetCandidateAssessment(CandidateAssessmentParams assessmentParams)
        {
            var assessments = await _repo.GetCandidateAssessment(assessmentParams);
            if(assessments == null) return BadRequest("Your parameters did not produce any result");

            return Ok(assessments);

        }

        [HttpGet("candidateAssessmentWithItems")]
        public async Task<ActionResult<CandidateAssessment>> GetCandidateAssessmentWithItems(CandidateAssessmentParams assessmentParams)
        {
            var assessments = await _repo.GetCandidateAssessmentWithItems(assessmentParams);
            if(assessments == null) return BadRequest("Your parameters did not produce any result");

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
    }
}