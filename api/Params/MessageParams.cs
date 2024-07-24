namespace api.Params
{
    public class MessageParams: PaginationParams
    {
        public string Container { get; set; } = "Inbox";
        public int CvRefId { get; set; }
        public string SenderEmail { get; set; }
        public string RecipientEmail { get; set; }
        public string SenderUsername { get; set; }
        public string RecipientUsername { get; set; }
    }

}