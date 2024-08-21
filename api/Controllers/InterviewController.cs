using System.Net.Http.Headers;
using System.Text.Json;
using api.DTOs.HR;
using api.Entities.Admin;
using api.Entities.HR;
using api.Entities.Identity;
using api.Errors;
using api.Extensions;
using api.Helpers;
using api.Interfaces.HR;
using api.Params.HR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;

namespace api.Controllers
{
    public class InterviewController : BaseApiController
    {
        private readonly IInterviewRepository _repo;
        private readonly UserManager<AppUser> _userManager;
        public InterviewController(IInterviewRepository repo, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _repo = repo;
        }

        [HttpGet("pagedlist")]
        public async Task<ActionResult<InterviewBriefDto>> GetInterviewPagedList([FromQuery]InterviewParams iParams) 
        {
            var lst = await _repo.GetInterviewPagedList(iParams);
            
            if(lst == null) return NotFound(new ApiException(400, "Not Found", "No matching data found"));
            
            Response.AddPaginationHeader(new PaginationHeader(lst.CurrentPage, lst.PageSize, 
                lst.TotalCount, lst.TotalPages));
            
            return Ok(lst);

        }


         [HttpGet("getorgeneratenew/{orderno}")]
        public async Task<ActionResult<Intervw>> GetOrGenerateInterviewR(int orderNo)
        {
            var obj = await _repo.GetOrGenerateInterviewR(orderNo);

            if(!string.IsNullOrEmpty(obj.Error)) return BadRequest(new ApiException(400, "Bad Request", obj.Error));

            return Ok(obj.intervw);
        }

 
        [HttpPost("savenew")]
        public async Task<ActionResult<Intervw>> SaveNewInterview(Intervw interview)
        {
            var obj = await _repo.SaveNewInterview(interview);

            if(!string.IsNullOrEmpty(obj.Error)) return BadRequest(new ApiException(400, "Bad Request", obj.Error));

            return Ok(obj.intervw);
        }
    
        [HttpPost("intervwitem")]
        public async Task<ActionResult<Intervw>> SaveNewInterviewItem(IntervwItem interviewitem)
        {
            var obj = await _repo.SaveNewInterviewItem(interviewitem);

            if(!string.IsNullOrEmpty(obj.Error)) return BadRequest(new ApiException(400, "Bad Request", obj.Error));

            return Ok(obj.intervwItem);
        }


        [HttpPost("intervwitemWithFiles"), DisableRequestSizeLimit]
        public async Task<ActionResult<IntervwItem>> Upload()
        {
            var appuser = await _userManager.FindByNameAsync(User.GetUsername());
            var username = User.GetUsername();
            var folderName = Path.Combine("Assets", "InterviewerComments");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            pathToSave = pathToSave.Replace(@"\\\\", @"\\");          
            var Attachments = new List<IntervwCandAttachment>();

            try
            {
                var modelData = JsonSerializer.Deserialize<IntervwItem>(Request.Form["data"],  
                        new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                
                var files = Request.Form.Files;
               
                var memoryStream = new MemoryStream();

                foreach (var file in files)
                {
                    if (file.Length==0) continue;
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    
                    var fullPath = Path.Combine(pathToSave, fileName);        //physical path
                    if(System.IO.File.Exists(fullPath)) System.IO.File.Delete(fullPath);
                    var dbPath = Path.Combine(folderName, fileName); //you can add this path to a list and then return all dbPaths to the client if require

                    using var stream = new FileStream(fullPath, FileMode.Create);
                    file.CopyTo(stream);
                    var cand = modelData.InterviewItemCandidates.Where(x => x.AttachmentFileNameWithPath == fileName).FirstOrDefault();
                    if(cand != null) cand.AttachmentFileNameWithPath = fullPath;     //change filename to full file name with path
                }

                var dtoErr = new InterviewItemWithErrDto();
                if(modelData.Id==0) {
                    dtoErr = await _repo.SaveNewInterviewItem(modelData);
                } else {
                    dtoErr = await _repo.EditInterviewItem(modelData);
                }

                if(!string.IsNullOrEmpty(dtoErr.Error)) {
                    return BadRequest(new ApiException(400, "Bad Request", dtoErr.Error));
                }
               
                return dtoErr.intervwItem;
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error" + ex.Message);
            }

            
        }
        
        
        [HttpPost("intervwitemWithFiles")]
        public async Task<ActionResult<Intervw>> SaveNewInterviewItemWithFiles(IntervwItem interviewitem)
        {
            var obj = await _repo.SaveNewInterviewItem(interviewitem);

            if(!string.IsNullOrEmpty(obj.Error)) return BadRequest(new ApiException(400, "Bad Request", obj.Error));

            return Ok(obj.intervwItem);
        }

        [HttpPut("intervw")]
        public async Task<ActionResult<Intervw>> UpdateInterview(Intervw intervw)
        {
            var obj = await _repo.EditInterview(intervw);

            if(obj == null) return BadRequest(new ApiException(400, "Bad Request", "Failed to edit the interview"));

            return Ok(obj);
        }

        [HttpPut("intervwitem")]
        public async Task<ActionResult<Intervw>> UpdateInterviewItem(IntervwItem intervwitem)
        {
            var obj = await _repo.EditInterviewItem(intervwitem);

            if(obj == null) return BadRequest(new ApiException(400, "Bad Request", obj.Error));

            return Ok(obj.intervwItem);
        }
        
    }
}