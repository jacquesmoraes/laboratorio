using API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddApplicationServices ( builder.Configuration );
builder.Services.AddControllers ( );

var app = builder.Build();

// Configure the HTTP request pipeline.
if ( app.Environment.IsDevelopment ( ) )
{
    app.MapOpenApi ( );
}

app.UseHttpsRedirection ( );

app.UseAuthorization ( );

app.MapControllers ( );

app.Run ( );
