using API.Extensions;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddApplicationServices ( builder.Configuration );
builder.Services.AddControllers ( );


var app = builder.Build();

// Configure the HTTP request pipeline.
if ( app.Environment.IsDevelopment ( ) )
{
  
   
     
}
 app.UseSwagger ( );
    app.UseSwaggerUI ( c =>
    {
        c.SwaggerEndpoint ( "/swagger/v1/swagger.json", "Minha API v1" );
      
    } );


app.UseHttpsRedirection ( );

app.UseAuthorization ( );

app.MapControllers ( );

app.Run ( );
