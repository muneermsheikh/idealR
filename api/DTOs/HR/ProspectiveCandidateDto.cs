using System.ComponentModel.DataAnnotations;

namespace api.DTOs.HR
{
    public class ProspectiveCandidateDto
    {
        public int Id {get; set;}
        public bool Checked {get; set;}
        [Required, MaxLength(1)]
        public string Gender { get; set; }
        [Required, MaxLength(9)]
        public string CategoryRef {get; set;}
        public int? OrderItemId {get; set;}
        [Required, MaxLength(12)]
        public string Source { get; set; }
        [Required, MaxLength(15)]
        public string ResumeId { get; set; }
        public int PersonId { get; set; }
        [Required]
        public DateTime? Date { get; set; }
        [MaxLength(50), Required]
        public string CandidateName { get; set; }
        [MaxLength(15), Required]
        public string MobileNo { get; set; }
        [MaxLength(15)]
        public string AlternatePhoneNo { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string WorkExperience { get; set; }
        public string Status { get; set; }
        public string UserName {get; set;}
        public string Remarks {get; set;}
    }
}