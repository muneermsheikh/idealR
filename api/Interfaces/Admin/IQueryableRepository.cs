using api.DTOs.Admin;
using api.Params.Admin;

namespace api.Interfaces.Admin
{
    public interface IQueryableRepository
    {
         Task<IQueryable<CVRefDto>> GetCVReDtoQueryable(CVRefParams refParams);
         IQueryable<CustomerAndOfficialsDto> GetCustomerAndOfficialQueryable(int customerId, string divn);
    }
}