using api.Entities.HR;
using api.Entities.Identity;

namespace api.DTOs.Admin
{
    public class AppUserNameAndRolesDto
    {
        public int AppUserId { get; set; }
        public string KnownAs { get; set; }
        public string Username {get; set;}
        public ICollection<string> Roles { get; set;}
     
    }
}

