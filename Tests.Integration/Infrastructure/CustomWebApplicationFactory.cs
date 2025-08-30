using Infra.Data;
using Infra.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Tests.Integration.Seeder;

namespace Tests.Integration.Infrastructure
{
    public class CustomWebApplicationFactory<TProgram>
        : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost ( IWebHostBuilder builder )
        {
            builder.UseEnvironment ( "Test" );

            builder.ConfigureServices ( async services =>
            {
                // Forçar configuração de autenticação para testes
                services.Configure<JwtBearerOptions> ( JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = false,
                        SignatureValidator = ( token, parameters ) => new JwtSecurityToken ( token ),
                        ValidateLifetime = false
                    };
                } );

                // Criar service provider e popular banco
                var serviceProvider = services.BuildServiceProvider();

                using ( var scope = serviceProvider.CreateScope ( ) )
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<ApplicationContext>();
                    var identityDb = scopedServices.GetRequiredService<AppIdentityDbContext>();
                    var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<TProgram>>>();

                    try
                    {
                        // Garantir que os bancos sejam criados
                        db.Database.EnsureCreated ( );
                        identityDb.Database.EnsureCreated ( );

                        // Popular dados de teste
                        await IdentityTestSeeder.SeedTestUsersAsync ( scopedServices );
                    }
                    catch ( Exception ex )
                    {
                        logger.LogError ( ex, "Erro ao popular banco de testes." );
                        throw;
                    }
                }
            } );
        }
    }
}