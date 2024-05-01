using api.Data;
using api.DTOs;
using api.Entities;
using api.Entities.Identity;
using api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Services
{
    public class UserServices : IUserServices
    {
        private readonly DataContext _context;
        public UserServices(DataContext context)
        {
            _context = context;
        }


        public async Task<bool> UpdateMember(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
            
            return await _context.SaveChangesAsync() > 0;

        }
    }
}