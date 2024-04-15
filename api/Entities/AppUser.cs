using System.ComponentModel.DataAnnotations;

namespace api.Entities
{
    public class AppUser
    {
        public int Id { get; set; }
        public string Gender { get; set; }
        public string UserName { get; set; }
        public byte[] PasswordHash {get; set;}
        public byte[] PasswordSalt {get; set;}
        public string KnownAs { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public DateTime Created { get; set; }=DateTime.UtcNow;
        public DateTime LastActive { get; set; }=DateTime.UtcNow;
        public string City { get; set; }
        public string Country { get; set; }
        public List<Photo> photos { get; set; }=new();
    }
}