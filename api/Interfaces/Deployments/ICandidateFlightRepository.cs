using api.DTOs.Process;
using api.Entities.Admin;
using api.Entities.Identity;
using api.Entities.Messages;
using api.Helpers;
using api.Interfaces.Messages;
using api.Params.Deployments;

namespace api.Interfaces.Deployments
{
    public interface ICandidateFlightRepository
    {
        Task<PagedList<CandidateFlightGrp>> GetAllCandidatesFlightsGrp(CandidateFlightParams parms);
        Task<string> EditCandidateFlight(CandidateFlightGrp candidateFlight);
        Task<string> DeleteCandidateFlight(int CandidateFlightId);
        Task<CandidateFlightGrp> InsertCandidateFlights(CandidateFlightGrp flight);
        Task<ICollection<int>> GetCandidateFlightIdsCVRefIds(ICollection<int> cvRefIds);
        Task<ICollection<CandidateFlightItem>> GetCandidateFlightItems(int flightid);
        Task<DepPendingDtoWithErr> InsertDepItemsWithCandFlightItems(DepItemsWithCandFightGrpDto deps, AppUser user);

        Task<string> GetOrGenerateTravelAdviseMessage(int flightid);
    }
}