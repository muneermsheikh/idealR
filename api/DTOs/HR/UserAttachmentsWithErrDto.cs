using api.Entities.HR;

namespace api.DTOs.HR
{
    public class UserAttachmentsWithErrDto
    {
        public ICollection<UserAttachment> UserAttachments {get; set;}
        public string ErrorString { get; set; }
    }
}