using api.DTOs.Admin;
using AutoMapper.Configuration.Conventions;

namespace api.DTOs.HR
{
    public class InterviewCandidateInviteDataDto
    {
        public int InterviewItemCandidateId { get; set; }
        public int ApplicationNo { get; set; }
        public int CandidateId { get; set; }
        public string CandidateTitle { get; set; }
        public string CandidateName { get; set; }
        public string ProfessionName { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCity { get; set; }
        public int CustomerId { get; set; }
        public string InterviewVenue { get; set; }
        public string VenueAddress { get; set; }
        public string VenueAddress2 { get; set; }
        public string VenueCityAndPIN { get; set; }
        public string SiteRepName { get; set; }
        public string SitePhoneNo { get; set; }
        public DateTime ScheduledAt { get; set; }
        public AppUserBriefDto userBriefDto { get; set; }
    }
}
            