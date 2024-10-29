using api.Entities.Messages;

namespace api.DTOs.Admin
{
    public class MessagesWithErrDto
    {
        public string ErrorString { get; set; }
        public ICollection<Message> Messages { get; set; }
        public ICollection<AppId> ApplicationIds { get; set; }
    }

    public class AppId
    {
        public int ApplicationNo { get; set;}
        public int InterviewItemCandidateId { get; set; }
    }
}

