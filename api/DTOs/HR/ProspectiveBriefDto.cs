namespace api.DTOs.HR
{
    public class ProspectiveBriefDto
    {
        public int Id { get; set; }
        public bool Checked { get; set; }
        public string Source { get; set; }
        public string PersonType { get; set; }
        public string PersonId { get; set; }
        public string Nationality { get; set; }
        public DateTime DateRegistered { get; set; }
        public string CandidateName { get; set; }
        public string CategoryRef { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }
        public string CurrentLocation { get; set; }
        public string WorkExperience { get; set; }
        public string Status { get; set; }
        public string Username { get; set; }
        public DateTime StatusDate { get; set; }
        public string Remarks { get; set; }
    }
}