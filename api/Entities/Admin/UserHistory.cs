using System.ComponentModel.DataAnnotations;

namespace api.Entities.Admin
{
    public class UserHtory : BaseEntity
    {
        public UserHtory()
        {
        }
        public string CategoryRef { get; set; }
        public string PersonType { get; set; }
        public int CandidateId { get; set; }
        public string PersonId { get; set; }
        public string Subject { get; set; }
        public string MobileNo {get; set;}
        public string Email { get; set; }
        public string Status {get; set;}
        public DateTime? StatusDate { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Username { get; set; }
        public DateTime? ConcludedOn {get; set;}
        //public ICollection<UserHisryItem> UserHistoItems {get; set;}

    }
}