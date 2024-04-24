using System.Diagnostics.Metrics;
using System.Text.Json;
using api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class Seed
    {
        public static async Task SeedUsers (UserManager<AppUser> userManager, 
            RoleManager<AppRole> roleManager) 
        {

            if (await userManager.Users.AnyAsync()) return;

            var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");

            var options = new JsonSerializerOptions{PropertyNameCaseInsensitive = true};

            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

            var roles = new List<AppRole> {
                new AppRole {Name = "Candidate"},
                new AppRole {Name = "Employee"},
                new AppRole {Name = "Client"},
                new AppRole {Name = "Admin"},
                new AppRole {Name = "HR Manager"},
                new AppRole {Name = "HR Supervisor"},
                new AppRole {Name = "HR Executive"},
                new AppRole {Name = "Asst HR Executive"},
                new AppRole {Name = "Accounts Manager"},
                new AppRole {Name = "Finance Manager"},
                new AppRole {Name = "Cashier"},
                new AppRole {Name = "Accountant"},
                new AppRole {Name = "Document Controller-Admin"},
                new AppRole {Name = "Document Controller-Processing"},
                new AppRole {Name = "Processing Manager"},
                new AppRole {Name = "Admin Manager"},
                new AppRole {Name = "Receptionist"},
                new AppRole {Name = "Marketing Manager"},
                new AppRole {Name = "Design Assessment Questions"},
                new AppRole {Name = "Register Selections and Rejections"},
                new AppRole {Name = "Approve release of documents"},
            };

            foreach(var role in roles) {
                await roleManager.CreateAsync(role);
            }
            
            int counter  =4;
            foreach(var user in users) {

                user.UserName = user.UserName.ToLower();
                await userManager.CreateAsync(user, "Pa$$w0rd");
                await userManager.AddToRoleAsync(user, roles[counter++].Name);
            }

            var admin = new AppUser{
                UserName = "Admin"
            };

            await userManager.CreateAsync(admin, "Pa$$w0rd");
            await userManager.AddToRolesAsync(admin, new[] {"Admin"});
            
        }
    }
}