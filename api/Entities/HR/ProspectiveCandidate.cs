using System.ComponentModel.DataAnnotations;

namespace api.Entities.HR
{
    public class ProspectiveCandidate: BaseEntity
    {
         public string Gender { get; set; }
        [MaxLength(12)]
        public string Source { get; set; }
        public DateTime? DateRegistered { get; set; }
        [MaxLength(35)]
        public string CategoryRef {get; set;}
        public int OrderItemId {get; set;}
        public int ProfessionId { get; set; }
        public string ProfessionName { get; set; }
        public string PersonType { get; set; }        
        [Required, MaxLength(15)]
        public string PersonId { get; set; }
        public string Nationality {get; set;}="Indian";
        [MaxLength(50)]
        public string ResumeTitle {get; set;}
        [MaxLength(50), Required]
        public string CandidateName { get; set; }
        public DateTime DateOfBirth { get; set; }
        [MaxLength(10)]
        public string Age { get; set; }
        [MaxLength(15), Required]
        public string PhoneNo { get; set; }
        [MaxLength(15)]
        public string AlternateNumber { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [EmailAddress]
        public string AlternateEmail { get; set; }
        public string CurrentLocation { get; set; }
        public string Address {get; set;}
        [Required]
        public string Education {get; set;}
        public string Ctc {get; set;}
        public string WorkExperience { get; set; }
        public string Status { get; set; }
        public DateTime? StatusDate { get; set; }
        public string Username { get; set; }
    }
}