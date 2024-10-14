namespace api.Params.Deployments
{
    public class CandidateFlightParams: PaginationParams
    {
        public DateTime DateOfFlight { get; set; }
        public string AirlineName { get; set; }
        public string FlightNo { get; set; }
        public string AirportOfBoarding { get; set; }
        public int OrderNo { get; set; }
        public int OrderItemId { get; set; }
    }
}
