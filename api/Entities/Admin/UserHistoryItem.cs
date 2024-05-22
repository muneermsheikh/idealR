using System.ComponentModel.DataAnnotations;

namespace api.Entities.Admin
{
    public class UserHistoryItem: BaseEntity
    {
        public int UserHistoryId { get; set; }
        public string IncomingOutgoing {get; set;}
        public string PhoneNo {get; set;} 
        [Required]
        public DateOnly DateOfContact { get; set; }
        [Required]
        public string Username {get; set;}
        [Required]
        public string GistOfDiscussions { get; set; }
        public string ContactResult { get; set; }
    }
}