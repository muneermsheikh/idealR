namespace api.Entities.Admin
{
    public class FlightDetail: BaseEntity
    {
        public string FlightNo { get; set; }
        public string AirportOfBoarding { get; set; }
        public string AirportOfDestination { get; set; }
        public string AirportVia { get; set; }
        public string FightNoVia { get; set; }
        public TimeSpan ETD_Boarding { get; set; }
        public TimeSpan ETA_Destination { get; set; }
        public TimeSpan ETA_Via { get; set; }
        public TimeSpan ETD_Via { get; set; }
    }
}