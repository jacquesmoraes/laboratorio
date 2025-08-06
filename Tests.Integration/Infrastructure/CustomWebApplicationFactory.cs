using Infra.Data;
using Infra.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tests.Integration.Seeder;

namespace Tests.Integration.Infrastructure
{
    public class CustomWebApplicationFactory<TProgram>
        : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost ( IWebHostBuilder builder )
        {
            builder.UseEnvironment ( "Test" );

            builder.ConfigureAppConfiguration ( ( context, config ) =>
            {
                var testConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "AppSettings.Tests.json");
                if ( File.Exists ( testConfigPath ) )
                {
                    config.AddJsonFile ( testConfigPath, optional: false );
                }
                else
                {
                    config.AddInMemoryCollection ( new Dictionary<string, string?>
                    {
                        ["Jwt:Key"] = "chave-fake-segura-para-testes-com-32-caracteres",
                        ["Jwt:Issuer"] = "LabSystem",
                        ["Jwt:Audience"] = "LabSystemClient"
                    } );
                }
            } );

            builder.ConfigureServices ( services =>
     {
         // Remove contextos com providers reais
         var descriptorApp = services.SingleOrDefault(
        d => d.ServiceType == typeof(DbContextOptions<ApplicationContext>));
         if ( descriptorApp != null ) services.Remove ( descriptorApp );

         var descriptorIdentity = services.SingleOrDefault(
        d => d.ServiceType == typeof(DbContextOptions<AppIdentityDbContext>));
         if ( descriptorIdentity != null ) services.Remove ( descriptorIdentity );

         // Adiciona contextos com banco in-memory
         services.AddDbContext<ApplicationContext> ( options =>
             options.UseInMemoryDatabase ( "TestAppDb" ) );

         services.AddDbContext<AppIdentityDbContext> ( options =>
             options.UseInMemoryDatabase ( "TestIdentityDb" ) );

         // Registra Identity para testes
         services.AddIdentity<ApplicationUser, IdentityRole> ( )
             .AddEntityFrameworkStores<AppIdentityDbContext> ( )
             .AddDefaultTokenProviders ( );

         // Cria banco e popula com dados
         var sp = services.BuildServiceProvider();
         using var scope = sp.CreateScope();
         var scopedServices = scope.ServiceProvider;

         var appDb = scopedServices.GetRequiredService<ApplicationContext>();
         var identityDb = scopedServices.GetRequiredService<AppIdentityDbContext>();
         var userManager = scopedServices.GetRequiredService<UserManager<ApplicationUser>>();
         var roleManager = scopedServices.GetRequiredService<RoleManager<IdentityRole>>();
         var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<TProgram>>>();

         appDb.Database.EnsureCreated ( );
         identityDb.Database.EnsureCreated ( );

         try
         {
             IdentityTestSeeder.SeedTestUsersAsync ( scopedServices ).Wait ( );
         }
         catch ( Exception ex )
         {
             logger.LogError ( ex, "Erro ao popular banco de testes." );
         }
     } );



        }
    }
}