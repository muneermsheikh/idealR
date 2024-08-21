using api.DTOs.HR;
using api.Entities.Admin.Order;
using api.Entities.HR;
using api.Entities.Master;
using api.Helpers;
using api.Params.HR;

namespace api.Interfaces.HR
{
    public interface IAssessmentQBankRepository
    {
         Task<ICollection<Profession>> GetExistingCategoriesInAssessmentQBank();  
        Task<PagedList<AssessmentQBankDto>> GetAssessmentQBanks(AssessmentQBankParams qParams);
        Task<ICollection<AssessmentQBankDto>> GetAssessmentStddQList(AssessmentQBankParams qParams);
        Task<AssessmentQBank> GetAssessmentQsOfACategoryByName(string categoryName);
        Task<ICollection<OrderAssessmentItemQ>> GetAssessmentQBankByOrderItemId(int orderitemid);
        Task<AssessmentQBank> UpdateAssessmentQBank(AssessmentQBank model);
        Task<AssessmentQBank> InsertAssessmentQBank(AssessmentQBank model);
        Task<AssessmentStddQ> InsertStddQ(AssessmentStddQ stddQ);
        Task<bool> DeleteAssessmentQBank(int questionId);
        Task<AssessmentStddQ> UpdateStddQ(AssessmentStddQ stddQ);
    }
}