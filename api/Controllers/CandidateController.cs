using System.Data.Common;
using System.Net.Http.Headers;
using System.Text.Json;
using api.DTOs;
using api.DTOs.HR;
using api.Entities.HR;
using api.Entities.Identity;
using api.Errors;
using api.Extensions;
using api.Helpers;
using api.Interfaces;
using api.Params.HR;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;

namespace api.Controllers
{
    [Authorize(Policy="HRMPolicy")]      //Roles: HR Manager, HR Supervisor, HR Executive, Admin, Admin Manager
    public class CandidateController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly ICandidateRepository _candidateRepo;
        private readonly ITokenService _tokenService;
        private readonly ILogger<CandidateController> _logger;

        private readonly DateTime _today = DateTime.UtcNow;
        public CandidateController(ICandidateRepository candidateRepo, UserManager<AppUser> userManager, 
            IMapper mapper, ITokenService tokenService, ILogger<CandidateController> logger)
        {
            _tokenService = tokenService;
            _logger = logger;

            _candidateRepo = candidateRepo;
            _userManager = userManager;
            _mapper = mapper;
        }

        
        [HttpGet]
        public async Task<ActionResult<PagedList<CandidateBriefDto>>> GetCandidates([FromQuery]CandidateParams candidateParams)
        { 
            var candidates = await _candidateRepo.GetCandidates(candidateParams);

            if(candidates == null) return NotFound("No matching candidates found");

            Response.AddPaginationHeader(new PaginationHeader(candidates.CurrentPage, 
                candidates.PageSize, candidates.TotalCount, candidates.TotalPages));
            
            return Ok(candidates);

        }

        [HttpGet("cvsAvailable")]
        public async Task<ActionResult<PagedList<cvsAvailableDto>>> GetAvailableCandidates([FromQuery]CandidateParams candidateParams)
        { 
            var candidates = await _candidateRepo.GetAvailableCandidates(candidateParams);

            if(candidates == null) return NotFound("No matching candidates found");

            Response.AddPaginationHeader(new PaginationHeader(candidates.CurrentPage, candidates.PageSize, 
                candidates.TotalCount, candidates.TotalPages));
            
            return Ok(candidates);

        }

        [HttpGet("briefbyparams")]
        public async Task<ActionResult<CandidateBriefDto>> GetCandidateBriefFromParams([FromQuery]CandidateParams candParams)
        {
            
            return await _candidateRepo.GetCandidateBriefFromParams(candParams);
        }

