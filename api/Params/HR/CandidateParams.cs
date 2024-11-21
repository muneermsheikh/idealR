namespace api.Params.HR
{
    public class CandidateParams: PaginationParams
    {
        public int Id { get; set; }
        public int ApplicationNoFrom{ get; set; }
        public int ApplicationNoUpto{ get; set; }
        public string CandidateName { get; set; }
        public string CategoryName { get; set; }
        public string PassportNo { get; set; }
        public int ProfessionId { get; set; }
        public int AgentId { get; set; }
        public string TypeOfCandidate { get; set; }
    }
}