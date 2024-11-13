using api.Entities.Identity;

namespace api.Entities.Messages
{
    public class Message: BaseEntity
    {
        public Message()
        {
        }


        public int CvRefId { get; set; }
        public int FlightId { get; set; }
        public string MessageType { get; set; }
        //public  int SenderAppUserId { get; set; }
        //public AppUser Sender {get; set;}
        public int SenderId { get; set; }
        public string SenderUsername { get; set; }
        public string SenderEmail { get; set;}
        //public AppUser Recipient { get; set; }
        //public int RecipientAppUserId { get; set; }
        public string RecipientUsername { get; set; }
        public int RecipientId { get; set; }  //Not AppUserId, but Ids like employeeid, contactid, InterviewItemCandidateId
                    //it is recognized based on context - MessageType
        public string RecipientEmail { get; set; }
        public string CCEmail { get; set; }
        public string BCCEmail { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public DateTime MessageComposedOn {get; set;}
        public DateTime MessageSentOn { get; set; } 
        public bool IsMessageSent { get; set; }
        public bool SenderDeleted { get; set; }=false;
        public bool RecipientDeleted { get; set; }=false;
        
    }
}