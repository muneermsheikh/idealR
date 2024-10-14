using api.Entities.Admin;

namespace api.DTOs.Process
{
    public class DepItemsWithCandFightGrpDto
    {
        public ICollection<DepItemToAddDto> DepItemsToAdd {get; set;}
        public CandidateFlightGrp CandFlightToAdd { get; set; }
    }
}