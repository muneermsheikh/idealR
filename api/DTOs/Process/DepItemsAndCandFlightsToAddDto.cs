using api.Entities.Admin;

namespace api.DTOs.Process
{
    public class DepItemsAndCandFlightsToAddDto
    {
        public ICollection<DepItemToAddDto> DepItemsToAdd {get; set;}
        public ICollection<CandidateFlight> CandFlightsToAdd { get; set; }
    }
}