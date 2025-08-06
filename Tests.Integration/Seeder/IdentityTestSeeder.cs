using Infra.Identity;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Integration.Seeder;

public static class IdentityTestSeeder
{
    public static async Task SeedTestUsersAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        var email = "admin@sistema.com";
        var password = "Pa$$w0rd";
        var role = "admin";

        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }

        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            var newUser = new ApplicationUser
            {
                Email = email,
                UserName = email,
                EmailConfirmed = true
            };

            await userManager.CreateAsync(newUser, password);
            await userManager.AddToRoleAsync(newUser, role);
        }
    }
}
