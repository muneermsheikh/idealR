using System.Net.Http.Headers;
using System.Text.Json;
using api.DTOs.Admin;
using api.DTOs.HR;
using api.DTOs.Process;
using api.Entities.Admin;
using api.Entities.Deployments;
using api.Entities.HR;
using api.Errors;
using api.Extensions;
using api.Interfaces;
using api.Interfaces.Admin;
using api.Interfaces.Deployments;
using api.Interfaces.HR;
using api.Params.HR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace api.Controllers
{
    [Authorize(Policy ="HRMPolicy")]
    public class FileUploadController : BaseApiController
    {
        private readonly DateTime _today = DateTime.UtcNow;
        private readonly IEmployeeRepository _empRepo;
        private readonly ICandidateRepository _candRepo;
        private readonly ICustomerRepository _custRepo;
        private readonly IInterviewRepository _intervwRepo;
        private readonly IDeploymentRepository _depRepo;


        public FileUploadController(
            IEmployeeRepository empRepo, 
            ICandidateRepository candRepo, 
            ICustomerRepository custRepo, 
            IInterviewRepository intervwRepo,
            IDeploymentRepository depRepo)
        {
            _intervwRepo = intervwRepo;
            _depRepo = depRepo;
            _custRepo = custRepo;
            _candRepo = candRepo;
            _empRepo = empRepo;
        }

        [HttpGet("downloadattachmentfile/{attachmentid:int}")]
        public async Task<ActionResult> DownloadFile(int attachmentid)
        {
            //var filePath = $"{candidateid}.txt"; // Here, you should validate the request and the existance of the file.
            //DirectoryInfo source = new DirectoryInfo(SourceDirectory);

            var attachment = await _candRepo.GetUserAttachmentById(attachmentid);
            if(string.IsNullOrEmpty(attachment.Name)) return BadRequest(new ApiException(400, "File Not available", "Attachment File Name not present"));
            if (attachment==null) return NotFound(new ApiException(402, "Not Found","the requested record does not exist"));
            var location = attachment.UploadedLocation;
            if(string.IsNullOrEmpty(location)) location = "D:\\IdealR_\\idealR\\api\\Assets\\Images";
            
            var FileName = location + "\\" + attachment.Name;
            
            if(!System.IO.File.Exists(FileName)) return BadRequest(new ApiException(400, "File not found", "the File " + attachment.Name + " does not exist"));

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(FileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            var bytes = await System.IO.File.ReadAllBytesAsync(FileName);
            var dto = File(bytes, contentType, Path.GetFileName(FileName));
            
            return dto;
        }

        [HttpGet("downloadfile")]
        public async Task<ActionResult> DownloadFileWithName([FromQuery] FullPathParams fParams)
        {
            var fullpath = fParams.FullPath;
            if(string.IsNullOrEmpty(fullpath)) return BadRequest(new ApiException(400, "Params not defined", "Params value not received"));

            if(!System.IO.File.Exists(fullpath)) return BadRequest(new ApiException(400, "Not Found", fullpath + " not found"));

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fullpath, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            
            var bytes = await System.IO.File.ReadAllBytesAsync(fullpath);
            var dto = File(bytes, contentType, Path.GetFileName(fullpath));
            
            return dto;
        }
     
        [HttpGet("deleteattachmentbyfullpath")]
        public async Task<ActionResult> DeleteAttachmentByPath([FromQuery] FullPathParams fParams)
        {
            var fullpath = fParams.FullPath;
            if(string.IsNullOrEmpty(fullpath)) return BadRequest(new ApiException(400, "Params not defined", "Params value not received"));

            if(!System.IO.File.Exists(fullpath)) return BadRequest(new ApiException(400, "Not Found", fullpath + " not found"));

            var deleted = await _depRepo.DeleteDeploymentAttachment(fullpath);

            if (!deleted) return BadRequest(new ApiException(400, "Bad Request", "Failed to delete the file"));

            return Ok(true);
                        
        }
        
        [HttpPost("interviewitem")]
        public async Task<ActionResult<InterviewItemWithErrDto>> UploadAndUpdateInterviewItem()
        {

            var dtoErr = new InterviewItemWithErrDto();

            var folderName = Path.Combine("Assets", "InterviewerComments");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            pathToSave = pathToSave.Replace(@"\\\\", @"\\");          

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

                    var cand = modelData.InterviewItemCandidates.ToList();
                    foreach(var cnd in cand) {
                        if(!string.IsNullOrEmpty(cnd.AttachmentFileNameWithPath) && cnd.AttachmentFileNameWithPath.Contains(fileName)) {
                            cnd.AttachmentFileNameWithPath=fullPath;
                            break;
                        }
                    }
                    
                }

                if(modelData.Id==0) {
                    dtoErr = await _intervwRepo.SaveNewInterviewItem(modelData, User.GetUsername());
                } else {
                    dtoErr = await _intervwRepo.EditInterviewItem(modelData, User.GetUsername());
                }

                return dtoErr;
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "Internal server error" + ex.Message);
            }


        }

        [HttpPost("InterviewerNote")]
        public async Task<ActionResult<string>> UploadInterviewItemCandidateAttachment()
        {

            var folderName = Path.Combine("Assets", "InterviewerComments");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            pathToSave = pathToSave.Replace(@"\\\\", @"\\");          

            try
            {
                var modelData = JsonSerializer.Deserialize<IntervwItemCandidate>(Request.Form["data"],  
                        new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                
                var files = Request.Form.Files;
               
                var memoryStream = new MemoryStream();

                foreach (var file in files) {
                    //var file = files.FirstOrDefault();

                    if (file.Length==0) return "Invalid file upload";

                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    
                    var fullPath = Path.Combine(pathToSave, fileName);        //physical path
                    if(System.IO.File.Exists(fullPath)) System.IO.File.Delete(fullPath);
                    var dbPath = Path.Combine(folderName, fileName); //you can add this path to a list and then return all dbPaths to the client if require

                    using var stream = new FileStream(fullPath, FileMode.Create);
                    file.CopyTo(stream);

                    if(!string.IsNullOrEmpty(modelData.AttachmentFileNameWithPath) 
                        && modelData.AttachmentFileNameWithPath.Contains(fileName)) {
                        modelData.AttachmentFileNameWithPath=fullPath;
                    }
                }
                var retDto = await _intervwRepo.UpdateInterviewCandidateAttachmentFileName(modelData) == null ? "" : "Error in updating attendance data";

                return retDto;
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error" + ex.Message);
            }
        }

        [HttpPost("uploadDepAttachment")]
        public async Task<ActionResult<string>> UploadDeploymentAttachment()
        {
            var folderName = Path.Combine("Assets", "DeploymentAttachments");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            pathToSave = pathToSave.Replace(@"\\\\", @"\\");          

            //try
            //{
                var modelData = JsonSerializer.Deserialize<DepItem>(Request.Form["data"],  
                    new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                
                var files = Request.Form.Files;
               
                var memoryStream = new MemoryStream();

                var file=files[0];

                if (file.Length==0) return null;

                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                
                var fullPath = Path.Combine(pathToSave, fileName);        //physical path
                if(System.IO.File.Exists(fullPath)) System.IO.File.Delete(fullPath);
                var dbPath = Path.Combine(folderName, fileName); //you can add this path to a list and then return all dbPaths to the client if require

                using var stream = new FileStream(fullPath, FileMode.Create);
                file.CopyTo(stream);

                modelData.FullPath = fullPath;
                var dtoErr = await _depRepo.EditDepItem(modelData);

                if(!string.IsNullOrEmpty(dtoErr)) {
                    return BadRequest(new ApiException(400, "Bad Request", dtoErr));
                }
               
                return fullPath;
           /* }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error" + ex.Message);
            }
            */
        }

        [HttpPost("prospectiveXLS"), DisableRequestSizeLimit]
        public  async Task<ActionResult<ReturnStringsDto>> ConvertProspectiveData()
        {
            var dtoErr = new ReturnStringsDto();

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
                        if(FileExtn != ".xlsx") {
                            dtoErr.ErrorString = "Only .XLSX files accepted";
                            return  dtoErr;
                        }
                        
                        var filename=file.FileName[..9].ToLower();

                        var fullPath = Path.Combine(pathToSave, fileName);
                        var dbPath = Path.Combine(folderName, fileName); //you can add this path to a list and then return all dbPaths to the client if require

                        if (System.IO.File.Exists(fullPath)) {
                            System.IO.File.Delete(fullPath);
                            //ErrorString="The file [" + file.FileName + "] already exists at " +  pathToSave + ". Either delete the file or move it, so that the file can be downloaded";
                            //return ErrorString;
                        }

                        using var stream = new FileStream(fullPath, FileMode.Create);
                        file.CopyTo(stream);

                        dtoErr = await _candRepo.WriteProspectiveExcelToDB(fullPath, User.GetUsername());

                        return dtoErr;
                    }
                
                } catch (Exception ex) {
                    throw new Exception(ex.Message);
                } 
            }
            
            if (!string.IsNullOrEmpty(ErrorString)) throw new Exception("Failed to read and update the prospective file");

            return Ok("");
        }

        
        [HttpPost("naukriprospectiveXLS"), DisableRequestSizeLimit]
        public  async Task<ActionResult<ReturnStringsDto>> ConvertProspectiveDataFromNaukri()
        {
            var dtoErr = new ReturnStringsDto();

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
                        if(FileExtn != ".xlsx") {
                            dtoErr.ErrorString = "Only .XLSX files accepted";
                            return  dtoErr;
                        }
                        
                        var filename=file.FileName[..9].ToLower();

                        var fullPath = Path.Combine(pathToSave, fileName);
                        var dbPath = Path.Combine(folderName, fileName); //you can add this path to a list and then return all dbPaths to the client if require

                        if (System.IO.File.Exists(fullPath)) {
                            System.IO.File.Delete(fullPath);
                            //ErrorString="The file [" + file.FileName + "] already exists at " +  pathToSave + ". Either delete the file or move it, so that the file can be downloaded";
                            //return ErrorString;
                        }

                        using var stream = new FileStream(fullPath, FileMode.Create);
                        file.CopyTo(stream);

                        dtoErr = await _candRepo.WriteProspectiveNaukriExcelToDB(fullPath, User.GetUsername());

                        return dtoErr;
                    }
                
                } catch (Exception ex) {
                    throw new Exception(ex.Message);
                } 
            }
            
            if (!string.IsNullOrEmpty(ErrorString)) throw new Exception("Failed to read and update the prospective file");

            return Ok("");
        }
             
        [HttpPost("customerXLS"), DisableRequestSizeLimit]
        public  async Task<ActionResult<string>> ConvertXLSToCustomerData()
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
                        if(FileExtn != ".xlsx") 
                            return "Only '.xlsx' files are accepted";
                        

                        var filename=file.FileName[..9].ToLower();

                        var fullPath = Path.Combine(pathToSave, fileName);
                        var dbPath = Path.Combine(folderName, fileName); //you can add this path to a list and then return all dbPaths to the client if require

                        if (System.IO.File.Exists(fullPath)) System.IO.File.Delete(fullPath);
                        

                        using var stream = new FileStream(fullPath, FileMode.Create);
                        file.CopyTo(stream);

                        var err = await _custRepo.WriteCustomerExcelToDB(fullPath, User.GetUsername());

                        if(!string.IsNullOrEmpty(err)) return  BadRequest(new ApiException(400, err, "Failed to copy the file to database" ) );
                        
                        return Ok("");
                     }
                
                } catch (Exception ex) {
                    throw new Exception(ex.Message);
                } 
            }
            
            if (!string.IsNullOrEmpty(ErrorString)) throw new Exception("Failed to read and update the prospective file");

            return Ok("");
        }

        [HttpPost("candidateXLS"), DisableRequestSizeLimit]
        public  async Task<ActionResult<string>> ConvertCandidateData()
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
                        if(FileExtn != ".xlsx") return "Only '.xlsx' files are accepted";
                        

                        var filename=file.FileName[..9].ToLower();

                        var fullPath = Path.Combine(pathToSave, fileName);
                        var dbPath = Path.Combine(folderName, fileName); //you can add this path to a list and then return all dbPaths to the client if require

                        if (System.IO.File.Exists(fullPath)) {
                            try{
                                // Try to delete the file.
                                System.IO.File.Delete(fullPath);
                            }
                            catch (IOException)
                            {
                                // We could not delete the file.
                                ErrorString="Failed to delete the file [" + file.FileName + "] at " +  pathToSave + ". Either delete the file or move it, so that the file can be downloaded";
                                return ErrorString;

                            }
                        }

                        using var stream = new FileStream(fullPath, FileMode.Create);
                        file.CopyTo(stream);

                        var errString = await _candRepo.WriteCandidateExcelToDB(fullPath, User.GetUsername());

                        if(string.IsNullOrEmpty(errString)) return Ok("");

                        return BadRequest(new ApiException(400, "Failed to copy the file to database", errString ) );
                    }
                
                } catch (Exception ex) {
                    return BadRequest(new ApiException(400, "Error in UploadController", ex.Message));
                } 
            }
            
            if (!string.IsNullOrEmpty(ErrorString)) throw new Exception("Failed to read and update the prospective file");

            return Ok("");
        }
        
        [HttpPost("employeeXLS"), DisableRequestSizeLimit]
        public  async Task<ActionResult<string>> ConvertEmployeeData()
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
                        if(FileExtn != ".xlsx") return "Only '.xlsx' files are accepted";
                        
                        var filename=file.FileName[..9].ToLower();

                        var fullPath = Path.Combine(pathToSave, fileName);
                        var dbPath = Path.Combine(folderName, fileName); //you can add this path to a list and then return all dbPaths to the client if require

                        if (System.IO.File.Exists(fullPath)) System.IO.File.Delete(fullPath);

                        using var stream = new FileStream(fullPath, FileMode.Create);
                        file.CopyTo(stream);

                        var errString = await _empRepo.WriteEmployeeExcelToDB(fullPath, User.GetUsername());

                        if(string.IsNullOrEmpty(errString)) return Ok("");

                        return BadRequest(new ApiException(400, errString, "Failed to copy the file to database" ) );
                    }
                
                } catch (Exception ex) {
                    throw new Exception(ex.Message);
                } 
            }
            
            if (!string.IsNullOrEmpty(ErrorString)) throw new Exception("Failed to read and update the prospective file");

            return Ok("");
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

            if(await _candRepo.AddAndSaveUserAttachments(attachments, User.GetUsername()) == null) 
                return BadRequest(new ApiException(400, "File(s) downloaded, but failed to save data to datanase"));

            return Ok();
        }
 
       
  }
}