using System.Net.Http.Headers;
using System.Text.Json;
using api.Entities.HR;
using api.Errors;
using api.Extensions;
using api.Interfaces;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using SQLitePCL;

namespace api.Controllers
{
    public class FileUploadController : BaseApiController
    {
        private readonly ICandidateRepository _candRepo;
        private readonly DateOnly _today = DateOnly.FromDateTime(DateTime.UtcNow);
        public FileUploadController(ICandidateRepository candRepo)
        {
            _candRepo = candRepo;
        }

        [HttpGet("downloadattachmentfile/{attachmentid:int}")]
        public async Task<ActionResult> DownloadFile(int attachmentid)
        {
            //var filePath = $"{candidateid}.txt"; // Here, you should validate the request and the existance of the file.
            //DirectoryInfo source = new DirectoryInfo(SourceDirectory);

            var attachment = await _candRepo.GetUserAttachmentById(attachmentid);
            if (attachment==null) return NotFound(new ApiException(402, "Not Found","the requested record does not exist"));

            var FileName=attachment.UploadedLocation + '/' + attachment.Name;
            if(string.IsNullOrEmpty(FileName)) return BadRequest("No URL found in the attachment record");

            if(!System.IO.File.Exists(FileName)) return NotFound("the File " + attachment.Name + " does not exist");

            //var FileName = "D:\\User Profile\\My Documents\\comments on emigration act 2021.docx";

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(FileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            //if (!File.Exists(FileName)) return false;

            var bytes = await System.IO.File.ReadAllBytesAsync(FileName);
            return File(bytes, contentType, Path.GetFileName(FileName));
        }

     
        [HttpGet("downloadprospectivefile/{prospectiveid:int}")]
        public async Task<ActionResult> DownloadProspectiveFile(int prospectiveid)
        {
            var FileName = "D:\\User Profile\\My Documents\\comments on emigration act 2021.docx";

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(FileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            //if (!File.Exists(FileName)) return false;

            var bytes = await System.IO.File.ReadAllBytesAsync(FileName);
            return File(bytes, contentType, Path.GetFileName(FileName));
        }

        [HttpPost("prospectiveorapplications"), DisableRequestSizeLimit]
        public  ActionResult ConvertProspectiveData()
        {
            //check for uploaded files
            var files = Request.Form.Files;

            string ErrorString="";
       
            if(files.Count > 0) 
            {
                try{
                    var folderName = Path.Combine("Assets", "Images");
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                    foreach (var file in files)
                    {
                        if(file.Length == 0) continue;

                        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        var FileExtn = Path.GetExtension(file.FileName);
                        if(FileExtn != ".xlsx") continue;

                        var filename=file.FileName.Substring(0,9).ToLower();

                        if(filename != "prospecti" && filename !="applicati") continue;

                        var fullPath = Path.Combine(pathToSave, fileName);
                        var dbPath = Path.Combine(folderName, fileName); //you can add this path to a list and then return all dbPaths to the client if require

                        if (System.IO.File.Exists(fullPath)) continue;

                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }
                        
                        /*if(filename=="prospecti") {
                            ErrorString = await _excelService.ReadAndSaveProspectiveXLToDb(fullPath, loggedInUser.loggedInEmployeeId, loggedInUser.DisplayName);
                        } else {
                            ErrorString = await _excelService.ReadAndSaveApplicationsXLToDb(fullPath, loggedInUser.loggedInEmployeeId, loggedInUser.DisplayName);
                        }
                        */
                    }
                
                } catch (Exception ex) {
                    throw new Exception(ex.Message);
                } 
            }
            
            if (!string.IsNullOrEmpty(ErrorString)) throw new Exception("Failed to read and update the prospective file");

            return Ok();
        }
        
       
        private static bool IsNumeric(string input) {
            return int.TryParse(input, out int test);
        }
        
        public async Task<ActionResult<ICollection<UserAttachment>>> UploadFile()
        {
            var attachments = new List<UserAttachment>();
            try
            {
                var modelData = JsonSerializer.Deserialize<UserAttachment>(Request.Form["data"],  
                    new JsonSerializerOptions {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase});
                
                if(modelData== null) return BadRequest(new ApiException(400, "bad Request", "attachment object not passed"));
                
                var appno = await _candRepo.GetApplicationNoFromCandidateId(modelData.CandidateId);
                
                var folderName = Path.Combine("Assets", "Images");

                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                pathToSave = pathToSave.Replace(@"\\\\", @"\\");          

                var files = Request.Form.Files;
                foreach (var file in files)
                {
                    if(file.Length == 0) continue;

                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    fileName = appno.ToString() + "-" + fileName;
                    if(System.IO.File.Exists(pathToSave + @"\" + fileName)) continue;
                    
                    //the filename syntax is: application No + "-" + filename
                    if(!fileName.Contains(appno.ToString().Trim())) fileName = appno.ToString().Trim() + "-" + fileName;
                    
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);

                    using var stream = new FileStream(fullPath, FileMode.Create);
                    file.CopyTo(stream);

                    var appuserid = await _candRepo.GetAppUserIdOfCandidate(appno);
                    
                    var attach = new UserAttachment {
                         AppUserId = appuserid, AttachmentType = modelData.AttachmentType, CandidateId=modelData.CandidateId,
                         Length = file.Length/1024, Name=file.Name, UploadedbyUserName = User.GetUsername(),
                          UploadedLocation=pathToSave, UploadedOn = _today                        
                    };

                    attachments.Add(attach);
                    
                }
            } catch (Exception ex) {
                return  BadRequest(new ApiException(400, "failed to upload the file(s)", ex.Message));
            }

            if(await _candRepo.AddUserAttachments(attachments, User.GetUsername()) == null) 
                return BadRequest(new ApiException(400, "File(s) downloaded, but failed to save data to datanase"));

            return Ok();
        }

       /*[HttpGet("LoadDocument")]
        public LoadedDocument LoadDocument()
        {
            string documentName = "invoice.docx";
            LoadedDocument document = new LoadedDocument()
            {
            DocumentData = Convert.ToBase64String(
                System.IO.File.ReadAllBytes("App_Data/" + documentName)),
            DocumentName = documentName
            };

            return document;
        }
        */

        private static string GetContentType(string path)
        {
            var provider = new FileExtensionContentTypeProvider();
            string contentType;
                        
            if (!provider.TryGetContentType(path, out contentType))
            {
                contentType = "application/octet-stream";
            }
                
            return contentType;
        }

    }
}