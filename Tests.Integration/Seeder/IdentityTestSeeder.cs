using Infra.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;


namespace Tests.Integration.Seeder;

public static class IdentityTestSeeder
{
    public static async Task SeedTestUsersAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        var email = "admin@sistema.com";
        var password = "Pa$$w0rd";
        var role = "admin";

        // Verifica se a role existe, se não, cria
        if (!await roleManager.RoleExistsAsync(role))
        {
            var newRole = new ApplicationRole
            {
                Name = role,
                NormalizedName = role.ToUpperInvariant()
            };
            var roleResult = await roleManager.CreateAsync(newRole);
            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException($"Falha ao criar role: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
            }
        }

        // Verifica se o usuário existe
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            var newUser = new ApplicationUser
            {
                Email = email,
                UserName = email,
                EmailConfirmed = true,
                IsActive = true,
                IsFirstLogin = false,
                DisplayName = "Admin User"
            };

            var createResult = await userManager.CreateAsync(newUser, password);
            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException($"Falha ao criar usuário: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
            }

            var addRoleResult = await userManager.AddToRoleAsync(newUser, role);
            if (!addRoleResult.Succeeded)
            {
                throw new InvalidOperationException($"Falha ao adicionar role: {string.Join(", ", addRoleResult.Errors.Select(e => e.Description))}");
            }
        }
        else
        {
            // Garantir que o usuário está ativo e com a role correta
            user.IsActive = true;
            user.IsFirstLogin = false;
            await userManager.UpdateAsync(user);
            
            var roles = await userManager.GetRolesAsync(user);
            if (!roles.Contains(role))
            {
                var addRoleResult = await userManager.AddToRoleAsync(user, role);
                if (!addRoleResult.Succeeded)
                {
                    throw new InvalidOperationException($"Falha ao adicionar role: {string.Join(", ", addRoleResult.Errors.Select(e => e.Description))}");
                }
            }
        }

        // Verificar se tudo foi criado corretamente
        var finalUser = await userManager.FindByEmailAsync(email);
        var finalRoles = await userManager.GetRolesAsync(finalUser!);
        Console.WriteLine($"[DEBUG] Usuário criado: {finalUser?.Email}, Ativo: {finalUser?.IsActive}, Roles: {string.Join(", ", finalRoles)}");
    }
}

