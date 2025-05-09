using Microsoft.AspNetCore.Identity;
using api.Entities.Messages;

namespace api.Entities.Identity
{
    public class AppUser: IdentityUser<int>
    {
        public string Gender { get; set; }
        public string KnownAs { get; set; }
        public string UserType { get; set; }
        public string Employer { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Position { get; set; }
        //public string LookingFor { get; set; }
        //public String Introduction { get; set; }

        //public List<Photo> photos { get; set; }=new();
        //public List<UserLike> LikedByUsers { get; set; }
        //public List<UserLike> LikedUsers { get; set; }
        //public List<Message> MessagesSent {get; set;}
        //public List<Message> MessagesReceived {get; set;}
        public ICollection<AppUserRole> UserRoles { get; set; }
    }
}