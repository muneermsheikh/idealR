using Microsoft.AspNetCore.Identity;

namespace api.Entities.Identity
{
    public class AppRole: IdentityRole<int>
    {
        public ICollection<AppUserRole> UserRoles { get; set; }

    }
}