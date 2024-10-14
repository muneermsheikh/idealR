using api.Entities.Messages;

namespace api.Interfaces.Messages
{
    public class MessageWithError
    {
        public MessageWithError()
        {
        }

        public ICollection<int> CVRefIdsInserted { get; set; }
        public ICollection<Message> Messages { get; set; }=new List<Message>();
        public string ErrorString { get; set; }
        public string Notification { get; set; }
    }
}