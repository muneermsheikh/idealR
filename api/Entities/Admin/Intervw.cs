namespace api.Entities.Admin
{
    public class Intervw: BaseEntity
    {
        public int OrderId { get; set; }
        public int OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public DateTime InterviewDateFrom { get; set; }
        public DateTime InterviewDateUpto { get; set; }
        public string InterviewVenues { get; set; }
        public string InterviewStatus { get; set; }
        public ICollection<IntervwItem> InterviewItems { get; set; }
    }
}