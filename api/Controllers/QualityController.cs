using api.Controllers;
using api.DTOs.Admin;
using api.Errors;
using api.Extensions;
using api.Helpers;
using api.Interfaces.Quality;
using api.Params.Objectives;
using DocumentFormat.OpenXml.Math;
using Microsoft.AspNetCore.Mvc;

namespace api
{
    public class QualityController : BaseApiController
    {
        private readonly IQualityRepository _qRepo;
        public QualityController(IQualityRepository qRepo)
        {
            _qRepo = qRepo;
        }

        [HttpGet("medicalObjectives")]
        public async Task<ActionResult<PagedList<MedicalObjective>>> GetMedicalObjectiveAsync([FromQuery]MedicalParams mParams)
        {
            var pagedList = await _qRepo.GetMedicalObjectives(mParams);

            if(pagedList.Count ==0 || pagedList == null) return BadRequest("No Objectives data available during the dates mentioned");

            Response.AddPaginationHeader(new PaginationHeader(pagedList.CurrentPage, 
                pagedList.PageSize, pagedList.TotalCount, pagedList.TotalPages));
            
            return Ok(pagedList);
        }


        [HttpGet("hrObjectives")]
        public async Task<ActionResult<PagedList<HRObjective>>> GetHRObjectiveAsync([FromQuery]MedicalParams mParams)
        {
            var pagedList = await _qRepo.GetHRObjectives(mParams);

            if(pagedList.Count ==0 || pagedList == null) return BadRequest("No HR Objectives data available during the dates mentioned");

            Response.AddPaginationHeader(new PaginationHeader(pagedList.CurrentPage, 
                pagedList.PageSize, pagedList.TotalCount, pagedList.TotalPages));
            
            return Ok(pagedList);
        }
    
        [HttpPut("pendingHRObjectives")]
        public async Task<ActionResult<PagedList<HRObjective>>> MarkHRTasksAsCompleted(MedicalParams medParams)
        {
            var pagedList = await _qRepo.GetPendingHRTasks(medParams);
            if(pagedList.Count ==0 || pagedList == null) return BadRequest("No Objectives data available during the dates mentioned");

            Response.AddPaginationHeader(new PaginationHeader(pagedList.CurrentPage, 
                pagedList.PageSize, pagedList.TotalCount, pagedList.TotalPages));
            
            return Ok(pagedList);
        }
        
        [HttpPut("updateHRTaskStatus")]
        public async Task<ActionResult<bool>> MarkHRTasksAsCompleted(ICollection<int> Ids)
        {
            return await _qRepo.SetHRTasksAsCompleted(Ids, User.GetUsername());
        }

        [HttpPost("assignToHRExecs")]
        public async Task<ActionResult<string>> AssignHRExecTasks(ICollection<int> orderItemIds)
        {
            var strErr = await _qRepo.AssignTasksToHRExecs(orderItemIds, User.GetUsername());
            
            if(string.IsNullOrEmpty(strErr)) return BadRequest(new ApiException(404, "failed to create tasks", strErr));

            return Ok(strErr);
        }
    }


}