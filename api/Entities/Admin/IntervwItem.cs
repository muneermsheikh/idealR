namespace api.Entities.Admin
{
    public class IntervwItem: BaseEntity
    {
        public int IntervwId { get; set; }
        public string InterviewVenue { get; set; }
        public int OrderItemId { get; set; }
        public int ProfessionId { get; set; }
        public string ProfessionName { get; set;}   
        public string InterviewMode { get; set; }
        public string InterviewerName { get; set; }
        public int EstimatedMinsToInterviewEachCandidate{ get; set; }=25;
        public ICollection<IntervwItemCandidate> InterviewItemCandidates { get; set; }
    }
}