using api.DTOs.Admin;
using api.Entities.Admin;
using api.Errors;
using api.Extensions;
using api.Helpers;
using api.Interfaces.Admin;
using api.Params.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    
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

        [HttpGet("idandknownas")]
        public async Task<ActionResult<ICollection<EmployeeIdAndKnownAsDto>>> GetEmployeIdAndKnownAs() 
        {
            var emps = await _empRepo.GetEmployeeIdAndKnownAs();

            return Ok(emps);
        }
        
        [Authorize(Policy = "HRMPolicy")]
        [HttpPut]
        public async Task<ActionResult<bool>> EditEmployee(Employee employee)
        {
            var email = employee.OfficialEmail;

            if (string.IsNullOrEmpty(email)) return BadRequest(new
                ApiException(400, "email Id for employee " + employee.FirstName + " " + employee.SecondName + " " + employee.FamilyName +
                " not provided"));

            if (string.IsNullOrEmpty(employee.OfficialPhoneNo ))
                return BadRequest(new ApiException(400, "Phone No not provided", "Failed to update the employee - The AppUserId exists, but email Id does not match"));
            
            return await _empRepo.EditEmployee(employee) == null 
            ? BadRequest(new ApiException(400, "Bad Request", "Failed to update the employee"))
            : Ok();

        }

        [Authorize(Policy = "HRMPolicy")]
        [HttpDelete("{employeeid}")]
        public async Task<ActionResult<bool>> DeleteEmployee(int employeeid)
        {
            return await _empRepo.DeleteEmployee(employeeid);
        }

        [Authorize(Policy = "HRMPolicy")]
        [HttpPost("employee")]
        public async Task<ActionResult<Employee>> AddNewEmployee(EmployeeToAddDto employee)
          {
               
               var emp = await _empRepo.AddNewEmployee(employee);

               if (emp == null) return BadRequest(new ApiException(402, "Bad Request", "Failed to add the employee"));
               
               return emp;
          }

          
    }
}