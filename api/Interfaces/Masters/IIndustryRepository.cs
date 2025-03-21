using api.DTOs.Admin;
using api.Entities.Master;
using api.Helpers;
using api.Params.Masters;

namespace api.Interfaces.Masters
{
    public interface IIndustryRepository
    {
        Task<ReturnIndustryDto> AddIndustry(Industry industry);
        Task<string> EditIndustry(Industry industry);
        Task<string> DeleteIndustry(string IndustryName);
        Task<string> DeleteIndustryById(int id);
        Task<Industry> GetIndustryFromId(int id);
        Task<ICollection<Industry>> GetIndustriesList();
        Task<PagedList<Industry>> GetIndustries(IndustryParams indParams);    
    }
}