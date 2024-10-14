namespace api.DTOs.Messages
{
    public class MessageToSendDto
    {
        public int Id { get; set; }
        public string SenderUsername { get; set; }
        public string RecipientAppUserId { get; set; }
        public string RecipientUsername { get; set; }
        public string ccEmailAddress { get; set; }
        public string Content { get; set; }
        public string Subject { get; set; }
    }
}