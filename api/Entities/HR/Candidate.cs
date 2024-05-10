using System.ComponentModel.DataAnnotations;

namespace api.Entities.HR
{
    public class Candidate: BaseEntity
    {
        public Candidate()
        {
        }


        public int ApplicationNo { get; set; }
//names                
        [Required, MaxLength(1)]
        public string Gender {get; set;}="M";
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string FamilyName { get; set; }
        //[Required]         
        public string KnownAs { get; set; }
        public string UserName { get; set; }
        public int? CustomerId { get; set; }
        public string Source {get; set;}
        //[Required]
        public DateOnly? DOB { get; set; }
        public string PpNo { get; set; } 
        public bool Ecnr { get; set; }
        public string Address {get; set;}
        public string City {get; set;}
        public string Country {get; set;}
        public string Pin {get; set;}
        public string Nationality {get; set;}
        [EmailAddress]
        public string Email { get; set; }
    //general
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public int AppUserId {get; set;}
        public string Status {get; set;} = "NotReferred";
        public string Qualifications {get; set;}
        public string PhotoUrl { get; set; }
        public ICollection<UserPhone> UserPhones {get; set;}
        public ICollection<UserProfession> UserProfessions {get; set;}
        public ICollection<UserExp> UserExperiences {get; set;}
        public string FullName {get => FirstName + " " + SecondName + " " + FamilyName;}
    }
}