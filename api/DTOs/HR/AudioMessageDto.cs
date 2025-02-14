namespace api.HR.DTOs
{
    public class AudioMessageDto
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string CandidateName { get; set; }
        public int ApplicationNo { get; set; }
        public string MessageText { get; set; }
        public DateTime DateComposed { get; set; }
        public DateTime DatePlayedback { get; set; }
        public string FileName { get; set; }
        public int FeedbackReceived { get; set; }=0;
    }
}