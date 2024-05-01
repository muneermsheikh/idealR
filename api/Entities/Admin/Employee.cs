using System.ComponentModel.DataAnnotations;
using api.Entities.HR;

namespace api.Entities.Admin
{
    public class Employee: BaseEntity
    {
        public int AppUserId { get; set; }
        public string Gender { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string FamilyName { get; set; }
        [Required]         
        public string KnownAs { get; set; }
        public string UserName { get; set; }
        [Required]
        public string Position { get; set; }
        public string Qualifications {get; set;}
        [Required]
        public DateTime DateOfBirth { get; set; }
        public string PlaceOfBirth { get; set; }
        public string AadharNo { get; set; }
        public string Nationality {get; set;} = "Indian";
        [EmailAddress]
        public string Email {get; set;}
        public DateTime DateOfJoining {get; set;}
        public string Department { get; set; }
        public string Remarks { get; set; }        
        public string EmployeeAddress {get; set;}
        public string EmployeePhone {get; set;}
        public string EmployeePhone2 {get; set;}
        public string EmployeeQualifications {get; set;}
        public string Status { get; set; } = "Employed";
        public string Address { get; set; }
        public string Address2 {get; set;}
        public string City { get; set;} 
        public string Country { get; set;}
        public ICollection<HRSkill> HRSkills {get; set;}
        public ICollection<OtherSkill> OtherSkills{get; set;}
    }
}