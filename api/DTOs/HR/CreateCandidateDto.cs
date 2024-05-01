using System.ComponentModel.DataAnnotations;
using api.Entities.HR;

namespace api.DTOs.HR
{
    public class CreateCandidateDto
    {
        public int ApplicationNo { get; set; }
        [Required, MaxLength(1)]
        public string Gender {get; set;}="M";
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string FamilyName { get; set; }
        public string KnownAs { get; set; }
        public string UserName { get; set; }
        public int? CustomerId { get; set; }
        public string Source {get; set;}
        public DateTime? DOB { get; set; }
        public string PpNo { get; set; } 
        public bool Ecnr { get; set; }
        public string Address {get; set;}
        public string City {get; set;}
        public string Pin {get; set;}
        public string Nationality {get; set;}
        [EmailAddress]
        public string Email { get; set; }
        public int AppUserId {get; set;}
        public string Status {get; set;} = "NotReferred";
        public string Qualifications {get; set;}
        public string PhotoUrl { get; set; }
        public ICollection<UserPhone> UserPhones {get; set;}
        public ICollection<UserProfession> UserProfessions {get; set;}
        public ICollection<UserExp> UserExperiences {get; set;}
    }
}