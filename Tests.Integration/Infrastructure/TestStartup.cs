using Infra.Data;
using Infra.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tests.Integration.Seeder;

namespace Tests.Integration.Infrastructure;

public class TestStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Configuração específica para testes
        services.AddControllers();

        // Adiciona contextos in-memory
        services.AddDbContext<ApplicationContext>(options =>
            options.UseInMemoryDatabase("TestAppDb"));

        services.AddDbContext<AppIdentityDbContext>(options =>
            options.UseInMemoryDatabase("TestIdentityDb"));

        // Adiciona Identity
        services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<AppIdentityDbContext>()
            .AddDefaultTokenProviders();

        // Adiciona autenticação JWT simplificada para testes
        services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = false,
                    ValidateLifetime = false,
                    SignatureValidator = (token, parameters) => new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(token)
                };
            });

        services.AddAuthorization();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        // Seed do banco de testes
        using var scope = app.ApplicationServices.CreateScope();
        var scopedServices = scope.ServiceProvider;

        var appDb = scopedServices.GetRequiredService<ApplicationContext>();
        var identityDb = scopedServices.GetRequiredService<AppIdentityDbContext>();
        var userManager = scopedServices.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scopedServices.GetRequiredService<RoleManager<ApplicationRole>>();

        appDb.Database.EnsureCreated();
        identityDb.Database.EnsureCreated();

        try
        {
            IdentityTestSeeder.SeedTestUsersAsync(scopedServices).Wait();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao popular banco de testes: {ex.Message}");
        }
    }
}
