using System.ComponentModel.DataAnnotations;

namespace api.Entities.HR
{
    public class Candidate: BaseEntity
    {
        public int ApplicationNo { get; set; }
//names                
        [Required, MaxLength(1)]
        public string Gender {get; set;}="M";
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string FamilyName { get; set; }
        //[Required]         
        public string KnownAs { get; set; }
        public int? CustomerId { get; set; }
        public string Source {get; set;}
        //[Required]
        public DateTime? DOB { get; set; }
        public string PpNo { get; set; } 
        public string AadharNo { get; set; }
        public string Ecnr { get; set; }
        public string Address {get; set;}
        public string City {get; set;}
        public string Pin {get; set;}
        public string Country { get; set; } = "India";
        public string Nationality {get; set;}="Indian";
        [EmailAddress]
        public string Email { get; set; }
    //general
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public int AppUserId {get; set;}
        public string Status {get; set;} = "NotReferred";
        public string Qualifications {get; set;}
        public ICollection<UserPhone> UserPhones {get; set;}
        public ICollection<UserProfession> UserProfessions {get; set;}
        public ICollection<UserExp> UserExperiences {get; set;}
        public ICollection<UserAttachment> UserAttachments {get; set;}
        public ICollection<UserQualification> UserQualifications {get; set;}
        public bool NotificationDesired {get; set;}
        public string Username { get; set; }
        public string FullName {get => FirstName + " " + SecondName + " " + FamilyName;}
    }
}