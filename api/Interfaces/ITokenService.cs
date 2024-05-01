using api.Entities;
using api.Entities.Identity;

namespace api.Interfaces
{
    public interface ITokenService
    {
         Task<string> CreateToken(AppUser user);
    }
}