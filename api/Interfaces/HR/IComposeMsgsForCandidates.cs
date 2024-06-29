using api.DTOs.Admin;
using api.DTOs.HR;
using api.Entities.Admin.Order;
using api.Entities.Messages;
using api.Interfaces.Messages;


namespace api.Interfaces.HR
{
    public interface IComposeMsgsForCandidates
    {
        MessageWithError AdviseCandidate_MedicallyFit(CandidateAdviseDto candDetail, DateTime TransactionDate);
        MessageWithError AdviseCandidate_VisaIssued(CandidateAdviseDto candDetail, DateTime TransactionDate); 
        MessageWithError AdviseCandidate_VisaRejected(CandidateAdviseDto candDetail, DateTime TransactionDate);
        MessageWithError AdviseCandidate_MedicallyUnfit(CandidateAdviseDto candDetail,DateTime TransactionDate);
        Task<MessageWithError> AdviseCandidate_TicketBooked(CandidateAdviseDto candDetail, DateTime TransactionDate);
        MessageWithError AdviseCandidate_EmigrationCleared(CandidateAdviseDto candDetail, DateTime TransactionDate);
        MessageWithError AdviseCandidate_OfferAccepted(CandidateAdviseDto candDetail, DateTime TransactionDate);
    }

}