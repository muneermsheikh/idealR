namespace api.Params.HR
{
    public class CandidateParams: PaginationParams
    {
        public int Id { get; set; }
        public int ApplicationNo { get; set; }
        public string CandidateName { get; set; }
        public string PassportNo { get; set; }
        public int ProfessionId { get; set; }
        public int AgentId { get; set; }
        public int OrderItemId { get; set; }
    }
}