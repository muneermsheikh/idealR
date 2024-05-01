using api.DTOs.HR;
using api.Entities.HR;
using api.Entities.Identity;
using api.Extensions;
using api.Helpers;
using api.Interfaces;
using api.Params.HR;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Authorize(Policy="HRMPolicy")]
    public class CandidateController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly ICandidateRepository _candidateRepo;
        public CandidateController(ICandidateRepository candidateRepo, UserManager<AppUser> userManager, IMapper mapper)
        {
            _candidateRepo = candidateRepo;
            _userManager = userManager;
            _mapper = mapper;
        }

        
        [HttpGet]
        public async Task<ActionResult<PagedList<CandidateBriefDto>>> GetCandidates([FromQuery]CandidateParams candidateParams)
        { 
            var candidates = await _candidateRepo.GetCandidates(candidateParams);

            if(candidates == null) return NotFound("No matching candidates found");

            Response.AddPaginationHeader(new PaginationHeader(candidates.CurrentPage, candidates.PageSize, 
                candidates.TotalCount, candidates.TotalPages));
            
            return Ok(candidates);

        }

        [HttpGet("byparams")]
        public async Task<ActionResult<Candidate>> GetCandidateByParams([FromQuery]CandidateParams candidateParams)
        { 
            var candidate = await _candidateRepo.GetCandidate(candidateParams);

            if(candidate == null) return NotFound("Not found");

            return Ok(candidate);

        }


        [HttpPost]
        public async Task<ActionResult> CreateCandidate(CreateCandidateDto createDto)
        {
            var newCandidate = _mapper.Map<Candidate>(createDto);

            if (await _candidateRepo.InsertCandidate(newCandidate)) return Ok();
            
            return BadRequest("Failed to create the Customer Object");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCandidate(int id)
        {
            if(await _candidateRepo.DeleteCandidate(id)) return Ok();
            
            return BadRequest("Failed to delete the candidate");
        }

        [HttpPut]
        public async Task<ActionResult> EditCandidate(Candidate candidate)
        {
            var updated = await _candidateRepo.UpdateCandidate(candidate);
            if (updated) return Ok();
            return BadRequest("Failed to update the candidate object");
        }

    }
}