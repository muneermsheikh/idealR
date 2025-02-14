namespace api.DTOs.HR
{
    public class ComposeCallRecordMessageDto
    {
        public string ModeOfAdvise { get; set; }
        public int ProspectiveId { get; set; }
        public int CandidateId {get; set;}
        public string CandidateTitle { get; set; }
        public string RecipientUsername { get; set; }
        public string SenderUsername { get; set; }
        public string CandidateResponse { get; set; }
        public string CandidateName { get; set; }
        public string KnownAs { get; set; }
        public string CandidateUsername { get; set; }
        public string EmailId { get; set; }
        public string PhoneNo { get; set; }
        public string Subject { get; set; }
        public string MessageText { get; set; }
        public DateTime DateComposed  { get; set; }
    }
}