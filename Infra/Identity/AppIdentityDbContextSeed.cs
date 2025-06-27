using Microsoft.AspNetCore.Identity;

namespace Infra.Identity
{
    public class AppIdentityDbContextSeed
    {
        public static async Task SeedUsersAsync(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            // Criar roles se não existirem
            var roles = new[] { "admin", "client" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new ApplicationRole { Name = role });
            }

            // Criar admin user se não existir
            var adminEmail = "admin@sistema.com";
            if (await userManager.FindByEmailAsync(adminEmail) is null)
            {
                var admin = new ApplicationUser
                {
                    DisplayName = "Administrador",
                    Email = adminEmail,
                    UserName = adminEmail,
                    IsActive = true,
                    IsFirstLogin = false,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(admin, "Pa$$w0rd");

                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "admin");
            }
        }
    }
}
