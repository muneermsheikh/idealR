using System.Runtime.CompilerServices;
using api.DTOs.Admin;
using api.Entities.Admin;
using api.Helpers;
using api.Interfaces.Admin;
using api.Params.Admin;

namespace api.Data.Repositories.Master
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly DataContext _context;
        public EmployeeRepository(DataContext context)
        {
            _context = context;
        }


        public Task<Employee> AddNewEmployee(Employee employee)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteEmployee(Employee employee)
        {
            throw new NotImplementedException();
        }

        public Task<bool> EditEmployee(Employee employee)
        {
            throw new NotImplementedException();
        }

        public async Task<Employee> GetEmployeeFromEmpId(int empId)
        {
            return await _context.Employees.FindAsync(empId);
        }

        public Task<PagedList<EmployeeBriefDto>> GetEmployeePaginated(EmployeeParams empParams)
        {
            throw new NotImplementedException();
        }

    }
}