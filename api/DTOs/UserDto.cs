namespace api.DTOs
{
    public class UserDto
    {
        public string UserName { get; set; }
        public string Employer { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Token { get; set; }
        //public string photoUrl { get; set; }
        public string KnownAs {get; set;}
        public string Gender { get; set; }
        public RACredentialsDto RACredentials { get; set; }
        
    }

    public class RACredentialsDto
    {
        public bool FinanceCredential { get; set; }
        public bool VisaCredential { get; set; }
        public bool SMSCredential { get; set; }
        public bool AudioMessageCredential { get; set; }

    }
}