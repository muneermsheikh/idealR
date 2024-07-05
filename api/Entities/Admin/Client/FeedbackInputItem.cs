using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace api.Entities.Admin.Client
{
    [NotMapped]
    public class FeedbackInputItem
    {
        public string FeedbackGroup { get; set; }
        public int QuestionNo { get; set; }
        [Required, MaxLength(150)]
        public string Question { get; set; }
        public string Prompt1 { get; set; }
        public string Prompt2 { get; set; }
        public string Prompt3 { get; set; }
        public string Prompt4 { get; set; }

        [Required, MaxLength(15)]
        public string Response { get; set; }
        public string Remarks { get; set; }
    }
}