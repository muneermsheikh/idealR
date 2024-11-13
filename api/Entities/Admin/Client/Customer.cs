using System.ComponentModel.DataAnnotations;

namespace api.Entities.Admin.Client
{
    public class Customer: BaseEntity
    {
        [Required]
        public string CustomerType { get; set; }
        [Required, MaxLength(100), MinLength(5)]
        public string CustomerName { get; set; }
        [Required, MaxLength(25), MinLength(5)]
        public string KnownAs { get; set; }
        public string Add { get; set; }
        public string Add2 { get; set; }
        [Required, MaxLength(50), MinLength(4)]
        public string City { get; set; }
        public string Pin { get; set; }
        public string District { get; set; }
        public string State { get; set; }
        public string Country { get; set; }="India";
        [Required, EmailAddress]
        public string Email {get; set;}
        public string Website {get; set;}
        public string Phone {get; set;}
        public string Phone2 {get; set;}
        public DateTime CreatedOn { get; set; }= DateTime.UtcNow;
        public string Introduction { get; set; }
        public string CustomerStatus {get; set;}="Active";
        public bool IsBlackListed { get; set; } = false;
        public ICollection<CustomerIndustry> CustomerIndustries { get; set; }
        public ICollection<CustomerOfficial> CustomerOfficials { get; set; }
        public virtual ICollection<AgencySpecialty> AgencySpecialties {get; set;}
       
    }
}