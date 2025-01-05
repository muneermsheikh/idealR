using System.ComponentModel.DataAnnotations;

namespace api.Entities.Admin
{
    

    public class RAOfficial: BaseEntity
    {
        public int RADetailId { get; set; }
        [MaxLength(100)]
        public string OfficialName { get; set; }
        [MaxLength(50)]
        public string Position { get; set; }
        [MaxLength(50)]
        public string Username { get; set; }
        [MaxLength(16)]
        public string MobileNo { get; set; }
        [EmailAddress]
        public string OfficialEmailId { get; set; }
        [MaxLength(15)]
        public ICollection<string> Roles { get; set; } = new List<string>();
    }
}