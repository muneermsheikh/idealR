using api.Entities.Messages;

namespace api.Interfaces.Messages
{
    public class MessageWithError
    {
        public MessageWithError()
        {
        }


        public List<Message> Messages { get; set; }
        public string ErrorString { get; set; }
        public string Notification { get; set; }
    }
}