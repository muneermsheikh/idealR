namespace api.Entities.Admin
{
    public class FlightDetail: BaseEntity
    {
        public string FlightNo { get; set; }
        public string AirportOfBoarding { get; set; }
        public string AirportOfDestination { get; set; }
        public string AirportVia { get; set; }
        public string FightNoVia { get; set; }
        public TimeOnly ETD_Boarding { get; set; }
        public TimeOnly ETA_Destination { get; set; }
        public TimeOnly ETA_Via { get; set; }
        public TimeOnly ETD_Via { get; set; }
    }
}