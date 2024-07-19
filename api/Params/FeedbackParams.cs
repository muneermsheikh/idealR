namespace api.Params
{
    public class FeedbackParams:PaginationParams
    {
        public int  Id { get; set; }
        public int CustomerId { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public DateOnly DateIssued { get; set; }
        public DateOnly DateReceived { get; set; }
    }

}