namespace api.Params
{
    public class UserParams: PaginationParams
    {
        public string Username { get; set; }
        public string Gender { get; set; }
        public string KnownAs { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }

}