namespace api.DTOs
{
    public class CandidateDto
    {
        public int Id { get; set; }
        public string Gender { get; set; }
        public string UserName { get; set; }
        public string KnownAs { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public DateTime Created { get; set; }=DateTime.UtcNow;
        public DateTime LastActive { get; set; }=DateTime.UtcNow;
        public string City { get; set; }
        public string Country { get; set; }
        public string Interests { get; set; }
        public string LookingFOR { get; set; }
        public String Introduction { get; set; }

        public List<PhotoDto> photos { get; set; }
        public int Age {get; set;}
    }
}