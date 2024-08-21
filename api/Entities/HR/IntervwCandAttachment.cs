namespace api.Entities.HR
{
    public class IntervwCandAttachment: BaseEntity
    {
        public int IntervwItemCandidateId { get; set; }
        public int CandidateId { get; set; }
        public int AttachmentSizeInBytes { get; set; }
        public string Url { get; set; }
        public string FileName { get; set; }
        public DateTime DateUploaded { get; set; }
    }
}