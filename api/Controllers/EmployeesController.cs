using System.Net.Http.Headers;
using System.Text.Json;
using api.DTOs.Admin;
using api.Entities.Admin;
using api.Entities.Master;
using api.Errors;
using api.Extensions;
using api.Helpers;
using api.Interfaces.Admin;
using api.Params.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Authorize(Policy="HRMPolicy")] //Roles: HR Manager, HR Supervisor, HR Executive, Admin, Admin Manager
    public class EmployeesController : BaseApiController
    {
        private readonly IEmployeeRepository _empRepo;
        public EmployeesController(IEmployeeRepository empRepo)
        {
            _empRepo = empRepo;
        }

        [Authorize(Policy ="AdminPolicy")]      //RequireRoles: Admin
        [HttpGet("employeepaged")]
        public async Task<ActionResult<PagedList<EmployeeBriefDto>>> GetEmployees([FromQuery]EmployeeParams empParams)
        {
            
                var pagedList = await _empRepo.GetEmployeePaginated(empParams);
                if (pagedList == null) return NotFound(new ApiException(404, "Bad Request", "No employees found matching the criteria"));
                
                Response.AddPaginationHeader(new PaginationHeader(pagedList.CurrentPage, 
                    pagedList.PageSize, pagedList.TotalCount, pagedList.TotalPages));
            
                return Ok(pagedList);
        }

        [HttpGet("byId/{id}")]
        public async Task<ActionResult<Employee>> GetEmployeeById(int id)
        {
            //var claim = this.HttpContext.User.Claims;
            var emp = await _empRepo.GetEmployeeFromEmpId(id);
            if (emp == null) return NotFound();
            return Ok(emp);
        }

        [HttpGet("getEmployeeAttachments/{employeeid}")]
        public async Task<ActionResult<ICollection<EmployeeAttachment>>> GetEmployeeAttachments(int employeeid)
        {
            var obj = await _empRepo.GetEmployeeAttachments(employeeid);

            if(obj == null) return BadRequest(new ApiException(400, "Check employee id", "No records found"));

            return Ok(obj);

        }

        [Authorize]
        [HttpGet("idandknownas")]
        public async Task<ActionResult<ICollection<EmployeeIdAndKnownAsDto>>> GetEmployeIdAndKnownAs() 
        {
            var emps = await _empRepo.GetEmployeeIdAndKnownAs();

            return Ok(emps);
        }
        
        [HttpPut]
        public async Task<ActionResult<bool>> EditEmployee(Employee employee)
        {
            var email = employee.Email;

            if (string.IsNullOrEmpty(email)) return BadRequest(new
                ApiException(400, "email Id for employee " + employee.FirstName + " " + employee.SecondName + " " + employee.FamilyName +
                " not provided"));
            
            if(employee.Id == 0) {
                var empWithErr = await _empRepo.AddNewEmployee(employee);
                return string.IsNullOrEmpty(empWithErr.Error) ? Ok(empWithErr.employee) : BadRequest();
            }

            if (string.IsNullOrEmpty(employee.PhoneNo ))
                return BadRequest(new ApiException(400, "Phone No not provided", "Failed to update the employee - The AppUserId exists, but email Id does not match"));
            
            return await _empRepo.EditEmployee(employee) == null 
            ? BadRequest(new ApiException(400, "Bad Request", "Failed to update the employee"))
            : Ok(true);

        }

        [HttpDelete("{employeeid}")]
        public async Task<ActionResult<bool>> DeleteEmployee(int employeeid)
        {
            return await _empRepo.DeleteEmployee(employeeid);
        }

        [HttpPut("updateWithAttachments")]
        public async Task<ActionResult<Employee>> UploadAndUpdateEmployee()
        {

            var folderName = Path.Combine("Assets", "EmployeeAttachments");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            pathToSave = pathToSave.Replace(@"\\\\", @"\\");          

            try
            {
                var modelData = JsonSerializer.Deserialize<Employee>(Request.Form["data"],  
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

                    var cand = modelData.EmployeeAttachments.ToList();
                    foreach(var cnd in cand) {
                        if(!string.IsNullOrEmpty(cnd.FullPath) && cnd.FullPath.Contains(fileName)) {
                            cnd.FullPath=fullPath;
                            break;
                        }
                    }
                    
                }

                var obj = new EmployeeWithErrDto();
                if(modelData.Id==0) {
                    obj = await _empRepo.AddNewEmployee(modelData);
                } else {
                    obj = await _empRepo.EditEmployee(modelData);
                }

                if(!string.IsNullOrEmpty(obj.Error)) {
                    return BadRequest(new ApiException(400, "Bad Request", obj.Error));
                }
               
                return Ok(obj.employee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error" + ex.Message);
            }


        }

        [HttpGet("skilldatas")]
        public async Task<ActionResult<ICollection<SkillData>>> GetSkillDatas()
        {
            var obj = await _empRepo.GetSkillDatas();

            if(obj == null) return BadRequest(new ApiException(400, "Not Found", "No records found in the database for Skill Datas"));

            return Ok(obj);
        }

        [HttpGet("industrylist")]
        public async Task<ActionResult<ICollection<Industry>>> GetIndustriesList()
        {
            var obj = await _empRepo.GetIndustriesList();

            if(obj == null) return BadRequest(new ApiException(400, "Bad Request", "Your instructions did not produce any data"));

            return Ok(obj);
        }
       
        [HttpGet("checkemailexists/{email}")]
        public async Task<ActionResult<string>> CheckEmailExists(string email)
        {
            var empname = await _empRepo.CheckEmailExists(email);

            return Ok(empname);
        }

        
        [HttpGet("checkaadharnoexists/{aadharno}")]
        public async Task<ActionResult<string>> CheckAadharExists(string aadharno)
        {
            var empname = await _empRepo.CheckAadharExists(aadharno);

            return Ok(empname);
        }

        //[HttpPost("updateAndUploadAttachments")]
        public async Task<ActionResult<bool>> UploadAndUpdateAttachment()
        {
            var folderName = Path.Combine("Assets", "EmployeeAttachments");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            pathToSave = pathToSave.Replace(@"\\\\", @"\\");          

            //try
            //{
                var modelData = JsonSerializer.Deserialize<ICollection<EmployeeAttachment>>(Request.Form["data"],  
                    new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                
                var files = Request.Form.Files;

                foreach(var file in files) {
                    var memoryStream = new MemoryStream();
                    if (file.Length==0) return null;
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                
                    var fullPath = Path.Combine(pathToSave, fileName);        //physical path
                    if(System.IO.File.Exists(fullPath)) System.IO.File.Delete(fullPath);
                    var dbPath = Path.Combine(folderName, fileName); //you can add this path to a list and then return all dbPaths to the client if require

                    using var stream = new FileStream(fullPath, FileMode.Create);
                    file.CopyTo(stream);
                    var modelFile = modelData.FirstOrDefault(x => x.FileName == file.FileName);
                    if(modelFile != null) modelFile.FullPath = fullPath;
                }

                var dtoErr = await _empRepo.EditEmployeeAttachments(modelData);

                if(!string.IsNullOrEmpty(dtoErr.Error)) {
                    return BadRequest(new ApiException(400, "Bad Request", dtoErr.Error));
                }
               
                return Ok(true);
           /* }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error" + ex.Message);
            }
            */
        }
    }
}