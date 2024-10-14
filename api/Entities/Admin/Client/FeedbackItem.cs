using System.ComponentModel.DataAnnotations;

namespace api.Entities.Admin.Client
{
    public class FeedbackItem: BaseEntity
    {
        public int CustomerFeedbackId { get; set; }
        public string FeedbackGroup { get; set; }
        public int QuestionNo { get; set; }
        [Required, MaxLength(150)]
        public string Question { get; set; }
        public string Prompt1 { get; set; }
        public string Prompt2 { get; set; }
        public string Prompt3 { get; set; }
        public string Prompt4 { get; set; }
        [MaxLength(15)]
        public string Response { get; set; }
        public string Remarks { get; set; }
    }
}