        [HttpGet("byid/{candidateid}")]
        public async Task<ActionResult<Candidate>> GetCandidateByParams(int candidateid)
        { 
            var candidate = await _candidateRepo.GetCandidateById(candidateid);

            if(candidate == null) return NotFound("Not found");

            return Ok(candidate);

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

        
        [HttpGet("emailexists")]
        public async Task<AppUser> AppUserFromEmail(string email)
        {
            var appuser = await _userManager.FindByEmailAsync(email);
            if (appuser == null) return null;
            return appuser;

        }
          
        [HttpGet("ppexists/{ppnumber}")]
        public async Task<ActionResult<bool>> CheckPPNumberExistsAsync([FromQuery] string ppnumber)
        {
            return await _candidateRepo.CheckPPExists(ppnumber);
        }
        
        
        [HttpGet("aadahrexists/{aadharno}")]
        public async Task<ActionResult<bool>> CheckAadharNoExistsAsync([FromQuery] string aadharno)
        {
            return await _candidateRepo.AadharNoExists(aadharno);
        }

        [HttpPost("RegisterByUpload"), DisableRequestSizeLimit]
        public async Task<ActionResult<ApiReturnDto>> Upload()
        {
            var appuser = await _userManager.FindByNameAsync(User.GetUsername());
            var username = User.GetUsername();
            var userattachments = new List<UserAttachment>();
            var ApplicationNoString = await _candidateRepo.GetNextApplicationNo();

            try
            {
                var modelData = JsonSerializer.Deserialize<RegisterDto>(Request.Form["data"],   //THE CNDIDATE OBJECT
                        new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                
                var files = Request.Form.Files;
                 //candidate is created, now check for any file attachment to download and save.
                //file in params will have the posted file
                
                var folderName = Path.Combine("Assets", "Images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                pathToSave = pathToSave.Replace(@"\\\\", @"\\");          

                var memoryStream = new MemoryStream();

                foreach (var file in files)
                {
                    if (file.Length==0) continue;
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    
                    var fullPath = Path.Combine(pathToSave, ApplicationNoString + "-" + fileName);        //physical path
                    if(System.IO.File.Exists(fullPath)) continue;
                    var dbPath = Path.Combine(folderName, fileName); //you can add this path to a list and then return all dbPaths to the client if require

                    using var stream = new FileStream(fullPath, FileMode.Create);
                    file.CopyTo(stream);

                    /*var attachment = new UserAttachment{ AppUserId = appuser.Id, AttachmentType = "", 
                        CandidateId = candidateCreated.Id, Name = ApplicationNoString + "-" + fileName, 
                        UploadedbyUserName = username, 
                        Length = Convert.ToInt32(file.Length/1024), 
                        UploadedLocation = pathToSave, UploadedOn = _today };
                    */
                    var attachmt = modelData.UserAttachments.Where(x => x.Name == file.FileName).FirstOrDefault();
                    if(attachmt != null) {
                        attachmt.AppUserId=appuser.Id;
                        attachmt.UploadedLocation=pathToSave;
                        attachmt.Length=file.Length/2048;
                        attachmt.UploadedOn = _today;
                        attachmt.Name = fileName;
                        attachmt.UploadedbyUserName=User.GetUsername();
                    }
                    
                    
                }

                //create the candidate object frist, because the file attachment to save later needs candidate.Id
                var CandidateDtoWithErr = await CreateAppUserAndCandidate(modelData, username);
                if(!string.IsNullOrEmpty(CandidateDtoWithErr.ErrorMessage)) {
                        return BadRequest(new ApiException(400, "Bad Request", CandidateDtoWithErr.ErrorMessage));
                }
               
                return CandidateDtoWithErr;
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error" + ex.Message);
            }

            
        }
        //registers individuals. For customers and vendors, it will register the users for customers that exist
        //the IFormFile collection has following prefixes to filenames:
        //pp: passport; ph: photo, ec: educational certificates, qc: qualification certificates
        private async Task<ApiReturnDto> CreateAppUserAndCandidate(RegisterDto registerDto, string Username) 
        {
            var tempPassword="TempPa#sword0";

            var dtoToReturn = new ApiReturnDto();

            //attempt to create AppUser
            var existingAppUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingAppUser != null) {
                dtoToReturn.ErrorMessage = "Email address is in use";
                return dtoToReturn; }

            if (!string.IsNullOrEmpty(registerDto.AadharNo) &&  await _candidateRepo.AadharNoExists(registerDto.AadharNo)) {
                dtoToReturn.ErrorMessage = "Aadhar Number is in use";
                return dtoToReturn; }

            if (!string.IsNullOrEmpty(registerDto.PpNo)) {
                if(await _candidateRepo.CheckPPExists(registerDto.PpNo)) {
                    dtoToReturn.ErrorMessage = "Passport Number is in use";
                    return dtoToReturn; 
                }
            }

            if (registerDto.UserPhones != null && registerDto.UserPhones.Count > 0)
            {
                foreach (var ph in registerDto.UserPhones) {
                    if (string.IsNullOrEmpty(ph.MobileNo)) {
                        dtoToReturn.ErrorMessage = "Mobile Number cannot be blank";
                        return dtoToReturn; 
                    } 
                }
            }
            
            //create and save AppUser 
            var user = new AppUser();

            user = new AppUser
            {
                KnownAs = registerDto.KnownAs,
                Gender = registerDto.Gender,
                PhoneNumber = registerDto.UserPhones.Where(x => x.IsMain).Select(x => x.MobileNo).FirstOrDefault(),
                Email = registerDto.Email,
                UserName = registerDto.Email
            };
            
            var result = await _userManager.CreateAsync(user, tempPassword);
            if (!result.Succeeded) {
                dtoToReturn.ErrorMessage = result.Errors.Select(x => x.Description).FirstOrDefault();
                return dtoToReturn;
            }

            var roleResult = await _userManager.AddToRoleAsync(user, "Candidate");

            var userDtoToReturn = new UserDto
            {
                KnownAs = user.KnownAs,
                Token = await _tokenService.CreateToken(user), 
                UserName = Username, 
                Gender = registerDto.Gender
                //Email = user.Email
            };

            registerDto.AppUserId = user.Id;   //   **TODO** check of any existing number in UserPhones 

            // create candidate entity
            var candidateCreated = await _candidateRepo.CreateCandidateAsync(registerDto, Username);
            if(candidateCreated == null) {
                //failed, delete appuser created
                await _userManager.DeleteAsync(user);
                dtoToReturn.ErrorMessage="Failed To Create the candidate";
                return dtoToReturn;
            } else {
                //user.loggedInEmployeeId=candidateCreated.Id;
                await _userManager.UpdateAsync(user);
            }
    
        
            dtoToReturn.ReturnInt = candidateCreated.ApplicationNo;
            return  dtoToReturn;
        }
          
        [HttpPut("updatecandidatewithfiles"), DisableRequestSizeLimit]
          public async Task<ActionResult<ApiReturnDto>> EditCandidateWithUpload()
          {
               var dtoToReturn = new ApiReturnDto();

               string applicationno="";

               var userattachmentlist = new List<UserAttachment>();

               try
               {
                    var modelData = JsonSerializer.Deserialize<Candidate>(Request.Form["data"],  
                         new JsonSerializerOptions {
                         PropertyNamingPolicy = JsonNamingPolicy.CamelCase});
                    
                    //var modelData = Request.Form["data"];
                    //CompanyId=null DOB null, remove entityaddress, remove userPassports,
                    if(!await _candidateRepo.UpdateCandidate(modelData)) {
                        dtoToReturn.ErrorMessage = "Failed to update candidate obkect";
                        return BadRequest(new ApiException(404, "Bad Request", "Failed to update candidate object"));

                    }
                    applicationno = modelData.ApplicationNo.ToString();
                    var folderName = Path.Combine("Assets", "Images");
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                    pathToSave = pathToSave.Replace(@"\\\\", @"\\");          
                    //D:\idealr_\IdealR\api\Assets\Images
                    //D:\\idealr_\\IdealR\api\\Assets\\Images 
                    //var attachmentTypes = modelData.UserAttachments;
                    var files = Request.Form.Files;
                    foreach (var file in files)
                    {
                         if(file.Length == 0) continue;
                         //files uploaded but not prsent in existing file attachments are the new files to be uploaded, and hence also to be added in USERaTTACHMENTS
                         //The userAttachments could already be having files uploaded earlier, and existing in the images folder, those are to be 
                         //ignored and not added to the _context.UserAttachments object
                        
                        var filenameWOPath = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                         //var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                         var fileName = applicationno + "-" + filenameWOPath;
                         if(System.IO.File.Exists(pathToSave + @"\" + fileName)) continue;
                         
                         //the filename syntax is: application No + "-" + filename
                         if(!fileName.Contains(applicationno.ToString().Trim())) fileName = applicationno.ToString().Trim() + "-" + fileName;
                         
                         var fullPath = Path.Combine(pathToSave, fileName);
                         var dbPath = Path.Combine(folderName, fileName);

                        using var stream = new FileStream(fullPath, FileMode.Create);
                        file.CopyTo(stream);

                        var attach = new UserAttachment {
                            CandidateId = modelData.Id, AppUserId = modelData.AppUserId,
                                Length=filenameWOPath.Length/1024,
                                Name=fileName, UploadedbyUserName=User.GetUsername(), 
                                UploadedLocation=pathToSave, UploadedOn=_today
                        };
                        
                        userattachmentlist.Add(attach);
                    }

                    //stream contains only new files uploaded.  ADd to it old files that
                    //existed, else they would be considered as not existing in the model
                    //and deleted in the following procedure
                    foreach(var item in modelData.UserAttachments) {
                        if(item.Name[..applicationno.Length] == applicationno ) userattachmentlist.Add(item);
                    }
                    var attachmentsUpdated = await _candidateRepo.AddAndSaveUserAttachments(userattachmentlist, User.GetUsername());                   
                    //candidateObject.UserAttachments=attachmentsUpdated;
                    dtoToReturn.ReturnInt=Convert.ToInt32(applicationno);
                    if(string.IsNullOrEmpty(dtoToReturn.ErrorMessage)) dtoToReturn.ErrorMessage="";
                    return Ok(dtoToReturn);
               }
               catch (Exception ex)
               {
                    return StatusCode(500, "Internal server error" + ex.Message);
               }
          }
        
        [HttpPost("userattachment")]
        public async Task<ActionResult<UserAttachment>> AddUserAttachment(UserAttachment userattachment)
        {
            var attachmentList = new List<UserAttachment>();
            attachmentList.Add(userattachment);

            var attachment = await _candidateRepo.AddAndSaveUserAttachments(attachmentList, User.GetUsername());
            if(attachment != null) return Ok(attachment.FirstOrDefault());

            return null;
        }
    
        [HttpPut("userattachments")]
        public async Task<ActionResult<UserAttachment>> UpdateAttachment(UserAttachment userattachment)
        {
            var attachmentlist = new List<UserAttachment>();
            attachmentlist.Add(userattachment);

            var obj = await _candidateRepo.UpdateCandidateAttachments(attachmentlist);

            if(obj != null) return obj.UserAttachments.FirstOrDefault();

            return BadRequest(new ApiException(400, "Bad Request", "failed to update the user attachment"));
        }
    
        [HttpGet("userattachments/{candidateid}")]
        public async Task<ActionResult<ICollection<UserAttachment>>> GetUserAttachmentsFromCandidateId(int candidateid)
        {
            var objs = await _candidateRepo.GetUserAttachmentByCandidateId(candidateid);

            return Ok(objs);
        }

        [HttpDelete("userattachment/{attachmentid}")]
        public async Task<ActionResult<bool>> DeleteUserAttachmentById(int attachmentid)
        {
            return await _candidateRepo.DeleteUserAttachment(attachmentid);
        }
    
        
    }

    
}