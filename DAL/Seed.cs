using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<AppUser> userManger, RoleManager<AppRole> roleManager)
        {
            if (await userManger.Users.AnyAsync()) return;

            // var userData = await System.IO.File.ReadAllTextAsync("../DAL/UserSeedData.json");
            // var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

            // var roles = new List<AppRole>
            // {
            //     new AppRole() { Name = "Member"},
            //     new AppRole() { Name = "Admin"},
            //     new AppRole() { Name = "Moderator"}
            // };

            // foreach (var role in roles)
            // {
            //     await roleManager.CreateAsync(role);
            // }

            // foreach (var user in users)
            // {
            //     // using var hmac = new HMACSHA512();

            //     // user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("12345"));
            //     // user.PasswordSalt = hmac.Key;

            //     user.UserName = user.UserName.ToLower();
            //     await userManger.CreateAsync(user, "123456");
            //     await userManger.AddToRoleAsync(user, "Member");
            // }

            var admin = new AppUser()
            {
                UserName = "admin"
            };
            await userManger.CreateAsync(admin, "123456");
            await userManger.AddToRolesAsync(admin, new string[] { "Admin", "Moderator" });
        }
    }
}