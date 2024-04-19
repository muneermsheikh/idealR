using api.DTOs;
using api.Entities;
using Microsoft.AspNetCore.Mvc;

namespace api.Interfaces
{
    public interface IUserServices
    {
         Task<bool> UpdateMember(AppUser user);
    }
}