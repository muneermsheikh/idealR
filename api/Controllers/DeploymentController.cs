using api.DTOs.Process;
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
    
        [HttpGet("deployStatusData}")]
        public async Task<ActionResult<Dep>> GetDeploymentStatusData ()
        {
            var data = await _depRepo.GetDeploymentStatusData();
            
            if(data == null) return NotFound(new ApiException(400, "No record found", ""));

            return Ok(data);
        }
    

        [HttpGet("deployments")]
        public async Task<ActionResult<PagedList<DeploymentPendingDto>>> GetDeployments (DeployParams depParams)
        {
            var data = await _depRepo.GetDeployments(depParams);
            
            if(data == null || data.Count ==0) return NotFound(new ApiException(400, "No data found", ""));

              Response.AddPaginationHeader(new PaginationHeader(data.CurrentPage, data.PageSize, 
                data.TotalCount, data.TotalPages));
            
            return Ok(data);
        }
    
        [HttpPost("depItems")]
        public async Task<ActionResult<bool>> AddDeploymentItems(ICollection<DepItemToAddDto> depItemsDto)
        {
            var strErr = await _depRepo.AddDeploymentTransactions(depItemsDto, User.GetUsername());
            if(!string.IsNullOrEmpty(strErr)) return BadRequest(new ApiException(400, "Failed to add the deployment item", strErr));

            return Ok("Succeeded in adding the deployment item");
        }
    
        [HttpPut("depItem")]
        public async Task<ActionResult<bool>> EditDeploymentItem(DepItem depItem)
        {
            var strErr  = await _depRepo.EditDepItem(depItem);

            if(!string.IsNullOrEmpty(strErr)) 
                return BadRequest(new ApiException(400, "Failed to update the deployment item", strErr));

            return Ok("the deployment item was successfully updated");
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

        

    }
}