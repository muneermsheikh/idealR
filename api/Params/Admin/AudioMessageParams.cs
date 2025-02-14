namespace api.Params.Admin
{
    public class AudioMessageParams: PaginationParams
    {
        public string RecipientUsername { get; set; }
        public string SenderUsername { get; set; }
        public string CandidateName { get; set; }
        public int ApplicationNo { get; set; }
        public DateTime DateComposed { get; set; }
        public int FeedbackReceived { get; set; }
    }
}