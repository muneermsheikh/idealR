using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace api.Entities.Admin.Client
{
    public class CustomerOfficial: BaseEntity
    {
        public int AppUserId { get; set; }
        public int CustomerId { get; set; }
        [Required, MinLength(4), MaxLength(6)]
        public string Gender { get; set; }="Male";
        [Required, MaxLength(4), MinLength(3)]
        public string Title { get; set; }="Mr.";
        [Required, MinLength(4), MaxLength(50)]
        public string OfficialName { get; set; }
        [Required, MinLength(4), MaxLength(25)]
        public string KnownAs {get; set;}
        [Required, MinLength(4), MaxLength(25)]
        public string UserName { get; set; } 
        public string Designation { get; set; }
        public string Divn {get; set;}  
        public string PhoneNo { get; set; }
        [Required, MinLength(10), MaxLength(15)]
        public string Mobile { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Status { get; set; }="Active";
        //public ICollection<CustomerIndustry> CustomerIndustries { get; set; } 
        //CustomerIndustry: AppUserRole
    }
}