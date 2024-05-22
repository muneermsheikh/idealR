using api.Entities.HR;
using api.Helpers;
using api.Params.Admin;

namespace api.Interfaces.Admin
{
    public interface IEmploymentRepository
    {
        Task<Employment> GenerateEmploymentObject(int selectionId);
        Task<string> SaveNewEmployment (Employment employment);
        Task<string> EditEmployment (Employment employment);
        Task<string> DeleteEmployment (int employmentId);
        Task<PagedList<Employment>> GetEmployments(EmploymentParams empParams);
    }
}