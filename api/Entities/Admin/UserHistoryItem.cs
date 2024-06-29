using System.ComponentModel.DataAnnotations;

namespace api.Entities.Admin
{
    public class CallRecodddddrdItem: BaseEntity
    {
        public int CallRecordId { get; set; }
        public string IncomingOutgoing {get; set;}
        public string PhoneNo {get; set;} 
        [Required]
        public DateTime DateOfContact { get; set; }
        [Required]
        public string Username {get; set;}
        [Required]
        public string GistOfDiscussions { get; set; }
        public string ContactResult { get; set; }
        public string NextAction { get; set; }
        public DateTime NextActionOn { get; set; }
        public bool ComposeEmail { get; set; }
        public bool ComposeSMS { get; set; }
    }
}