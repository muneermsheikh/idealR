using System.ComponentModel.DataAnnotations;
using api.Entities.Deployments;

namespace api.Entities.Admin
{
    public class CandidateFlight: BaseEntity
    {
        public int DepId { get; set; }
        public int DepItemId { get; set; }
        public int CvRefId { get; set; }
        public int ApplicationNo { get; set; }
        public string CandidateName { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCity { get; set; }
        public DateTime DateOfFlight { get; set; }
        
        public string FlightNo { get; set; }
        public string AirportOfBoarding { get; set; }
        public string AirportOfDestination { get; set; }
        public DateTime ETD_Boarding { get; set; }
        public DateTime ETA_Destination { get; set; }
        public string AirportVia { get; set; }
        public string FightNoVia { get; set; }
        public DateTime? ETA_Via { get; set; }
        public DateTime? ETD_Via { get; set; }
        [MaxLength(250)]
        public string FullPath {get; set;}
    }
}