using System.ComponentModel.DataAnnotations;

namespace api.Entities.Admin
{
    public class UserHistory : BaseEntity
    {
        public UserHistory()
        {
        }
        public string CategoryRef { get; set; }
        public string Gender { get; set; }
        public string Age {get; set;}
        public int ApplicationNo {get; set;}
        public int? CandidateId {get; set;}
        public string ResumeId { get; set; }
        public string City { get; set; }
        public string CandidateName {get; set;}
        public string EmailId { get; set; }
        public string AlternateEmailId { get; set; }
        public string MobileNo {get; set;}
        public string AlternatePhoneNo { get; set; }
        public string Education { get; set; }
        [MaxLength(25)]
        public string WorkExperience { get; set; }
        
        public DateOnly CreatedOn {get; set;}
        public bool Concluded { get; set; }
        public string Status {get; set;}
        public DateOnly StatusDate { get; set; }
        public string UserName { get; set; }
        public DateOnly? ConcludedOn {get; set;}
        public string ConcludedByUsername { get; set; }
        public ICollection<UserHistoryItem> UserHistoryItems {get; set;}

    }
}