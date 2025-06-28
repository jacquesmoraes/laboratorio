using API.Extensions;
using API.Middleware;
using Infra.Data;
using Infra.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();
builder.Services.AddControllers ( );
builder.Services.AddApplicationServices ( builder.Configuration );
builder.Services.AddIdentityServices ( builder.Configuration );
builder.Services.AddSwaggerDocumentation ( );

var app = builder.Build();


app.UseStaticFiles ( );
app.UseMiddleware<ExceptionMiddleware> ( );
app.UseStatusCodePagesWithReExecute ( "/errors/{0}" );
// Middleware de documentação
if ( app.Environment.IsDevelopment ( ) || app.Environment.IsEnvironment ( "Test" ) )

{
    app.UseSwaggerDocumention ( );
    
}



app.UseAuthentication ( );
app.UseAuthorization ( );


app.MapControllers ( );



// Seed dos bancos
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var context = services.GetRequiredService<ApplicationContext>();
var logger = services.GetRequiredService<ILogger<Program>>();

try
{
    if ( !app.Environment.IsEnvironment ( "Test" ) )
    {
        await context.Database.MigrateAsync ( );
        await ApplicationContextSeed.SeedAsync ( context, logger );

        var identityContext = services.GetRequiredService<AppIdentityDbContext>();
        await identityContext.Database.MigrateAsync ( );

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
        await AppIdentityDbContextSeed.SeedUsersAsync ( userManager, roleManager );

    }

}
catch ( Exception ex )
{
    logger.LogError ( ex, "An error occurred while migrating or seeding the database" );
    throw;
}

app.Run ( );
public partial class Program { }
