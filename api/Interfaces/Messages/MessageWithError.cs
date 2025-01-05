using api.Entities.Messages;

namespace api.Interfaces.Messages
{
    public class MessageWithError
    {
        public MessageWithError()
        {
        }

        public ICollection<int> CvRefIdsInserted { get; set; }=new List<int>();
        public ICollection<Message> Messages { get; set; }=new List<Message>();
        public string ErrorString { get; set; }
        public string Notification { get; set; }
    }
}