namespace api.Params.Admin
{
    public class CVRefParams: PaginationParams
    {
        public CVRefParams()
        {
        }


        public int CandidateId { get; set; }
        public string CandidateName { get; set; }
        public int ApplicationNo { get; set; }
        public int ProfessionId { get; set; }
        public int AgentId { get; set; }
        public int CustomerId {get; set;}
        public string CustomerName {get; set;}
        public int OrderId { get; set; }
        public int OrderItemId { get; set; }
        public string RefStatus {get; set;}
    }
}