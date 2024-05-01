namespace api.DTOs.HR
{
    public class CandidateBriefDto
    {
        public int ApplicationNo { get; set; }
        public string FirstName { get; set; }
        public string FamilyName { get; set; }
        //[Required]         
        public string KnownAs { get; set; }
        public string City {get; set;}
        public string Email { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public string Status {get; set;} = "NotReferred";
    }
}