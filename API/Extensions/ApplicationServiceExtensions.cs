using API.Models;
using Applications.Contracts;
using Applications.Mapping;
using Applications.Services;
using Core.Interfaces;
using Core.Models.Payments;
using Core.Models.ServiceOrders;
using Infra.Data;
using Infra.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices ( this IServiceCollection services, IConfiguration config )
        {
            services.AddControllers ( );
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

            services.AddEndpointsApiExplorer ( );



            services.Configure<ApiBehaviorOptions> ( options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                    .Where(e => e.Value is not null && e.Value.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors)
                    .Select(x => x.ErrorMessage)
                    .ToList();
                    var response = new ApiValidationErrorResponse
                    {
                        Errors = errors
                    };
                    return new BadRequestObjectResult ( response );
                };
            } );

            services
                .AddControllers ( )
                .AddJsonOptions ( opt =>
                {
                    opt.JsonSerializerOptions.Converters.Add ( new JsonStringEnumConverter ( ) );
                } );

            services.AddDbContext<ApplicationContext> ( options =>
                options.UseNpgsql ( config.GetConnectionString ( "DefaultConnection" ),
                 o => o.UseQuerySplittingBehavior ( QuerySplittingBehavior.SplitQuery ) ) );
            services.AddScoped ( typeof ( IGenericRepository<> ), typeof ( GenericRepository<> ) );
            services.AddScoped ( typeof ( IGenericService<> ), typeof ( GenericService<> ) );
            services.AddScoped<IGenericService<Payment>, GenericService<Payment>> ( );

            services.AddScoped<IGenericService<ServiceOrder>, GenericService<ServiceOrder>> ( );
            services.AddScoped<ITablePriceItemService, TablePriceItemService> ( );
            services.AddScoped<ITablePriceService, TablePriceService> ( );
            services.AddScoped<IUnitOfWork, UnitOfWork> ( );
            services.AddScoped<IClientService, ClientService> ( );
            services.AddScoped<IClientAreaService, ClientAreaService> ( );
            services.AddScoped<IShadeService, ShadeService> ( );
            services.AddScoped<IWorkTypeService, WorkTypeService> ( );
            services.AddScoped<IPaymentService, PaymentService> ( );
            services.AddScoped<ISectorService, SectorService> ( );

            services.AddScoped<IServiceOrderService, ServiceOrderService> ( );
            services.AddScoped<ISystemSettingsService, SystemSettingsService> ( );
            services.AddScoped<IBillingService, BillingService> ( );
            services.AddAutoMapper ( typeof ( ProductionMappingProfile ).Assembly );
            return services;
        }
    }
}
