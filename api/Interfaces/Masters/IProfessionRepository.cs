using api.DTOs.Admin;
using api.Entities.Master;
using api.Helpers;
using api.Params.Masters;

namespace api.Interfaces.Masters
{
    public interface IProfessionRepository
    {
            Task<Profession> AddProfession(Profession profession);
            Task<string> EditProfession(Profession profession);
            Task<string> DeleteProfession(string ProfessionName);
            Task<string> DeleteProfessionById(int professionid);
            Task<PagedList<Profession>> GetProfessions(ProfessionParams pParams);
            Task<ICollection<Profession>> GetProfessionList();
            Task<Profession> GetProfessionById(int professionid);
            //Task<string> ReadProfessionExcelAndWriteToDB(string FileNameWithPath, string Username);
            Task<ReturnStringsDto> WriteProfessisonExcelToDB(string fileNameWithPath, string Username);
            
    }
}