
using FPTBook_v3.Constants;
using Microsoft.AspNetCore.Identity;
using NuGet.Common;

namespace FPTBook_v3.Data
{
    public class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider service)
        {
            //Seed Role
            var userManager = service.GetService<UserManager<ApplicationUser>>();
            var roleManager = service.GetService<RoleManager<IdentityRole>>();
            await roleManager.CreateAsync(new IdentityRole(Role.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Role.User.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Role.Owner.ToString()));


            var user = new ApplicationUser
            {
                UserName = "User@gmail.com",
                Email = "User@gmail.com",
                User_Name = "User",


            };

            var userInDb = await userManager.FindByEmailAsync(user.Email);
            if (userInDb == null)
            {
                await userManager.CreateAsync(user, "User@123");
                await userManager.AddToRoleAsync(user, Role.User.ToString());
            }

            var owner = new ApplicationUser
            {
                UserName = "Owner@gmail.com",
                Email = "Owner@gmail.com",
                User_Name = "Owner",
                
            };

            var ownerInDb = await userManager.FindByEmailAsync(owner.Email);
            if (userInDb == null)
            {
                await userManager.CreateAsync(owner, "Owner@123");
                await userManager.AddToRoleAsync(owner, Role.Owner.ToString());
            }

            var admin = new ApplicationUser
            {
                UserName = "Admin@gmail.com",
                Email = "Admin@gmail.com",
                User_Name = "Admin",

            };

            var adminInDb = await userManager.FindByEmailAsync(admin.Email);
            if (userInDb == null)
            {
                await userManager.CreateAsync(admin, "Admin@123");
                await userManager.AddToRoleAsync(admin, Role.Admin.ToString());
            }
        }
    }
}
