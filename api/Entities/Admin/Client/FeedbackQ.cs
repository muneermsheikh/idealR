namespace api.Entities.Admin.Client
{
    public class FeedbackQ: BaseEntity
    {
        public string FeedbackGroup { get; set; }
        public int QuestionNo { get; set; }
        public string Question { get; set; }
        public string Prompt1 { get; set; }
        public string Prompt2 { get; set; }
        public string Prompt3 { get; set; }
        public string Prompt4 { get; set; }
    }
}