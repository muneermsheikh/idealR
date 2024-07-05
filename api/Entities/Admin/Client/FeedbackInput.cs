using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace api.Entities.Admin.Client
{
    [NotMapped]
    public class FeedbackInput
    {
        public int FeedbackId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string City  { get; set; }
        public string OfficialName { get; set; }
        public string Designation { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public DateTime DateIssued { get; set; }
        public string CustomerSuggestion { get; set; }
        public string GradeAssessedByClient { get; set; }
        public ICollection<FeedbackInputItem> FeedbackInputItems { get; set; }
    }
}