using System.ComponentModel.DataAnnotations;
using DocumentFormat.OpenXml.Wordprocessing;

namespace api.Entities.Admin
{
    public class FlightDetail: BaseEntity
    {
        [MaxLength(15)]
        public string Airline { get; set; }
        [MaxLength(10)]
        public string FlightNo { get; set; }
        [MaxLength(20)]
        public string AirportOfBoarding { get; set; }
        [MaxLength(20)]
        public string AirportOfDestination { get; set; }
        [MaxLength(20)]
        public string AirportVia { get; set; }
        [MaxLength(20)]
        public string AirlineVia { get; set; }
        [MaxLength(20)]
        public string FightNoVia { get; set; }
        [MaxLength(10)]
        public string ETD_Boarding { get; set; }
        [MaxLength(10)]
        public string ETA_Destination { get; set; }
        [MaxLength(10)]
        public string ETA_Via { get; set; }
        [MaxLength(10)]
        public string ETD_Via { get; set; }
    }
}