namespace api.Entities.HR
{
    public class UserAttachment: BaseEntity
    {
        public int CandidateId { get; set; }
        public int AppUserId { get; set; }
        public string AttachmentType { get; set; }
        public long Length { get; set; }
        public string Name { get; set; }
        public string UploadedLocation { get; set; }
        public string UploadedbyUserName { get; set; }
        public DateTime UploadedOn { get; set; }
    }
}