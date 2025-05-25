using Applications.Contracts;
using Applications.Mapping;
using Applications.Services;
using Core.Interfaces;
using Core.Models.Clients;
using Core.Models.ServiceOrders;
using Infra.Data;
using Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices ( this IServiceCollection services, IConfiguration config )
        {
            services.AddControllers ( );
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
           
            services.AddEndpointsApiExplorer ( );

            services.AddSwaggerGen ( c =>
            {
                c.SwaggerDoc ( "v1", new OpenApiInfo { Title = "Minha API", Version = "v1" } );
            } );

            services.AddDbContext<ApplicationContext> ( options =>
                options.UseNpgsql ( config.GetConnectionString ( "DefaultConnection" ),
                 o => o.UseQuerySplittingBehavior ( QuerySplittingBehavior.SplitQuery ) ) );
            services.AddScoped ( typeof ( IGenericRepository<> ), typeof ( GenericRepository<> ) );
            services.AddScoped ( typeof ( IGenericService<> ), typeof ( GenericService<> ) );
            services.AddScoped<IGenericService<PerClientPayment>, GenericService<PerClientPayment>> ( );
           
            services.AddScoped<IGenericService<ServiceOrder>, GenericService<ServiceOrder>> ( );
            services.AddScoped<ITablePriceItemService, TablePriceItemService> ( );
            services.AddScoped<ITablePriceService, TablePriceService> ( );
            services.AddScoped<IUnitOfWork, UnitOfWork> ( );
            services.AddScoped<IClientService, ClientService> ( );
            services.AddScoped<IWorkTypeService, WorkTypeService> ( );
            services.AddScoped<ISectorService, SectorService> ( );
            services.AddScoped<IServiceOrderService, ServiceOrderService> ( );
            services.AddAutoMapper ( typeof ( ProductionMappingProfile ).Assembly );
            return services;
        }
    }
}
