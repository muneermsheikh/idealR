

namespace api.DTOs.HR
{
    public class InterviewBriefDto
    {
        public int Id { get; set; }
        public bool Checked { get; set; }
        public int OrderId {get; set;}
        public int OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public int  CustomerId {get; set;}
        public string CustomerName { get; set; }
        public string InterviewVenues { get; set; }
        public DateTime InterviewDateFrom { get; set; }
        public DateTime InterviewDateUpto { get; set; }
        public string InterviewStatus {get; set;}
    }
}