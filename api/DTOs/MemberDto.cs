namespace api.DTOs
{
    public class MemberDto
    {
        public int Id { get; set; }
        public string Gender { get; set; }
        public string UserName { get; set; }
        public string KnownAs { get; set; }
        public int  Age { get; set; }
        public DateTime DateOfBirth { get; set; }
        //public DateTime Created { get; set; }=DateTime.UtcNow;
        // <summary>
        //public DateTime LastActive { get; set; }=DateTime.UtcNow;
        // </summary>
        public string City { get; set; }
        public string Country { get; set; }
        public string Position { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public ICollection<string> Roles { get; set; }
        
    }
}