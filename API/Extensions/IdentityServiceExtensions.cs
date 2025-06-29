namespace API.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices ( this IServiceCollection services, IConfiguration config )
        {
            services.AddDbContext<AppIdentityDbContext> ( opt =>
            {
                opt.UseNpgsql ( config.GetConnectionString ( "IdentityConnection" ) );
            } );

            services.AddIdentityCore<ApplicationUser> ( opt =>
            {
                opt.Password.RequireDigit = true;
                opt.Password.RequiredLength = 6;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireNonAlphanumeric = false;

            } )
            .AddRoles<ApplicationRole> ( )
            .AddEntityFrameworkStores<AppIdentityDbContext> ( )
            .AddSignInManager<SignInManager<ApplicationUser>> ( )
            .AddDefaultTokenProviders ( );

            var jwtKey = config["Jwt:Key"];
            var jwtIssuer = config["Jwt:Issuer"] ?? "LabSystem";
            var jwtAudience = config["Jwt:Audience"] ?? "LabSystemClient";

            if ( !string.IsNullOrWhiteSpace ( jwtKey ) )
            {
                var key = Encoding.UTF8.GetBytes(jwtKey);

                services.AddAuthentication ( JwtBearerDefaults.AuthenticationScheme )
                    .AddJwtBearer ( opt =>
                    {
                        opt.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey ( key ),
                            ValidIssuer = jwtIssuer,
                            ValidAudience = jwtAudience,
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ClockSkew = TimeSpan.Zero
                        };
                    } );
            }
            else
            {
                // Fallback for tests (without real token validation)
                services.AddAuthentication ( options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                } )
                .AddJwtBearer ( options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = false,
                        SignatureValidator = ( token, parameters ) => new JwtSecurityToken ( token ),
                        ValidateLifetime = false
                    };
                } );
            }

            services.AddAuthorization ( );

            return services;
        }
    }
}
