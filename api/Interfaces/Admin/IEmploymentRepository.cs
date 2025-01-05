using api.DTOs.Admin;
using api.Entities.HR;
using api.Helpers;
using api.Params.Admin;

namespace api.Interfaces.Admin
{
    public interface IEmploymentRepository
    {
        Task<int> SaveNewEmployment (Employment employment);
        Task<int> EditEmployment (Employment employment, string Username);
        Task<string> DeleteEmployment (int employmentId);
        Task<PagedList<Employment>> GetEmployments(EmploymentParams empParams);
        Task<Employment> GetOrGenerateEmploymentFromSelDecId(int SelDecisionId);
        Task<string> RegisterOfferAcceptance(ICollection<OfferConclusionDto> dtos, string Username);
    }
}