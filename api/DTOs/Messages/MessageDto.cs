namespace api.DTOs.Messages
{
    public class MessageDto
    {
        public int Id { get; set; }
        public int SenderAppUserId { get; set; }
        public string SenderUsername { get; set; }
        public string RecipientAppUserId { get; set; }
        public string RecipientUsername { get; set; }
        public string Content { get; set; }
    }
}