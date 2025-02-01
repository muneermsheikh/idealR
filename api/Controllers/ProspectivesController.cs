using System.IO;
using System.Net.Http.Headers;
using System.Text.Json;
using api.DTOs.HR;
using api.Entities.Messages;
using api.Extensions;
using api.Helpers;
using api.Interfaces.HR;
using api.Params.HR;
using DocumentFormat.OpenXml.Office2016.Drawing.Command;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Authorize(Policy ="HRMPolicy")]    //RequireRole("HR Manager", "HR Supervisor", "HR Executive", "Admin", "Admin Manager"));
    public class ProspectivesController : BaseApiController
    {
        public IProspectiveCandidatesRepository _ProspectiveRepo { get;set; }
        public ProspectivesController(IProspectiveCandidatesRepository prospectiveRepo)
        {
            _ProspectiveRepo = prospectiveRepo;
        }

        [HttpGet("pagedlist")]
        public async Task<ActionResult<PagedList<ProspectiveBriefDto>>> GetProspectivePagedList([FromQuery]ProspectiveCandidateParams pParams)
        {
            var pagedList = await _ProspectiveRepo.GetProspectivePagedList(pParams);

            if(pagedList.Count ==0) return Ok(null);    //  return BadRequest(new ApiException(400,"Bad Request", "failed to retrieve matching orders"));

            Response.AddPaginationHeader(new PaginationHeader(pagedList.CurrentPage, 
                pagedList.PageSize, pagedList.TotalCount, pagedList.TotalPages));
            
            return Ok(pagedList);
            
        }

       [HttpGet("list/{orderno}/{status}")]
        public async Task<ActionResult<ICollection<ProspectiveBriefDto>>> GetProspectiveList(string orderno, string status)
        {
            var pagedList = await _ProspectiveRepo.GetProspectiveList(orderno, status);

            if(pagedList.Count ==0) return Ok(null);    //  return BadRequest(new ApiException(400,"Bad Request", "failed to retrieve matching orders"));

            return Ok(pagedList);
            
        }

       
        [HttpGet("headers/{status}")]
        public async Task<ActionResult<ICollection<ProspectiveHeaderDto>>> GetProspectiveHeaders(string status)
        {
            var dto = await _ProspectiveRepo.GetProspectiveHeaders(status);

            return Ok(dto);
        }

        [HttpDelete("delete/{prospectiveid}")]
        public async Task<ActionResult<bool>> DeleteProspectiveCandidate(int prospectiveid)
        {
            return await _ProspectiveRepo.DeleteProspectiveCandidate(prospectiveid);

        }


        [HttpPut("convertProspective/{prospectiveid}")]
        public async Task<ActionResult<int>> ConvertProspectiveToCandidate(int prospectiveid)
        {
            var appno = await _ProspectiveRepo.ConvertProspectiveToCandidate(prospectiveid, User.GetUsername());

            return Ok(appno);
        }

        [HttpPost("ComposeMessages")]
        public async Task<ActionResult<ICollection<ComposeCallRecordMsgReturnDto>>> ComposeCallingRecordMessage(ICollection<ComposeCallRecordMessageDto> dtos)
        {
            var dto = await _ProspectiveRepo.ComposeCallRecordMessages(dtos, User.GetUsername());            
            return Ok(dto);                
        }

        [HttpPost("uploadAudioFiles")]
        public async Task<ActionResult<bool>> SaveAudioFiles(ICollection<IFormFile> audiofiles )
        {
           
            var folderName = Path.Combine("Assets", "InterviewerComments");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            pathToSave = pathToSave.Replace(@"\\\\", @"\\");          

            var audiosToSave = new List<AudioMessage>();

            try
            {
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

                    await file.CopyToAsync(memoryStream);
                    var fileBytes = memoryStream.ToArray();
                    var audioToSave = new AudioMessage {
                        FileName = file.Name,
                        ContentType = file.ContentType,
                        RecipientUsername = "RecipientUsername",
                        SenderUsername = "senderusername",
                        MessageText = ""
                    };

                    audiosToSave.Add(audioToSave);
                }
                

                }
            catch (Exception ex)
            {
                
                return StatusCode(500, "Internal server error" + ex.Message);
            }

            return true;
        }
    }
}