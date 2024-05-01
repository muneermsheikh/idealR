namespace api.DTOs
{
    public class AppDto
    {
        public string UserName { get; set; }
        public string Token { get; set; }
        public string photoUrl { get; set; }
        public string KnownAs {get; set;}
        public string Gender { get; set; }
        public DateOnly DateOfBirth {get; set;}
        public DateOnly Created {get; set;}
        public DateOnly LastActive {get; set;}
        public string City { get; set; }
        public string Country { get; set; }

    }
}