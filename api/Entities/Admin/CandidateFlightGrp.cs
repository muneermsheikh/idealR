using System.ComponentModel.DataAnnotations;
using api.Entities.Deployments;

namespace api.Entities.Admin
{
    public class CandidateFlightGrp: BaseEntity
    {
        public DateTime DateOfFlight { get; set; }
        public int OrderNo { get; set; }
        public int CustomerId {get; set;}
        public string AirlineName { get; set; }
        public string FlightNo { get; set; }
        public string AirportOfBoarding { get; set; }
        public string AirportOfDestination { get; set; }
        public DateTime ETD_Boarding { get; set; }
        public DateTime ETA_Destination { get; set; }
        [MaxLength(18)]
        public string ETD_BoardingString { get; set; }
        [MaxLength(18)]
        public string ETA_DestinationString { get; set; }
        
        public string AirportVia { get; set; }
        public string FightNoVia { get; set; }
        public DateTime? ETA_Via { get; set; }
        public DateTime? ETD_Via { get; set; }

        [MaxLength(18)]
        public string ETA_ViaString { get; set; }
        [MaxLength(18)]
        public string ETD_ViaString { get; set; }

        [MaxLength(250)]
        public string FullPath {get; set;}
        public ICollection<CandidateFlightItem> CandidateFlightItems { get; set; }
        
    }
}