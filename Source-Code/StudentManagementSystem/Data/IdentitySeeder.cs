using Microsoft.AspNetCore.Identity;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Data
{
    public static class IdentitySeeder
    {
        private const string AdminEmail = "admin@studentsystem.com";
        private const string AdminPassword = "Admin@12345";

        public static async Task SeedAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var roleManager =
                scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var userManager =
                scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles =
            {
                "Admin",
                "Student"
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var admin = await userManager.FindByEmailAsync(AdminEmail);

            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = AdminEmail,
                    Email = AdminEmail,
                    EmailConfirmed = true,
                    FullName = "System Administrator"
                };

                var result = await userManager.CreateAsync(
                    admin,
                    AdminPassword);

                if (!result.Succeeded)
                {
                    throw new Exception(
                        "Unable to create default administrator: " +
                        string.Join(", ",
                            result.Errors.Select(e => e.Description)));
                }
            }

            if (!await userManager.IsInRoleAsync(admin, "Admin"))
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}