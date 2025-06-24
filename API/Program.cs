using API.Extensions;
using API.Middleware;
using Infra.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers ( );
builder.Services.AddApplicationServices ( builder.Configuration );

builder.Services.AddSwaggerDocumentation ( );

var app = builder.Build();



app.UseMiddleware<ExceptionMiddleware> ( );
app.UseStatusCodePagesWithReExecute ( "/errors/{0}" );
// Middleware de documentação
if ( app.Environment.IsDevelopment ( ) )
{
    app.UseSwaggerDocumention ( );
}

app.UseStaticFiles ( );

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
    await context.Database.MigrateAsync ( );
    await ApplicationContextSeed.SeedAsync ( context, logger );
}
catch ( Exception ex )
{
    logger.LogError ( ex, "An error occurred while migrating or seeding the database" );
    throw;
}
app.Run ( );
