using api.Entities.Master;
using api.Helpers;
using api.Params.Masters;

namespace api.Interfaces.Masters
{
    public interface IQualificationRepository
    {
         Task<string> AddQualification(string QualificationName);
         Task<string> EditQualification(Qualification qualification);
         Task<string> DeleteQualificationById(int id);
        Task<Qualification> GetQualificationById(int id);
         Task<ICollection<Qualification>> GetQualificationList();
    }
}