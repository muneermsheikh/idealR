namespace api.Entities.Admin
{
    public class Feedback
    {
        public int Id { get; set; }
        public int CustomerId {get; set;}
        public string CustomerName { get; set; }
        public DateOnly IssuedOn { get; set; }
        public DateOnly? ReceivedOn { get; set; }
        public string HowReceived { get; set; }
        public ICollection<FeedbackItem> FeedbackItems { get; set; }
    }
    public class FeedbackItem
    {
        public int Id { get; set; }
        public int FeedbackId { get; set; }
        public string FeedbackGroup { get; set; }
        public int FeedbackQNo { get; set; }
        public string FeedbackQuestion { get; set; }
        public bool IsMandatory { get; set; }
        public string Response { get; set; }
        public string Remarks { get; set; }
        public string TextForLevel1 { get; set; }
        public string TextForLevel2 { get; set; }
        public string TextForLevel3 { get; set; }
        public string TextForLevel4 { get; set; }
    }
}