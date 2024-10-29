using System.ComponentModel.DataAnnotations;
using api.Entities.HR;

namespace api.DTOs
{
    public class RegisterDto
    {
        public int AppUserId { get; set; }
        [Required] public string Username { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string FamilyName { get; set; }
        [Required] public string KnownAs { get; set; }
        public string ReferredByName { get; set; }
        public string Email { get; set; }
        [Required] public string Gender { get; set; }
        public string AadharNo { get; set; }
        public string PpNo { get; set; }
        public bool Ecnr { get; set; }
        public DateTime? DateOfBirth { get; set; } // Note this must be optional or the required validator will not work
        public string Address { get; set; }
        [Required] public string City { get; set; }
        public string Pin { get; set; }
        public string Country { get; set; } = "India";
        /*[Required, StringLength(8, MinimumLength = 4)]
        public string Password { get; set; }*/
        public string Source { get; set; }
        
        public ICollection<UserPhone> UserPhones { get; set; }
        public ICollection<UserProfession> UserProfessions { get; set; }
        public ICollection<UserQualification> UserQualifications { get; set; }
        public ICollection<UserAttachment> UserAttachments { get; set; }
        public bool NotificationDesired { get; set; }
        public string Nationality { get; set; }
        public int CompanyId { get; set; }
        public DateTime DOB { get; set; }
        public string Role { get; set; }
    }

}
