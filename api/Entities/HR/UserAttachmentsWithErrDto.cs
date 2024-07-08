namespace api.Entities.HR
{
    public class UserAttachmentsWithErrDto
    {
        public ICollection<UserAttachment> UserAttachments { get; set; }
        public string ErrorString { get; set; }
    }
}