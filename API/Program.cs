using API.Extensions;
using API.Middleware;
using Infra.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddApplicationServices ( builder.Configuration );
builder.Services.AddControllers ( );


var app = builder.Build();


if ( app.Environment.IsDevelopment ( ) )
{
    app.UseSwagger ( );
    app.UseSwaggerUI ( c =>
    {
        c.SwaggerEndpoint ( "/swagger/v1/swagger.json", "Minha API v1" );
    } );
}


app.UseMiddleware<ExceptionMiddleware> ( );
app.UseStaticFiles(); // necessário para servir os arquivos de /wwwroot

app.UseStatusCodePagesWithReExecute ( "/errors/{0}" );

app.UseHttpsRedirection ( );
app.UseAuthorization ( );

app.MapControllers ( );
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var context = services.GetRequiredService<ApplicationContext>();
var logger = services.GetRequiredService<ILogger<Program>>();

try
{
    await context.Database.MigrateAsync ( );
    await ApplicationContextSeed.SeedAsync ( context, logger );
}
catch ( Exception ex )
{
    logger.LogError ( ex, "An error occurred while migrating or seeding the database" );
    throw;
}
app.Run ( );

