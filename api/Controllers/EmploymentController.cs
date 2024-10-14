using System.Net.Http.Headers;
using System.Text.Json;
using api.DTOs.Admin;
using api.Entities.HR;
using api.Errors;
using api.Extensions;
using api.Helpers;
using api.Interfaces.Admin;
using api.Params.Admin;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Authorize(Policy ="HRMPolicy")]        //RequireRole("HR Manager", "HR Supervisor", "HR Executive", "Admin", "Admin Manager"));
    public class EmploymentController : BaseApiController
    {
        private readonly IEmploymentRepository _employmentRepo;
        public EmploymentController(IEmploymentRepository employmentRepo)
        {
            _employmentRepo = employmentRepo;
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<Employment>>> GetEmployments([FromQuery] EmploymentParams employmentParams)
        {
            var pagedList  = await _employmentRepo.GetEmployments(employmentParams);
            
            if(pagedList == null) return BadRequest(new ApiException(400, "Not Found Error", "failed to return matching records"));

            Response.AddPaginationHeader(new PaginationHeader(pagedList.CurrentPage, 
                pagedList.PageSize, pagedList.TotalCount, pagedList.TotalPages));
            
            return Ok(pagedList);

        }

        [HttpGet("employmentfromSelId/{selDecId}")]
        public async Task<ActionResult<Employment>> GetEmploymentFromSelId (int selDecId)
        {
            var emp = await _employmentRepo.GetOrGenerateEmploymentFromSelDecId(selDecId);

            if(emp == null) return BadRequest("Failed to get the employment data");

            return Ok(emp);
        }


        [HttpPost("savewithupload")]
        public async Task<ActionResult<string>> UploadAndUpdateInterviewItem()
        {
            var folderName = Path.Combine("Assets", "Employments");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            pathToSave = pathToSave.Replace(@"\\\\", @"\\");          

            try
            {
                var modelData = JsonSerializer.Deserialize<Employment>(Request.Form["data"],  
                        new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                
                var files = Request.Form.Files;
               
                var memoryStream = new MemoryStream();

                var file=files[0];

                if (file.Length > 0) {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    
                    var fullPath = Path.Combine(pathToSave, fileName);        //physical path
                    if(System.IO.File.Exists(fullPath)) {
                        return BadRequest(new ApiException(400, "File already exists", "The file already exists at location " 
                            + pathToSave + ". Please choose another file or delete the file at the existing location"));
                    }

                    var dbPath = Path.Combine(folderName, fileName); 

                    using var stream = new FileStream(fullPath, FileMode.Create);
                    file.CopyTo(stream);
                    modelData.OfferAttachmentFullPath=fullPath;
                }
                var errString="";
                
                if(modelData.Id==0) {
                    errString = await _employmentRepo.SaveNewEmployment(modelData);
                } else {
                    errString = await _employmentRepo.EditEmployment(modelData, User.GetUsername());
                }

                if(!string.IsNullOrEmpty(errString)) {
                    throw new Exception ( errString);
                }
               
                return "";  
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error" + ex.Message);
            }


        }

       
        [HttpDelete("{employmentid}")]
        public async Task<ActionResult<bool>> DeleteEmployment(int employmentid)
        {
            var strErr = await _employmentRepo.DeleteEmployment(employmentid);

            if(!string.IsNullOrEmpty(strErr)) return BadRequest(new ApiException(400,"Error in deleting the employment object", strErr));

            return Ok("Employment objected deleted successfully");
        }

    
    
        [HttpPut("offeraccepted")]
        public async Task<ActionResult<bool>> RegisterOfferAcceptance(ICollection<OfferConclusionDto> dto)
        {
            dto = dto.Where(x => !"acceptedrejected".Contains(x.AcceptedString.ToLower())).ToList();

            if(dto.Count == 0) return BadRequest(new ApiException(400, "invalid accepted String", "accepted value are 'Accepted' or 'Rejected"));
            
            var errorString= await _employmentRepo.RegisterOfferAcceptance(dto, User.GetUsername());

            if(!string.IsNullOrEmpty(errorString)) return BadRequest(errorString);

            return Ok("offer acceptance registered");
        }

    }
}