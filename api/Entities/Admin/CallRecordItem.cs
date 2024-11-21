using System.ComponentModel.DataAnnotations;

namespace api.Entities.Admin
{
    public class CallRecordItem: BaseEntity
    {
        public int CallRecordId { get; set; }
        [Required, MaxLength(3)]
        public string IncomingOutgoing {get; set;}
        public string PhoneNo {get; set;} 
        public string Email { get; set; }
        [Required]
        public DateTime DateOfContact { get; set; }
        public string Username {get; set;}
        public string GistOfDiscussions { get; set; }
        [Required, MaxLength(50)]
        public string ContactResult { get; set; }
        public string NextAction { get; set; }
        public DateTime NextActionOn { get; set; }
        public string AdvisoryBy { get; set; }
    }
}