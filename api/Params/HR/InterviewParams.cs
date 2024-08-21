namespace api.Params.HR
{
    public class InterviewParams: PaginationParams
    {

        public int CustomerId { get; set; }
        public int CandidateId { get; set; }
        public int OrderId { get; set; }
        public int OrderNo { get; set; }
        public string InterviewStatus { get; set; }
    }
}