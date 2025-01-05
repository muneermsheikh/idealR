using api.DTOs.Admin;
using api.DTOs.HR;
using api.Entities.Admin.Order;
using api.Entities.Messages;
using api.Interfaces.Messages;


namespace api.Interfaces.HR
{
    public interface IComposeMsgsForCandidates
    {
        MessageWithError AdviseCandidate_OfferAccepted(CandidateAdviseDto candDetail, DateTime TransactionDate);
        Task<MessageWithError> AdviseCandidate_DeploymentStatus(CandidateAdviseDto candDetail, DateTime TransactionDate, string ProcessName);
    }

}