using api.Entities.Admin;
using api.Entities.Deployments;

namespace api.DTOs.Process
{
    public class DepItemWithFlightDto
    {
        public DepItem DepItem { get; set; }
        public CandidateFlight candidateFlight { get; set; }    
    }
}