using api.Entities.HR;

namespace api.DTOs.HR
{
    public class CandidateBriefDto
    {
        public int Id { get; set; }
        public int ApplicationNo { get; set; }
        public string FullName {get; set;}
        public string KnownAs { get; set; }
        public string ReferredByName { get; set; }
        public string City {get; set;}
        public string Email { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public ICollection<UserProfession> userProfessions { get; set; }

        public string Status {get; set;} = "NotReferred";
    }
}