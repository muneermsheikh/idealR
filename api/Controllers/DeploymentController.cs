using api.DTOs.Process;
using api.Entities.Admin;
using api.Entities.Deployments;
using api.Errors;
using api.Extensions;
using api.Helpers;
using api.Interfaces.Deployments;
using api.Params.Deployments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Authorize(Policy = "ProcessPolicy")]
    public class DeploymentController : BaseApiController
    {
        private readonly IDeploymentRepository _depRepo;
        public DeploymentController(IDeploymentRepository depRepo)
        {
            _depRepo = depRepo;
        }

        [HttpGet("bycvrefid/{cvrefid}")]
        public async Task<ActionResult<Dep>> GetDeploymentByCVRefId (int cvrefid)
        {
            var data = await _depRepo.GetDeploymentByCVRefId(cvrefid);
            
            if(data == null) return NotFound(new ApiException(400, "No record found", ""));

            return Ok(data);
        }
    
        [HttpGet("deployStatusData")]
        public async Task<ActionResult<DeployStatus>> GetDeploymentStatusData ()
        {
            var data = await _depRepo.GetDeploymentStatusData();
            
            if(data == null) return NotFound(new ApiException(400, "No record found", ""));

            return Ok(data);
        }
    
        [HttpGet("deployStatusSeqAndNames")]
        public async Task<ActionResult<DeployStatusAndName>> GetDeploymentStatusSeqAndNames ()
        {
            var data = await _depRepo.GetDeploymentSeqAndStatus();
            
            if(data == null) return NotFound(new ApiException(400, "No record found", ""));

            return Ok(data);
        }
    

        [HttpGet("deployments")]
        public async Task<ActionResult<PagedList<DeploymentPendingDto>>> GetDeployments ([FromQuery]DeployParams depParams)
        {
            var data = await _depRepo.GetDeployments(depParams);
            
            if(data == null || data.Count ==0) return NotFound(new ApiException(400, "No data found", ""));

              Response.AddPaginationHeader(new PaginationHeader(data.CurrentPage, data.PageSize, 
                data.TotalCount, data.TotalPages));
            
            return Ok(data);
        }

        [HttpGet("deployment/{id}")]
        public async Task<ActionResult<Dep>> GetDeploymentWithItems(int id)
        {
            var dep = await _depRepo.GetDeploymentByDepId(id);

            if(dep==null) return NotFound(new ApiException(400, "Not Found", "Failed to get the deployment record"));

            return Ok(dep);
        }
    
        [HttpPost("depItems")]
        public async Task<ActionResult<ICollection<DeploymentPendingDto>>> AddDeploymentItems(ICollection<DepItemToAddDto> depItemsDto)
        {
            var dtos = await _depRepo.AddDeploymentItems(depItemsDto, User.GetUsername());
            if(!string.IsNullOrEmpty(dtos.ErrorString)) return BadRequest(new ApiException(400, "Failed to add the deployment item", ""));

            return Ok(dtos.deploymentPendingDtos);
        }
    
        [HttpPut("deployment")]
        public async Task<ActionResult<string>> EditDeployment(Dep dep)
        {
            var errString = await _depRepo.EditDeployment(dep);
            if(!string.IsNullOrEmpty(errString)) return Ok(errString);

            return Ok("");
        }

        [HttpPut("depItem")]
        public async Task<ActionResult<bool>> EditDeploymentItem(DepItem depItem)
        {
            var strErr  = await _depRepo.EditDepItem(depItem);

            if(!string.IsNullOrEmpty(strErr)) 
                return BadRequest(new ApiException(400, "Failed to update the deployment item", strErr));

            return Ok("the deployment item was successfully updated");
        }

        [HttpPost("insertItemsWithFlight")]
        public async Task<ActionResult<ICollection<DeploymentPendingDto>>> InsertDepItemsWithFlights(ICollection<DepItemWithFlightDto> depItemsWithFlight)
        {
            var dto = await _depRepo.InsertDepItemsWithFlights(depItemsWithFlight, User.GetUsername());

            if(!string.IsNullOrEmpty(dto.ErrorString)) return BadRequest(new ApiException(400, "Failed to insert the dept items", dto.ErrorString));

           return Ok(dto.deploymentPendingDtos);
 
        }

        [HttpDelete("depItem/{depItemId}")]
        public async Task<ActionResult<bool>> DeleteDeploymentItem(int depItemId)
        {
            var strErr = await _depRepo.DeleteDepItem(depItemId);
            if(!string.IsNullOrEmpty(strErr)) return BadRequest(new ApiException(400, "Failed to delete the dep Item", strErr));

            return Ok("the deployment item was deleted successfully");
        }

        
        [HttpDelete("dep/{depId}")]
        public async Task<ActionResult<bool>> DeleteDeployment(int depId)
        {
            var strErr = await _depRepo.DeleteDep(depId);
            if(!string.IsNullOrEmpty(strErr)) return BadRequest(new ApiException(400, "Failed to delete the dep Item", strErr));

            return Ok("the deployment record was deleted successfully");
        }

        [HttpGet("nextDepItemFromCVRef/{cvrefid}")]
        public async Task<ActionResult<int>> GetNextDepItemToAddFomCVRefId(int cvrefid)
        {
            var depItem = await _depRepo.GetNextDepItemToAddFomCVRefId(cvrefid);

            if(depItem==null) return Ok("Failed to generate the Deployment Item object");

            return Ok(depItem);
        }

        [HttpGet("flightdata")]
        public async Task<ActionResult<FlightDetail>> GetFlightData()
        {
            var data = await _depRepo.GetFlightData();
            if(data ==null) return NotFound(new ApiException(400, "Not Found", "No flight data available to retrieve"));
            return Ok(data);
        }

        [HttpGet("candidateflight/{cvrefid}")]
        public async Task<ActionResult<CandidateFlight>> GetCandidateFlightFromCVRefId(int cvrefid)
        {
            return await _depRepo.GetCandidateFlightFromCVRefId(cvrefid);
        }

        [HttpDelete("CandidateFlight/{candidateFlightId}")]
        public async Task<ActionResult<string>> DeleteCandidateFlight(int candidateFlightId)
        {
            var deleted = await _depRepo.DeleteCandidateFlight(candidateFlightId);

            if(deleted=="") return BadRequest(new ApiException(400, "Failure", "Failed to delete the Candidate Flight"));

            return Ok("");
        }

        [HttpPut("CandidateFlight")]
        public async Task<ActionResult<string>> EditCandidateFlight(CandidateFlight candidateFlight)
        {
            var updated = await _depRepo.EditCandidateFlight(candidateFlight);
            return updated;
        }

    }
}