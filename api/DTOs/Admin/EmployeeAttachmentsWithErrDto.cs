using api.Entities.Admin;

namespace api.DTOs.Admin
{
    public class EmployeeAttachmentsWithErrDto
    {
        public string Error { get; set; }
        public ICollection<EmployeeAttachment>  employeeAttachments { get; set; }
    }
}