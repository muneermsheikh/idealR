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
        Task<PagedList<AssessmentQBankDto>> GetAssessmentQBanks(AssessmentQBankParams qParams);
        Task<PagedList<AssessmentBank>> GetQBankPaginated(AssessmentQBankParams qParams);
        Task<ICollection<OrderAssessmentItemQ>> GetAssessmentQBankByOrderItemId(int orderitemid);
        Task<bool> UpdateAssessmentBank(AssessmentBank model);
        Task<bool> InsertAssessmentBank(AssessmentBank model);
        Task<AssessmentBankQ> InsertStddQ(AssessmentBankQ stddQ);
        Task<bool> DeleteAssessmentBankQ(int questionId);
        Task<AssessmentBankQ> UpdateStddQ(AssessmentBankQ stddQ);
        Task<AssessmentBank> GetOrCreateCustomAssessmentQsForAProfession(int professionid);
    }
}