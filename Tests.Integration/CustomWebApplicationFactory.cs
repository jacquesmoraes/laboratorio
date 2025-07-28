using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Tests.Integration;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            
            var testConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "AppSettings.Tests.json");
            if (File.Exists(testConfigPath))
            {
                config.AddJsonFile(testConfigPath, optional: false);
            }
            else
            {
                // Tenta o caminho relativo ao projeto API
                var apiProjectPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "API", "AppSettings.Tests.json");
                if (File.Exists(apiProjectPath))
                {
                    config.AddJsonFile(apiProjectPath, optional: false);
                }
                else
                {
                    // Se não encontrar, adiciona configuração JWT inline para testes
                    config.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["Jwt:Key"] = "chave-fake-segura-para-testes-com-32-caracteres",
                        ["Jwt:Issuer"] = "LabSystem",
                        ["Jwt:Audience"] = "LabSystemClient"
                    });
                }
            }
        });
    }
}