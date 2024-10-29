namespace api.Entities.Admin
{
    public class IntervwItem: BaseEntity
    {
        public int IntervwId { get; set; }
        public string InterviewVenue { get; set; }
        public string VenueAddress { get; set; }
        public string VenueAddress2 { get; set; }
        public string VenueCityAndPIN { get; set; }
        public string SiteRepName { get; set; }
        public string SitePhoneNo { get; set; }
        public int OrderItemId { get; set; }
        public int ProfessionId { get; set; }
        public string ProfessionName { get; set;}   
        public string InterviewMode { get; set; }
        public string InterviewerName { get; set; }
        public int EstimatedMinsToInterviewEachCandidate{ get; set; }=25;
        public ICollection<IntervwItemCandidate> InterviewItemCandidates { get; set; }
    }
}