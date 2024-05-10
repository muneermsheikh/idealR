using api.DTOs.Admin;
using api.Entities.Admin;
using api.Helpers;
using api.Params.Admin;

namespace api.Interfaces.Admin
{
    public interface IEmployeeRepository
    {
         Task<bool> EditEmployee(Employee employee);
         Task<bool> DeleteEmployee(Employee employee);
         Task<Employee> AddNewEmployee(Employee employee);
         Task<PagedList<EmployeeBriefDto>> GetEmployeePaginated(EmployeeParams empParams);
         Task<Employee> GetEmployeeFromEmpId (int empId);
    }
}