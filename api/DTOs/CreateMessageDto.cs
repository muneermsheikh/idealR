
namespace api.DTOs
{
    public class CreateMessageDto
    {
        public string MessageType { get; set; }
        public string RecipientUsername { get; set; }
        public string Content { get; set; }
    }
}