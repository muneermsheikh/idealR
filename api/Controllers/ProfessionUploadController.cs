using System.Net.Http.Headers;
using System.Text.Json;
using api.DTOs.Admin;
using api.Entities.Master;
using api.Errors;
using api.Extensions;
using api.Interfaces.Masters;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    public class ProfessionUploadController : BaseApiController
    {
        private readonly IProfessionRepository _profRepo;
        public ProfessionUploadController(IProfessionRepository profRepo)
        {
            _profRepo = profRepo;
        }

 
        [HttpPost("professionsFromXLS")]
        public async Task<ActionResult<SingleStringDto>> UploadAndUpdateInterviewItem()
        {
            var dtoErr = new SingleStringDto();

            var folderName = Path.Combine("Assets", "TemporaryFiles");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            pathToSave = pathToSave.Replace(@"\\\\", @"\\");          

            try
            {
                //var modelData = JsonSerializer.Deserialize<ICollection<Profession>>(Request.Form["data"],  
                        //new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                
                var files = Request.Form.Files;

                var file = Request.Form.Files[0];

                var memoryStream = new MemoryStream();
            
                if (file.Length==0)  return BadRequest(new ApiException(400, "Bad Request", "File uploaded is empty"));

                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    
                var fullPath = Path.Combine(pathToSave, fileName);        //physical path
                if(System.IO.File.Exists(fullPath)) System.IO.File.Delete(fullPath);
                var dbPath = Path.Combine(folderName, fileName); //you can add this path to a list and then return all dbPaths to the client if required

                using var stream = new FileStream(fullPath, FileMode.Create);
                file.CopyTo(stream);

                var stErr  = await _profRepo.WriteProfessisonExcelToDB(fullPath, User.GetUsername());
                dtoErr.StringValue = stErr.ErrorString;
                return dtoErr;
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "Internal server error" + ex.Message);
            }


        }


    }
}