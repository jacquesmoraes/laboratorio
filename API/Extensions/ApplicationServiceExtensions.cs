using Applications.Interfaces;
using Applications.Mapping;
using Applications.Services;
using Core.Interfaces;
using Infra.Data;
using Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices ( this IServiceCollection services, IConfiguration config )
        {
            services.AddControllers ( );
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            services.AddOpenApi ( );
            services.AddDbContext<ApplicationContext> ( options =>
                options.UseNpgsql ( config.GetConnectionString ( "DefaultConnection" ) ) );
            services.AddScoped ( typeof ( IGenericRepository<> ), typeof ( GenericRepository<> ) );
            services.AddScoped ( typeof ( IGenericService<> ), typeof ( GenericService<> ) );
            
            services.AddScoped<ITablePriceItemService, TablePriceItemService> ( );
            
            services.AddScoped<IWorkTypeService, WorkTypeService> ( );
            services.AddAutoMapper(typeof ( MappingProfile ).Assembly );
            return services;
        }
    }
}
