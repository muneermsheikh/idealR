using api.Entities.Admin;

namespace api.DTOs.Admin
{
    public class EmployeeWithErrDto
    {
        public string Error { get; set; }
        public Employee employee { get; set; }
    }
}