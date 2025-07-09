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

            var environment = config["ASPNETCORE_ENVIRONMENT"] ?? "Development";

            if ( environment == "Test" )
            {
                // for tests, use InMemory
                services.AddDbContext<ApplicationContext> ( options =>
                    options.UseInMemoryDatabase ( "TestDb" ) );
            }
            else
            {
                // for production/development, use PostgreSQL
                services.AddDbContext<ApplicationContext> ( options =>
                    options.UseNpgsql ( config.GetConnectionString ( "DefaultConnection" ),
                     o => o.UseQuerySplittingBehavior ( QuerySplittingBehavior.SplitQuery ) ) );
            }
            services.AddCors(opt =>
            {
                opt.AddPolicy(
                    name: "AllowOrigin",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                    });
            });



            services.AddScoped ( typeof ( IGenericRepository<> ), typeof ( GenericRepository<> ) );
            services.AddScoped ( typeof ( IGenericService<> ), typeof ( GenericService<> ) );
            services.AddScoped<IGenericService<Payment>, GenericService<Payment>> ( );
            services.AddScoped<IIdentityService, IdentityService> ( );
            services.AddScoped<IGenericService<ServiceOrder>, GenericService<ServiceOrder>> ( );
            
            services.AddScoped<ITablePriceService, TablePriceService> ( );
            services.AddScoped<IUnitOfWork, UnitOfWork> ( );
            services.AddScoped<UpdateOverdueStatusFilter> ( );
            services.AddScoped<IClientService, ClientService> ( );
            services.AddScoped<IClientAreaService, ClientAreaService> ( );
            services.AddScoped<IShadeService, ShadeService> ( );
            services.AddScoped<IWorkTypeService, WorkTypeService> ( );
            services.AddScoped<IPaymentService, PaymentService> ( );
            services.AddScoped<ISectorService, SectorService> ( );
            services.AddScoped<IScheduleService, ScheduleService> ( );
            services.AddScoped<IServiceOrderService, ServiceOrderService> ( );
            services.AddScoped<ISystemSettingsService, SystemSettingsService> ( );
            services.AddScoped<IBillingService, BillingService> ( );
            services.AddAutoMapper ( typeof ( ProductionMappingProfile ).Assembly );
            return services;
        }
    }
}
