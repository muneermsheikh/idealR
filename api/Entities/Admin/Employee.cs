using System.ComponentModel.DataAnnotations;
using api.Entities.HR;
using AutoMapper.Configuration.Conventions;

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
        public string DisplayName {get; set;}
        public string UserName { get; set; }
        [Required]
        public string Position { get; set; }
        public string Qualification {get; set;}
        [Required]
        public DateTime DateOfBirth { get; set; }
        public string PlaceOfBirth { get; set; }
        public string AadharNo { get; set; }
       [EmailAddress]
        public string Email {get; set;}
        public string PhoneNo { get; set; }
        public string Phone2 { get; set; }
        public DateTime DateOfJoining {get; set;}
        public string Department { get; set; }
        public string Remarks { get; set; }        
        public string Status { get; set; } = "Employed";
        public string Address { get; set; }
        public string Address2 {get; set;}
        public string City { get; set;} 
        public ICollection<HRSkill> HRSkills {get; set;}
        public ICollection<EmployeeOtherSkill> EmployeeOtherSkills{get; set;}
        public ICollection<EmployeeAttachment> EmployeeAttachments {get; set;}  
    }
}