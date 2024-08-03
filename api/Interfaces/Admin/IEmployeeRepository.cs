using api.DTOs.Admin;
using api.Entities.Admin;
using api.Helpers;
using api.Params.Admin;

namespace api.Interfaces.Admin
{
    public interface IEmployeeRepository
    {
         Task<Employee> EditEmployee(Employee employee);
         Task<bool> DeleteEmployee(int employeeid);
         Task<Employee> AddNewEmployee(EmployeeToAddDto employee);
         Task<PagedList<EmployeeBriefDto>> GetEmployeePaginated(EmployeeParams empParams);
         Task<Employee> GetEmployeeFromEmpId (int empId);
         Task<ICollection<EmployeeIdAndKnownAsDto>> GetEmployeeIdAndKnownAs ();
         Task<string> WriteEmployeeExcelToDB(string fileNameWithPath, string Username);
    }
}