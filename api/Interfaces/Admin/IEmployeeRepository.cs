using api.DTOs.Admin;
using api.Entities.Admin;
using api.Entities.Master;
using api.Helpers;
using api.Params.Admin;

namespace api.Interfaces.Admin
{
    public interface IEmployeeRepository
    {
         Task<EmployeeWithErrDto> EditEmployee(Employee employee);
         Task<bool> DeleteEmployee(int employeeid);
         Task<EmployeeWithErrDto> AddNewEmployee(Employee employee);
         Task<PagedList<EmployeeBriefDto>> GetEmployeePaginated(EmployeeParams empParams);
         Task<Employee> GetEmployeeFromEmpId (int empId);
         Task<ICollection<EmployeeAttachment>> GetEmployeeAttachments(int employeeid);
         Task<ICollection<EmployeeIdAndKnownAsDto>> GetEmployeeIdAndKnownAs ();
         Task<ICollection<SkillData>> GetSkillDatas();
         Task<ICollection<Industry>> GetIndustriesList();
         Task<string> CheckAadharExists(string aadharno);
         Task<EmployeeAttachmentsWithErrDto> EditEmployeeAttachments(ICollection<EmployeeAttachment> model);
         Task<string> CheckEmailExists(string email);
         Task<string> WriteEmployeeExcelToDB(string fileNameWithPath, string Username);
    }
}