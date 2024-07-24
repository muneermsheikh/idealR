namespace api.Params
{
    public class FeedbackParams:PaginationParams
    {
        public int  Id { get; set; }
        public int CustomerId { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public DateTime DateIssued { get; set; }
        public DateTime DateReceived { get; set; }
    }

}