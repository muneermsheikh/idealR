using System.ComponentModel.DataAnnotations;

namespace api.Entities.Admin
{
    public class RADetail: BaseEntity
    {
        public int RAId { get; set; }
        [MaxLength(100)]
        public string RAName {get; set;}
        [MaxLength(150)]
        public string Address { get; set; }
        [MaxLength(150)]
        public string Address2 {get; set;}
        [MaxLength(50)]
        public string City {get; set;}
        [MaxLength(10)]
        public string PIN {get; set;}
        [MaxLength(16)]
        public string Phoneno {get; set;}
        [MaxLength(16)]
        public string Mobile {get; set;}
        public ICollection<RAOfficial> RAOfficials {get; set;}
    }
}