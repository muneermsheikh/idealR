namespace api.DTOs.Admin
{
    public class UsernameAndEmailDto
    {
        public UsernameAndEmailDto()
        {
        }

        public string KnownAs { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Position { get; set; }
        public int AppUserId { get; set; }
    }
}