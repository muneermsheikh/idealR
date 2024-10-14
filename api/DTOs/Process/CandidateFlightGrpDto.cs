namespace api.DTOs.Process
{
    public class CandidateFlightGrpDto
    {
        public int Id { get; set; }
        public DateTime DateOfFlight { get; set; }
        public int OrderNo { get; set; }
        public string AirlineName { get; set; }
        public string FlightNo { get; set; }
        public string AirportOfBoarding { get; set; }
        public string AirportOfDestination { get; set; }
        public DateTime ETD_Boarding { get; set; }
        public DateTime ETA_Destination { get; set; }
        public string AirportVia { get; set; }
        public string FightNoVia { get; set; }
        public DateTime? ETA_Via { get; set; }
        public DateTime? ETD_Via { get; set; }
        public int CountOfPax { get; set; }
    }
}